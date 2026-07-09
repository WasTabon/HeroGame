using UnityEngine;

public class KillZoneTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            enemy.KillInstant();
            return;
        }

        DrawnObject drawn = other.GetComponentInParent<DrawnObject>();
        if (drawn != null)
        {
            Destroy(drawn.gameObject);
        }
    }
}
