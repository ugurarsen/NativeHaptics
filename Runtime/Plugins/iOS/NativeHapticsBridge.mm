#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <AudioToolbox/AudioToolbox.h>

#if __has_include(<CoreHaptics/CoreHaptics.h>)
#import <CoreHaptics/CoreHaptics.h>
#define NH_COREHAPTICS 1
#endif

// This bridge is only ever built for Apple mobile targets. Unity's UNITY_IOS
// define is NOT guaranteed to be present in every compilation (e.g. manual or
// non-Unity Xcode builds), so gate on the SDK's TARGET_OS_IPHONE instead.
#if TARGET_OS_IPHONE
#define NH_APPLE 1
#else
#define NH_APPLE 0
#endif

// Verbose per-call diagnostics compile out unless enabled.
#define NH_VERBOSE_LOGS 0
#if NH_VERBOSE_LOGS
#define NHVerboseLog(...) NSLog(__VA_ARGS__)
#else
#define NHVerboseLog(...) ((void)0)
#endif

__attribute__((constructor))
static void NH_BridgeInit(void)
{
    NSLog(@"[NativeHaptics] Bridge loaded (v1.0.0).");
}

static UIImpactFeedbackGenerator *s_impactGenerators[5]    = {nil};
static UINotificationFeedbackGenerator *s_notificationGenerator = nil;
static UISelectionFeedbackGenerator *s_selectionGenerator    = nil;

#if NH_COREHAPTICS
static CHHapticEngine *g_engine = nil;
static BOOL g_engineRunning = NO;
static id<CHHapticPatternPlayer> g_activePlayer = nil;
#endif

static dispatch_once_t g_token;
static void InitGenerators(void)
{
    dispatch_once(&g_token, ^{
        if (@available(iOS 11.0, *))
        {
            s_impactGenerators[0] = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleLight];
            s_impactGenerators[1] = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleMedium];
            s_impactGenerators[2] = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleHeavy];
            s_impactGenerators[3] = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleRigid];
            s_impactGenerators[4] = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleSoft];
        }
        if (@available(iOS 10.0, *))
        {
            s_notificationGenerator = [[UINotificationFeedbackGenerator alloc] init];
            s_selectionGenerator = [[UISelectionFeedbackGenerator alloc] init];
        }
    });
}

#if NH_COREHAPTICS
static BOOL CoreHapticsUsable(void)
{
    if (![CHHapticEngine class]) return NO;
    if (![[CHHapticEngine capabilitiesForHardware] supportsHaptics]) return NO;
    static BOOL logged = NO;
    if (!logged)
    {
        logged = YES;
        NHVerboseLog(@"[NativeHaptics] CoreHaptics hardware supported.");
    }
    return YES;
}

static BOOL EnsureEngine(void)
{
    if (!g_engine)
    {
        NSError *err = nil;
        g_engine = [[CHHapticEngine alloc] initAndReturnError:&err];
        if (err) NSLog(@"[NativeHaptics] CoreHaptics error: %@", err.localizedDescription);
        if (!g_engine) return NO;

        g_engine.playsHapticsOnly = YES;

        g_engine.stoppedHandler = ^(CHHapticEngineStoppedReason reason) {
            NHVerboseLog(@"[NativeHaptics] CoreHaptics engine stopped (reason: %ld).", (long)reason);
            g_engineRunning = NO;
        };
        g_engine.resetHandler = ^{
            NHVerboseLog(@"[NativeHaptics] CoreHaptics engine reset.");
            g_engineRunning = NO;
        };
    }

    if (!g_engineRunning)
    {
        NSError *err = nil;
        if (![g_engine startAndReturnError:&err])
        {
            if (err) NSLog(@"[NativeHaptics] CoreHaptics error: %@", err.localizedDescription);
            return NO;
        }
        g_engineRunning = YES;
        NHVerboseLog(@"[NativeHaptics] CoreHaptics engine started.");
    }
    return YES;
}

