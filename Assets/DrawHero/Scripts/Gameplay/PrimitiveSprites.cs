using UnityEngine;

public static class PrimitiveSprites
{
    private static Sprite squareSprite;
    private static Sprite circleSprite;

    public static Sprite Square()
    {
        if (squareSprite != null) return squareSprite;

        Texture2D tex = new Texture2D(4, 4);
        Color[] pixels = new Color[16];
        for (int i = 0; i < 16; i++) pixels[i] = Color.white;
        tex.SetPixels(pixels);
        tex.Apply();
        tex.filterMode = FilterMode.Bilinear;

        squareSprite = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4, 0, SpriteMeshType.FullRect, new Vector4(2, 2, 2, 2));
        return squareSprite;
    }

    public static Sprite Circle()
    {
        if (circleSprite != null) return circleSprite;

        int size = 64;
        Texture2D tex = new Texture2D(size, size);
        Vector2 c = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f - 1f;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), c);
                tex.SetPixel(x, y, d <= radius ? Color.white : new Color(1, 1, 1, 0));
            }
        }
        tex.Apply();
        circleSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        return circleSprite;
    }
}
