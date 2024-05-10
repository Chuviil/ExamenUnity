using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float timeToDestroy = 10f;

    private void OnEnable()
    {
        Invoke(nameof(DestroyBullet), timeToDestroy);
    }

    void Update()
    {
        transform.position += transform.forward * (speed * Time.deltaTime);
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Bullet")) return;
        DestroyBullet();
    }
}
