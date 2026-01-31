# ValenciaGameJam2026 - Documentación

## Objetivo
Proyecto base con personaje en 3D, cámara en tercera persona, disparo, ataque cuerpo a cuerpo y enemigos que reciben daño y atacan.

## Scripts principales
- Jugador: [Assets/Scripts/Player/ThirdPersonPlayerController.cs](Assets/Scripts/Player/ThirdPersonPlayerController.cs)
- Cámara: [Assets/Scripts/Camera/ThirdPersonCameraController.cs](Assets/Scripts/Camera/ThirdPersonCameraController.cs)
- Armas/jugador: [Assets/Scripts/Combat/WeaponShooter.cs](Assets/Scripts/Combat/WeaponShooter.cs)
- Bala: [Assets/Scripts/Combat/Bullet.cs](Assets/Scripts/Combat/Bullet.cs)
- Vida/daño: [Assets/Scripts/Combat/Health.cs](Assets/Scripts/Combat/Health.cs)
- Enemigo: [Assets/Scripts/Combat/EnemyController.cs](Assets/Scripts/Combat/EnemyController.cs)
- Enemigo side-scroller: [Assets/Scripts/Combat/EnemySideScrollerAI.cs](Assets/Scripts/Combat/EnemySideScrollerAI.cs)

## Capas (Layers)
Usa estas capas para separar colisiones:
- Player
- PlayerWeapon
- Enemy
- EnemyWeapon

Recomendación de matriz de colisiones (Project Settings > Physics):
- Player vs EnemyWeapon: activar
- Enemy vs PlayerWeapon: activar
- PlayerWeapon vs Player: desactivar
- EnemyWeapon vs Enemy: desactivar

Esto evita que las armas dañen a su propio bando.

## Cómo se recibe el daño
El daño se aplica llamando a `Health.TakeDamage(float amount)` sobre el objeto objetivo o alguno de sus padres.
- La bala busca `Health` en el collider impactado o sus padres y aplica daño.
- Los ataques cuerpo a cuerpo hacen un `OverlapSphere` y llaman a `TakeDamage` si encuentran `Health`.

## Métodos importantes
- `Health.TakeDamage(float amount)`: reduce vida y destruye el objeto si llega a 0.
- `Bullet.Init(float speed, float damageAmount, BulletPool pool, Vector3 direction, Collider[] ignoreColliders)`: configura la velocidad, daño, dirección y colisiones ignoradas.
- `EnemyController.SetTarget(Transform newTarget)`: asigna un objetivo al enemigo en runtime.
- `EnemySideScrollerAI.SetTarget(Transform newTarget)`: asigna objetivo para IA side-scroller.
- `WeaponShooter.FireInDirection(Vector3 direction)`: dispara en una dirección específica (para diana/aim).
- `WeaponShooter.SetAimTarget(Transform target)`: dispara hacia una diana si está activado `useAimTarget`.

## Guía: construir el jugador
1. Crea el GameObject jugador.
2. Añade `CharacterController`.
3. Añade `PlayerInput` con el Input Actions del proyecto (Action Map “Player”).
4. Añade `ThirdPersonPlayerController`.
5. Añade `WeaponShooter`.
6. Añade `Health` si quieres que el jugador reciba daño.
7. Crea un hijo llamado `Muzzle` delante del jugador y asígnalo en `WeaponShooter`.
8. Asigna capas:
   - Jugador: `Player`
   - Armas del jugador (balas): `PlayerWeapon`

Notas clave del disparo:
- `WeaponShooter` puede disparar hacia delante o hacia una diana (`aimTarget`).
- Si una bala aparece y desaparece inmediatamente, revisa:
   - Que `PlayerWeapon` no colisione con `Player` en la matriz de Physics.
   - Que el `Muzzle` esté fuera del collider del jugador.

## Guía: construir la cámara
1. En la Main Camera, añade `ThirdPersonCameraController`.
2. Asigna el `target` al jugador.
3. Asigna `playerInput` al `PlayerInput` del jugador.
4. Ajusta sensibilidad, offset y suavizado según prefieras.

