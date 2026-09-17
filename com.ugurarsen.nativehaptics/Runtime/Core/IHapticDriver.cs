namespace NativeHaptics.Core
{
    public interface IHapticDriver
    {
        bool IsSupported();
        void PlayHapticType(HapticType type, float outputScale);
        void PlayOneShot(float duration, float intensity);
        void PlayAmplitudeEnvelope(float duration, float amplitude, float frequency);
        void PlayEnvelope(float[] amplitudeSteps, float stepDurationMs);
        void PlayPattern(long[] timings, int[] amplitudes);
        void Cancel();
    }
}
