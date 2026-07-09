using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

public class SetupIteration3
{
    private static readonly Color ColorBg = new Color(0.102f, 0.102f, 0.180f, 1f);
    private static readonly Color ColorPrimary = new Color(0.290f, 0.565f, 0.886f, 1f);
    private static readonly Color ColorAccent = new Color(0.961f, 0.651f, 0.137f, 1f);
    private static readonly Color ColorEnemy = new Color(0.85f, 0.30f, 0.35f, 1f);
    private static readonly Color ColorText = Color.white;

    [MenuItem("Tools/DrawHero/Setup Game Scene (Iteration 3)")]
    public static void SetupGameScene()
    {
        string dir = "Assets/DrawHero/Scenes";
        string scenePath = dir + "/Game.unity";

        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        AttachKillZoneTrigger();
        SpawnTestEnemies();

        ResultPopup popup = BuildResultPopup();

        GameObject lc = GameObject.Find("LevelController");
        if (lc == null) lc = new GameObject("LevelController");
        LevelController controller = lc.GetComponent<LevelController>();
        if (controller == null) controller = lc.AddComponent<LevelController>();
        controller.resultPopup = popup;

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("DrawHero Iteration 3: enemies, damage, win/lose added. Draw objects to crush enemies.");
    }

    private static void AttachKillZoneTrigger()
    {
        GameObject killZone = GameObject.Find("KillZone");
        if (killZone == null)
        {
            killZone = new GameObject("KillZone");
            killZone.transform.position = new Vector3(0, -16f, 0);
            BoxCollider2D col = killZone.AddComponent<BoxCollider2D>();
            col.size = new Vector2(80, 2);
            col.isTrigger = true;
        }

        if (killZone.GetComponent<KillZoneTrigger>() == null)
            killZone.AddComponent<KillZoneTrigger>();
    }

    private static void SpawnTestEnemies()
    {
        GameObject existing = GameObject.Find("Enemies");
        if (existing != null)
            Object.DestroyImmediate(existing);

        GameObject root = new GameObject("Enemies");

        CreateEnemy(root.transform, new Vector3(-4f, -7f, 0), 30f);
        CreateEnemy(root.transform, new Vector3(0f, -7f, 0), 30f);
        CreateEnemy(root.transform, new Vector3(4f, -7f, 0), 30f);
    }

    private static void CreateEnemy(Transform parent, Vector3 pos, float hp)
    {
        GameObject enemy = new GameObject("Enemy");
        enemy.transform.SetParent(parent);
        enemy.transform.position = pos;

        SpriteRenderer sr = enemy.AddComponent<SpriteRenderer>();
        sr.sprite = GetCircleSprite();
        sr.color = ColorEnemy;
        sr.sortingOrder = 3;

        Rigidbody2D body = enemy.AddComponent<Rigidbody2D>();
        body.mass = 3f;
        body.gravityScale = 1f;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        body.freezeRotation = false;

        CircleCollider2D col = enemy.AddComponent<CircleCollider2D>();
        col.radius = 0.5f;

        Enemy e = enemy.AddComponent<Enemy>();
        e.maxHP = hp;
    }

