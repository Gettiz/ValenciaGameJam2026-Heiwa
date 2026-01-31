using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stopDistance = 1.2f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Attack")]
    [SerializeField] private Transform attackOrigin;
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private LayerMask playerMask;

    private Rigidbody rb;
    private float nextAttackTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (attackOrigin == null)
        {
            attackOrigin = transform;
        }
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;
        float distance = toTarget.magnitude;

        if (distance > stopDistance)
        {
            Vector3 direction = toTarget.normalized;
            Vector3 nextPosition = rb.position + direction * (moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(nextPosition);

            Quaternion targetRot = Quaternion.LookRotation(direction, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime));
        }
    }

    private void Update()
    {
        if (target == null)
        {
            return;
        }

        if (Time.time < nextAttackTime)
        {
            return;
        }

        Vector3 origin = attackOrigin != null ? attackOrigin.position : transform.position;
        Collider[] hits = Physics.OverlapSphere(origin, attackRange, playerMask, QueryTriggerInteraction.Ignore);
        if (hits.Length > 0)
        {
            nextAttackTime = Time.time + attackCooldown;
            for (int i = 0; i < hits.Length; i++)
            {
                Health health = hits[i].GetComponentInParent<Health>();
                if (health != null)
                {
                    health.TakeDamage(attackDamage);
                }
            }
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = attackOrigin != null ? attackOrigin.position : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, attackRange);
    }
}
