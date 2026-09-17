using UnityEngine;

namespace NativeHaptics.Core
{
    public class AndroidHapticDriver : IHapticDriver
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        private AndroidJavaObject _vibrator;
        private AndroidJavaObject _effectClass;
        private int _sdkVersion;

        public AndroidHapticDriver()
        {
            using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                _vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");
            }

            using (var versionClass = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                _sdkVersion = versionClass.GetStatic<int>("SDK_INT");
            }
        }

        public bool IsSupported()
        {
            return _vibrator != null && _vibrator.Call<bool>("hasVibrator");
        }

        private void EnsureEffectClass()
        {
            if (_sdkVersion >= 26 && _effectClass == null)
                _effectClass = new AndroidJavaClass("android.os.VibrationEffect");
        }

        public void PlayHapticType(HapticType type, float outputScale)
        {
            if (!IsSupported()) return;
            outputScale = Mathf.Clamp01(outputScale);

            switch (type)
            {
                case HapticType.Selection:
                    PlayOneShot(0.008f, 0.5f * outputScale);
                    break;
                case HapticType.Success:
                    PlayPattern(new long[] { 0, 10, 22, 12 }, new int[] { 0, 150, 0, 255 });
                    break;
                case HapticType.Warning:
                    PlayPattern(new long[] { 0, 10, 15, 10 }, new int[] { 0, 200, 0, 150 });
                    break;
                case HapticType.Failure:
                    PlayPattern(new long[] { 0, 8, 12, 8, 12, 10 }, new int[] { 0, 220, 0, 180, 0, 120 });
                    break;
                case HapticType.LightImpact:
                    PlayOneShot(0.008f, 0.4f * outputScale);
                    break;
                case HapticType.MediumImpact:
                    PlayOneShot(0.012f, 0.65f * outputScale);
                    break;
                case HapticType.HeavyImpact:
                    PlayOneShot(0.020f, 1.0f * outputScale);
                    break;
                case HapticType.RigidImpact:
                    PlayOneShot(0.008f, 1.0f * outputScale);
                    break;
                case HapticType.SoftImpact:
                    PlayOneShot(0.015f, 0.45f * outputScale);
                    break;
                case HapticType.None:
                default:
                    break;
            }
        }

        public void PlayOneShot(float duration, float intensity)
        {
            if (!IsSupported()) return;

            long ms = (long)(duration * 1000f);
            if (ms < 1L) ms = 1L;
            int amp = Mathf.Clamp((int)(intensity * 255f), 1, 255);

            if (_sdkVersion >= 26)
            {
                EnsureEffectClass();
                using (var effect = _effectClass.CallStatic<AndroidJavaObject>("createOneShot", ms, amp))
                {
                    _vibrator.Call("vibrate", effect);
                }
            }
            else
            {
                _vibrator.Call("vibrate", ms);
            }
        }

        public void PlayAmplitudeEnvelope(float duration, float amplitude, float frequency)
        {
            PlayEnvelope(BuildFlatEnvelope(duration, amplitude), 30f);
        }

        public void PlayEnvelope(float[] amplitudeSteps, float stepDurationMs)
        {
            if (!IsSupported()) return;
            if (amplitudeSteps == null || amplitudeSteps.Length == 0) return;
            if (stepDurationMs <= 0f) stepDurationMs = 30f;

            if (_sdkVersion >= 26)
            {
                EnvelopeScheduler.Play(this, amplitudeSteps, stepDurationMs);
            }
            else
            {
                long totalMs = (long)(amplitudeSteps.Length * stepDurationMs);
                if (totalMs < 1L) totalMs = 1L;
                _vibrator.Call("vibrate", totalMs);
            }
        }

        internal void VibrateRaw(long ms, int amp)
        {
            if (_sdkVersion >= 26)
            {
                EnsureEffectClass();
                using (var effect = _effectClass.CallStatic<AndroidJavaObject>("createOneShot", ms, amp))
                {
                    _vibrator.Call("vibrate", effect);
                }
            }
        }

        internal void CancelImmediate()
        {
            if (_vibrator != null)
                _vibrator.Call("cancel");
        }

        public void PlayPattern(long[] timings, int[] amplitudes)
        {
            if (!IsSupported()) return;
            if (timings == null || timings.Length == 0) return;

            if (_sdkVersion >= 26)
            {
                EnsureEffectClass();
                if (amplitudes != null && amplitudes.Length == timings.Length)
                {
                    int[] safeAmplitudes = new int[amplitudes.Length];
                    for (int i = 0; i < amplitudes.Length; i++)
                        safeAmplitudes[i] = Mathf.Clamp(amplitudes[i], 0, 255);

                    using (var effect = _effectClass.CallStatic<AndroidJavaObject>("createWaveform", timings, safeAmplitudes, -1))
                    {
                        _vibrator.Call("vibrate", effect);
                    }
                }
                else
                {
                    using (var effect = _effectClass.CallStatic<AndroidJavaObject>("createWaveform", timings, -1))
                    {
                        _vibrator.Call("vibrate", effect);
                    }
                }
            }
            else
            {
                long totalMs = 0;
                for (int i = 0; i < timings.Length; i++) totalMs += timings[i];
                _vibrator.Call("vibrate", totalMs);
            }
        }

        public void Cancel()
        {
            EnvelopeScheduler.Stop();
            CancelImmediate();
        }

        internal static float[] BuildFlatEnvelope(float duration, float amplitude)
        {
            int n = Mathf.Max(1, Mathf.RoundToInt(duration * 1000f / 30f));
            float[] steps = new float[n];
            for (int i = 0; i < n; i++) steps[i] = Mathf.Clamp01(amplitude);
            return steps;
        }
#else
        public bool IsSupported() => false;
        public void PlayHapticType(HapticType type, float outputScale) { }
        public void PlayOneShot(float duration, float intensity) { }
        public void PlayAmplitudeEnvelope(float duration, float amplitude, float frequency) { }
        public void PlayEnvelope(float[] amplitudeSteps, float stepDurationMs) { }
        public void PlayPattern(long[] timings, int[] amplitudes) { }
        public void Cancel() { }
#endif
    }
}
