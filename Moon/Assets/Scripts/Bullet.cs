using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Damage { get; set; }
    public bool FromEnemy { get; set; }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Enemy>(out var enemy))
            enemy.AddDamage(Damage);
        if (collision.gameObject.TryGetComponent<Player>(out var player) && FromEnemy)
            player.AddDamage(Damage);
        Destroy(gameObject);
    }
}
