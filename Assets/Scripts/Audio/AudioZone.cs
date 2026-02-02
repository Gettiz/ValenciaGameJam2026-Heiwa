using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AudioZone : MonoBehaviour
{
    [SerializeField] private AudioClip zoneMusic;
    [SerializeField] private bool loop = true;
    [SerializeField] private bool playOnce = true;

    private bool triggered;

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playOnce && triggered)
        {
            return;
        }

        if (zoneMusic == null)
        {
            return;
        }

        AudioManager.PlayMusicStatic(zoneMusic, loop);
        triggered = true;
    }
}
