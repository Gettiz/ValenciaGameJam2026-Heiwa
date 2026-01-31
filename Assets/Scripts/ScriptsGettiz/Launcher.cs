using UnityEngine;

public class Launcher : MonoBehaviour
{
    [Header("Settings")]
    public GameObject prefabToSpawn;
    public Transform spawnPoint;
    public Vector3 launchDirection;
    public float launchForce = 10f;

    private GameObject currentInstance;

    void Update()
    {
        // If there is no current object, spawn a new one
        if (currentInstance == null)
        {
            SpawnAndLaunch();
        }
    }

    void SpawnAndLaunch()
    {
        currentInstance = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
        
        Rigidbody rb = currentInstance.GetComponent<Rigidbody>();

        launchDirection = transform.forward;

        if (rb != null)
        {
            Debug.Log("Launching");
            rb.AddForce(launchDirection.normalized * launchForce, ForceMode.Impulse);
        }
    }
}