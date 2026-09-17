using System;
using NativeHaptics;
using NH = NativeHaptics.NativeHaptics;
using UnityEngine;

namespace Lofelt.NiceVibrations
{
    public static class HapticPatterns
    {
        public enum PresetType
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
            None = -1
        }

        public static void PlayPreset(PresetType preset)
        {
            switch (preset)
            {
                case PresetType.Selection: NH.PlayHapticType(HapticType.Selection); break;
                case PresetType.Success: NH.PlayHapticType(HapticType.Success); break;
                case PresetType.Warning: NH.PlayHapticType(HapticType.Warning); break;
                case PresetType.Failure: NH.PlayHapticType(HapticType.Failure); break;
                case PresetType.LightImpact: NH.PlayHapticType(HapticType.LightImpact); break;
                case PresetType.MediumImpact: NH.PlayHapticType(HapticType.MediumImpact); break;
                case PresetType.HeavyImpact: NH.PlayHapticType(HapticType.HeavyImpact); break;
                case PresetType.RigidImpact: NH.PlayHapticType(HapticType.RigidImpact); break;
                case PresetType.SoftImpact: NH.PlayHapticType(HapticType.SoftImpact); break;
                case PresetType.None: default: break;
            }
        }

        public static void PlayEmphasis(float amplitude, float frequency)
        {
            NH.PlayOneShot(0.025f, amplitude);
        }

        public static void PlayConstant(float amplitude, float frequency, float duration)
        {
            NH.PlayConstant(amplitude, frequency, duration);
        }
    }

    public class HapticClip : ScriptableObject
    {
        public string json;
    }

    public static class HapticController
    {
        private static HapticPatterns.PresetType _fallbackPreset = HapticPatterns.PresetType.None;

        public static HapticPatterns.PresetType fallbackPreset
        {
            get { return _fallbackPreset; }
            set { _fallbackPreset = value; }
        }

        public static bool hapticsEnabled
        {
            get { return NH.HapticsEnabled; }
            set { NH.HapticsEnabled = value; }
        }

        public static float outputLevel
        {
            get { return NH.OutputLevel; }
            set { NH.OutputLevel = value; }
        }

        public static float clipLevel = 1.0f;

        public static float clipFrequencyShift = 0.0f;

        public static void Load(HapticClip clip) { }

        public static bool IsLoaded() { return false; }

        public static void Play(HapticClip clip) { }

        public static void Stop() { NH.Cancel(); }

        public static void Seek(float time) { }

        public static void Loop(bool loop) { }

        public static void Reset() { NH.Cancel(); }

        public static event Action playbackFinished;
    }

    public static class DeviceCapabilities
    {
        public static int version
        {
            get
            {
#if UNITY_IOS && !UNITY_EDITOR
                return 130000;
#elif UNITY_ANDROID && !UNITY_EDITOR
                return 260000;
#else
                return 0;
#endif
            }
        }

        public static bool isVersionSupported
        {
            get { return version > 0; }
        }

        public static bool meetsAdvancedRequirements
        {
            get { return NH.IsSupported(); }
        }
    }
}