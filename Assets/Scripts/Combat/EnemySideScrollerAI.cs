using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemySideScrollerAI : MonoBehaviour
{
    public enum PatrolMode
    {
        None,
        BetweenPoints,
        Range
    }

    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Movement")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float chaseSpeedMultiplier = 1.2f;
    [SerializeField] private float stopDistance = 0.6f;
    [SerializeField] private bool lockZAxis = true;

    [Header("Patrol")]
    [SerializeField] private PatrolMode patrolMode = PatrolMode.BetweenPoints;
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float rangeLeft = 2f;
    [SerializeField] private float rangeRight = 2f;
    [SerializeField] private float waitAtTurn = 0.2f;
    [SerializeField] private bool startFacingRight = true;

    [Header("Detection")]
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private float boxWidth = 6f;
    [SerializeField] private float boxHeight = 2.5f;
    [SerializeField] private float boxDepth = 2f;
    [SerializeField] private float loseBoxExpand = 1f;
    [SerializeField] private Vector3 detectOffset;
    [SerializeField] private float verticalTolerance = 1.5f;
    [SerializeField] private bool enableVerticalTolerance = true;

    [Header("Attack")]
    [SerializeField] private Transform attackOrigin;
    [SerializeField] private float attackRange = 0.8f;
    [SerializeField] private float attackCooldown = 1.2f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private LayerMask attackMask;
    [SerializeField] private float attackVerticalWindow = 1.2f;
    [SerializeField] private float attackVerticalCenterOffset;

    [Header("Animator (opcional)")]
    [SerializeField] private Animator animator;
    [SerializeField] private string runningBool = "isRunning";
    [SerializeField] private string attackTrigger = "attacking";

    private Rigidbody rb;
    private bool facingRight;
    private bool chasing;
    private float nextAttackTime;
    private float nextTurnTime;
    private float startZ;
    private float rangeCenterX;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        if (attackOrigin == null)
        {
            attackOrigin = transform;
        }

        startZ = transform.position.z;
        rangeCenterX = transform.position.x;
        facingRight = startFacingRight;
        ApplyFacingToScale();
    }

    private void Update()
    {
        if (target == null)
        {
            return;
        }

        bool inDetectArea = IsTargetInBox(boxWidth, boxHeight, boxDepth);
        bool inLoseArea = IsTargetInBox(boxWidth + loseBoxExpand * 2f, boxHeight + loseBoxExpand * 2f, boxDepth + loseBoxExpand * 2f);

        bool verticalOk = !enableVerticalTolerance || Mathf.Abs(target.position.y - transform.position.y) <= verticalTolerance;
        if (!chasing && inDetectArea && verticalOk)
        {
            chasing = true;
        }
        else if (chasing && (!inLoseArea || !verticalOk))
        {
            chasing = false;
        }

        if (chasing)
        {
            TryAttack();
        }

        if (animator != null)
        {
            bool moving = Mathf.Abs(rb.linearVelocity.x) > 0.01f;
            animator.SetBool(runningBool, moving);
        }
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            return;
        }

        if (chasing)
        {
            ChaseTarget();
        }
        else
        {
            Patrol();
        }
    }

    private void ChaseTarget()
    {
        Vector3 delta = target.position - transform.position;
        float distance = Mathf.Abs(delta.x);
        if (distance <= stopDistance)
        {
            SetVelocity(0f);
            return;
        }

        float dirX = Mathf.Sign(delta.x);
        SetVelocity(dirX * speed * chaseSpeedMultiplier);
        SetFacing(dirX > 0f);
    }

    private void Patrol()
    {
        if (Time.time < nextTurnTime)
        {
            SetVelocity(0f);
            return;
        }

        switch (patrolMode)
        {
            case PatrolMode.BetweenPoints:
                PatrolBetweenPoints();
                break;
            case PatrolMode.Range:
                PatrolRange();
                break;
            case PatrolMode.None:
                SetVelocity(0f);
                break;
        }
    }

    private void PatrolBetweenPoints()
    {
        if (pointA == null || pointB == null)
        {
            SetVelocity(0f);
            return;
        }

        Transform targetPoint = facingRight ? pointB : pointA;
        float dirX = Mathf.Sign(targetPoint.position.x - transform.position.x);
        SetVelocity(dirX * speed);

        if (Mathf.Abs(transform.position.x - targetPoint.position.x) <= 0.05f)
        {
            ScheduleTurn();
        }
    }

    private void PatrolRange()
    {
        float leftBound = rangeCenterX - rangeLeft;
        float rightBound = rangeCenterX + rangeRight;

        if (facingRight && transform.position.x >= rightBound)
        {
            ScheduleTurn();
        }
        else if (!facingRight && transform.position.x <= leftBound)
        {
            ScheduleTurn();
        }

        float dirX = facingRight ? 1f : -1f;
        SetVelocity(dirX * speed);
    }

    private void TryAttack()
    {
        if (Time.time < nextAttackTime)
        {
            return;
        }

        if (Mathf.Abs(target.position.y - (transform.position.y + attackVerticalCenterOffset)) > attackVerticalWindow)
        {
            return;
        }

        Vector3 origin = attackOrigin != null ? attackOrigin.position : transform.position;
        Collider[] hits = Physics.OverlapSphere(origin, attackRange, attackMask, QueryTriggerInteraction.Ignore);
        if (hits.Length == 0)
        {
            return;
        }

        nextAttackTime = Time.time + attackCooldown;
        if (animator != null && !string.IsNullOrEmpty(attackTrigger))
        {
            animator.SetTrigger(attackTrigger);
        }

        for (int i = 0; i < hits.Length; i++)
        {
            Health health = hits[i].GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage(attackDamage);
            }
        }
    }

    private void SetVelocity(float velocityX)
    {
        Vector3 vel = rb.linearVelocity;
        vel.x = velocityX;
        vel.z = 0f;
        rb.linearVelocity = vel;

        if (lockZAxis)
        {
            Vector3 pos = rb.position;
            pos.z = startZ;
            rb.MovePosition(pos);
        }
    }

    private void ScheduleTurn()
    {
        facingRight = !facingRight;
        ApplyFacingToScale();
        nextTurnTime = Time.time + Mathf.Max(0f, waitAtTurn);
    }

    private void SetFacing(bool right)
    {
        if (facingRight == right)
        {
            return;
        }

        facingRight = right;
        ApplyFacingToScale();
    }

    private void ApplyFacingToScale()
    {
        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * (facingRight ? 1f : -1f);
        transform.localScale = s;
    }

    private bool IsTargetInBox(float width, float height, float depth)
    {
        int sign = facingRight ? 1 : -1;
        Vector3 center = transform.position + new Vector3(detectOffset.x * sign, detectOffset.y, detectOffset.z);
        Vector3 half = new Vector3(Mathf.Max(0.1f, width), Mathf.Max(0.1f, height), Mathf.Max(0.1f, depth)) * 0.5f;
        Collider[] hits = Physics.OverlapBox(center, half, Quaternion.identity, playerMask, QueryTriggerInteraction.Ignore);
        return hits.Length > 0;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void OnDrawGizmosSelected()
    {
        int sign = facingRight ? 1 : -1;
        Vector3 center = transform.position + new Vector3(detectOffset.x * sign, detectOffset.y, detectOffset.z);
        Vector3 half = new Vector3(boxWidth, boxHeight, boxDepth) * 0.5f;
        Gizmos.color = new Color(0f, 1f, 0.25f, 0.5f);
        Gizmos.DrawWireCube(center, half * 2f);

        Vector3 loseHalf = new Vector3(boxWidth + loseBoxExpand * 2f, boxHeight + loseBoxExpand * 2f, boxDepth + loseBoxExpand * 2f) * 0.5f;
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.45f);
        Gizmos.DrawWireCube(center, loseHalf * 2f);

        Vector3 origin = attackOrigin != null ? attackOrigin.position : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, attackRange);
    }
}
