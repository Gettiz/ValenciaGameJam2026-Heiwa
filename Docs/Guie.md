Guía rápida (últimos añadidos)

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