// Hold the active player at class scope so ARC does not release it when the
// exported function returns; stop + release it on the next vibration or Cancel.
static void StopActivePlayer(void)
{
    if (g_activePlayer)
    {
        [g_activePlayer stopAtTime:CHHapticTimeImmediate error:nil];
        g_activePlayer = nil;
    }
}
#endif

// Fallback for devices without CoreHaptics (iOS < 13) or broken engines:
// best-effort timed pulse sequence using the cached impact generator.
// UIKit feedback is main-thread only, so schedule the timing on a dedicated
// pulse queue and hop back to the main queue for the actual impact call.
static dispatch_queue_t g_pulseQueue;
static unsigned g_pulseToken = 0;
static dispatch_once_t g_pulseOnce;
static void EnsurePulseQueue(void)
{
    dispatch_once(&g_pulseOnce, ^{
        g_pulseQueue = dispatch_queue_create("com.ugurarsen.nativehaptics.pulses", DISPATCH_QUEUE_SERIAL);
    });
}

static void PlayPulseSequence(const long *timings, const int *amplitudes, int length, float scale)
{
    EnsurePulseQueue();
    unsigned token = ++g_pulseToken;
    long lastOffset = 0;
    for (int i = 0; i < length; i++)
    {
        if (i % 2 == 1 && timings[i] > 0)
        {
            float a = amplitudes ? fminf(1.0f, (float)amplitudes[i] / 255.0f) : 0.75f;
            a *= scale;
            long at = lastOffset;
            dispatch_after(dispatch_time(DISPATCH_TIME_NOW, (int64_t)(at * NSEC_PER_MSEC)), g_pulseQueue, ^{
                if (token != g_pulseToken) return;
                dispatch_async(dispatch_get_main_queue(), ^{
                    if (token != g_pulseToken) return;
#if NH_APPLE
                    if (@available(iOS 11.0, *))
                    {
                        UIImpactFeedbackGenerator *gen = s_impactGenerators[1];
                        [gen prepare];
                        NHVerboseLog(@"[NativeHaptics] UIKit pulse triggered.");
                        if (@available(iOS 13.0, *))
                        {
                            [gen impactOccurredWithIntensity:fmaxf(0.1f, a)];
                        }
                        else
                        {
                            [gen impactOccurred];
                        }
                    }
#endif
                });
            });
        }
        lastOffset += timings[i];
    }
}

#if NH_COREHAPTICS
static BOOL PlayPatternCoreHaptics(const long *timings, const int *amplitudes, int length, float scale)
{
    if (!CoreHapticsUsable() || !EnsureEngine())
    {
        return NO;
    }

    StopActivePlayer();

    NSMutableArray *events = [NSMutableArray array];
    double t = 0.0;
    for (int i = 0; i < length; i++)
    {
        double d = ((double)timings[i]) / 1000.0;
        if (i % 2 == 1 && d > 0.0)
        {
            float amp = amplitudes ? fminf(1.0f, (float)amplitudes[i] / 255.0f) : 0.75f;
            amp *= scale;
            if (amp > 0.002f)
            {
                CHHapticEventParameter *intensity =
                    [[CHHapticEventParameter alloc] initWithParameterID:CHHapticEventParameterIDHapticIntensity
                                                                  value:fmaxf(0.05f, amp)];
                CHHapticEventParameter *sharpness =
                    [[CHHapticEventParameter alloc] initWithParameterID:CHHapticEventParameterIDHapticSharpness
                                                                  value:0.0f];
                CHHapticEvent *ev = [[CHHapticEvent alloc]
                    initWithEventType:CHHapticEventTypeHapticContinuous
                           parameters:@[intensity, sharpness]
                         relativeTime:t
                             duration:d];
                [events addObject:ev];
            }
        }
        t += d;
    }

    if (events.count == 0) return NO;

    NSError *err = nil;
    CHHapticPattern *pattern = [[CHHapticPattern alloc] initWithEvents:events parameters:@[] error:&err];
    if (!pattern || err)
    {
        if (err) NSLog(@"[NativeHaptics] CoreHaptics error: %@", err.localizedDescription);
        return NO;
    }

    err = nil;
    id<CHHapticPatternPlayer> player = [g_engine createPlayerWithPattern:pattern error:&err];
    if (err || !player)
    {
        if (err) NSLog(@"[NativeHaptics] CoreHaptics error: %@", err.localizedDescription);
        return NO;
    }

    err = nil;
    if ([player startAtTime:CHHapticTimeImmediate error:&err])
    {
        g_activePlayer = player;
        NHVerboseLog(@"[NativeHaptics] CoreHaptics pattern playing (%lu events).", (unsigned long)events.count);
        return YES;
    }
    if (err) NSLog(@"[NativeHaptics] CoreHaptics error: %@", err.localizedDescription);
    return NO;
}

