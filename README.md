# Native Haptics

[![openupm](https://img.shields.io/npm/v/com.ugurarsen.nativehaptics?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.ugurarsen.nativehaptics/)
[![License: LGPL v3](https://img.shields.io/badge/License-LGPL_v3-blue.svg)](LICENSE.md)
[![Unity](https://img.shields.io/badge/Unity-2019.4%2B-blue.svg)](https://unity.com/)

**Modern, zero-dependency native haptics engine for Unity.**

A lightweight, full-featured drop-in replacement for the abandoned NiceVibrations asset. Uses only public operating-system APIs — **no native `.so` / `.a` binaries**.

## Platform Support

- **Android:** `VibrationEffect` on API 26+, automatic fallback on older devices.
- **iOS:** `UIImpactFeedbackGenerator`, `UISelectionFeedbackGenerator`, `UINotificationFeedbackGenerator`, plus CoreHaptics on iOS 13+.
- **Unity Editor / WebGL / Standalone:** Safe no-op.

## Installation

### OpenUPM — Recommended

```bash
openupm add com.ugurarsen.nativehaptics
```

### Unity Package Manager — Git URL

**Window → Package Manager → + → Add package from git URL...**

```text
https://github.com/ugurarsen/NativeHaptics.git?path=com.ugurarsen.nativehaptics
```

### Manifest.json

```json
{
  "dependencies": {
    "com.ugurarsen.nativehaptics": "https://github.com/ugurarsen/NativeHaptics.git?path=com.ugurarsen.nativehaptics"
  }
}
```

## Quick Start

```csharp
using NativeHaptics;

NativeHaptics.PlayHapticType(HapticType.Success);
```

## Features

| Feature | Support |
| :------ | :------ |
| Selection / Success / Warning / Failure presets | iOS + Android |
| Impact presets (Light/Medium/Heavy/Rigid/Soft) | iOS + Android |
| One-shot burst | iOS + Android |
| Continuous / constant feedback | iOS (CoreHaptics) + Android (sustained) |
| Amplitude envelope (curves) | iOS (CoreHaptics curve) + Android (stepped renderer) |
| Custom waveform patterns | iOS (CoreHaptics) + Android API 26+ |
| Global enable / mute | All platforms |
| Global output multiplier | All platforms |
| Android API < 26 fallback | Yes |
| Automatic `VIBRATE` manifest injection | Yes |
| NiceVibrations compatibility layer | Yes |
| Native `.so` / `.a` binaries | **None** |

## Repository Structure

```text
NativeHaptics/
└── com.ugurarsen.nativehaptics/   # UPM package
    ├── Editor/                     # Android manifest injection
    ├── Runtime/                    # Drivers, envelope scheduler, shims
    ├── Samples~/                   # Demo scene
    ├── package.json
    └── README.md                   # Full package documentation
```

## Documentation

Full API documentation, compatibility layers (Lofelt / MoreMountains NiceVibrations), migration guide and code samples are in the [package README](com.ugurarsen.nativehaptics/README.md).

## License

LGPL v3 — see [LICENSE.md](com.ugurarsen.nativehaptics/LICENSE.md).