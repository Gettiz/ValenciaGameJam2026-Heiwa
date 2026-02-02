using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private int initialSize = 20;
    [SerializeField] private Transform container;

    private readonly Queue<Bullet> pool = new Queue<Bullet>();

    private void Awake()
    {
        if (container == null)
        {
            container = transform;
        }

        Prewarm();
    }

    private void Prewarm()
    {
        if (bulletPrefab == null)
        {
            return;
        }

        for (int i = 0; i < initialSize; i++)
        {
            Bullet bullet = CreateBullet();
            pool.Enqueue(bullet);
        }
    }

    private Bullet CreateBullet()
    {
        Bullet bullet = Instantiate(bulletPrefab, container);
        bullet.gameObject.SetActive(false);
        return bullet;
    }

    public Bullet Spawn(Vector3 position, Quaternion rotation, float speed, float damage)
    {
        return Spawn(position, rotation, rotation * Vector3.forward, speed, damage, null);
    }

    public Bullet Spawn(Vector3 position, Quaternion rotation, Vector3 direction, float speed, float damage, Collider[] ignoreColliders)
    {
        Bullet bullet = pool.Count > 0 ? pool.Dequeue() : CreateBullet();
        bullet.transform.SetPositionAndRotation(position, rotation);
        bullet.gameObject.SetActive(true);
        bullet.Init(speed, damage, this, direction, ignoreColliders);
        return bullet;
    }

    public void Release(Bullet bullet)
    {
        if (bullet == null)
        {
            return;
        }

        bullet.gameObject.SetActive(false);
        bullet.transform.SetParent(container);
        pool.Enqueue(bullet);
    }
}