static BOOL PlayEnvelopeCoreHaptics(const float *amplitudes, float stepDurationSec, int length)
{
    if (!CoreHapticsUsable() || !EnsureEngine())
    {
        return NO;
    }

    StopActivePlayer();

    double total = (double)length * stepDurationSec;
    NSMutableArray *curvePoints = [NSMutableArray array];
    for (int i = 0; i < length; i++)
    {
        float a = fmaxf(0.0f, fminf(1.0f, amplitudes[i]));
        CHHapticParameterCurveControlPoint *pt =
            [[CHHapticParameterCurveControlPoint alloc] initWithRelativeTime:(double)i * stepDurationSec
                                                                        value:a];
        [curvePoints addObject:pt];
    }

    CHHapticParameterCurve *curve = [[CHHapticParameterCurve alloc]
        initWithParameterID:CHHapticDynamicParameterIDHapticIntensityControl
               controlPoints:curvePoints
                relativeTime:0.0];
    if (!curve) return NO;

    CHHapticEvent *event = [[CHHapticEvent alloc]
        initWithEventType:CHHapticEventTypeHapticContinuous
               parameters:@[
                   [[CHHapticEventParameter alloc] initWithParameterID:CHHapticEventParameterIDHapticIntensity value:0.0f],
                   [[CHHapticEventParameter alloc] initWithParameterID:CHHapticEventParameterIDHapticSharpness value:0.0f]
               ]
             relativeTime:0.0
                 duration:total];
    NSError *err = nil;
    CHHapticPattern *pattern = [[CHHapticPattern alloc]
        initWithEvents:@[event] parameterCurves:@[curve] error:&err];
    if (!pattern || err)
    {
        if (err) NSLog(@"[NativeHaptics] CoreHaptics error: %@", err.localizedDescription);
        return NO;
    }

    err = nil;
    id<CHHapticPatternPlayer> player = [g_engine createPlayerWithPattern:pattern error:&err];
    if (err || !player)
    {
        if (err) NSLog(@"[NativeHaptics] CoreHaptics error: %@", err.localizedDescription);
        return NO;
    }

    err = nil;
    if ([player startAtTime:CHHapticTimeImmediate error:&err])
    {
        g_activePlayer = player;
        NHVerboseLog(@"[NativeHaptics] CoreHaptics envelope playing (%.2fs).", total);
        return YES;
    }
    if (err) NSLog(@"[NativeHaptics] CoreHaptics error: %@", err.localizedDescription);
    return NO;
}
#endif

