🌐 **Languages:** English · [Türkçe](README.tr.md) · [Français](README.fr.md) · [Español](README.es.md) · [简体中文](README.zh-Hans.md)

# Native Haptics

![Cover Image](Documentation~/cover.png)

[![openupm](https://img.shields.io/npm/v/com.ugurarsen.nativehaptics?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.ugurarsen.nativehaptics/)
[![License: LGPL v3](https://img.shields.io/badge/License-LGPL_v3-blue.svg)](LICENSE.md)
[![Unity](https://img.shields.io/badge/Unity-2019.4+-blue.svg)](https://unity.com/)

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

> This package uses **flat UPM layout**: `package.json` is at the repo root,
> so the install URL does **not** need a `?path=` fragment.

---

## Quick Start

```csharp
using NativeHaptics;

NativeHaptics.PlayHapticType(HapticType.Success);
```

That's it. See the in-package documentation (Documentation~) for detailed usage patterns, compatibility layers, and platform notes.

---

## What's Inside

| Feature | Support |
|---|---|
| Preset haptics (Selection, Success, Warning, Failure, Impacts) | iOS + Android |
| One-shot burst (`PlayOneShot`) | iOS + Android |
| Constant vibration (`PlayConstant`) | iOS + Android |
| Amplitude envelope (`PlayEnvelope`) | iOS + Android |
| Custom waveform (`PlayPattern`) | iOS + Android |
| Global enable / mute (`HapticsEnabled`) | All |
| Global output level (`OutputLevel`) | All |
| Cancel (`Cancel`) | All |
| NiceVibrations compatibility shim | Yes |
| MoreMountains.NiceVibrations compatibility shim | Yes |
| Native `.so` / `.a` binaries | **None** |
| Android API < 26 fallback | Yes |
| Editor / WebGL / Standalone no-op | Yes |

---

## Documentation

For detailed API reference, usage examples, platform behavior notes, migration guide, and compatibility layer documentation, open the **View documentation** button in the Unity Package Manager card or browse:

[Documentation~/index.md](Documentation~/index.md)

---

## License

LGPL v3 — see [LICENSE.md](LICENSE.md).
