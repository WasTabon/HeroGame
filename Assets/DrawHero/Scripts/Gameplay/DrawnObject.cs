using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DrawnObject : MonoBehaviour
{
    public static float LineThickness = 0.28f;
    public static float MassPerUnit = 0.35f;

    private Rigidbody2D body;

    public void Build(List<Vector2> worldPoints, Color color)
    {
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
            sr.sprite = PrimitiveSprites.Square();
            sr.color = color;
            sr.drawMode = SpriteDrawMode.Sliced;
            sr.size = new Vector2(segLength + LineThickness, LineThickness);
            sr.sortingOrder = 5;
        }

        body.mass = Mathf.Max(0.5f, totalLength * MassPerUnit);

        transform.localScale = Vector3.one * 0.8f;
        transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack);

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySfx("pop");

        if (HapticManager.Instance != null)
            HapticManager.Instance.Light();
    }
}
