using UnityEngine;

public class HoverTransform : MonoBehaviour
{
    public float hoverSpeed = 2f;
    public float hoverDistance = 0.5f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float newY = Mathf.Sin(Time.time * hoverSpeed) * hoverDistance;
        
        transform.position = startPos + new Vector3(0, newY, 0);
    }
}
