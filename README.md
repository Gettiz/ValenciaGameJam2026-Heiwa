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
- `Bullet.Init(float speed, float damageAmount)`: configura la velocidad y el daño de la bala.
- `EnemyController.SetTarget(Transform newTarget)`: asigna un objetivo al enemigo en runtime.

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

## Guía: arma, bala y daño
1. Crea un prefab de bala con `Rigidbody`, `Collider` y `Bullet`.
2. Pon la bala en capa `PlayerWeapon` o `EnemyWeapon` según quién dispare.
3. En `WeaponShooter`, asigna el prefab de la bala y el `muzzle`.
4. Ajusta `bulletSpeed`, `bulletDamage` y `fireCooldown`.

## Acciones de Input (resumen)
En tu Input Actions asset:
- `Move` (Vector2): WASD + Left Stick
- `Look` (Vector2): Mouse Delta + Right Stick
- `Jump` (Button): Space + Gamepad South
- `Fire` (Button): F + Right Trigger
- `Attack` (Button): Mouse Left o Q + Gamepad West
