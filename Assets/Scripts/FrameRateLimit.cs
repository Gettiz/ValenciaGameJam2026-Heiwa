using UnityEngine;

public class FrameRateLimit : MonoBehaviour
{
    void Awake()
    {
        //Fixed Some Issues In High Frame Rate
        QualitySettings.vSyncCount = 0; 
        
        Application.targetFrameRate = 60;
    }
}
