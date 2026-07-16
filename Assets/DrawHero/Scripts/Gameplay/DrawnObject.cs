using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DrawnObject : MonoBehaviour
{
    public static float LineThickness = 0.28f;
    public static float MassPerUnit = 0.35f;

    private Rigidbody2D body;
    private bool isBomb;
    private bool bombTriggered;

    public Sprite segmentSprite;

    public void Build(List<Vector2> worldPoints, Color color)
    {
        Build(worldPoints, color, false, false);
    }

    public void Build(List<Vector2> worldPoints, Color color, bool heavy, bool bomb)
    {
        isBomb = bomb;
        gameObject.layer = LayerMask.NameToLayer("Default");

        body = gameObject.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Dynamic;
        body.gravityScale = 1f;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;

        Vector2 center = Vector2.zero;
        for (int i = 0; i < worldPoints.Count; i++)
            center += worldPoints[i];
        center /= worldPoints.Count;

        transform.position = center;

        float totalLength = 0f;

        for (int i = 0; i < worldPoints.Count - 1; i++)
        {
            Vector2 a = worldPoints[i] - center;
            Vector2 b = worldPoints[i + 1] - center;

            Vector2 mid = (a + b) * 0.5f;
            Vector2 dir = b - a;
            float segLength = dir.magnitude;
            totalLength += segLength;

            if (segLength < 0.001f) continue;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            GameObject segGo = new GameObject("Segment_" + i);
            segGo.transform.SetParent(transform, false);
            segGo.transform.localPosition = mid;
            segGo.transform.localRotation = Quaternion.Euler(0, 0, angle);

            CapsuleCollider2D capsule = segGo.AddComponent<CapsuleCollider2D>();
            capsule.direction = CapsuleDirection2D.Horizontal;
            capsule.size = new Vector2(segLength + LineThickness, LineThickness);

            SpriteRenderer sr = segGo.AddComponent<SpriteRenderer>();
            sr.sprite = segmentSprite != null ? segmentSprite : PrimitiveSprites.Square();
            sr.color = bomb ? new Color(0.9f, 0.3f, 0.2f) : (heavy ? new Color(0.5f, 0.5f, 0.6f) : color);
            sr.drawMode = SpriteDrawMode.Sliced;
            sr.size = new Vector2(segLength + LineThickness, LineThickness);
            sr.sortingOrder = 5;
        }

        body.mass = Mathf.Max(0.5f, totalLength * MassPerUnit);
        if (heavy)
            body.mass *= 2f;

        transform.localScale = Vector3.one * 0.8f;
        transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack);

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySfx("pop");

        if (HapticManager.Instance != null)
            HapticManager.Instance.Light();
    }

    private float lastImpactTime;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Time.time - lastImpactTime < 0.15f) return;

        float impact = collision.relativeVelocity.magnitude;
        if (impact < 4f) return;

        lastImpactTime = Time.time;

        Vector2 point = collision.GetContact(0).point;
        SimpleParticleBurst.Spawn(point, new Color(1f, 1f, 1f, 0.6f), 4, 0.8f);

        if (impact > 9f && CameraShake.Instance != null)
            CameraShake.Instance.Shake(0.2f, 0.15f);

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySfx("click", 0.6f);

        if (isBomb && !bombTriggered && impact > 5f)
        {
            bombTriggered = true;
            Explode();
        }
    }

    private void Explode()
    {
        Vector2 center = transform.position;
        float radius = 3f;

        SimpleParticleBurst.Spawn(center, new Color(1f, 0.6f, 0.2f), 16, 3f);

        if (CameraShake.Instance != null)
            CameraShake.Instance.Shake(0.7f, 0.4f);

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySfx("pop", 0.5f);

        if (HapticManager.Instance != null)
            HapticManager.Instance.Heavy();

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius);
        for (int i = 0; i < hits.Length; i++)
        {
            Enemy enemy = hits[i].GetComponentInParent<Enemy>();
            if (enemy != null && !enemy.IsDead)
                enemy.TakeDamage(100f);

            Rigidbody2D rb = hits[i].attachedRigidbody;
            if (rb != null && rb.bodyType == RigidbodyType2D.Dynamic)
            {
                Vector2 dir = ((Vector2)rb.transform.position - center).normalized;
                rb.AddForce(dir * 15f, ForceMode2D.Impulse);
            }
        }

        Destroy(gameObject, 0.05f);
    }
}