## Guía: construir un enemigo
1. Crea el GameObject enemigo.
2. Añade `Rigidbody` (no kinematic) y un `Collider`.
3. Añade `Health`.
4. Añade `EnemyController`.
5. Asigna `target` al jugador (o llama a `SetTarget`).
6. Configura `playerMask` para que incluya la capa `Player`.
7. Asigna la capa del enemigo a `Enemy`.

## Guía: enemigo side-scroller (3D)
1. Crea el enemigo con `Rigidbody` y `Collider`.
2. Añade `EnemySideScrollerAI`.
3. Asigna `target` al jugador.
4. Configura `playerMask` (detección) y `attackMask` (daño).
5. Ajusta `boxWidth/boxHeight/boxDepth` para el área de detección.
6. Ajusta `attackRange`, `attackDamage` y `attackCooldown`.
7. Activa `lockZAxis` para mantener el enemigo en el plano del side-scroller.

## Guía: arma, bala y daño
1. Crea un prefab de bala con `Rigidbody`, `Collider` y `Bullet`.
2. Pon la bala en capa `PlayerWeapon` o `EnemyWeapon` según quién dispare.
3. En `WeaponShooter`, asigna el prefab de la bala y el `muzzle`.
4. Ajusta `bulletSpeed`, `bulletDamage` y `fireCooldown`.

## Pooling y proyectiles múltiples
Puedes tener múltiples pools para distintos tipos de proyectil (fuego, rayos, etc.).
- Crea un `BulletPool` por tipo.
- Cambia el `bulletPool` activo en el arma cuando cambias de ataque.

## Disparo con diana (dirección explícita)
Llama a `WeaponShooter.FireInDirection(Vector3 direction)` para disparar hacia una dirección concreta.
Si prefieres apuntar a un objeto, asigna `aimTarget` y activa `useAimTarget`.

## Side-scrolling 3D: notas generales
- El movimiento principal es en X (y en Y para saltos). Z se mantiene fijo.
- Para cámara, usa un offset fijo y sigue el eje X del jugador.
- Evita rotaciones libres en Z para mantener la perspectiva de “2.5D”.

## Cambiar escenas y conservar estado
### Opción sencilla (recomendada para empezar)
1. Crea un GameObject `GameSession` con un script `GameSession`.
2. Llama `DontDestroyOnLoad(gameObject)` en `Awake`.
3. Guarda en este objeto: nivel actual, vida del jugador, inventario y flags del mundo.
4. Al cargar una escena nueva, el `GameSession` reinyecta datos en el jugador y objetos.

### Guardado simple (PlayerPrefs)
Usa `PlayerPrefs` solo para datos pequeños: volumen, opciones y progreso simple.
- Ejemplo: nivel desbloqueado, cantidad de monedas.

### Guardado robusto (JSON)
1. Crea una clase `SaveData` serializable.
2. Convierte a JSON con `JsonUtility.ToJson`.
3. Guarda en `Application.persistentDataPath`.
4. Carga al iniciar el juego y rellena el estado del mundo.

## Tamaño de escena y rendimiento
- No hay un “tamaño máximo” fijo: depende de los assets y del hardware.
- Prioriza el rendimiento con:
   - Pooling de balas y enemigos.
   - LODs o meshes simples para fondo.
   - Desactivar objetos lejanos.
   - Usar escenas aditivas (streaming) si el nivel es grande.
   - Reducir físicas complejas fuera de cámara.

## Cómo trabajar el proyecto (resumen)
1. Define capas y matriz de colisiones.
2. Crea prefabs: jugador, bala, enemigo.
3. Usa pooling para proyectiles.
4. Mantén la lógica del side-scroller en X/Z fijo.
5. Documenta cada cambio en este README.

## Sección de implementación rápida
- **Jugador**: `ThirdPersonPlayerController` + `WeaponShooter` + `Health`.
- **Enemigo**: `EnemySideScrollerAI` + `Health`.
- **Cámara**: seguimiento en X con offset fijo.
- **Guardado**: `GameSession` + JSON en persistentDataPath.

## Acciones de Input (resumen)
En tu Input Actions asset:
- `Move` (Vector2): WASD + Left Stick
- `Look` (Vector2): Mouse Delta + Right Stick
- `Jump` (Button): Space + Gamepad South
- `Fire` (Button): F + Right Trigger
- `Attack` (Button): Mouse Left o Q + Gamepad West
