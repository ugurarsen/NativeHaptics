using System;
using System.Diagnostics;
using NativeHaptics.Core;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace NativeHaptics
{
    public static class NativeHaptics
    {
        private static IHapticDriver _driver;

        private static bool _hapticsEnabled = true;
        private static float _outputLevel = 1.0f;

        // Transient-tap debounce: drop duplicate calls within 25ms to prevent a
        // 50+ calls-per-frame spam that can lock the hardware.
        private static long _lastPlayTimestamp;
        private static readonly long ThrottleTicks = Stopwatch.Frequency / 40L;

        private static bool PassesThrottle()
        {
            long now = Stopwatch.GetTimestamp();
            if (now - _lastPlayTimestamp < ThrottleTicks) return false;
            _lastPlayTimestamp = now;
            return true;
        }

        public static bool HapticsEnabled
        {
            get { return _hapticsEnabled; }
            set
            {
                if (_hapticsEnabled && !value) Cancel();
                _hapticsEnabled = value;
            }
        }

        public static float OutputLevel
        {
            get { return _outputLevel; }
            set { _outputLevel = Mathf.Max(0f, value); }
        }

        public static IHapticDriver Driver
        {
            get
            {
                if (_driver == null) _driver = CreateDriver();
                return _driver;
            }
        }

        public static bool IsSupported()
        {
            return Driver.IsSupported();
        }

        public static void PlayHapticType(HapticType type)
        {
            if (!_hapticsEnabled || type == HapticType.None) return;
            if (!PassesThrottle()) return;
            Driver.PlayHapticType(type, _outputLevel);
        }

        public static void PlayOneShot(float duration, float intensity)
        {
            if (!_hapticsEnabled || duration <= 0f) return;
            if (!PassesThrottle()) return;
            float scaled = Mathf.Clamp01(intensity * _outputLevel);
            Driver.PlayOneShot(duration, scaled);
        }

        public static void PlayConstant(float amplitude, float frequency, float duration)
        {
            if (!_hapticsEnabled || duration <= 0f) return;
            int n = Mathf.Max(1, Mathf.RoundToInt(duration * 1000f / EnvelopeStepMs));
            float[] steps = new float[n];
            for (int i = 0; i < n; i++) steps[i] = Mathf.Clamp01(amplitude);
            PlayEnvelope(steps, EnvelopeStepMs);
        }

        public static void PlayEnvelope(float[] amplitudeSteps, float stepDurationMs)
        {
            if (!_hapticsEnabled || amplitudeSteps == null || amplitudeSteps.Length == 0) return;
            if (!PassesThrottle()) return;
            if (stepDurationMs <= 0f) stepDurationMs = EnvelopeStepMs;
            float[] scaled = new float[amplitudeSteps.Length];
            for (int i = 0; i < scaled.Length; i++)
                scaled[i] = Mathf.Clamp01(amplitudeSteps[i] * _outputLevel);
            Driver.Cancel();
            Driver.PlayEnvelope(scaled, stepDurationMs);
        }

        public static void PlayPattern(long[] timings, int[] amplitudes)
        {
            if (!_hapticsEnabled) return;
            if (!PassesThrottle()) return;
            int[] scaled = null;
            if (amplitudes != null)
            {
                scaled = new int[amplitudes.Length];
                for (int i = 0; i < amplitudes.Length; i++)
                    scaled[i] = Mathf.Clamp((int)(amplitudes[i] * _outputLevel), 0, 255);
            }
            Driver.Cancel();
            Driver.PlayPattern(timings, scaled);
        }

        public static void Cancel()
        {
            Driver.Cancel();
        }

        internal const float EnvelopeStepMs = 30f;

        private static IHapticDriver CreateDriver()
        {
            IHapticDriver driver;
#if UNITY_ANDROID && !UNITY_EDITOR
            driver = new AndroidHapticDriver();
#elif UNITY_IOS && !UNITY_EDITOR
            driver = new IOSHapticDriver();
#elif UNITY_EDITOR
            Debug.Log("[NativeHaptics] Editor: haptics are no-ops in the editor.");
            driver = new NullHapticDriver();
#else
            driver = new NullHapticDriver();
#endif
            Debug.Log($"[NativeHaptics] Driver initialized: {driver.GetType().Name}");
            return driver;
        }
    }

    internal class NullHapticDriver : IHapticDriver
    {
        public bool IsSupported() => false;
        public void PlayHapticType(HapticType type, float outputScale) { }
        public void PlayOneShot(float duration, float intensity) { }
        public void PlayAmplitudeEnvelope(float duration, float amplitude, float frequency) { }
        public void PlayEnvelope(float[] amplitudeSteps, float stepDurationMs) { }
        public void PlayPattern(long[] timings, int[] amplitudes) { }
        public void Cancel() { }
    }
}