using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class WeaponShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform muzzle;
    [SerializeField] private Bullet bulletPrefab;

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

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        fireAction = playerInput.actions["Fire"];
        attackAction = playerInput.actions["Attack"];
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
        if (Time.time < nextFireTime || bulletPrefab == null || muzzle == null)
        {
            return;
        }

        nextFireTime = Time.time + fireCooldown;

        Bullet bullet = Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);
        bullet.Init(bulletSpeed, bulletDamage);
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
            Health health = hits[i].GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage(meleeDamage);
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
