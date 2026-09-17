# Native Haptics (Türkçe)

🌐 **Diller:** [English](README.md) · Türkçe · [Français](README.fr.md) · [Español](README.es.md) · [简体中文](README.zh-Hans.md)

![Kapak Görseli](Documentation~/cover.png)

**Unity için modern, sıfır bağımlılıklı native haptics (dokunsal geri bildirim) motoru.**

`com.ugurarsen.nativehaptics`, artık geliştirilmeyen NiceVibrations paketinin yerine geçebilecek, hafif ve tam özellikli bir pakettir.

Yalnızca herkese açık işletim sistemi API'lerini kullanır ve **hiçbir native `.so` / `.a` ikili dosyası içermez**; bu sayede `liblofelt_sdk.so` gibi eski native ikili dosya bağımlılıklarından kaçınır.

## Platform Desteği

* **Android:** API 26+ üzerinde `VibrationEffect`, eski cihazlarda otomatik geri düşüş (fallback) ile.
* **iOS:** `UIImpactFeedbackGenerator`, `UISelectionFeedbackGenerator` ve `UINotificationFeedbackGenerator`.
* **Unity Editor / WebGL / Standalone:** Güvenli no-op (hiçbir şey yapmaz).

---

## Kurulum

### OpenUPM — Önerilen

```bash
openupm add com.ugurarsen.nativehaptics
```

### Unity Package Manager — Git URL

Şunu açın:

**Window → Package Manager → + → Add package from git URL...**

Ardından şunu yapıştırın:

```text
https://github.com/ugurarsen/NativeHaptics.git
```

### `Packages/manifest.json`

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

## Hızlı Başlangıç

```csharp
using NativeHaptics;

NativeHaptics.PlayHapticType(HapticType.Success);
```

Bu kadar. Ayrıntılı kullanım örnekleri, uyumluluk katmanları ve platforma özel notlar için paket içi dokümantasyona (Documentation~) bakın.

---

## Neler Var?

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

## Dokümantasyon

Ayrıntılı API referansı, kullanım örnekleri, platform davranış notları, geçiş (migration) rehberi ve uyumluluk katmanı dokümantasyonu için Unity Package Manager kartındaki **View documentation** düğmesini açın veya şuraya göz atın:

[Documentation~/index.tr.md](Documentation~/index.tr.md)

---

## Lisans

LGPL v3 — bkz. [LICENSE.md](LICENSE.md).

