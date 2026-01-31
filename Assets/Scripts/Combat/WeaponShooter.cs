using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class WeaponShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform muzzle;
    [SerializeField] private BulletPool bulletPool;
    [SerializeField] private Transform aimTarget;
    [SerializeField] private bool useAimTarget;
    [SerializeField] private RayToPointer pointerAim;
    [SerializeField] private bool usePointerAim;
    [SerializeField] private bool useMuzzleRotation = true;

    [Header("Shooting")]
    [SerializeField] private float bulletSpeed = 25f;
    [SerializeField] private float bulletDamage = 10f;
    [SerializeField] private float fireCooldown = 0.2f;

    [Header("Melee")]
    [SerializeField] private Transform meleeOrigin;
    [SerializeField] private float meleeRange = 1.2f;
    [SerializeField] private float meleeDamage = 20f;
    [SerializeField] private LayerMask meleeMask;

    private PlayerInput playerInput;
    private InputAction fireAction;
    private InputAction attackAction;
    private float nextFireTime;
    private Collider[] ownerColliders;
    private Vector3 customAimDirection;
    private bool hasCustomAimDirection;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        fireAction = playerInput.actions["Fire"];
        attackAction = playerInput.actions["Attack"];
        ownerColliders = GetComponentsInParent<Collider>();
    }

    private void Update()
    {
        if (fireAction != null && fireAction.WasPressedThisFrame())
        {
            Shoot();
        }

        if (attackAction != null && attackAction.WasPressedThisFrame())
        {
            MeleeAttack();
        }
    }

    private void Shoot()
    {
        if (Time.time < nextFireTime || bulletPool == null || muzzle == null)
        {
            return;
        }

        nextFireTime = Time.time + fireCooldown;
        Vector3 direction = GetShootDirection();
        Quaternion rotation = useMuzzleRotation
            ? muzzle.rotation
            : (direction.sqrMagnitude > 0.0001f ? Quaternion.LookRotation(direction, Vector3.up) : muzzle.rotation);
        bulletPool.Spawn(muzzle.position, rotation, direction, bulletSpeed, bulletDamage, ownerColliders);
    }

    private Vector3 GetShootDirection()
    {
        if (hasCustomAimDirection && customAimDirection.sqrMagnitude > 0.0001f)
        {
            return customAimDirection.normalized;
        }

        if (useAimTarget && aimTarget != null)
        {
            return (aimTarget.position - muzzle.position).normalized;
        }

        if (usePointerAim && pointerAim != null)
        {
            Vector3 aimPoint = pointerAim.GetAimPoint();
            return (aimPoint - muzzle.position).normalized;
        }

        return muzzle.forward;
    }

    public void FireInDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.0001f)
        {
            return;
        }

        customAimDirection = direction.normalized;
        hasCustomAimDirection = true;
        Shoot();
        hasCustomAimDirection = false;
    }

    public void SetAimTarget(Transform target)
    {
        aimTarget = target;
        useAimTarget = target != null;
    }

    private void MeleeAttack()
    {
        if (meleeOrigin == null)
        {
            return;
        }

        Collider[] hits = Physics.OverlapSphere(meleeOrigin.position, meleeRange, meleeMask, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < hits.Length; i++)
        {
            IDamageable damageable = hits[i].GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.Damage(meleeDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (meleeOrigin == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(meleeOrigin.position, meleeRange);
    }
}
