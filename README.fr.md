# Native Haptics (Français)

🌐 **Langues :** [English](README.md) · [Türkçe](README.tr.md) · Français · [Español](README.es.md) · [简体中文](README.zh-Hans.md)

![Image de couverture](Documentation~/cover.png)

**Moteur de retour haptique natif moderne et sans dépendance pour Unity.**

`com.ugurarsen.nativehaptics` est un remplacement léger et complet pour le package NiceVibrations, désormais abandonné.

Il n'utilise que des API publiques du système d'exploitation et ne contient **aucun binaire natif `.so` / `.a`**, évitant ainsi les dépendances binaires natives obsolètes telles que `liblofelt_sdk.so`.

## Compatibilité des plateformes

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

> Ce package utilise une **structure UPM plate (flat)** : `package.json` se trouve à la racine du dépôt, donc l'URL d'installation n'a **pas besoin** d'un fragment `?path=`.

---

## Démarrage rapide

```csharp
using NativeHaptics;

NativeHaptics.PlayHapticType(HapticType.Success);
```

C'est tout. Consultez la documentation intégrée au package (Documentation~) pour des modèles d'utilisation détaillés, les couches de compatibilité et les notes spécifiques aux plateformes.

---

## Contenu du package

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

## Documentation

Pour la référence API détaillée, des exemples d'utilisation, des notes sur le comportement par plateforme, le guide de migration et la documentation des couches de compatibilité, ouvrez le bouton **View documentation** dans la fiche du Unity Package Manager ou consultez :

[Documentation~/index.md](Documentation~/index.md)

---

## Licence

LGPL v3 — voir [LICENSE.md](LICENSE.md).

