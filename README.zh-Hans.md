# Native Haptics（简体中文）

🌐 **语言:** [English](README.md) · [Türkçe](README.tr.md) · [Français](README.fr.md) · [Español](README.es.md) · 简体中文

![封面图](Documentation~/cover.png)

**面向 Unity 的现代、零依赖原生触觉反馈引擎。**

`com.ugurarsen.nativehaptics` 是已废弃的 NiceVibrations 资源包的轻量级、功能完整的直接替代方案。

它仅使用公开的操作系统 API，**不包含任何原生 `.so` / `.a` 二进制文件**，从而避免了对 `liblofelt_sdk.so` 等旧版原生二进制文件的依赖。

## 平台支持

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

> 本包采用**扁平（flat）UPM 结构**：`package.json` 位于仓库根目录，因此安装地址**不需要**添加 `?path=` 片段。

---

## 快速开始

```csharp
using NativeHaptics;

NativeHaptics.PlayHapticType(HapticType.Success);
```

就这么简单。详细的使用方式、兼容层说明和平台注意事项，请参见包内文档（Documentation~）。

---

## 包含内容

| 功能 | 支持情况 |
|---|---|
| 预设触觉效果（Selection、Success、Warning、Failure、Impacts） | iOS + Android |
| 一次性脉冲（`PlayOneShot`） | iOS + Android |
| 恒定振动（`PlayConstant`） | iOS + Android |
| 振幅包络（`PlayEnvelope`） | iOS + Android |
| 自定义波形（`PlayPattern`） | iOS + Android |
| 全局启用 / 静音（`HapticsEnabled`） | 全平台 |
| 全局输出等级（`OutputLevel`） | 全平台 |
| 取消（`Cancel`） | 全平台 |
| NiceVibrations 兼容层 | 支持 |
| MoreMountains.NiceVibrations 兼容层 | 支持 |
| 原生 `.so` / `.a` 二进制文件 | **无** |
| Android API < 26 回退支持 | 支持 |
| Editor / WebGL / Standalone 空操作 | 支持 |

---

## 文档

如需详细的 API 参考、使用示例、平台行为说明、迁移指南以及兼容层文档，请打开 Unity Package Manager 卡片中的 **View documentation** 按钮，或浏览：

[Documentation~/index.zh-Hands.md](Documentation~/index.zh-Hands.md)

---

## 许可证

LGPL v3 — 详见 [LICENSE.md](LICENSE.md)。
