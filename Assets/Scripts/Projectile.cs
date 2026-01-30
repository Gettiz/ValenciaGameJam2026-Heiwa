using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[DisallowMultipleComponent]
public sealed class Projectile : MonoBehaviour
{
    [SerializeField, Min(0f)] private float damage = 10f;
    [SerializeField, Min(0.1f)] private float lifetime = 5f;
    [SerializeField] private LayerMask collisionLayers = ~0;
    [SerializeField] private GameObject impactEffect;

    private Rigidbody body;
    private bool hasImpacted;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void OnEnable()
    {
        hasImpacted = false;
        CancelInvoke();
        Invoke(nameof(DestroySelf), lifetime);
    }

    public void Launch(Vector3 velocity)
    {
        body.linearVelocity = velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasImpacted || !ShouldCollideWith(other.gameObject))
        {
            return;
        }

        HandleImpact(other.gameObject, other.ClosestPoint(transform.position));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasImpacted || !ShouldCollideWith(collision.gameObject))
        {
            return;
        }

        Vector3 hitPoint = collision.GetContact(0).point;
        HandleImpact(collision.gameObject, hitPoint);
    }

    private bool ShouldCollideWith(GameObject other)
    {
        int otherLayerMask = 1 << other.layer;
        return (collisionLayers.value & otherLayerMask) != 0 && other != gameObject;
    }

    private void HandleImpact(GameObject hitObject, Vector3 hitPoint)
    {
        hasImpacted = true;
        ApplyDamage(hitObject);
        SpawnImpactEffect(hitPoint);
        DestroySelf();
    }

    private void ApplyDamage(GameObject hitObject)
    {
        if (hitObject.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable.TakeDamage(damage);
        }
    }

    private void SpawnImpactEffect(Vector3 position)
    {
        if (!impactEffect)
        {
            return;
        }

        GameObject effectInstance = Instantiate(impactEffect, position, Quaternion.identity);
        Destroy(effectInstance, 2f);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
