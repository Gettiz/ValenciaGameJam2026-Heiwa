Guía rápida (últimos añadidos)

## Índice
- [1) Guardado simple (PlayerPrefs)](#1-guardado-simple-playerprefs)
- [2) Flujo menú → historia → selector](#2-flujo-menú--historia--selector)
- [3) Diapositivas de historia](#3-diapositivas-de-historia)
- [4) Selector de niveles y bloqueo](#4-selector-de-niveles-y-bloqueo)
- [5) Mostrar paneles con botones](#5-mostrar-paneles-con-botones)
- [6) Audio (música + SFX)](#6-audio-música--sfx)
- [7) Botón de idioma (solo panel)](#7-botón-de-idioma-solo-panel)
- [8) Enemigo solo disparo (rango y gizmos)](#8-enemigo-solo-disparo-rango-y-gizmos)
- [9) Curación con objetos (Health Pickup)](#9-curación-con-objetos-health-pickup)
- [10) HUD y pausa (reinicio, salir y volver al menú)](#10-hud-y-pausa-reinicio-salir-y-volver-al-menú)

## 1) Guardado simple (PlayerPrefs)
Archivo: [Assets/Scripts/UI/SaveSystem.cs](Assets/Scripts/UI/SaveSystem.cs)

**Qué hace:**
- Guarda si hay partida, escena actual y nivel desbloqueado.

- Se guarda automáticamente en `LevelSelectController.SelectLevel`, con: `SaveSystem.SaveProgress(sceneName, maxUnlockedLevel)`

Si necesitas guardar desde otro sitio (al terminar nivel), llama: `SaveSystem.SaveProgress(SceneManager.GetActiveScene().name, nuevoMaxNivel);`

**Uso típico:**
- `SaveSystem.SaveProgress(scene, maxUnlockedLevel)`
- `SaveSystem.HasSave()`
- `SaveSystem.GetLastScene("Level1")`
- Para borrar guardado `SaveSystem.Clear()`

PlayerPref es el sistema de guardado simple de Unity. Se usa asi
- `PlayerPref.SetInt("HasSave", 1); PlayerPref.Save();`
- Leer: `int hasSave = PlayerPrefs.GetInt("HasSave", 0);`
- Borrar: `PlayerPrefs.DeleteKey("HasSave");`
Se guarda en disco automáticamente al llamar `Save()` (o al salir del juego). En este proyecto ya lo usamos en `SaveSystem`. Si quieres, te explico cómo leer/escribir más datos.
---

## 2) Flujo menú → historia → selector
Archivo: [Assets/Scripts/UI/MainMenuController.cs](Assets/Scripts/UI/MainMenuController.cs)

**Configurar en el inspector:**
- `Menu Panel`, `Credits Panel`, `Slideshow Panel`, `Level Select Panel`
- `Slideshow Controller`
- `Fallback Continue Scene`

**Botones:**
- Iniciar → `OnStartPressed`
- Continuar → `OnContinuePressed`
- Créditos → `OnCreditsPressed`
- Volver → `OnBackToMenuPressed`

---

## 3) Diapositivas de historia
Archivo: [Assets/Scripts/UI/SlideshowController.cs](Assets/Scripts/UI/SlideshowController.cs)

**Asignar:**
- `displayImage` (UI Image)
- `slides` (lista de sprites)
- `slideDuration` y `autoPlay`

**Botones:**
- Siguiente → `NextSlide`
- Anterior → `PreviousSlide`
- Saltar → `Skip`

**Al terminar:**
- En `onFinished`, asigna `MainMenuController.OnSlideshowFinished`.

---

## 4) Selector de niveles y bloqueo
Archivo: [Assets/Scripts/UI/LevelSelectController.cs](Assets/Scripts/UI/LevelSelectController.cs)

**Configurar:**
- `levelSceneNames` (3 escenas)
- `maxUnlockedLevel = 1`
- `lockedMessagePanel`

**Botones:**
- Nivel 1 → `SelectLevel(0)`
- Nivel 2 → `SelectLevel(1)` (bloqueado)
- Nivel 3 → `SelectLevel(2)` (bloqueado)

---

## 5) Mostrar paneles con botones
Archivo: [Assets/Scripts/UI/UIPanelSwitcher.cs](Assets/Scripts/UI/UIPanelSwitcher.cs)

**Uso:**
- Llama `ShowPanel(panel)` para activar uno y desactivar los demás.

---

## 6) Audio (música + SFX)
Archivo: [Assets/Scripts/Audio/AudioManager.cs](Assets/Scripts/Audio/AudioManager.cs)

**Configurar:**
- Objeto con 2 `AudioSource`
- Asignar `musicSource` y `sfxSource`

**Llamadas:**
- `AudioManager.Instance.PlayMusic(clip)`
- `AudioManager.Instance.PlaySfx(clip)`

**Desde cualquier script:**
- `AudioManager.PlayMusicStatic(clip, true)`
- `AudioManager.PlaySfxStatic(clip)`

**Música por zonas:**
Archivo: [Assets/Scripts/Audio/AudioZone.cs](Assets/Scripts/Audio/AudioZone.cs)

- Añade `AudioZone` a un objeto con Collider (isTrigger = true).
- Asigna `zoneMusic` y `loop`.
- Al entrar, cambia la música con el AudioManager.

---

## 7) Botón de idioma (solo panel)
**Objetivo:** mostrar un panel de selección de idioma sin implementar la traducción todavía.

**Pasos rápidos:**
- Crea un panel `LanguagePanel` y déjalo desactivado.
- En el botón “Idioma”, agrega OnClick → `SetActive(true)` al panel.
- Para cerrar: botón “Volver” → `SetActive(false)`.

Si prefieres usar un switcher, añade `UIPanelSwitcher` y llama `ShowPanel(LanguagePanel)`.

---

## 8) Enemigo solo disparo (rango y gizmos)
Archivo: [Assets/Scripts/Combat/EnemySideScrollerAI.cs](Assets/Scripts/Combat/EnemySideScrollerAI.cs)

**Solo disparo (sin melee):**
- No asignes `attackOrigin`/`attackMask` o pon `attackRange = 0`.
- Activa `Enable Shooting`.
- Asigna `BulletPool` y `Shoot Origin`.

**Rango de disparo:**
- `shootRange` define la distancia horizontal en la que empieza a disparar.
- `shootCooldown` controla la cadencia.

**Gizmo de rango:**
- Al seleccionar el enemigo verás un círculo azul en el `shootRange`.

---

## 9) Curación con objetos (Health Pickup)
Archivos:
- [Assets/Scripts/ScriptsGettiz/CharacterHealth/HealthPlayer.cs](Assets/Scripts/ScriptsGettiz/CharacterHealth/HealthPlayer.cs)
- [Assets/Scripts/ScriptsGettiz/CharacterHealth/HealthPickup.cs](Assets/Scripts/ScriptsGettiz/CharacterHealth/HealthPickup.cs)

**Cómo usar:**
- Crea un prefab del objeto de curación.
- Añade un Collider y marca `Is Trigger`.
- Añade el script `HealthPickup`.
- Ajusta `healAmount` (y opcional `pickupSfx`).

**Resultado:** al tocarlo, el jugador se cura y el objeto se destruye.

---

## 10) HUD y pausa (reinicio, salir y volver al menú)
Archivo: [Assets/Scripts/ScriptsGettiz/CanvasScripts/PauseBehavior.cs](Assets/Scripts/ScriptsGettiz/CanvasScripts/PauseBehavior.cs)

**Funciones disponibles para botones:**
- `SwitchPause()` → pausa/reanuda.
- `ReturnToMainMenu()` → vuelve a la escena de menú.
- `RestartScene()` → reinicia la escena actual.
- `QuitGame()` → cierra la aplicación.

**Asignación de escena del menú:**
- En el Inspector puedes asignar la escena directamente en `mainMenuScene` (solo Editor). El nombre se sincroniza automáticamente.

**Notas:**
- `HealthPlayer` ya no reinicia ni cierra el juego; ahora lo gestiona el HUD.

