# Guia rapida Unity 3D para Jam de 48h

## 1. Definicion rapida del proyecto
- Mantener alcance minimo: una mecanica central, un nivel jugable, una meta clara.
- Elegir estilo grafico simple (low poly, toon, estilizado) que puedas producir rapido.
- Definir loop de juego en 3 frases antes de abrir Unity.

## 2. Configuracion inicial del proyecto 3D
- Version recomendada para jam: Unity LTS mas reciente instalada en tu maquina.
- Plantilla: Core 3D URP o Core 3D segun preferencia. URP ofrece mejor iluminacion ligera con menos coste.
- Carpetas sugeridas: `Assets/Art`, `Assets/Prefabs`, `Assets/Scenes`, `Assets/Scripts`, `Assets/UI`.
- Ajustes clave: en Project Settings > Time ajustar Fixed Timestep a 0.02 (50 fps de fisica) y desactivar VSync si compensa build rapida.

## 3. Terreno y entorno
- Terreno rapido: GameObject > 3D Object > Terrain. Escala tipica (500, 500, 500). Usa Tools > Paint Terrain para esculpir.
- Texturas: crea Terrain Layer con al menos dos texturas y mezcla para evitar repeticion.
- Alternativa: usa ProBuilder (package oficial) para crear piezas modulares si el terreno es interior.
- Iluminacion rapida: una Directional Light como sol, ajustar intensidad a 1.1 aprox y Color Temperature 6500K. Activar Environment Lighting con Skybox coherente.
- PostProcess: en URP, crear Volume global con Bloom y Color Adjustments suaves.

## 4. Fisicas y Rigidbody en 3D
- Rigidbody se encarga de integrar el movimiento fisico. Ajusta `Mass`, `Drag`, `Angular Drag`. Mantener Mass entre 1 y 3 para personajes.
- `Use Gravity` activa caida. `Is Kinematic` solo cuando manipulas via script sin fisica (ej: cinemáticas).
- Siempre combinar Rigidbody con Collider (Capsule para personajes, Box para props). Ajustar `Collision Detection` a `Continuous` para evitar atravesar objetos a alta velocidad.
- Forces:
  - `AddForce` empuja con fuerza lineal.
  - `MovePosition` y `MoveRotation` para movimientos suaves basados en fisica en `FixedUpdate()`.
- Layer Collision Matrix (Project Settings > Physics) sirve para ignorar colisiones innecesarias y optimizar.

```csharp
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyNotes : MonoBehaviour
{
    public float jumpForce = 6f;
    Rigidbody _body;

    void Awake()
    {
        _body = GetComponent<Rigidbody>();
        _body.freezeRotation = true; // evita vuelco al moverse
    }

    void FixedUpdate()
    {
        if (Input.GetButtonDown("Jump"))
            _body.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}
```

## 5. Movimiento del jugador
- En 3D el personaje suele moverse en plano XZ con camara libre.
- Separar entrada (Input System) de fisica: leer entrada en `Update`, mover en `FixedUpdate` usando Rigidbody.
- Usar `Camera.main` para obtener direccion relativa a la camara si quieres control tipo third-person.

### Nota rapida sobre errores de Input System
- Si el proyecto usa el paquete Input System nuevo (Project Settings > Player > Active Input Handling = Input System Package), cualquier llamada al API clasico (`Input.GetAxis`, `Input.GetButtonDown`, etc.) lanzara `InvalidOperationException`.
- Solucion: cambia `Active Input Handling` a Both si necesitas compatibilidad temporal o migra tus lecturas a `InputAction`. Ejemplo:

```csharp
Vector2 move = playerInput.actions["Move"].ReadValue<Vector2>();
bool jumpPressed = playerInput.actions["Jump"].triggered;
```

- En scripts heredados, quita las referencias a `UnityEngine.Input` y usa las acciones configuradas en tu `InputActionAsset` para mantener coherente la gestion de entrada.

