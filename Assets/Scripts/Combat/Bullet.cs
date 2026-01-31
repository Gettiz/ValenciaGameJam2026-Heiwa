using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifetime = 4f;
    [SerializeField] private LayerMask damageMask;

    private Rigidbody rb;
    private Collider bulletCollider;
    private float damage;
    private BulletPool pool;
    private float lifeTimer;
    private readonly List<Collider> ignoredColliders = new List<Collider>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        bulletCollider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        lifeTimer = lifetime;
    }

    public void Init(float speed, float damageAmount, BulletPool ownerPool, Vector3 direction, Collider[] ignoreColliders)
    {
        RestoreIgnoredCollisions();
        damage = damageAmount;
        pool = ownerPool;
        Vector3 shootDirection = direction.sqrMagnitude > 0.0001f ? direction.normalized : transform.forward;
        rb.linearVelocity = shootDirection * speed;

        if (bulletCollider != null && ignoreColliders != null)
        {
            for (int i = 0; i < ignoreColliders.Length; i++)
            {
                Collider col = ignoreColliders[i];
                if (col == null)
                {
                    continue;
                }

                Physics.IgnoreCollision(bulletCollider, col, true);
                ignoredColliders.Add(col);
            }
        }
    }

    private void Update()
    {
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            Release();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        bool hitLayer = ((1 << other.gameObject.layer) & damageMask.value) != 0;
        if(!hitLayer)
        {
            Release();
            return;
        }
            Health health = other.collider.GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
    
        Release();
    }

    private void Release()
    {
        if (pool != null)
        {
            pool.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        RestoreIgnoredCollisions();
        if (rb == null)
        {
            return;
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void RestoreIgnoredCollisions()
    {
        if (bulletCollider == null)
        {
            ignoredColliders.Clear();
            return;
        }

        for (int i = 0; i < ignoredColliders.Count; i++)
        {
            Collider col = ignoredColliders[i];
            if (col == null)
            {
                continue;
            }

            Physics.IgnoreCollision(bulletCollider, col, false);
        }

        ignoredColliders.Clear();
    }
}
