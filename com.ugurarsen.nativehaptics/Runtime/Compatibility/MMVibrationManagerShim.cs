using System;
using NativeHaptics;
using NH = NativeHaptics.NativeHaptics;
using UnityEngine;

namespace MoreMountains.NiceVibrations
{
    public enum HapticTypes
    {
        Selection = 0,
        Success = 1,
        Warning = 2,
        Failure = 3,
        LightImpact = 4,
        MediumImpact = 5,
        HeavyImpact = 6,
        RigidImpact = 7,
        SoftImpact = 8,
        None = 9
    }

    public static class MMVibrationManager
    {
        public static bool HapticsActive = true;

        public static void Haptic(HapticTypes type, bool withRumble = false, MonoBehaviour coroutineSupport = null, int controllerId = -1)
        {
            if (!HapticsActive) return;

            switch (type)
            {
                case HapticTypes.Selection: NH.PlayHapticType(HapticType.Selection); break;
                case HapticTypes.Success: NH.PlayHapticType(HapticType.Success); break;
                case HapticTypes.Warning: NH.PlayHapticType(HapticType.Warning); break;
                case HapticTypes.Failure: NH.PlayHapticType(HapticType.Failure); break;
                case HapticTypes.LightImpact: NH.PlayHapticType(HapticType.LightImpact); break;
                case HapticTypes.MediumImpact: NH.PlayHapticType(HapticType.MediumImpact); break;
                case HapticTypes.HeavyImpact: NH.PlayHapticType(HapticType.HeavyImpact); break;
                case HapticTypes.RigidImpact: NH.PlayHapticType(HapticType.RigidImpact); break;
                case HapticTypes.SoftImpact: NH.PlayHapticType(HapticType.SoftImpact); break;
                case HapticTypes.None: default: break;
            }
        }

        public static void Vibrate()
        {
            if (HapticsActive) NH.PlayHapticType(HapticType.MediumImpact);
        }

        public static void Vibrate(float duration)
        {
            if (HapticsActive) NH.PlayOneShot(duration, 0.5f);
        }

        public static void SetHapticsActive(bool active)
        {
            HapticsActive = active;
            NH.HapticsEnabled = active;
        }

        public static void StopAllHaptics(bool showOnlyAndroidWarning = false)
        {
            NH.Cancel();
        }

        public static void StopContinuousHaptic(bool showOnlyAndroidWarning = false)
        {
            NH.Cancel();
        }

        public static void TransientHaptic(bool rumble, float intensity, float sharpness, bool ahapStopsOnCondition = false, float conditionThreshold = 0f, float conditionThresholdTime = 0f, bool conditionsRepeat = false, bool conditionsResetAfterMaximumTime = false, float maximumTime = 0f, float minimumInvokeTimeAfterImpact = 0f, int controllerId = -1, MonoBehaviour coroutineSupport = null, bool threaded = false)
        {
            if (HapticsActive) NH.PlayOneShot(0.05f, intensity);
        }

        public static void TransientHaptic(float intensity, float sharpness, bool withRumble = false, MonoBehaviour coroutineSupport = null, int controllerId = -1, bool threaded = false)
        {
            if (HapticsActive) NH.PlayOneShot(0.05f, intensity);
        }

        public static void ContinuousHaptic(float intensity, float sharpness, float duration, HapticTypes fallbackPreset = HapticTypes.None, MonoBehaviour coroutineSupport = null, bool withRumble = false, int controllerId = -1, bool threaded = false)
        {
            if (HapticsActive) NH.PlayOneShot(duration, intensity);
        }

        public static void UpdateContinuousHaptic(float intensity, float sharpness, bool threaded = false) { }

        public static void StopContinuousHaptic() { NH.Cancel(); }

        public static void AndroidVibrate(long milliseconds)
        {
            if (HapticsActive) NH.PlayOneShot(milliseconds / 1000f, 0.5f);
        }

        public static void AndroidVibrate(long milliseconds, int amplitude)
        {
            if (!HapticsActive) return;
            NH.PlayOneShot(milliseconds / 1000f, amplitude / 255f);
        }

        public static void AndroidVibrate(long[] pattern, int repeat)
        {
            if (HapticsActive) NH.PlayPattern(pattern, null);
        }

        public static void AndroidVibrate(long[] pattern, int[] amplitudes, int repeat)
        {
            if (HapticsActive) NH.PlayPattern(pattern, amplitudes);
        }
    }
}