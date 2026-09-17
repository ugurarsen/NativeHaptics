# Native Haptics

![Cover Image](Documentation~/cover.png)

[ English | [Türkçe](#türkçe) | [Français](#français) | [Español](#español) | [简体中文](#简体中文) ]

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

> This package uses **flat UPM layout**: `package.json` is at the repo root, so the install URL does **not** need a `?path=` fragment.

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

<br>

---

<a id="türkçe"></a>
## Türkçe

![Kapak Görseli](Documentation~/cover.png)

**Unity için modern, sıfır bağımlılıklı native haptics (dokunsal geri bildirim) motoru.**

`com.ugurarsen.nativehaptics`, artık geliştirilmeyen NiceVibrations paketinin yerine geçebilecek, hafif ve tam özellikli bir pakettir.

Yalnızca herkese açık işletim sistemi API'lerini kullanır ve **hiçbir native `.so` / `.a` ikili dosyası içermez**; bu sayede `liblofelt_sdk.so` gibi eski native ikili dosya bağımlılıklarından kaçınır.

### Platform Desteği

* **Android:** API 26+ üzerinde `VibrationEffect`, eski cihazlarda otomatik geri düşüş (fallback) ile.
* **iOS:** `UIImpactFeedbackGenerator`, `UISelectionFeedbackGenerator` ve `UINotificationFeedbackGenerator`.
* **Unity Editor / WebGL / Standalone:** Güvenli no-op (hiçbir şey yapmaz).

---

### Kurulum

#### OpenUPM — Önerilen

```bash
openupm add com.ugurarsen.nativehaptics
```

#### Unity Package Manager — Git URL

Şunu açın:

**Window → Package Manager → + → Add package from git URL...**

Ardından şunu yapıştırın:

```text
https://github.com/ugurarsen/NativeHaptics.git
```

#### `Packages/manifest.json`

Şunu ekleyin:

```json
{
  "dependencies": {
    "com.ugurarsen.nativehaptics": "https://github.com/ugurarsen/NativeHaptics.git"
  }
}
```

> Bu paket **düz (flat) UPM yapısı** kullanır: `package.json` deponun kök dizinindedir, bu yüzden kurulum adresine `?path=` eki eklemeye gerek yoktur.

---

### Hızlı Başlangıç

```csharp
using NativeHaptics;

NativeHaptics.PlayHapticType(HapticType.Success);
```

Bu kadar. Ayrıntılı kullanım örnekleri, uyumluluk katmanları ve platforma özel notlar için paket içi dokümantasyona (Documentation~) bakın.

---

### Neler Var?

| Özellik | Destek |
|---|---|
| Hazır haptik şablonlar (Selection, Success, Warning, Failure, Impacts) | iOS + Android |
| Tek seferlik burst (`PlayOneShot`) | iOS + Android |
| Sabit titreşim (`PlayConstant`) | iOS + Android |
| Genlik zarfı / envelope (`PlayEnvelope`) | iOS + Android |
| Özel dalga formu (`PlayPattern`) | iOS + Android |
| Genel etkinleştirme / sessize alma (`HapticsEnabled`) | Tümü |
| Genel çıkış seviyesi (`OutputLevel`) | Tümü |
| İptal (`Cancel`) | Tümü |
| NiceVibrations uyumluluk katmanı | Var |
| MoreMountains.NiceVibrations uyumluluk katmanı | Var |
| Native `.so` / `.a` ikili dosyaları | **Yok** |
| Android API < 26 geri düşüş | Var |
| Editor / WebGL / Standalone no-op | Var |

---

### Dokümantasyon

Ayrıntılı API referansı, kullanım örnekleri, platform davranış notları, geçiş (migration) rehberi ve uyumluluk katmanı dokümantasyonu için Unity Package Manager kartındaki **View documentation** düğmesini açın veya şuraya göz atın:

[Documentation~/index.md](Documentation~/index.md)

---

### Lisans

LGPL v3 — bkz. [LICENSE.md](LICENSE.md).

<br>

---

<a id="français"></a>
## Français

![Image de couverture](Documentation~/cover.png)

**Moteur de retour haptique natif moderne et sans dépendance pour Unity.**

`com.ugurarsen.nativehaptics` est un remplacement léger et complet pour le package NiceVibrations, désormais abandonné.

Il n'utilise que des API publiques du système d'exploitation et ne contient **aucun binaire natif `.so` / `.a`**, évitant ainsi les dépendances binaires natives obsolètes telles que `liblofelt_sdk.so`.

### Compatibilité des plateformes

* **Android :** `VibrationEffect` sur API 26+, avec repli automatique sur les appareils plus anciens.
* **iOS :** `UIImpactFeedbackGenerator`, `UISelectionFeedbackGenerator` et `UINotificationFeedbackGenerator`.
* **Unity Editor / WebGL / Standalone :** No-op sécurisé.

---

### Installation

#### OpenUPM — Recommandé

```bash
openupm add com.ugurarsen.nativehaptics
```

#### Unity Package Manager — URL Git

Ouvrez :

**Window → Package Manager → + → Add package from git URL...**

Puis collez :

```text
https://github.com/ugurarsen/NativeHaptics.git
```

#### `Packages/manifest.json`

Ajoutez :

```json
{
  "dependencies": {
    "com.ugurarsen.nativehaptics": "https://github.com/ugurarsen/NativeHaptics.git"
  }
}
```

> Ce package utilise une **structure UPM plate (flat)** : `package.json` se trouve à la racine du dépôt, donc l'URL d'installation n'a **pas besoin** d'un fragment `?path=`.

---

### Démarrage rapide

```csharp
using NativeHaptics;

NativeHaptics.PlayHapticType(HapticType.Success);
```

C'est tout. Consultez la documentation intégrée au package (Documentation~) pour des modèles d'utilisation détaillés, les couches de compatibilité et les notes spécifiques aux plateformes.

---

### Contenu du package

| Fonctionnalité | Support |
|---|---|
| Haptiques prédéfinis (Selection, Success, Warning, Failure, Impacts) | iOS + Android |
| Impulsion unique (`PlayOneShot`) | iOS + Android |
| Vibration constante (`PlayConstant`) | iOS + Android |
| Enveloppe d'amplitude (`PlayEnvelope`) | iOS + Android |
| Forme d'onde personnalisée (`PlayPattern`) | iOS + Android |
| Activation / silence globaux (`HapticsEnabled`) | Toutes |
| Niveau de sortie global (`OutputLevel`) | Toutes |
| Annulation (`Cancel`) | Toutes |
| Couche de compatibilité NiceVibrations | Oui |
| Couche de compatibilité MoreMountains.NiceVibrations | Oui |
| Binaires natifs `.so` / `.a` | **Aucun** |
| Repli Android API < 26 | Oui |
| No-op Editor / WebGL / Standalone | Oui |

---

### Documentation

Pour la référence API détaillée, des exemples d'utilisation, des notes sur le comportement par plateforme, le guide de migration et la documentation des couches de compatibilité, ouvrez le bouton **View documentation** dans la fiche du Unity Package Manager ou consultez :

[Documentation~/index.md](Documentation~/index.md)

---

### Licence

LGPL v3 — voir [LICENSE.md](LICENSE.md).

<br>

---

<a id="español"></a>
## Español

![Imagen de portada](Documentation~/cover.png)

**Motor de hápticos nativo, moderno y sin dependencias para Unity.**

`com.ugurarsen.nativehaptics` es un reemplazo ligero y completo para el paquete abandonado NiceVibrations.

Utiliza únicamente APIs públicas del sistema operativo y no contiene **ningún binario nativo `.so` / `.a`**, evitando dependencias binarias nativas obsoletas como `liblofelt_sdk.so`.

### Compatibilidad de plataformas

* **Android:** `VibrationEffect` en API 26+, con retroceso automático (fallback) en dispositivos más antiguos.
* **iOS:** `UIImpactFeedbackGenerator`, `UISelectionFeedbackGenerator` y `UINotificationFeedbackGenerator`.
* **Unity Editor / WebGL / Standalone:** No-op seguro.

---

### Instalación

#### OpenUPM — Recomendado

```bash
openupm add com.ugurarsen.nativehaptics
```

#### Unity Package Manager — URL de Git

Abre:

**Window → Package Manager → + → Add package from git URL...**

Luego pega:

```text
https://github.com/ugurarsen/NativeHaptics.git
```

#### `Packages/manifest.json`

Añade:

```json
{
  "dependencies": {
    "com.ugurarsen.nativehaptics": "https://github.com/ugurarsen/NativeHaptics.git"
  }
}
```

> Este paquete usa una **estructura UPM plana (flat)**: `package.json` está en la raíz del repositorio, por lo que la URL de instalación **no** necesita un fragmento `?path=`.

---

### Inicio rápido

```csharp
using NativeHaptics;

NativeHaptics.PlayHapticType(HapticType.Success);
```

Eso es todo. Consulta la documentación incluida en el paquete (Documentation~) para ver patrones de uso detallados, capas de compatibilidad y notas específicas de plataforma.

---

### Qué incluye

| Característica | Soporte |
|---|---|
| Hápticos predefinidos (Selection, Success, Warning, Failure, Impacts) | iOS + Android |
| Impulso único (`PlayOneShot`) | iOS + Android |
| Vibración constante (`PlayConstant`) | iOS + Android |
| Envolvente de amplitud (`PlayEnvelope`) | iOS + Android |
| Forma de onda personalizada (`PlayPattern`) | iOS + Android |
| Activación / silencio globales (`HapticsEnabled`) | Todas |
| Nivel de salida global (`OutputLevel`) | Todas |
| Cancelar (`Cancel`) | Todas |
| Capa de compatibilidad con NiceVibrations | Sí |
| Capa de compatibilidad con MoreMountains.NiceVibrations | Sí |
| Binarios nativos `.so` / `.a` | **Ninguno** |
| Retroceso para Android API < 26 | Sí |
| No-op en Editor / WebGL / Standalone | Sí |

---

### Documentación

Para la referencia completa de la API, ejemplos de uso, notas de comportamiento por plataforma, guía de migración y documentación de las capas de compatibilidad, abre el botón **View documentation** en la tarjeta del Unity Package Manager o consulta:

[Documentation~/index.md](Documentation~/index.md)

---

### Licencia

LGPL v3 — ver [LICENSE.md](LICENSE.md).

<br>

---

<a id="简体中文"></a>
## 简体中文

![封面图](Documentation~/cover.png)

**面向 Unity 的现代、零依赖原生触觉反馈引擎。**

`com.ugurarsen.nativehaptics` 是已废弃的 NiceVibrations 资源包的轻量级、功能完整的直接替代方案。

它仅使用公开的操作系统 API，**不包含任何原生 `.so` / `.a` 二进制文件**，从而避免了对 `liblofelt_sdk.so` 等旧版原生二进制文件的依赖。

### 平台支持

* **Android：** 在 API 26+ 上使用 `VibrationEffect`，在旧设备上自动回退。
* **iOS：** `UIImpactFeedbackGenerator`、`UISelectionFeedbackGenerator` 和 `UINotificationFeedbackGenerator`。
* **Unity Editor / WebGL / Standalone：** 安全空操作。

---

### 安装方式

#### OpenUPM — 推荐

```bash
openupm add com.ugurarsen.nativehaptics
```

#### Unity Package Manager — Git URL

打开：

**Window → Package Manager → + → Add package from git URL...**

然后粘贴：

```text
https://github.com/ugurarsen/NativeHaptics.git
```

#### `Packages/manifest.json`

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

### 快速开始

```csharp
using NativeHaptics;

NativeHaptics.PlayHapticType(HapticType.Success);
```

就这么简单。详细的使用方式、兼容层说明和平台注意事项，请参见包内文档（Documentation~）。

---

### 包含内容

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

### 文档

如需详细的 API 参考、使用示例、平台行为说明、迁移指南以及兼容层文档，请打开 Unity Package Manager 卡片中的 **View documentation** 按钮，或浏览：

[Documentation~/index.md](Documentation~/index.md)

---

### 许可证

LGPL v3 — 详见 [LICENSE.md](LICENSE.md)。
