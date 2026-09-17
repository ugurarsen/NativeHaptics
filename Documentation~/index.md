# Native Haptics

🌐 **Languages:** English · [Türkçe](index.tr.md) · [Français](index.fr.md) · [Español](index.es.md) · [简体中文](index.zh-Hans.md)

![Cover Image](cover.png)

[![openupm](https://img.shields.io/npm/v/com.ugurarsen.nativehaptics?label=openupm\&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.ugurarsen.nativehaptics/)
[![License: LGPL v3](https://img.shields.io/badge/License-LGPL_v3-blue.svg)](../LICENSE.md)
[![Unity](https://img.shields.io/badge/Unity-2019.4%2B-blue.svg)](https://unity.com/)

**Modern, zero-dependency native haptics engine for Unity.**

`com.ugurarsen.nativehaptics` is a lightweight, full-featured drop-in replacement for the abandoned NiceVibrations asset.

It uses only public operating-system APIs and contains **no native `.so` / `.a` binaries**, avoiding legacy native binary dependencies such as `liblofelt_sdk.so`.

### Platform Support

* **Android:** `VibrationEffect` on API 26+, with automatic fallback on older devices.
* **iOS:** `UIImpactFeedbackGenerator`, `UISelectionFeedbackGenerator`, and `UINotificationFeedbackGenerator`.
* **Unity Editor / WebGL / Standalone:** Safe no-op.

---

## Installation

### OpenUPM — Recommended

```bash
openupm add com.ugurarsen.nativehaptics
```

### Unity Package Manager — Git URL

Open:

**Window → Package Manager → + → Add package from git URL...**

Then paste:

```text
https://github.com/ugurarsen/NativeHaptics.git
```

### `Packages/manifest.json`

Add:

```json
{
  "dependencies": {
    "com.ugurarsen.nativehaptics": "https://github.com/ugurarsen/NativeHaptics.git"
  }
}
```

---

# Quick Start

Add:

```csharp
using NativeHaptics;
```

Then call any haptic directly:

```csharp
NativeHaptics.PlayHapticType(HapticType.Success);
```

That's it.

---

# Common Usage

## Presets

Copy and use:

```csharp
using NativeHaptics;

NativeHaptics.PlayHapticType(HapticType.Selection);
NativeHaptics.PlayHapticType(HapticType.Success);
NativeHaptics.PlayHapticType(HapticType.Warning);
NativeHaptics.PlayHapticType(HapticType.Failure);

NativeHaptics.PlayHapticType(HapticType.LightImpact);
NativeHaptics.PlayHapticType(HapticType.MediumImpact);
NativeHaptics.PlayHapticType(HapticType.HeavyImpact);
NativeHaptics.PlayHapticType(HapticType.RigidImpact);
NativeHaptics.PlayHapticType(HapticType.SoftImpact);
```

### Available Presets

| Preset         | Description             |
| :------------- | :---------------------- |
| `Selection`    | Selection / UI feedback |
| `Success`      | Successful action       |
| `Warning`      | Warning feedback        |
| `Failure`      | Failed action           |
| `LightImpact`  | Light physical impact   |
| `MediumImpact` | Medium physical impact  |
| `HeavyImpact`  | Heavy physical impact   |
| `RigidImpact`  | Rigid impact            |
| `SoftImpact`   | Soft impact             |

---

## Custom One-Shot

Create a single haptic burst.

**Parameters:**

* `duration` — seconds
* `intensity` — `0.0` to `1.0`

```csharp
NativeHaptics.PlayOneShot(
    0.1f,   // duration
    0.8f    // intensity
);
```

---

## Continuous Haptic

Play feedback at a constant amplitude for a specific duration.

**Parameters:**

* `amplitude` — `0.0` to `1.0`
* `frequency` — reserved (not used by the current drivers)
* `duration` — seconds

```csharp
NativeHaptics.PlayConstant(
    0.6f,    // amplitude
    0.0f,    // frequency
    1.5f     // duration
);
```

Platform behavior:

* **Android (API 26+):** sustained vibration for the full duration.
* **iOS (13+):** sustained CoreHaptics event for the full duration. On iOS < 13 (or devices without a CoreHaptics engine) it falls back to transient impact pulses at the given amplitude.
* **Older Android (< API 26):** sustained vibration for the full duration, amplitude ignored.

---

## Amplitude Envelope

Play a time-varying amplitude curve. This is the closest equivalent to Lofelt's
continuous amplitude control.

**Parameters:**

* `amplitudeSteps` — normalized `0.0` to `1.0` samples of the envelope (any length)
* `stepDurationMs` — milliseconds each sample lasts

```csharp
// A 0.5s "swell then fade" curve: 10 samples, 50ms each
NativeHaptics.PlayEnvelope(
    new float[] { 0.0f, 0.4f, 0.8f, 1.0f, 0.9f, 0.7f, 0.5f, 0.3f, 0.15f, 0.0f },
    50f
);
```

Platform behavior:

* **iOS (13+):** single CoreHaptics continuous event driven by a `CHHapticParameterCurve` — smooth native ramp, zero GC, no stepping artifacts. iOS < 13 falls back to timed pulses.
* **Android (API 26+):** a real-time stepped renderer (`EnvelopeScheduler`) re-issues amplitude steps every `stepDurationMs` with slight overlap so the Linear Resonant Actuator stays energized. This mirrors how Lofelt drove Android amplitude.
* **Older Android (< API 26):** amplitude is constant, total duration played as one long buzz.

---

## Custom Waveform

Create your own vibration pattern.

**Android API 26+ / iOS 13+**

```csharp
NativeHaptics.PlayPattern(
    new long[] { 0, 35, 60, 45, 80 },
    new int[]  { 0, 255, 0, 180, 100 }
);
```

### Pattern format

`timings` are milliseconds:

```text
{ delay, vibration, pause, vibration, pause, ... }
```

`amplitudes` use Android's `0–255` range:

```text
0   = off
255 = maximum
```

---

# Global Settings

## Enable / Disable Haptics

Mute all haptic feedback:

```csharp
NativeHaptics.HapticsEnabled = false;
```

Enable again:

```csharp
NativeHaptics.HapticsEnabled = true;
```

---

## Output Level

Scale the intensity of **all** haptic output globally — presets, one-shots, constants and waveforms:

```csharp
NativeHaptics.OutputLevel = 0.7f;
```

For example:

```text
1.0 = 100%
0.7 = 70%
0.5 = 50%
0.0 = 0%
```

---

## Stop Haptics

Immediately stop any active vibration:

```csharp
NativeHaptics.Cancel();
```

---

# Features

| Feature                                         | Support         |
| :---------------------------------------------- | :-------------- |
| Selection / Success / Warning / Failure presets | iOS + Android   |
| Impact presets                                  | iOS + Android   |
| Amplitude control                               | iOS + Android   |
| One-shot burst                                  | iOS + Android   |
| Continuous / constant feedback                  | iOS (CoreHaptics) + Android (sustained) |
| Amplitude envelope (curves)                     | iOS (CoreHaptics curve) + Android (stepped renderer) |
| Custom waveform patterns                        | iOS (CoreHaptics) + Android API 26+ |
| Global enable / mute                            | All platforms   |
| Global output multiplier                        | All platforms   |
| Android API < 26 fallback                       | Yes             |
| Editor / WebGL / Standalone safe no-op          | Yes             |
| Automatic `VIBRATE` manifest injection          | Yes             |
| NiceVibrations compatibility layer              | Yes             |
| Native `.so` / `.a` binaries                    | **None**        |

---

# NiceVibrations Compatibility

If your project already uses NiceVibrations, you can continue using the existing API.

## Lofelt.NiceVibrations

```csharp
using Lofelt.NiceVibrations;

HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
HapticPatterns.PlayPreset(HapticPatterns.PresetType.Warning);
HapticPatterns.PlayPreset(HapticPatterns.PresetType.Failure);

HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);
HapticPatterns.PlayPreset(HapticPatterns.PresetType.RigidImpact);
HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);
```

### Continuous / Constant

```csharp
HapticPatterns.PlayConstant(
    0.8f,   // amplitude
    0.0f,   // frequency
    1.0f    // duration
);
```

### Controller

```csharp
HapticController.hapticsEnabled = true;
HapticController.outputLevel = 0.5f;
HapticController.Stop();
```

### Capability Checks

```csharp
bool supported = DeviceCapabilities.isVersionSupported;
bool advanced = DeviceCapabilities.meetsAdvancedRequirements;
```

---

# MoreMountains.NiceVibrations Compatibility

Existing MoreMountains.NiceVibrations calls can also continue to work.

```csharp
using MoreMountains.NiceVibrations;
```

## Presets

```csharp
MMVibrationManager.Haptic(HapticTypes.Selection);
MMVibrationManager.Haptic(HapticTypes.Success);
MMVibrationManager.Haptic(HapticTypes.Warning);
MMVibrationManager.Haptic(HapticTypes.Failure);

MMVibrationManager.Haptic(HapticTypes.LightImpact);
MMVibrationManager.Haptic(HapticTypes.MediumImpact);
MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
MMVibrationManager.Haptic(HapticTypes.RigidImpact);
MMVibrationManager.Haptic(HapticTypes.SoftImpact);
```

## Direct Helpers

```csharp
MMVibrationManager.Vibrate();

MMVibrationManager.TransientHaptic(
    0.8f,   // intensity
    0.5f    // sharpness
);

MMVibrationManager.ContinuousHaptic(
    0.6f,   // intensity
    0.5f,   // sharpness
    2.0f    // duration
);

MMVibrationManager.StopContinuousHaptic();
```

## Android-Specific

```csharp
MMVibrationManager.AndroidVibrate(
    150,     // duration in ms
    200      // amplitude: 1–255
);
```

Custom waveform:

```csharp
MMVibrationManager.AndroidVibrate(
    new long[] { 0, 40, 50, 40 },
    new int[]  { 0, 255, 0, 150 },
    -1
);
```

## Global State

```csharp
MMVibrationManager.SetHapticsActive(false);

MMVibrationManager.StopAllHaptics();
```

---

# Migration from NiceVibrations

If you are replacing the original NiceVibrations package:

### 1. Remove the old asset

Delete the existing NiceVibrations asset folder from your project.

Make sure legacy native binaries such as:

```text
liblofelt_sdk.so
```

are also removed.

### 2. Install Native Haptics

Install:

```text
com.ugurarsen.nativehaptics
```

using OpenUPM or the Unity Package Manager.

### 3. Rebuild

Rebuild your Unity project normally.

Existing calls using:

```csharp
HapticPatterns
```

or:

```csharp
MMVibrationManager
```

can continue to work through the compatibility layers.

---

## Important: `.haptic` Files

Decoding and playback of the proprietary `.haptic` JSON clips created for the deprecated Lofelt Studio are intentionally not included, as that format was driven by the legacy C++ Rust-based native SDK.

For new projects, use:

* Built-in presets
* `PlayOneShot`
* `PlayConstant`
* `PlayPattern`
* Android waveform APIs

---

# API Cheat Sheet

| What you want           | Use                                       |
| :---------------------- | :----------------------------------------- |
| Selection feedback      | `PlayHapticType(HapticType.Selection)`    |
| Success feedback        | `PlayHapticType(HapticType.Success)`      |
| Warning feedback        | `PlayHapticType(HapticType.Warning)`      |
| Failure feedback        | `PlayHapticType(HapticType.Failure)`      |
| Impact feedback         | `PlayHapticType(HapticType.MediumImpact)` |
| Custom burst            | `PlayOneShot()`                           |
| Continuous vibration    | `PlayConstant()`                          |
| Amplitude envelope      | `PlayEnvelope()`                          |
| Custom waveform         | `PlayPattern()`                           |
| Disable haptics         | `HapticsEnabled = false`                  |
| Change global intensity | `OutputLevel = 0.7f`                      |
| Stop vibration          | `Cancel()`                                |

---

# Why Native Haptics?

* Zero native SDK binaries
* No `.so` / `.a` dependencies
* Public Android and iOS APIs
* Lightweight Unity integration
* Automatic Android fallback
* NiceVibrations compatibility
* Global intensity control
* Custom Android waveforms
* Safe no-op outside supported platforms

---

## License

LGPL v3 — see [LICENSE.md](../LICENSE.md).
