# Native Haptics (Français)

🌐 **Langues :** [English](index.md) · [Türkçe](index.tr.md) · Français · [Español](index.es.md) · [简体中文](index.zh-Hans.md)

![Image de couverture](cover.png)

[![openupm](https://img.shields.io/npm/v/com.ugurarsen.nativehaptics?label=openupm\&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.ugurarsen.nativehaptics/)
[![License: LGPL v3](https://img.shields.io/badge/License-LGPL_v3-blue.svg)](../LICENSE.md)
[![Unity](https://img.shields.io/badge/Unity-2019.4%2B-blue.svg)](https://unity.com/)

**Moteur de retour haptique natif moderne et sans dépendance pour Unity.**

`com.ugurarsen.nativehaptics` est un remplacement léger et complet pour le package NiceVibrations, désormais abandonné.

Il n'utilise que des API publiques du système d'exploitation et ne contient **aucun binaire natif `.so` / `.a`**, évitant ainsi les dépendances binaires natives obsolètes telles que `liblofelt_sdk.so`.

### Compatibilité des plateformes

* **Android :** `VibrationEffect` sur API 26+, avec repli automatique sur les appareils plus anciens.
* **iOS :** `UIImpactFeedbackGenerator`, `UISelectionFeedbackGenerator` et `UINotificationFeedbackGenerator`.
* **Unity Editor / WebGL / Standalone :** No-op sécurisé.

---

## Installation

### OpenUPM — Recommandé

```bash
openupm add com.ugurarsen.nativehaptics
```

### Unity Package Manager — URL Git

Ouvrez :

**Window → Package Manager → + → Add package from git URL...**

Puis collez :

```text
https://github.com/ugurarsen/NativeHaptics.git
```

### `Packages/manifest.json`

Ajoutez :

```json
{
  "dependencies": {
    "com.ugurarsen.nativehaptics": "https://github.com/ugurarsen/NativeHaptics.git"
  }
}
```

---

# Démarrage rapide

Ajoutez :

```csharp
using NativeHaptics;
```

Puis appelez directement un retour haptique :

```csharp
NativeHaptics.PlayHapticType(HapticType.Success);
```

C'est tout.

---

# Utilisation courante

## Préréglages

Copiez et utilisez :

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

### Préréglages disponibles

| Préréglage     | Description                        |
| :------------- | :---------------------------------- |
| `Selection`    | Retour de sélection / interface     |
| `Success`      | Action réussie                       |
| `Warning`      | Retour d'avertissement                |
| `Failure`      | Action échouée                        |
| `LightImpact`  | Impact physique léger                 |
| `MediumImpact` | Impact physique moyen                 |
| `HeavyImpact`  | Impact physique fort                  |
| `RigidImpact`  | Impact rigide                         |
| `SoftImpact`   | Impact doux                           |

---

## Impulsion unique personnalisée (One-Shot)

Crée une impulsion haptique unique.

**Paramètres :**

* `duration` — en secondes
* `intensity` — de `0.0` à `1.0`

```csharp
NativeHaptics.PlayOneShot(
    0.1f,   // durée
    0.8f    // intensité
);
```

---

## Retour haptique continu

Joue un retour à amplitude constante pendant une durée spécifique.

**Paramètres :**

* `amplitude` — de `0.0` à `1.0`
* `frequency` — réservé (non utilisé par les pilotes actuels)
* `duration` — en secondes

```csharp
NativeHaptics.PlayConstant(
    0.6f,    // amplitude
    0.0f,    // fréquence
    1.5f     // durée
);
```

Comportement par plateforme :

* **Android (API 26+) :** vibration continue pendant toute la durée.
* **iOS (13+) :** événement CoreHaptics continu pendant toute la durée. Sur iOS < 13 (ou les appareils sans moteur CoreHaptics), un repli vers des impulsions d'impact transitoires à l'amplitude donnée est effectué.
* **Android plus ancien (< API 26) :** vibration continue pendant toute la durée, l'amplitude est ignorée.

---

## Enveloppe d'amplitude

Joue une courbe d'amplitude variant dans le temps. C'est l'équivalent le plus proche du contrôle d'amplitude continu de Lofelt.

**Paramètres :**

* `amplitudeSteps` — échantillons normalisés de `0.0` à `1.0` de l'enveloppe (longueur libre)
* `stepDurationMs` — durée en millisecondes de chaque échantillon

```csharp
// Une courbe "montée puis fondu" de 0,5 s : 10 échantillons, 50 ms chacun
NativeHaptics.PlayEnvelope(
    new float[] { 0.0f, 0.4f, 0.8f, 1.0f, 0.9f, 0.7f, 0.5f, 0.3f, 0.15f, 0.0f },
    50f
);
```

Comportement par plateforme :

* **iOS (13+) :** un seul événement CoreHaptics continu piloté par une `CHHapticParameterCurve` — rampe native fluide, zéro GC, aucun artefact de palier. iOS < 13 se replie vers des impulsions minutées.
* **Android (API 26+) :** un moteur de rendu par paliers en temps réel (`EnvelopeScheduler`) réémet les paliers d'amplitude toutes les `stepDurationMs` avec un léger chevauchement afin que l'actionneur résonant linéaire reste actif. Cela reproduit la manière dont Lofelt pilotait l'amplitude sur Android.
* **Android plus ancien (< API 26) :** l'amplitude est constante, la durée totale est jouée comme un seul long bourdonnement.

---

## Forme d'onde personnalisée

Créez votre propre motif de vibration.

**Android API 26+ / iOS 13+**

```csharp
NativeHaptics.PlayPattern(
    new long[] { 0, 35, 60, 45, 80 },
    new int[]  { 0, 255, 0, 180, 100 }
);
```

### Format du motif

`timings` est exprimé en millisecondes :

```text
{ délai, vibration, pause, vibration, pause, ... }
```

`amplitudes` utilise la plage `0–255` d'Android :

```text
0   = éteint
255 = maximum
```

---

# Paramètres globaux

## Activer / désactiver les haptiques

Couper tout retour haptique :

```csharp
NativeHaptics.HapticsEnabled = false;
```

Réactiver :

```csharp
NativeHaptics.HapticsEnabled = true;
```

---

## Niveau de sortie

Ajustez l'intensité de **toutes** les sorties haptiques globalement — préréglages, impulsions uniques, constantes et formes d'onde :

```csharp
NativeHaptics.OutputLevel = 0.7f;
```

Par exemple :

```text
1.0 = 100 %
0.7 = 70 %
0.5 = 50 %
0.0 = 0 %
```

---

## Arrêter les haptiques

Arrête immédiatement toute vibration active :

```csharp
NativeHaptics.Cancel();
```

---

# Fonctionnalités

| Fonctionnalité                                   | Support                                                  |
| :------------------------------------------------ | :-------------------------------------------------------- |
| Préréglages Selection / Success / Warning / Failure | iOS + Android                                            |
| Préréglages d'impact                              | iOS + Android                                             |
| Contrôle de l'amplitude                            | iOS + Android                                             |
| Impulsion unique                                   | iOS + Android                                             |
| Retour continu / constant                          | iOS (CoreHaptics) + Android (continu)                     |
| Enveloppe d'amplitude (courbes)                    | iOS (courbe CoreHaptics) + Android (rendu par paliers)    |
| Motifs de forme d'onde personnalisés               | iOS (CoreHaptics) + Android API 26+                       |
| Activation / silence globaux                       | Toutes les plateformes                                    |
| Multiplicateur de sortie global                    | Toutes les plateformes                                    |
| Repli Android API < 26                             | Oui                                                        |
| No-op sécurisé Editor / WebGL / Standalone         | Oui                                                        |
| Injection automatique de la permission `VIBRATE`   | Oui                                                        |
| Couche de compatibilité NiceVibrations             | Oui                                                        |
| Binaires natifs `.so` / `.a`                       | **Aucun**                                                  |

---

# Compatibilité NiceVibrations

Si votre projet utilise déjà NiceVibrations, vous pouvez continuer à utiliser l'API existante.

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

### Continu / Constant

```csharp
HapticPatterns.PlayConstant(
    0.8f,   // amplitude
    0.0f,   // fréquence
    1.0f    // durée
);
```

### Contrôleur

```csharp
HapticController.hapticsEnabled = true;
HapticController.outputLevel = 0.5f;
HapticController.Stop();
```

### Vérifications de capacité

```csharp
bool supported = DeviceCapabilities.isVersionSupported;
bool advanced = DeviceCapabilities.meetsAdvancedRequirements;
```

---

# Compatibilité MoreMountains.NiceVibrations

Les appels MoreMountains.NiceVibrations existants peuvent également continuer à fonctionner.

```csharp
using MoreMountains.NiceVibrations;
```

## Préréglages

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

## Aides directes

```csharp
MMVibrationManager.Vibrate();

MMVibrationManager.TransientHaptic(
    0.8f,   // intensité
    0.5f    // netteté
);

MMVibrationManager.ContinuousHaptic(
    0.6f,   // intensité
    0.5f,   // netteté
    2.0f    // durée
);

MMVibrationManager.StopContinuousHaptic();
```

## Spécifique à Android

```csharp
MMVibrationManager.AndroidVibrate(
    150,     // durée en ms
    200      // amplitude : 1–255
);
```

Forme d'onde personnalisée :

```csharp
MMVibrationManager.AndroidVibrate(
    new long[] { 0, 40, 50, 40 },
    new int[]  { 0, 255, 0, 150 },
    -1
);
```

## État global

```csharp
MMVibrationManager.SetHapticsActive(false);

MMVibrationManager.StopAllHaptics();
```

---

# Migration depuis NiceVibrations

Si vous remplacez le package NiceVibrations d'origine :

### 1. Supprimer l'ancien package

Supprimez le dossier de l'asset NiceVibrations existant de votre projet.

Assurez-vous que les binaires natifs obsolètes tels que :

```text
liblofelt_sdk.so
```

sont également supprimés.

### 2. Installer Native Haptics

Installez :

```text
com.ugurarsen.nativehaptics
```

via OpenUPM ou le Unity Package Manager.

### 3. Recompiler

Recompilez normalement votre projet Unity.

Les appels existants utilisant :

```csharp
HapticPatterns
```

ou :

```csharp
MMVibrationManager
```

peuvent continuer à fonctionner grâce aux couches de compatibilité.

---

## Important : fichiers `.haptic`

Le décodage et la lecture des clips JSON `.haptic` propriétaires créés pour le Lofelt Studio, désormais abandonné, ne sont volontairement pas inclus, car ce format était piloté par l'ancien SDK natif en C++/Rust.

Pour les nouveaux projets, utilisez :

* Les préréglages intégrés
* `PlayOneShot`
* `PlayConstant`
* `PlayPattern`
* Les API de forme d'onde Android

---

# Aide-mémoire API

| Ce que vous voulez         | À utiliser                                |
| :--------------------------- | :------------------------------------------ |
| Retour de sélection           | `PlayHapticType(HapticType.Selection)`    |
| Retour de succès              | `PlayHapticType(HapticType.Success)`      |
| Retour d'avertissement        | `PlayHapticType(HapticType.Warning)`      |
| Retour d'échec                | `PlayHapticType(HapticType.Failure)`      |
| Retour d'impact                | `PlayHapticType(HapticType.MediumImpact)` |
| Impulsion personnalisée       | `PlayOneShot()`                           |
| Vibration continue             | `PlayConstant()`                          |
| Enveloppe d'amplitude          | `PlayEnvelope()`                          |
| Forme d'onde personnalisée     | `PlayPattern()`                           |
| Désactiver les haptiques       | `HapticsEnabled = false`                  |
| Changer l'intensité globale    | `OutputLevel = 0.7f`                      |
| Arrêter la vibration           | `Cancel()`                                |

---

# Pourquoi Native Haptics ?

* Zéro binaire de SDK natif
* Aucune dépendance `.so` / `.a`
* API publiques Android et iOS
* Intégration Unity légère
* Repli Android automatique
* Compatibilité NiceVibrations
* Contrôle de l'intensité globale
* Formes d'onde Android personnalisées
* No-op sécurisé en dehors des plateformes prises en charge

---

## Licence

LGPL v3 — voir [LICENSE.md](../LICENSE.md).