    private static ResultPopup BuildResultPopup()
    {
        GameObject canvasGo = GameObject.Find("GameCanvas");
        if (canvasGo == null)
        {
            canvasGo = new GameObject("GameCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas c = canvasGo.GetComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;
        }

        Transform existing = canvasGo.transform.Find("ResultPopup");
        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);

        GameObject popupGo = new GameObject("ResultPopup", typeof(RectTransform));
        popupGo.transform.SetParent(canvasGo.transform, false);
        RectTransform popupRt = popupGo.GetComponent<RectTransform>();
        popupRt.anchorMin = Vector2.zero;
        popupRt.anchorMax = Vector2.one;
        popupRt.offsetMin = Vector2.zero;
        popupRt.offsetMax = Vector2.zero;

        GameObject backdropGo = new GameObject("Backdrop", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
        backdropGo.transform.SetParent(popupGo.transform, false);
        backdropGo.GetComponent<Image>().color = Color.black;
        RectTransform bdRt = backdropGo.GetComponent<RectTransform>();
        bdRt.anchorMin = Vector2.zero;
        bdRt.anchorMax = Vector2.one;
        bdRt.offsetMin = Vector2.zero;
        bdRt.offsetMax = Vector2.zero;
        CanvasGroup backdrop = backdropGo.GetComponent<CanvasGroup>();

        GameObject panelGo = new GameObject("Panel", typeof(RectTransform), typeof(Image));
        panelGo.transform.SetParent(popupGo.transform, false);
        panelGo.GetComponent<Image>().color = new Color(0.14f, 0.14f, 0.24f, 1f);
        RectTransform panelRt = panelGo.GetComponent<RectTransform>();
        panelRt.anchorMin = new Vector2(0.5f, 0.5f);
        panelRt.anchorMax = new Vector2(0.5f, 0.5f);
        panelRt.pivot = new Vector2(0.5f, 0.5f);
        panelRt.sizeDelta = new Vector2(760, 900);
        panelRt.anchoredPosition = Vector2.zero;

        RectTransform title = CreateText(panelGo.transform, "Title", "LEVEL COMPLETE", 56, ColorText);
        title.anchorMin = new Vector2(0.5f, 1f);
        title.anchorMax = new Vector2(0.5f, 1f);
        title.pivot = new Vector2(0.5f, 1f);
        title.sizeDelta = new Vector2(700, 140);
        title.anchoredPosition = new Vector2(0, -60);

        Button nextBtn = CreateButton(panelGo.transform, "NextButton", "NEXT", new Color(0.4f, 0.75f, 0.4f));
        RectTransform nextRt = nextBtn.GetComponent<RectTransform>();
        nextRt.anchorMin = new Vector2(0.5f, 0.5f);
        nextRt.anchorMax = new Vector2(0.5f, 0.5f);
        nextRt.pivot = new Vector2(0.5f, 0.5f);
        nextRt.sizeDelta = new Vector2(500, 150);
        nextRt.anchoredPosition = new Vector2(0, 120);

        Button retryBtn = CreateButton(panelGo.transform, "RetryButton", "RETRY", ColorPrimary);
        RectTransform retryRt = retryBtn.GetComponent<RectTransform>();
        retryRt.anchorMin = new Vector2(0.5f, 0.5f);
        retryRt.anchorMax = new Vector2(0.5f, 0.5f);
        retryRt.pivot = new Vector2(0.5f, 0.5f);
        retryRt.sizeDelta = new Vector2(500, 150);
        retryRt.anchoredPosition = new Vector2(0, -60);

        Button menuBtn = CreateButton(panelGo.transform, "MenuButton", "MENU", ColorAccent);
        RectTransform menuRt = menuBtn.GetComponent<RectTransform>();
        menuRt.anchorMin = new Vector2(0.5f, 0.5f);
        menuRt.anchorMax = new Vector2(0.5f, 0.5f);
        menuRt.pivot = new Vector2(0.5f, 0.5f);
        menuRt.sizeDelta = new Vector2(500, 150);
        menuRt.anchoredPosition = new Vector2(0, -240);

        ResultPopup popup = popupGo.AddComponent<ResultPopup>();
        popup.backdrop = backdrop;
        popup.panel = panelRt;
        popup.titleText = title.GetComponent<TextMeshProUGUI>();
        popup.nextButton = nextBtn;
        popup.retryButton = retryBtn;
        popup.menuButton = menuBtn;

        popupGo.SetActive(false);

        return popup;
    }

    private static RectTransform CreateText(Transform parent, string name, string text, float fontSize, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        return go.GetComponent<RectTransform>();
    }

    private static Button CreateButton(Transform parent, string name, string text, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button), typeof(ButtonPunch));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = color;

        GameObject textGo = new GameObject("Label", typeof(RectTransform));
        textGo.transform.SetParent(go.transform, false);
        TextMeshProUGUI tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 44;
        tmp.color = ColorText;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        RectTransform textRt = textGo.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;

        return go.GetComponent<Button>();
    }

    private static Sprite circleSprite;

    private static Sprite GetCircleSprite()
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
