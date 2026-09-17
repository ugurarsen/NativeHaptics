using Lofelt.NiceVibrations;
using MoreMountains.NiceVibrations;
using NativeHaptics;
using NH = NativeHaptics.NativeHaptics;
using UnityEngine;
using UnityEngine.UI;

namespace NativeHaptics.Demo
{
    public class NativeHapticsDemo : MonoBehaviour
    {
        public Button modernSuccessButton;
        public Button modernOneShotButton;
        public Button compatPresetButton;
        public Button compatHapticButton;
        public Slider intensitySlider;
        public Text statusText;

        void Start()
        {
            modernSuccessButton.onClick.AddListener(OnModernSuccess);
            modernOneShotButton.onClick.AddListener(OnModernOneShot);
            compatPresetButton.onClick.AddListener(OnCompatPreset);
            compatHapticButton.onClick.AddListener(OnCompatHaptic);

            string status = $"Modern API supports haptics: {NH.IsSupported()}";
            status += $"\nDeviceCapabilities.meetsAdvancedRequirements: {DeviceCapabilities.meetsAdvancedRequirements}";
            if (intensitySlider != null) status += $"\nIntensity: {intensitySlider.value:F2}";
            if (statusText != null) statusText.text = status;
        }

        void OnModernSuccess()
        {
            NH.PlayHapticType(HapticType.Success);
        }

        void OnModernOneShot()
        {
            float intensity = intensitySlider != null ? intensitySlider.value : 1.0f;
            NH.PlayOneShot(0.1f, intensity);
        }

        void OnCompatPreset()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Warning);
        }

        void OnCompatHaptic()
        {
            MMVibrationManager.Haptic(HapticTypes.Failure);
        }
    }
}