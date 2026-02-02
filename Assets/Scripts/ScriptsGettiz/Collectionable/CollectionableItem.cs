using UnityEngine;

public class CollectionableItem : MonoBehaviour
{
    public AudioSource CollectionableItemSource;
    public AudioClip maskSound;
    private void OnTriggerEnter(Collider other)
    {
        Collectionables manager = other.GetComponent<Collectionables>();
            
        if (manager != null)
        {
            manager.AddItem();
            
            if (CollectionableItemSource != null && maskSound != null)
            {
                CollectionableItemSource.PlayOneShot(maskSound);
            }
            
            if (TryGetComponent(out MeshRenderer renderer))
            {
                renderer.enabled = false;
            }
            
            if (TryGetComponent(out Collider col))
            {
                col.enabled = false;
            }
            
            Destroy(gameObject, maskSound.length);
        }
    }
}
