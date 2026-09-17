# Native Haptics (Türkçe)

🌐 **Diller:** [English](index.md) · Türkçe · [Français](index.fr.md) · [Español](index.es.md) · [简体中文](index.zh-Hans.md)

![Kapak Görseli](cover.png)

[![openupm](https://img.shields.io/npm/v/com.ugurarsen.nativehaptics?label=openupm\&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.ugurarsen.nativehaptics/)
[![License: LGPL v3](https://img.shields.io/badge/License-LGPL_v3-blue.svg)](../LICENSE.md)
[![Unity](https://img.shields.io/badge/Unity-2019.4%2B-blue.svg)](https://unity.com/)

**Unity için modern, sıfır bağımlılıklı native haptics (dokunsal geri bildirim) motoru.**

`com.ugurarsen.nativehaptics`, artık geliştirilmeyen NiceVibrations paketinin yerine geçebilecek, hafif ve tam özellikli bir pakettir.

Yalnızca herkese açık işletim sistemi API'lerini kullanır ve **hiçbir native `.so` / `.a` ikili dosyası içermez**; bu sayede `liblofelt_sdk.so` gibi eski native ikili dosya bağımlılıklarından kaçınır.

### Platform Desteği

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

---

# Hızlı Başlangıç

Şunu ekleyin:

```csharp
using NativeHaptics;
```

Ardından herhangi bir haptiği doğrudan çağırın:

```csharp
NativeHaptics.PlayHapticType(HapticType.Success);
```

Bu kadar.

---

# Genel Kullanım

## Hazır Şablonlar (Presets)

Kopyalayıp kullanın:

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

### Mevcut Şablonlar

| Şablon         | Açıklama                    |
| :------------- | :--------------------------- |
| `Selection`    | Seçim / arayüz geri bildirimi |
| `Success`      | Başarılı işlem                |
| `Warning`      | Uyarı geri bildirimi          |
| `Failure`      | Başarısız işlem                |
| `LightImpact`  | Hafif fiziksel darbe           |
| `MediumImpact` | Orta şiddette fiziksel darbe    |
| `HeavyImpact`  | Ağır fiziksel darbe             |
| `RigidImpact`  | Sert darbe                      |
| `SoftImpact`   | Yumuşak darbe                   |

---

## Özel Tek Seferlik Titreşim (One-Shot)

Tek seferlik bir haptik patlama (burst) oluşturur.

**Parametreler:**

* `duration` — saniye
* `intensity` — `0.0` ile `1.0` arası

```csharp
NativeHaptics.PlayOneShot(
    0.1f,   // süre (duration)
    0.8f    // yoğunluk (intensity)
);
```

---

## Sürekli Haptik

Belirli bir süre boyunca sabit genlikte geri bildirim çalar.

**Parametreler:**

* `amplitude` — `0.0` ile `1.0` arası
* `frequency` — ayrılmış (mevcut sürücüler tarafından kullanılmıyor)
* `duration` — saniye

```csharp
NativeHaptics.PlayConstant(
    0.6f,    // genlik (amplitude)
    0.0f,    // frekans (frequency)
    1.5f     // süre (duration)
);
```

Platform davranışı:

* **Android (API 26+):** tüm süre boyunca sürekli titreşim.
* **iOS (13+):** tüm süre boyunca sürekli bir CoreHaptics olayı. iOS < 13'te (veya CoreHaptics motoru olmayan cihazlarda) verilen genlikte kısa darbe (transient impact) atımlarına geri döner.
* **Eski Android (API < 26):** tüm süre boyunca sürekli titreşim, genlik dikkate alınmaz.

---

## Genlik Zarfı (Amplitude Envelope)

Zamanla değişen bir genlik eğrisi çalar. Bu, Lofelt'in sürekli genlik kontrolüne en yakın karşılıktır.

**Parametreler:**

* `amplitudeSteps` — zarfın (herhangi bir uzunlukta) `0.0` ile `1.0` arası normalize edilmiş örnekleri
* `stepDurationMs` — her bir örneğin süreceği milisaniye

```csharp
// 0.5 saniyelik bir "yükselip sönme" eğrisi: 10 örnek, her biri 50ms
NativeHaptics.PlayEnvelope(
    new float[] { 0.0f, 0.4f, 0.8f, 1.0f, 0.9f, 0.7f, 0.5f, 0.3f, 0.15f, 0.0f },
    50f
);
```

Platform davranışı:

* **iOS (13+):** `CHHapticParameterCurve` tarafından yönlendirilen tek bir sürekli CoreHaptics olayı — pürüzsüz native rampa, sıfır GC (bellek toplama), basamaklanma (stepping) artefaktı yok. iOS < 13'te zamanlanmış darbelere geri döner.
* **Android (API 26+):** gerçek zamanlı, basamaklı bir işleyici (`EnvelopeScheduler`), Doğrusal Rezonant Aktüatörün (LRA) enerjili kalması için hafif örtüşmeyle her `stepDurationMs`'de bir genlik adımını yeniden gönderir. Bu, Lofelt'in Android genliğini nasıl sürdüğünü yansıtır.
* **Eski Android (API < 26):** genlik sabittir, toplam süre tek bir uzun vızıltı olarak çalınır.

---

## Özel Dalga Formu (Waveform)

Kendi titreşim deseninizi oluşturun.

**Android API 26+ / iOS 13+**

```csharp
NativeHaptics.PlayPattern(
    new long[] { 0, 35, 60, 45, 80 },
    new int[]  { 0, 255, 0, 180, 100 }
);
```

### Desen (Pattern) Formatı

`timings` milisaniye cinsindendir:

```text
{ gecikme, titreşim, duraklama, titreşim, duraklama, ... }
```

`amplitudes` Android'in `0–255` aralığını kullanır:

```text
0   = kapalı
255 = maksimum
```

---

# Genel Ayarlar

## Haptikleri Etkinleştirme / Devre Dışı Bırakma

Tüm haptik geri bildirimi sessize alın:

```csharp
NativeHaptics.HapticsEnabled = false;
```

Tekrar etkinleştirin:

```csharp
NativeHaptics.HapticsEnabled = true;
```

---

## Çıkış Seviyesi (Output Level)

**Tüm** haptik çıkışının yoğunluğunu genel olarak ölçeklendirin — hazır şablonlar, tek seferlik titreşimler, sabit titreşimler ve dalga formları dahil:

```csharp
NativeHaptics.OutputLevel = 0.7f;
```

Örneğin:

```text
1.0 = %100
0.7 = %70
0.5 = %50
0.0 = %0
```

---

## Haptikleri Durdurma

Aktif herhangi bir titreşimi anında durdurun:

```csharp
NativeHaptics.Cancel();
```

---

# Özellikler

| Özellik                                          | Destek                                                |
| :------------------------------------------------ | :----------------------------------------------------- |
| Selection / Success / Warning / Failure şablonları | iOS + Android                                            |
| Darbe (Impact) şablonları                          | iOS + Android                                            |
| Genlik kontrolü                                    | iOS + Android                                            |
| Tek seferlik titreşim (one-shot)                   | iOS + Android                                            |
| Sürekli / sabit geri bildirim                      | iOS (CoreHaptics) + Android (sürekli)                    |
| Genlik zarfı (eğriler)                             | iOS (CoreHaptics eğrisi) + Android (basamaklı işleyici)  |
| Özel dalga formu desenleri                         | iOS (CoreHaptics) + Android API 26+                      |
| Genel etkinleştirme / sessize alma                 | Tüm platformlar                                          |
| Genel çıkış çarpanı                                | Tüm platformlar                                          |
| Android API < 26 geri düşüş                        | Var                                                       |
| Editor / WebGL / Standalone güvenli no-op          | Var                                                       |
| Otomatik `VIBRATE` manifest eklemesi               | Var                                                       |
| NiceVibrations uyumluluk katmanı                   | Var                                                       |
| Native `.so` / `.a` ikili dosyaları                | **Yok**                                                  |

---

# NiceVibrations Uyumluluğu

Projeniz zaten NiceVibrations kullanıyorsa, mevcut API'yi kullanmaya devam edebilirsiniz.

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

### Sürekli / Sabit

```csharp
HapticPatterns.PlayConstant(
    0.8f,   // genlik (amplitude)
    0.0f,   // frekans (frequency)
    1.0f    // süre (duration)
);
```

### Controller

```csharp
HapticController.hapticsEnabled = true;
HapticController.outputLevel = 0.5f;
HapticController.Stop();
```

### Yetenek Kontrolleri (Capability Checks)

```csharp
bool supported = DeviceCapabilities.isVersionSupported;
bool advanced = DeviceCapabilities.meetsAdvancedRequirements;
```

---

# MoreMountains.NiceVibrations Uyumluluğu

Mevcut MoreMountains.NiceVibrations çağrıları da çalışmaya devam edebilir.

```csharp
using MoreMountains.NiceVibrations;
```

## Hazır Şablonlar

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

## Doğrudan Yardımcılar (Helpers)

```csharp
MMVibrationManager.Vibrate();

MMVibrationManager.TransientHaptic(
    0.8f,   // yoğunluk (intensity)
    0.5f    // keskinlik (sharpness)
);

MMVibrationManager.ContinuousHaptic(
    0.6f,   // yoğunluk (intensity)
    0.5f,   // keskinlik (sharpness)
    2.0f    // süre (duration)
);

MMVibrationManager.StopContinuousHaptic();
```

## Android'e Özel

```csharp
MMVibrationManager.AndroidVibrate(
    150,     // milisaniye cinsinden süre
    200      // genlik: 1–255
);
```

Özel dalga formu:

```csharp
MMVibrationManager.AndroidVibrate(
    new long[] { 0, 40, 50, 40 },
    new int[]  { 0, 255, 0, 150 },
    -1
);
```

## Genel Durum

```csharp
MMVibrationManager.SetHapticsActive(false);

MMVibrationManager.StopAllHaptics();
```

---

# NiceVibrations'tan Geçiş

Orijinal NiceVibrations paketinin yerine geçiyorsanız:

### 1. Eski paketi kaldırın

Projenizden mevcut NiceVibrations paket klasörünü silin.

Aşağıdaki gibi eski native ikili dosyaların da:

```text
liblofelt_sdk.so
```

kaldırıldığından emin olun.

### 2. Native Haptics'i kurun

Şunu kurun:

```text
com.ugurarsen.nativehaptics
```

OpenUPM veya Unity Package Manager kullanarak.

### 3. Yeniden derleyin

Unity projenizi normal şekilde yeniden derleyin.

Şunu kullanan mevcut çağrılar:

```csharp
HapticPatterns
```

veya:

```csharp
MMVibrationManager
```

uyumluluk katmanları sayesinde çalışmaya devam edebilir.

---

## Önemli: `.haptic` Dosyaları

Kaldırılmış Lofelt Studio için oluşturulan özel `.haptic` JSON klipçiklerinin çözümlenmesi ve oynatılması bilerek dahil edilmemiştir, çünkü bu format eski C++/Rust tabanlı native SDK tarafından çalıştırılıyordu.

Yeni projeler için şunları kullanın:

* Yerleşik hazır şablonlar
* `PlayOneShot`
* `PlayConstant`
* `PlayPattern`
* Android dalga formu API'leri

---

# API Hızlı Referans

| Ne istiyorsunuz         | Ne kullanmalısınız                        |
| :----------------------- | :------------------------------------------ |
| Seçim geri bildirimi      | `PlayHapticType(HapticType.Selection)`    |
| Başarı geri bildirimi     | `PlayHapticType(HapticType.Success)`      |
| Uyarı geri bildirimi      | `PlayHapticType(HapticType.Warning)`      |
| Başarısızlık geri bildirimi | `PlayHapticType(HapticType.Failure)`    |
| Darbe geri bildirimi      | `PlayHapticType(HapticType.MediumImpact)` |
| Özel patlama (burst)      | `PlayOneShot()`                           |
| Sürekli titreşim          | `PlayConstant()`                          |
| Genlik zarfı              | `PlayEnvelope()`                          |
| Özel dalga formu          | `PlayPattern()`                           |
| Haptikleri devre dışı bırak | `HapticsEnabled = false`                |
| Genel yoğunluğu değiştir  | `OutputLevel = 0.7f`                      |
| Titreşimi durdur          | `Cancel()`                                |

---

# Neden Native Haptics?

* Sıfır native SDK ikili dosyası
* `.so` / `.a` bağımlılığı yok
* Herkese açık Android ve iOS API'leri
* Hafif Unity entegrasyonu
* Otomatik Android geri düşüşü
* NiceVibrations uyumluluğu
* Genel yoğunluk kontrolü
* Özel Android dalga formları
* Desteklenmeyen platformlarda güvenli no-op

---

## Lisans

LGPL v3 — bkz. [LICENSE.md](../LICENSE.md).
