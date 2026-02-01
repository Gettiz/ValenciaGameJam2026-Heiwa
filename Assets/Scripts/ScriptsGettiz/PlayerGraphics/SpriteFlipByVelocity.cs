using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFlipByVelocity : MonoBehaviour
{
    public Rigidbody rb; // asigna el Rigidbody del jugador (3D)
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (rb == null) rb = GetComponentInParent<Rigidbody>();
    }

    private void Update()
    {
        if (rb == null) return;
        if (rb.linearVelocity.x > 0.01f) sr.flipX = false;
        else if (rb.linearVelocity.x < -0.01f) sr.flipX = true;
    }
}