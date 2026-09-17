using System.Collections;
using UnityEngine;

#if UNITY_ANDROID && !UNITY_EDITOR
namespace NativeHaptics.Core
{
    internal static class EnvelopeScheduler
    {
        private static EnvelopeSchedulerBehaviour _behaviour;

        public static void Play(AndroidHapticDriver driver, float[] amplitudeSteps, float stepDurationMs)
        {
            EnsureBehaviour();
            _behaviour.Play(driver, amplitudeSteps, stepDurationMs);
        }

        public static void Stop()
        {
            if (_behaviour != null) _behaviour.StopEnvelope();
        }

        private static void EnsureBehaviour()
        {
            if (_behaviour != null) return;
            var go = new GameObject("NativeHaptics.EnvelopeScheduler");
            Object.DontDestroyOnLoad(go);
            _behaviour = go.AddComponent<EnvelopeSchedulerBehaviour>();
        }
    }

    internal class EnvelopeSchedulerBehaviour : MonoBehaviour
    {
        private Coroutine _current;
        private AndroidHapticDriver _driver;

        public void Play(AndroidHapticDriver driver, float[] amplitudeSteps, float stepDurationMs)
        {
            StopEnvelope();
            _driver = driver;
            _current = StartCoroutine(Run(amplitudeSteps, stepDurationMs));
        }

        public void StopEnvelope()
        {
            if (_current != null)
            {
                StopCoroutine(_current);
                _current = null;
            }
            if (_driver != null)
            {
                _driver.CancelImmediate();
                _driver = null;
            }
        }

        private IEnumerator Run(float[] steps, float stepDurationMs)
        {
            long stepMs = (long)Mathf.Max(1f, stepDurationMs * 1000f);
            long overlapMs = (long)Mathf.Max(stepDurationMs * 1000f * 1.175f, stepMs);
            WaitForSecondsRealtime wait = new WaitForSecondsRealtime(stepDurationMs * 0.001f);

            for (int i = 0; i < steps.Length; i++)
            {
                if (_current == null || _driver == null) yield break;
                int amp = Mathf.Clamp(Mathf.RoundToInt(steps[i] * 255f), 1, 255);
                _driver.VibrateRaw(overlapMs, amp);
                if (i < steps.Length - 1)
                    yield return wait;
            }

            _driver = null;
        }
    }
}
#endif