using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public GameObject target;
    public float followSpeed;

    private void LateUpdate()
    {
        Vector3 newpos = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
        transform.position = Vector3.Slerp(transform.position, newpos, followSpeed * Time.deltaTime);
    }
}
