using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace NativeHaptics.Core
{
    public class IOSHapticDriver : IHapticDriver
    {
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern bool NativeHaptics_IsSupported();

        [DllImport("__Internal")]
        private static extern void NativeHaptics_PlayImpact(int style, float intensity);

        [DllImport("__Internal")]
        private static extern void NativeHaptics_PlayNotification(int type, float intensity);

        [DllImport("__Internal")]
        private static extern void NativeHaptics_PlaySelection(float intensity);

        [DllImport("__Internal")]
        private static extern void NativeHaptics_PlayOneShot(float duration, float amplitude);

        [DllImport("__Internal")]
        private static extern void NativeHaptics_PlayPattern([In] long[] timings, [In] int[] amplitudes, int length);

        [DllImport("__Internal")]
        private static extern void NativeHaptics_PlayEnvelope([In] float[] amplitudeSteps, float stepDurationSec, int length);

        [DllImport("__Internal")]
        private static extern void NativeHaptics_Cancel();

        public bool IsSupported()
        {
            return NativeHaptics_IsSupported();
        }

        public void PlayHapticType(HapticType type, float outputScale)
        {
            outputScale = Mathf.Clamp01(outputScale);

            switch (type)
            {
                case HapticType.Selection:
                    NativeHaptics_PlaySelection(outputScale);
                    break;
                case HapticType.Success:
                    NativeHaptics_PlayNotification(1, outputScale);
                    break;
                case HapticType.Warning:
                    NativeHaptics_PlayNotification(2, outputScale);
                    break;
                case HapticType.Failure:
                    NativeHaptics_PlayNotification(3, outputScale);
                    break;
                case HapticType.LightImpact:
                    NativeHaptics_PlayImpact(0, outputScale);
                    break;
                case HapticType.MediumImpact:
                    NativeHaptics_PlayImpact(1, outputScale);
                    break;
                case HapticType.HeavyImpact:
                    NativeHaptics_PlayImpact(2, outputScale);
                    break;
                case HapticType.RigidImpact:
                    NativeHaptics_PlayImpact(3, outputScale);
                    break;
                case HapticType.SoftImpact:
                    NativeHaptics_PlayImpact(4, outputScale);
                    break;
                case HapticType.None:
                default:
                    break;
            }
        }

        public void PlayOneShot(float duration, float intensity)
        {
            NativeHaptics_PlayOneShot(duration, Mathf.Clamp01(intensity));
        }

        public void PlayAmplitudeEnvelope(float duration, float amplitude, float frequency)
        {
            PlayEnvelope(BuildFlatEnvelope(duration, amplitude), 30f);
        }

        public void PlayEnvelope(float[] amplitudeSteps, float stepDurationMs)
        {
            if (amplitudeSteps == null || amplitudeSteps.Length == 0) return;
            if (stepDurationMs <= 0f) stepDurationMs = 30f;
            float[] safe = new float[amplitudeSteps.Length];
            for (int i = 0; i < safe.Length; i++)
                safe[i] = Mathf.Clamp01(amplitudeSteps[i]);
            NativeHaptics_PlayEnvelope(safe, stepDurationMs * 0.001f, safe.Length);
        }

        public void PlayPattern(long[] timings, int[] amplitudes)
        {
            if (timings == null || timings.Length == 0) return;
            if (amplitudes != null && amplitudes.Length != timings.Length)
                amplitudes = null;
            NativeHaptics_PlayPattern(timings, amplitudes, timings.Length);
        }

        public void Cancel()
        {
            NativeHaptics_Cancel();
        }

        private static float[] BuildFlatEnvelope(float duration, float amplitude)
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
