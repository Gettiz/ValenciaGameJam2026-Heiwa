using UnityEngine;

[DisallowMultipleComponent]
public sealed class WeaponShooter : MonoBehaviour
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform muzzleTransform;
    [SerializeField, Min(1f)] private float projectileSpeed = 28f;
    [SerializeField, Min(0f)] private float fireCooldown = 0.25f;
    [SerializeField] private AudioSource fireAudio;

    private float cooldownTimer;

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    public bool TryShoot()
    {
        return TryShoot(transform.forward);
    }

    public bool TryShoot(Vector3 direction)
    {
        if (cooldownTimer > 0f || !projectilePrefab || !muzzleTransform)
        {
            return false;
        }

        Vector3 shootDirection = direction.sqrMagnitude > 0.0001f ? direction.normalized : transform.forward;
        Quaternion rotation = Quaternion.LookRotation(shootDirection, Vector3.up);
        Projectile projectileInstance = Instantiate(projectilePrefab, muzzleTransform.position, rotation);
        projectileInstance.Launch(shootDirection * projectileSpeed);
        cooldownTimer = fireCooldown;

        if (fireAudio)
        {
            fireAudio.Play();
        }

        return true;
    }
}