```csharp
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMover : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float turnSpeed = 12f;
    Rigidbody _body;
    Vector2 _input;

    void Awake()
    {
        _body = GetComponent<Rigidbody>();
        _body.freezeRotation = true;
    }

    void Update()
    {
        _input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        _input = Vector2.ClampMagnitude(_input, 1f);
    }

    void FixedUpdate()
    {
        Vector3 camForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up).normalized;
        Vector3 targetDir = camForward * _input.y + camRight * _input.x;

        if (targetDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(targetDir, Vector3.up);
            _body.MoveRotation(Quaternion.Slerp(_body.rotation, targetRot, turnSpeed * Time.fixedDeltaTime));
            Vector3 move = targetDir * moveSpeed;
            _body.MovePosition(_body.position + move * Time.fixedDeltaTime);
        }
        else
        {
            _body.velocity = new Vector3(0f, _body.velocity.y, 0f);
        }
    }
}
```

## 6. Camara third person orbit
- Configura un `InputAction` tipo Vector2 llamado `Look` (binding a Mouse/Delta o Right Stick) y referencia el mismo `PlayerInput` que usa el jugador.
- Ajusta sensibilidad y limites verticales para evitar mareos y clipping.

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Vector2 sensitivity = new Vector2(90f, 70f);
    [SerializeField] Vector2 pitchLimits = new Vector2(-35f, 65f);
    [SerializeField] float followLerp = 12f;
    [SerializeField] float distance = 6f;

    float _yaw;
    float _pitch;

    void Start()
    {
        if (!playerInput && target)
            playerInput = target.GetComponent<PlayerInput>();

        if (target)
        {
            Vector3 offset = transform.position - target.position;
            Quaternion angles = Quaternion.LookRotation(-offset, Vector3.up);
            _yaw = angles.eulerAngles.y;
            _pitch = angles.eulerAngles.x > 180f ? angles.eulerAngles.x - 360f : angles.eulerAngles.x;
        }
    }

    void LateUpdate()
    {
        if (!target || !playerInput) return;

        Vector2 lookDelta = playerInput.actions["Look"].ReadValue<Vector2>();
        _yaw += lookDelta.x * sensitivity.x * Time.deltaTime;
        _pitch -= lookDelta.y * sensitivity.y * Time.deltaTime;
        _pitch = Mathf.Clamp(_pitch, pitchLimits.x, pitchLimits.y);

        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        Vector3 desiredPosition = target.position - rotation * Vector3.forward * distance;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, followLerp * Time.deltaTime);
        transform.rotation = rotation;
    }
}
```

## 7. Animaciones y Animator
- Importar modelos como Humanoid para reutilizar animaciones. Configurar en Inspector > Rig > Animation Type = Humanoid.
- Crear Animator Controller con estados Idle, Run, Jump, Attack. Usar parametros `Float Speed`, `Bool IsGrounded`.
- `Animator.SetFloat("Speed", velocidad)` y `SetTrigger` para acciones unicas. Evitar triggers repetidos con `ResetTrigger` si es necesario.
- Usa `AnimatorOverrideController` si varios personajes comparten esqueleto pero usan clips distintos.

```csharp
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(PlayerMover))]
public class PlayerAnimationBinder : MonoBehaviour
{
    public float damping = 0.1f;
    Animator _anim;
    PlayerMover _mover;

    void Awake()
    {
        _anim = GetComponent<Animator>();
        _mover = GetComponent<PlayerMover>();
    }

