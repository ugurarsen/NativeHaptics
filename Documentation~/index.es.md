# Native Haptics (Español)

🌐 **Idiomas:** [English](index.md) · [Türkçe](index.tr.md) · [Français](index.fr.md) · Español · [简体中文](index.zh-Hans.md)

![Imagen de portada](cover.png)

[![openupm](https://img.shields.io/npm/v/com.ugurarsen.nativehaptics?label=openupm\&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.ugurarsen.nativehaptics/)
[![License: LGPL v3](https://img.shields.io/badge/License-LGPL_v3-blue.svg)](../LICENSE.md)
[![Unity](https://img.shields.io/badge/Unity-2019.4%2B-blue.svg)](https://unity.com/)

**Motor de hápticos nativo, moderno y sin dependencias para Unity.**

`com.ugurarsen.nativehaptics` es un reemplazo ligero y completo para el paquete abandonado NiceVibrations.

Utiliza únicamente APIs públicas del sistema operativo y no contiene **ningún binario nativo `.so` / `.a`**, evitando dependencias binarias nativas obsoletas como `liblofelt_sdk.so`.

### Compatibilidad de plataformas

* **Android:** `VibrationEffect` en API 26+, con retroceso automático en dispositivos más antiguos.
* **iOS:** `UIImpactFeedbackGenerator`, `UISelectionFeedbackGenerator` y `UINotificationFeedbackGenerator`.
* **Unity Editor / WebGL / Standalone:** No-op seguro.

---

## Instalación

### OpenUPM — Recomendado

```bash
openupm add com.ugurarsen.nativehaptics
```

### Unity Package Manager — URL de Git

Abre:

**Window → Package Manager → + → Add package from git URL...**

Luego pega:

```text
https://github.com/ugurarsen/NativeHaptics.git
```

### `Packages/manifest.json`

Añade:

```json
{
  "dependencies": {
    "com.ugurarsen.nativehaptics": "https://github.com/ugurarsen/NativeHaptics.git"
  }
}
```

---

# Inicio rápido

Añade:

```csharp
using NativeHaptics;
```

Luego llama directamente a cualquier háptico:

```csharp
NativeHaptics.PlayHapticType(HapticType.Success);
```

Eso es todo.

---

# Uso común

## Preajustes

Copia y usa:

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

### Preajustes disponibles

| Preajuste      | Descripción                          |
| :------------- | :------------------------------------ |
| `Selection`    | Retroalimentación de selección / UI   |
| `Success`      | Acción exitosa                         |
| `Warning`      | Retroalimentación de advertencia       |
| `Failure`      | Acción fallida                         |
| `LightImpact`  | Impacto físico ligero                  |
| `MediumImpact` | Impacto físico medio                   |
| `HeavyImpact`  | Impacto físico fuerte                  |
| `RigidImpact`  | Impacto rígido                         |
| `SoftImpact`   | Impacto suave                          |

---

## Impulso único personalizado (One-Shot)

Crea un único impulso háptico.

**Parámetros:**

* `duration` — segundos
* `intensity` — de `0.0` a `1.0`

```csharp
NativeHaptics.PlayOneShot(
    0.1f,   // duración
    0.8f    // intensidad
);
```

---

## Háptico continuo

Reproduce retroalimentación a amplitud constante durante una duración específica.

**Parámetros:**

* `amplitude` — de `0.0` a `1.0`
* `frequency` — reservado (no usado por los controladores actuales)
* `duration` — segundos

```csharp
NativeHaptics.PlayConstant(
    0.6f,    // amplitud
    0.0f,    // frecuencia
    1.5f     // duración
);
```

Comportamiento por plataforma:

* **Android (API 26+):** vibración sostenida durante toda la duración.
* **iOS (13+):** evento CoreHaptics sostenido durante toda la duración. En iOS < 13 (o dispositivos sin motor CoreHaptics) se recurre a pulsos de impacto transitorios con la amplitud indicada.
* **Android más antiguo (< API 26):** vibración sostenida durante toda la duración, la amplitud se ignora.

---

## Envolvente de amplitud

Reproduce una curva de amplitud que varía en el tiempo. Es el equivalente más cercano al control de amplitud continuo de Lofelt.

**Parámetros:**

* `amplitudeSteps` — muestras normalizadas de `0.0` a `1.0` de la envolvente (de cualquier longitud)
* `stepDurationMs` — milisegundos que dura cada muestra

```csharp
// Una curva "aumenta y luego se desvanece" de 0.5 s: 10 muestras, 50 ms cada una
NativeHaptics.PlayEnvelope(
    new float[] { 0.0f, 0.4f, 0.8f, 1.0f, 0.9f, 0.7f, 0.5f, 0.3f, 0.15f, 0.0f },
    50f
);
```

Comportamiento por plataforma:

* **iOS (13+):** un único evento CoreHaptics continuo controlado por una `CHHapticParameterCurve` — rampa nativa suave, cero recolección de basura (GC), sin artefactos de escalonamiento. iOS < 13 recurre a pulsos temporizados.
* **Android (API 26+):** un renderizador escalonado en tiempo real (`EnvelopeScheduler`) reemite los pasos de amplitud cada `stepDurationMs` con un ligero solapamiento para que el actuador de resonancia lineal se mantenga activo. Esto refleja cómo Lofelt controlaba la amplitud en Android.
* **Android más antiguo (< API 26):** la amplitud es constante, la duración total se reproduce como un único zumbido largo.

---

## Forma de onda personalizada

Crea tu propio patrón de vibración.

**Android API 26+ / iOS 13+**

```csharp
NativeHaptics.PlayPattern(
    new long[] { 0, 35, 60, 45, 80 },
    new int[]  { 0, 255, 0, 180, 100 }
);
```

### Formato del patrón

`timings` está en milisegundos:

```text
{ retraso, vibración, pausa, vibración, pausa, ... }
```

`amplitudes` usa el rango `0–255` de Android:

```text
0   = apagado
255 = máximo
```

---

# Configuración global

## Activar / desactivar hápticos

Silencia toda la retroalimentación háptica:

```csharp
NativeHaptics.HapticsEnabled = false;
```

Vuelve a activarla:

```csharp
NativeHaptics.HapticsEnabled = true;
```

---

## Nivel de salida

Escala la intensidad de **toda** la salida háptica de forma global — preajustes, impulsos únicos, constantes y formas de onda:

```csharp
NativeHaptics.OutputLevel = 0.7f;
```

Por ejemplo:

```text
1.0 = 100 %
0.7 = 70 %
0.5 = 50 %
0.0 = 0 %
```

---

## Detener hápticos

Detiene inmediatamente cualquier vibración activa:

```csharp
NativeHaptics.Cancel();
```

---

# Características

| Característica                                    | Soporte                                                  |
| :--------------------------------------------------- | :--------------------------------------------------------- |
| Preajustes Selection / Success / Warning / Failure   | iOS + Android                                              |
| Preajustes de impacto                                | iOS + Android                                              |
| Control de amplitud                                  | iOS + Android                                              |
| Impulso único                                        | iOS + Android                                              |
| Retroalimentación continua / constante               | iOS (CoreHaptics) + Android (sostenida)                    |
| Envolvente de amplitud (curvas)                      | iOS (curva CoreHaptics) + Android (renderizador escalonado) |
| Patrones de forma de onda personalizados             | iOS (CoreHaptics) + Android API 26+                        |
| Activación / silencio global                         | Todas las plataformas                                      |
| Multiplicador de salida global                       | Todas las plataformas                                      |
| Retroceso Android API < 26                           | Sí                                                          |
| No-op seguro en Editor / WebGL / Standalone          | Sí                                                          |
| Inyección automática del permiso `VIBRATE`           | Sí                                                          |
| Capa de compatibilidad con NiceVibrations            | Sí                                                          |
| Binarios nativos `.so` / `.a`                        | **Ninguno**                                                 |

---

# Compatibilidad con NiceVibrations

Si tu proyecto ya usa NiceVibrations, puedes seguir usando la API existente.

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

### Continuo / Constante

```csharp
HapticPatterns.PlayConstant(
    0.8f,   // amplitud
    0.0f,   // frecuencia
    1.0f    // duración
);
```

### Controlador

```csharp
HapticController.hapticsEnabled = true;
HapticController.outputLevel = 0.5f;
HapticController.Stop();
```

### Verificaciones de capacidad

```csharp
bool supported = DeviceCapabilities.isVersionSupported;
bool advanced = DeviceCapabilities.meetsAdvancedRequirements;
```

---

# Compatibilidad con MoreMountains.NiceVibrations

Las llamadas existentes de MoreMountains.NiceVibrations también pueden seguir funcionando.

```csharp
using MoreMountains.NiceVibrations;
```

## Preajustes

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

## Ayudantes directos

```csharp
MMVibrationManager.Vibrate();

MMVibrationManager.TransientHaptic(
    0.8f,   // intensidad
    0.5f    // nitidez
);

MMVibrationManager.ContinuousHaptic(
    0.6f,   // intensidad
    0.5f,   // nitidez
    2.0f    // duración
);

MMVibrationManager.StopContinuousHaptic();
```

## Específico de Android

```csharp
MMVibrationManager.AndroidVibrate(
    150,     // duración en ms
    200      // amplitud: 1–255
);
```

Forma de onda personalizada:

```csharp
MMVibrationManager.AndroidVibrate(
    new long[] { 0, 40, 50, 40 },
    new int[]  { 0, 255, 0, 150 },
    -1
);
```

## Estado global

```csharp
MMVibrationManager.SetHapticsActive(false);

MMVibrationManager.StopAllHaptics();
```

---

# Migración desde NiceVibrations

Si estás reemplazando el paquete NiceVibrations original:

### 1. Elimina el paquete anterior

Elimina la carpeta del asset NiceVibrations existente de tu proyecto.

Asegúrate de eliminar también binarios nativos heredados como:

```text
liblofelt_sdk.so
```

### 2. Instala Native Haptics

Instala:

```text
com.ugurarsen.nativehaptics
```

usando OpenUPM o el Unity Package Manager.

### 3. Recompila

Recompila tu proyecto de Unity con normalidad.

Las llamadas existentes que usan:

```csharp
HapticPatterns
```

o:

```csharp
MMVibrationManager
```

pueden seguir funcionando gracias a las capas de compatibilidad.

---

## Importante: archivos `.haptic`

La decodificación y reproducción de los clips JSON `.haptic` propietarios creados para el descontinuado Lofelt Studio no se incluyen intencionalmente, ya que ese formato dependía del SDK nativo heredado basado en C++/Rust.

Para proyectos nuevos, usa:

* Preajustes integrados
* `PlayOneShot`
* `PlayConstant`
* `PlayPattern`
* APIs de forma de onda de Android

---

# Hoja de referencia de la API

| Lo que quieres              | Usa                                        |
| :----------------------------- | :-------------------------------------------- |
| Retroalimentación de selección  | `PlayHapticType(HapticType.Selection)`    |
| Retroalimentación de éxito      | `PlayHapticType(HapticType.Success)`      |
| Retroalimentación de advertencia | `PlayHapticType(HapticType.Warning)`     |
| Retroalimentación de fallo       | `PlayHapticType(HapticType.Failure)`     |
| Retroalimentación de impacto     | `PlayHapticType(HapticType.MediumImpact)` |
| Impulso personalizado            | `PlayOneShot()`                           |
| Vibración continua               | `PlayConstant()`                          |
| Envolvente de amplitud           | `PlayEnvelope()`                          |
| Forma de onda personalizada      | `PlayPattern()`                           |
| Desactivar hápticos              | `HapticsEnabled = false`                  |
| Cambiar la intensidad global     | `OutputLevel = 0.7f`                      |
| Detener la vibración             | `Cancel()`                                |

---

# ¿Por qué Native Haptics?

* Cero binarios de SDK nativo
* Sin dependencias `.so` / `.a`
* APIs públicas de Android e iOS
* Integración ligera con Unity
* Retroceso automático en Android
* Compatibilidad con NiceVibrations
* Control de intensidad global
* Formas de onda personalizadas en Android
* No-op seguro fuera de las plataformas compatibles

---

## Licencia

LGPL v3 — ver [LICENSE.md](../LICENSE.md).