// Fallback to a plain one-shot for empty/invalid patterns.
extern "C" {

// Read-only capability query: safe off the main thread and must return a value
// synchronously, so it is intentionally NOT deferred to the main queue.
bool NativeHaptics_IsSupported(void)
{
    NHVerboseLog(@"[NativeHaptics] Bridge called: %s", __FUNCTION__);
#if NH_APPLE
    if (@available(iOS 11.0, *))
    {
        return [UIImpactFeedbackGenerator class] != nil;
    }
#endif
    return false;
}

void NativeHaptics_PlayImpact(int style, float intensity)
{
    dispatch_async(dispatch_get_main_queue(), ^{
        NHVerboseLog(@"[NativeHaptics] Bridge called: %s (style: %d, intensity: %.2f)", __FUNCTION__, (int)style, intensity);
#if NH_APPLE
        if (@available(iOS 11.0, *))
        {
            int s = style;
            if (s < 0 || s > 4) s = 1;
            if (!s_impactGenerators[s])
            {
                s_impactGenerators[s] = [[UIImpactFeedbackGenerator alloc] initWithStyle:(UIImpactFeedbackStyle)s];
            }
            UIImpactFeedbackGenerator *gen = s_impactGenerators[s];
            [gen prepare];
            if (@available(iOS 13.0, *))
            {
                [gen impactOccurredWithIntensity:fmaxf(0.0f, fminf(1.0f, intensity))];
            }
            else
            {
                [gen impactOccurred];
            }
            NHVerboseLog(@"[NativeHaptics] Physical UIKit impact fired! (style: %d)", s);
        }
#endif
    });
}

void NativeHaptics_PlayNotification(int type, float intensity)
{
    dispatch_async(dispatch_get_main_queue(), ^{
        NHVerboseLog(@"[NativeHaptics] Bridge called: %s (type: %d, intensity: %.2f)", __FUNCTION__, (int)type, intensity);
#if NH_APPLE
        if (@available(iOS 11.0, *))
        {
            InitGenerators();
            UINotificationFeedbackType feedbackType;
            switch (type)
            {
                case 0: feedbackType = UINotificationFeedbackTypeSuccess; break;
                case 1: feedbackType = UINotificationFeedbackTypeSuccess; break;
                case 2: feedbackType = UINotificationFeedbackTypeWarning; break;
                case 3: feedbackType = UINotificationFeedbackTypeError; break;
                default: feedbackType = UINotificationFeedbackTypeSuccess; break;
            }
            [s_notificationGenerator prepare];
            [s_notificationGenerator notificationOccurred:feedbackType];
        }
#endif
    });
}

void NativeHaptics_PlaySelection(float intensity)
{
    dispatch_async(dispatch_get_main_queue(), ^{
        NHVerboseLog(@"[NativeHaptics] Bridge called: %s (intensity: %.2f)", __FUNCTION__, intensity);
#if NH_APPLE
        if (@available(iOS 10.0, *))
        {
            if (!s_selectionGenerator)
            {
                s_selectionGenerator = [[UISelectionFeedbackGenerator alloc] init];
            }
            [s_selectionGenerator prepare];
            [s_selectionGenerator selectionChanged];
            NHVerboseLog(@"[NativeHaptics] Physical UIKit selection fired!");
        }
#endif
    });
}

void NativeHaptics_PlayOneShot(float duration, float amplitude)
{
    dispatch_async(dispatch_get_main_queue(), ^{
        NHVerboseLog(@"[NativeHaptics] Bridge called: %s (duration: %.2f, amplitude: %.2f)", __FUNCTION__, duration, amplitude);
#if NH_APPLE
        if (@available(iOS 11.0, *))
        {
            InitGenerators();
            UIImpactFeedbackGenerator *gen = s_impactGenerators[1];
            [gen prepare];
            NHVerboseLog(@"[NativeHaptics] UIKit impact triggered: %ld", (long)1);
            if (@available(iOS 13.0, *))
            {
                [gen impactOccurredWithIntensity:fmaxf(0.0f, fminf(1.0f, amplitude))];
            }
            else
            {
                [gen impactOccurred];
            }
        }
#endif
    });
}

void NativeHaptics_PlayPattern(const long *timings, const int *amplitudes, int length)
{
    if (timings == NULL || length <= 0) return;

    // The marshaled buffers are only valid during this call, so copy them
    // before deferring execution to the main queue.
    long *timingsCopy = (long *)malloc((size_t)length * sizeof(long));
    int *ampCopy = NULL;
    if (amplitudes != NULL) ampCopy = (int *)malloc((size_t)length * sizeof(int));
    if (!timingsCopy || (amplitudes != NULL && !ampCopy))
    {
        free(timingsCopy);
        free(ampCopy);
        return;
    }
    memcpy(timingsCopy, timings, (size_t)length * sizeof(long));
    if (ampCopy) memcpy(ampCopy, amplitudes, (size_t)length * sizeof(int));

    dispatch_async(dispatch_get_main_queue(), ^{
        NHVerboseLog(@"[NativeHaptics] Bridge called: %s (length: %d, amplitudes: %@)", __FUNCTION__, (int)length, ampCopy ? @"yes" : @"no");
        InitGenerators();
#if NH_COREHAPTICS
        if (@available(iOS 13.0, *))
        {
            if (PlayPatternCoreHaptics(timingsCopy, ampCopy, length, 1.0f))
            {
                free(timingsCopy);
                free(ampCopy);
                return;
            }
            NHVerboseLog(@"[NativeHaptics] CoreHaptics unavailable, falling back to UIKit pulses.");
        }
#endif
        PlayPulseSequence(timingsCopy, ampCopy, length, 1.0f);
        free(timingsCopy);
        free(ampCopy);
    });
}

void NativeHaptics_PlayEnvelope(const float *amplitudes, float stepDurationSec, int length)
{
    if (amplitudes == NULL || length <= 0 || stepDurationSec <= 0.0f) return;

    float *ampCopy = (float *)malloc((size_t)length * sizeof(float));
    if (!ampCopy) return;
    memcpy(ampCopy, amplitudes, (size_t)length * sizeof(float));

    dispatch_async(dispatch_get_main_queue(), ^{
        NHVerboseLog(@"[NativeHaptics] Bridge called: %s (length: %d, step: %.3fs)", __FUNCTION__, (int)length, stepDurationSec);
        InitGenerators();
#if NH_COREHAPTICS
        if (@available(iOS 13.0, *))
        {
            if (PlayEnvelopeCoreHaptics(ampCopy, stepDurationSec, length))
            {
                free(ampCopy);
                return;
            }
            NHVerboseLog(@"[NativeHaptics] CoreHaptics unavailable, falling back to UIKit pulses.");
        }
#endif
        EnsurePulseQueue();
        unsigned token = ++g_pulseToken;
        for (int i = 0; i < length; i++)
        {
            float a = fmaxf(0.0f, fminf(1.0f, ampCopy[i]));
            long at = (long)((double)i * stepDurationSec * 1000.0);
            dispatch_after(dispatch_time(DISPATCH_TIME_NOW, (int64_t)(at * NSEC_PER_MSEC)), g_pulseQueue, ^{
                if (token != g_pulseToken) return;
                dispatch_async(dispatch_get_main_queue(), ^{
                    if (token != g_pulseToken) return;
#if NH_APPLE
                    if (@available(iOS 11.0, *))
                    {
                        UIImpactFeedbackGenerator *gen = s_impactGenerators[1];
                        [gen prepare];
                        NHVerboseLog(@"[NativeHaptics] UIKit pulse triggered.");
                        if (@available(iOS 13.0, *))
                        {
                            [gen impactOccurredWithIntensity:fmaxf(0.1f, a)];
                        }
                        else
                        {
                            [gen impactOccurred];
                        }
                    }
#endif
                });
            });
        }
        free(ampCopy);
    });
}

void NativeHaptics_Cancel(void)
{
    dispatch_async(dispatch_get_main_queue(), ^{
        NHVerboseLog(@"[NativeHaptics] Bridge called: %s", __FUNCTION__);
        EnsurePulseQueue();
        g_pulseToken++;
#if NH_COREHAPTICS
        // Stop only the active player and keep the engine warm for instant next play.
        StopActivePlayer();
#endif
    });
}

} // extern "C"