# Native Haptics（简体中文）

🌐 **语言:** [English](index.md) · [Türkçe](index.tr.md) · [Français](index.fr.md) · [Español](index.es.md) · 简体中文

![封面图](cover.png)

[![openupm](https://img.shields.io/npm/v/com.ugurarsen.nativehaptics?label=openupm\&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.ugurarsen.nativehaptics/)
[![License: LGPL v3](https://img.shields.io/badge/License-LGPL_v3-blue.svg)](../LICENSE.md)
[![Unity](https://img.shields.io/badge/Unity-2019.4%2B-blue.svg)](https://unity.com/)

**面向 Unity 的现代、零依赖原生触觉反馈引擎。**

`com.ugurarsen.nativehaptics` 是已废弃的 NiceVibrations 资源包的轻量级、功能完整的直接替代方案。

它仅使用公开的操作系统 API，**不包含任何原生 `.so` / `.a` 二进制文件**，从而避免了对 `liblofelt_sdk.so` 等旧版原生二进制文件的依赖。

### 平台支持

* **Android：** 在 API 26+ 上使用 `VibrationEffect`，在旧设备上自动回退。
* **iOS：** `UIImpactFeedbackGenerator`、`UISelectionFeedbackGenerator` 和 `UINotificationFeedbackGenerator`。
* **Unity Editor / WebGL / Standalone：** 安全空操作。

---

## 安装方式

### OpenUPM — 推荐

```bash
openupm add com.ugurarsen.nativehaptics
```

### Unity Package Manager — Git URL

打开：

**Window → Package Manager → + → Add package from git URL...**

然后粘贴：

```text
https://github.com/ugurarsen/NativeHaptics.git
```

### `Packages/manifest.json`

添加：

```json
{
  "dependencies": {
    "com.ugurarsen.nativehaptics": "https://github.com/ugurarsen/NativeHaptics.git"
  }
}
```

---

# 快速开始

添加：

```csharp
using NativeHaptics;
```

然后直接调用任意触觉反馈：

```csharp
NativeHaptics.PlayHapticType(HapticType.Success);
```

就是这么简单。

---

# 常见用法

## 预设效果

复制并使用：

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

### 可用预设

| 预设            | 说明                     |
| :-------------- | :----------------------- |
| `Selection`     | 选择 / 界面反馈           |
| `Success`       | 操作成功                  |
| `Warning`       | 警告反馈                  |
| `Failure`       | 操作失败                  |
| `LightImpact`   | 轻度物理冲击               |
| `MediumImpact`  | 中度物理冲击               |
| `HeavyImpact`   | 重度物理冲击               |
| `RigidImpact`   | 刚性冲击                   |
| `SoftImpact`    | 柔和冲击                   |

---

## 自定义一次性脉冲（One-Shot）

创建一次性的触觉脉冲。

**参数：**

* `duration` — 秒
* `intensity` — `0.0` 到 `1.0`

```csharp
NativeHaptics.PlayOneShot(
    0.1f,   // 时长
    0.8f    // 强度
);
```

---

## 连续触觉反馈

在指定时长内以恒定振幅播放反馈。

**参数：**

* `amplitude` — `0.0` 到 `1.0`
* `frequency` — 预留字段（当前驱动未使用）
* `duration` — 秒

```csharp
NativeHaptics.PlayConstant(
    0.6f,    // 振幅
    0.0f,    // 频率
    1.5f     // 时长
);
```

平台行为：

* **Android（API 26+）：** 在整个时长内持续振动。
* **iOS（13+）：** 在整个时长内持续触发 CoreHaptics 事件。在 iOS 13 以下（或没有 CoreHaptics 引擎的设备）会回退为在指定振幅下的瞬时冲击脉冲。
* **旧版 Android（API < 26）：** 在整个时长内持续振动，忽略振幅参数。

---

## 振幅包络（Amplitude Envelope）

播放随时间变化的振幅曲线。这是与 Lofelt 连续振幅控制最接近的等价功能。

**参数：**

* `amplitudeSteps` — 包络的归一化采样点，范围 `0.0` 到 `1.0`（长度不限）
* `stepDurationMs` — 每个采样点持续的毫秒数

```csharp
// 一条 0.5 秒的“先增强后淡出”曲线：10 个采样点，每个 50 毫秒
NativeHaptics.PlayEnvelope(
    new float[] { 0.0f, 0.4f, 0.8f, 1.0f, 0.9f, 0.7f, 0.5f, 0.3f, 0.15f, 0.0f },
    50f
);
```

平台行为：

* **iOS（13+）：** 由 `CHHapticParameterCurve` 驱动的单个持续 CoreHaptics 事件——平滑的原生渐变、零 GC 开销、无阶梯状伪影。iOS 13 以下会回退为定时脉冲。
* **Android（API 26+）：** 一个实时的分步渲染器（`EnvelopeScheduler`）每隔 `stepDurationMs` 重新下发一次振幅步进，并带有轻微重叠，以保持线性谐振致动器（LRA）持续振动。这模拟了 Lofelt 在 Android 上驱动振幅的方式。
* **旧版 Android（API < 26）：** 振幅保持恒定，总时长以一次长时间的嗡鸣播放。

---

## 自定义波形

创建你自己的振动模式。

**Android API 26+ / iOS 13+**

```csharp
NativeHaptics.PlayPattern(
    new long[] { 0, 35, 60, 45, 80 },
    new int[]  { 0, 255, 0, 180, 100 }
);
```

### 模式格式

`timings` 以毫秒为单位：

```text
{ 延迟, 振动, 暂停, 振动, 暂停, ... }
```

`amplitudes` 使用 Android 的 `0–255` 范围：

```text
0   = 关闭
255 = 最大
```

---

# 全局设置

## 启用 / 禁用触觉反馈

静音所有触觉反馈：

```csharp
NativeHaptics.HapticsEnabled = false;
```

重新启用：

```csharp
NativeHaptics.HapticsEnabled = true;
```

---

## 输出等级

全局缩放**所有**触觉输出的强度——包括预设效果、一次性脉冲、恒定振动和自定义波形：

```csharp
NativeHaptics.OutputLevel = 0.7f;
```

例如：

```text
1.0 = 100%
0.7 = 70%
0.5 = 50%
0.0 = 0%
```

---

## 停止触觉反馈

立即停止任何正在进行的振动：

```csharp
NativeHaptics.Cancel();
```

---

# 功能特性

| 功能                                       | 支持情况                                              |
| :------------------------------------------- | :------------------------------------------------------ |
| Selection / Success / Warning / Failure 预设 | iOS + Android                                            |
| 冲击类预设                                    | iOS + Android                                            |
| 振幅控制                                      | iOS + Android                                            |
| 一次性脉冲                                    | iOS + Android                                            |
| 连续 / 恒定反馈                               | iOS（CoreHaptics）+ Android（持续振动）                  |
| 振幅包络（曲线）                              | iOS（CoreHaptics 曲线）+ Android（分步渲染器）           |
| 自定义波形模式                                | iOS（CoreHaptics）+ Android API 26+                      |
| 全局启用 / 静音                               | 全平台                                                    |
| 全局输出倍增器                                | 全平台                                                    |
| Android API < 26 回退支持                     | 支持                                                       |
| Editor / WebGL / Standalone 安全空操作        | 支持                                                       |
| 自动注入 `VIBRATE` 权限                       | 支持                                                       |
| NiceVibrations 兼容层                        | 支持                                                       |
| 原生 `.so` / `.a` 二进制文件                  | **无**                                                     |

---

# NiceVibrations 兼容性

如果你的项目已经在使用 NiceVibrations，可以继续使用现有的 API。

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

### 连续 / 恒定

```csharp
HapticPatterns.PlayConstant(
    0.8f,   // 振幅
    0.0f,   // 频率
    1.0f    // 时长
);
```

### 控制器

```csharp
HapticController.hapticsEnabled = true;
HapticController.outputLevel = 0.5f;
HapticController.Stop();
```

### 能力检测

```csharp
bool supported = DeviceCapabilities.isVersionSupported;
bool advanced = DeviceCapabilities.meetsAdvancedRequirements;
```

---

# MoreMountains.NiceVibrations 兼容性

现有的 MoreMountains.NiceVibrations 调用同样可以继续工作。

```csharp
using MoreMountains.NiceVibrations;
```

## 预设效果

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

## 直接调用的辅助方法

```csharp
MMVibrationManager.Vibrate();

MMVibrationManager.TransientHaptic(
    0.8f,   // 强度
    0.5f    // 锐度
);

MMVibrationManager.ContinuousHaptic(
    0.6f,   // 强度
    0.5f,   // 锐度
    2.0f    // 时长
);

MMVibrationManager.StopContinuousHaptic();
```

## Android 专用

```csharp
MMVibrationManager.AndroidVibrate(
    150,     // 时长（毫秒）
    200      // 振幅：1–255
);
```

自定义波形：

```csharp
MMVibrationManager.AndroidVibrate(
    new long[] { 0, 40, 50, 40 },
    new int[]  { 0, 255, 0, 150 },
    -1
);
```

## 全局状态

```csharp
MMVibrationManager.SetHapticsActive(false);

MMVibrationManager.StopAllHaptics();
```

---

# 从 NiceVibrations 迁移

如果你正在替换原有的 NiceVibrations 包：

### 1. 移除旧资源包

从项目中删除现有的 NiceVibrations 资源文件夹。

确保同时移除以下旧版原生二进制文件：

```text
liblofelt_sdk.so
```

### 2. 安装 Native Haptics

通过 OpenUPM 或 Unity Package Manager 安装：

```text
com.ugurarsen.nativehaptics
```

### 3. 重新构建

正常重新构建你的 Unity 项目。

使用以下方式的现有调用：

```csharp
HapticPatterns
```

或：

```csharp
MMVibrationManager
```

均可通过兼容层继续正常工作。

---

## 重要提示：`.haptic` 文件

出于兼容性考虑，本包有意不支持解码和播放为已废弃的 Lofelt Studio 创建的专有 `.haptic` JSON 剪辑文件，因为该格式依赖于旧版基于 C++/Rust 的原生 SDK。

对于新项目，请使用：

* 内置预设效果
* `PlayOneShot`
* `PlayConstant`
* `PlayPattern`
* Android 波形 API

---

# API 速查表

| 你想要的效果            | 使用方法                                    |
| :------------------------ | :-------------------------------------------- |
| 选择反馈                    | `PlayHapticType(HapticType.Selection)`    |
| 成功反馈                    | `PlayHapticType(HapticType.Success)`      |
| 警告反馈                    | `PlayHapticType(HapticType.Warning)`      |
| 失败反馈                    | `PlayHapticType(HapticType.Failure)`      |
| 冲击反馈                    | `PlayHapticType(HapticType.MediumImpact)` |
| 自定义脉冲                   | `PlayOneShot()`                           |
| 连续振动                    | `PlayConstant()`                          |
| 振幅包络                    | `PlayEnvelope()`                          |
| 自定义波形                   | `PlayPattern()`                           |
| 禁用触觉反馈                 | `HapticsEnabled = false`                  |
| 更改全局强度                 | `OutputLevel = 0.7f`                      |
| 停止振动                    | `Cancel()`                                |

---

# 为什么选择 Native Haptics？

* 零原生 SDK 二进制文件
* 无 `.so` / `.a` 依赖
* 公开的 Android 和 iOS API
* 轻量级 Unity 集成
* 自动 Android 回退
* NiceVibrations 兼容性
* 全局强度控制
* 自定义 Android 波形
* 在不支持的平台上安全空操作

---

## 许可证

LGPL v3 — 详见 [LICENSE.md](../LICENSE.md)。
