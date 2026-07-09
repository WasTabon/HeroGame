using UnityEngine;
using DG.Tweening;

public class SimpleParticleBurst : MonoBehaviour
{
    public static void Spawn(Vector2 position, Color color, int count = 8, float spread = 1.8f)
    {
        GameObject root = new GameObject("Burst");
        root.transform.position = position;

        for (int i = 0; i < count; i++)
        {
            GameObject p = new GameObject("Particle");
            p.transform.SetParent(root.transform);
            p.transform.position = position;

            SpriteRenderer sr = p.AddComponent<SpriteRenderer>();
            sr.sprite = GetParticleSprite();
            sr.color = color;
            sr.sortingOrder = 20;

            float size = Random.Range(0.15f, 0.35f);
            p.transform.localScale = Vector3.one * size;

            float angle = Random.Range(0f, Mathf.PI * 2f);
            float dist = Random.Range(spread * 0.4f, spread);
            Vector2 target = position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;

            p.transform.DOMove(target, 0.5f).SetEase(Ease.OutQuad);
            p.transform.DOScale(0f, 0.5f).SetEase(Ease.InQuad);
            sr.DOFade(0f, 0.5f).SetEase(Ease.InQuad);
        }

        Destroy(root, 0.6f);
    }

    private static Sprite particleSprite;

    private static Sprite GetParticleSprite()
    {
        if (particleSprite != null) return particleSprite;

        int size = 16;
        Texture2D tex = new Texture2D(size, size);
        Vector2 c = new Vector2(size / 2f, size / 2f);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), c) / (size / 2f);
                float a = Mathf.Clamp01(1f - d);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        }
        tex.Apply();
        particleSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        return particleSprite;
    }
}