    void Update()
    {
        float planarVelocity = new Vector3(_mover.GetComponent<Rigidbody>().velocity.x, 0f, _mover.GetComponent<Rigidbody>().velocity.z).magnitude;
        _anim.SetFloat("Speed", planarVelocity, damping, Time.deltaTime);
    }
}
```

## 8. Montar personajes en escena
1. Importar FBX + texturas. Ajustar escala (Rig tab: Avatar Definition > Create From This Model).
2. Crear prefab: arrastrar modelo a escena, configurar colliders y scripts, luego arrastrar a carpeta Prefabs.
3. Asignar capa `Player` o `Enemy` para filtros.
4. Ajustar pivot: si queda flotando, modificar anchor en app 3D o usar empty parent para corregir.

## 9. Enemigos que siguen y atacan
- Opcion rapida: Unity NavMesh. Proceso: Window > AI > Navigation > Bake. Asignar `NavMeshAgent` a enemigo y usar destino = posicion jugador.
- Alternativa sin NavMesh: seguimiento directo con `Vector3.MoveTowards` y `Physics.Raycast` para chequeo de obstaculos.
- Programar maquina de estados minima (Idle, Chase, Attack) para evitar comportamiento lineal.

```csharp
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyChaser : MonoBehaviour
{
    public Transform target;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    NavMeshAgent _agent;
    float _lastAttack;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (!target) return;
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > attackRange)
        {
            _agent.isStopped = false;
            _agent.SetDestination(target.position);
        }
        else
        {
            _agent.isStopped = true;
            if (Time.time - _lastAttack >= attackCooldown)
            {
                _lastAttack = Time.time;
                Debug.Log("Enemy attacks");
            }
        }
    }
}
```

## 10. Sistema de vida y dano basico
- Crear interfaz `IDamageable` para reutilizar.

```csharp
public interface IDamageable
{
    void TakeDamage(int amount);
}

using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    public int maxHealth = 100;
    public System.Action<int> onHealthChanged;
    public System.Action onDeath;
    int _current;

    void Awake()
    {
        _current = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        _current = Mathf.Max(_current - amount, 0);
        onHealthChanged?.Invoke(_current);
        if (_current == 0) onDeath?.Invoke();
    }
}
```

## 11. UI rapida y mejores practicas
- Canvas en modo Screen Space - Overlay para HUD. Ajustar Reference Resolution con Canvas Scaler (ej: 1920x1080) y `Match Width Or Height` = 0.5.
- Usar Layout Groups para alinear elementos y evitar posiciones absolutas.
- Colores: mantener paleta reducida (3-4 colores) coherente con arte. Contraste alto para legibilidad.
- Jerarquia sugerida: `Canvas/HUD/HealthBar`, `Canvas/PauseMenu`, `Canvas/Tutorial`.
- Mantener textos cortos, iconografia clara. Evitar fuentes poco legibles.

```csharp
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Health trackedHealth;

    void Awake()
    {
        trackedHealth.onHealthChanged += UpdateBar;
        UpdateBar(trackedHealth.maxHealth);
    }

    void UpdateBar(int value)
    {
        slider.value = (float)value / trackedHealth.maxHealth;
    }
}
```

## 12. Flujo recomendado jam 48h
- Hora 0-4: definir concepto, referencias visuales, documentar scope, crear repositorio.
- Hora 4-12: prototipo jugable sin arte final; jugador puede moverse y completar objetivo basico.
- Hora 12-24: integrar arte placeholder, bloquear nivel, crear enemigos basicos.
- Hora 24-36: pulir gameplay, agregar audio, iterar UI.
- Hora 36-42: pruebas, equilibrar dificultad, arreglar bugs.
- Hora 42-48: menus, build final, pruebas en maquina destino.
- Reservar al menos 3 horas para QA y packaging.

## 13. Consejos rapidos
- Iterar en escenas separadas: escena Sandbox para pruebas y principal para juego.
- Usar prefabs variantes en vez de duplicar objetos.
- Guardar versiones de escena con sufijos `_v1`, `_backup` para evitar perdidas.
- Controlar tiempo con check-ins cada 2 horas. Reuniones cortas si trabajas en equipo.
- Audio: usar Librerias libres (FreeSound, Sonniss GDC packs) y ajustar volumen a -10 dB por defecto.

## 14. Herramientas utiles (si hay tiempo)
- Cinemachine: camaras dinamicas rapidas.
- ProBuilder + PolyBrush: nivelado y pintado rapido.
- Post Processing (URP/HDRP) para color grading ligero.
- TextMeshPro: textos nitidos, ofrece controles avanzados.

## 15. Checklist de cierre
- Build limpia sin errores en Console.
- Escena principal referenciada en Build Settings.
- Input reconfigurable o instrucciones visibles.
- UI escalable (probar 16:9 y 16:10 minimo).
- Creditos o pagina con controles.
- Plan B en caso de bug: desactivar feature en 5 minutos.
