using UnityEngine;

public class DestroyRewindCubes : MonoBehaviour
{
    public LayerMask layer;
    void OnTriggerEnter(Collider collider)
    {
        if ((layer.value & (1 << collider.gameObject.layer)) > 0)
        {
            Destroy(collider.gameObject);
        }
    }
}