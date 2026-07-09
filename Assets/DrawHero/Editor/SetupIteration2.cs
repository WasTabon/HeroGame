using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

public class SetupIteration2
{
    private static readonly Color ColorBg = new Color(0.102f, 0.102f, 0.180f, 1f);
    private static readonly Color ColorPrimary = new Color(0.290f, 0.565f, 0.886f, 1f);
    private static readonly Color ColorAccent = new Color(0.961f, 0.651f, 0.137f, 1f);
    private static readonly Color ColorGround = new Color(0.20f, 0.20f, 0.32f, 1f);
    private static readonly Color ColorText = Color.white;

    [MenuItem("Tools/DrawHero/Setup Game Scene (Iteration 2)")]
    public static void SetupGameScene()
    {
        string dir = "Assets/DrawHero/Scenes";
        string scenePath = dir + "/Game.unity";

        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        RemoveOldPlaceholder();

        SetupCamera();
        SetupPhysics();
        CreateGround();
        CreateKillZone();

        GameObject gm = GetOrCreate("GameManager");
        if (gm.GetComponent<GameManager>() == null)
            gm.AddComponent<GameManager>();

        GameObject ink = GetOrCreate("InkManager");
        if (ink.GetComponent<InkManager>() == null)
            ink.AddComponent<InkManager>();

        GameObject drawer = GetOrCreate("DrawingController");
        if (drawer.GetComponent<DrawingController>() == null)
            drawer.AddComponent<DrawingController>();

        BuildHUD();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("DrawHero Iteration 2: Game scene set up. Draw with mouse/touch, objects fall with physics.");
    }

    private static void RemoveOldPlaceholder()
    {
        GameObject oldController = GameObject.Find("GameSceneController");
        if (oldController != null)
            Object.DestroyImmediate(oldController);

        GameObject canvas = GameObject.Find("GameCanvas");
        if (canvas != null)
        {
            Transform safeArea = canvas.transform.Find("SafeArea");
            if (safeArea != null)
            {
                Transform placeholder = safeArea.Find("Placeholder");
                if (placeholder != null)
                    Object.DestroyImmediate(placeholder.gameObject);

                Transform levelLabel = safeArea.Find("LevelLabel");
                if (levelLabel != null)
                    Object.DestroyImmediate(levelLabel.gameObject);

                Transform backBtn = safeArea.Find("BackButton");
                if (backBtn != null)
                    Object.DestroyImmediate(backBtn.gameObject);
            }
        }
    }

    private static void SetupCamera()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            cam = camGo.AddComponent<Camera>();
        }

        cam.orthographic = true;
        cam.orthographicSize = 10f;
        cam.transform.position = new Vector3(0, 0, -10f);
        cam.backgroundColor = ColorBg;
        cam.clearFlags = CameraClearFlags.SolidColor;
    }

    private static void SetupPhysics()
    {
        Physics2D.gravity = new Vector2(0, -19.6f);
    }

    private static void CreateGround()
    {
        GameObject ground = GetOrCreate("Ground");
        ground.transform.position = new Vector3(0, -9f, 0);

        BoxCollider2D col = ground.GetComponent<BoxCollider2D>();
        if (col == null) col = ground.AddComponent<BoxCollider2D>();
        col.size = new Vector2(40, 2);

        SpriteRenderer sr = ground.GetComponent<SpriteRenderer>();
        if (sr == null) sr = ground.AddComponent<SpriteRenderer>();
        sr.sprite = GetSquareSprite();
        sr.color = ColorGround;
        sr.drawMode = SpriteDrawMode.Sliced;
        sr.size = new Vector2(40, 2);
        sr.sortingOrder = 0;
    }

    private static void CreateKillZone()
    {
        GameObject killZone = GetOrCreate("KillZone");
        killZone.transform.position = new Vector3(0, -16f, 0);

        BoxCollider2D col = killZone.GetComponent<BoxCollider2D>();
        if (col == null) col = killZone.AddComponent<BoxCollider2D>();
        col.size = new Vector2(80, 2);
        col.isTrigger = true;
    }

    private static void BuildHUD()
    {
        GameObject canvasGo = GameObject.Find("GameCanvas");
        Canvas canvas;
        if (canvasGo == null)
        {
            canvasGo = new GameObject("GameCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;
        }
        else
        {
            canvas = canvasGo.GetComponent<Canvas>();
        }

        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            new GameObject("EventSystem",
                typeof(UnityEngine.EventSystems.EventSystem),
                typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }

        Transform safeAreaT = canvasGo.transform.Find("SafeArea");
        GameObject safeArea;
        if (safeAreaT == null)
        {
            safeArea = new GameObject("SafeArea", typeof(RectTransform), typeof(SafeAreaFitter));
            safeArea.transform.SetParent(canvasGo.transform, false);
            RectTransform sart = safeArea.GetComponent<RectTransform>();
            sart.anchorMin = Vector2.zero;
            sart.anchorMax = Vector2.one;
            sart.offsetMin = Vector2.zero;
            sart.offsetMax = Vector2.zero;
        }
        else
        {
            safeArea = safeAreaT.gameObject;
        }

        Transform existingHud = safeArea.transform.Find("HUD");
        if (existingHud != null)
            Object.DestroyImmediate(existingHud.gameObject);

        GameObject hudGo = new GameObject("HUD", typeof(RectTransform));
        hudGo.transform.SetParent(safeArea.transform, false);
        RectTransform hudRt = hudGo.GetComponent<RectTransform>();
        hudRt.anchorMin = Vector2.zero;
        hudRt.anchorMax = Vector2.one;
        hudRt.offsetMin = Vector2.zero;
        hudRt.offsetMax = Vector2.zero;

        GameObject topBar = CreatePanel(hudGo.transform, "TopBar", new Color(0f, 0f, 0f, 0.25f));
        RectTransform topRt = topBar.GetComponent<RectTransform>();
        topRt.anchorMin = new Vector2(0f, 1f);
        topRt.anchorMax = new Vector2(1f, 1f);
        topRt.pivot = new Vector2(0.5f, 1f);
        topRt.sizeDelta = new Vector2(0, 140);
        topRt.anchoredPosition = Vector2.zero;

        GameObject inkBg = CreatePanel(topBar.transform, "InkBarBg", new Color(1f, 1f, 1f, 0.15f));
        RectTransform inkBgRt = inkBg.GetComponent<RectTransform>();
        inkBgRt.anchorMin = new Vector2(0f, 0.5f);
        inkBgRt.anchorMax = new Vector2(0f, 0.5f);
        inkBgRt.pivot = new Vector2(0f, 0.5f);
        inkBgRt.sizeDelta = new Vector2(420, 60);
        inkBgRt.anchoredPosition = new Vector2(40, 0);

        GameObject inkFillGo = CreatePanel(inkBg.transform, "InkFill", ColorPrimary);
        Image inkFill = inkFillGo.GetComponent<Image>();
        inkFill.type = Image.Type.Filled;
        inkFill.fillMethod = Image.FillMethod.Horizontal;
        inkFill.fillOrigin = 0;
        inkFill.fillAmount = 1f;
        RectTransform fillRt = inkFillGo.GetComponent<RectTransform>();
        fillRt.anchorMin = Vector2.zero;
        fillRt.anchorMax = Vector2.one;
        fillRt.offsetMin = Vector2.zero;
        fillRt.offsetMax = Vector2.zero;

        RectTransform inkTextRt = CreateText(inkBg.transform, "InkText", "100 / 100", 32, ColorText);
        inkTextRt.anchorMin = Vector2.zero;
        inkTextRt.anchorMax = Vector2.one;
        inkTextRt.offsetMin = Vector2.zero;
        inkTextRt.offsetMax = Vector2.zero;

        Button backBtn = CreateButton(topBar.transform, "BackButton", "<", ColorAccent);
        RectTransform backRt = backBtn.GetComponent<RectTransform>();
        backRt.anchorMin = new Vector2(1f, 0.5f);
        backRt.anchorMax = new Vector2(1f, 0.5f);
        backRt.pivot = new Vector2(1f, 0.5f);
        backRt.sizeDelta = new Vector2(90, 90);
        backRt.anchoredPosition = new Vector2(-40, 0);

        Button clearBtn = CreateButton(topBar.transform, "ClearButton", "CLR", ColorAccent);
        RectTransform clearRt = clearBtn.GetComponent<RectTransform>();
        clearRt.anchorMin = new Vector2(1f, 0.5f);
        clearRt.anchorMax = new Vector2(1f, 0.5f);
        clearRt.pivot = new Vector2(1f, 0.5f);
        clearRt.sizeDelta = new Vector2(110, 90);
        clearRt.anchoredPosition = new Vector2(-150, 0);

        Button muteBtn = CreateButton(topBar.transform, "MuteButton", "SFX", ColorPrimary);
        RectTransform muteRt = muteBtn.GetComponent<RectTransform>();
        muteRt.anchorMin = new Vector2(1f, 0.5f);
        muteRt.anchorMax = new Vector2(1f, 0.5f);
        muteRt.pivot = new Vector2(1f, 0.5f);
        muteRt.sizeDelta = new Vector2(120, 90);
        muteRt.anchoredPosition = new Vector2(-280, 0);
        TextMeshProUGUI muteLabel = muteBtn.GetComponentInChildren<TextMeshProUGUI>();

        GameHUD hud = hudGo.AddComponent<GameHUD>();
        hud.inkFill = inkFill;
        hud.inkText = inkTextRt.GetComponent<TextMeshProUGUI>();
        hud.backButton = backBtn;
        hud.clearButton = clearBtn;
        hud.muteButton = muteBtn;
        hud.muteLabel = muteLabel;
    }

    private static GameObject GetOrCreate(string name)
    {
        GameObject go = GameObject.Find(name);
        if (go == null)
            go = new GameObject(name);
        return go;
    }

    private static GameObject CreatePanel(Transform parent, string name, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = color;
        return go;
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
        tmp.fontSize = 34;
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

    private static Sprite squareSprite;

    private static Sprite GetSquareSprite()
    {
        if (squareSprite != null) return squareSprite;
        Texture2D tex = new Texture2D(4, 4);
        Color[] pixels = new Color[16];
        for (int i = 0; i < 16; i++) pixels[i] = Color.white;
        tex.SetPixels(pixels);
        tex.Apply();
        squareSprite = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4, 0, SpriteMeshType.FullRect, new Vector4(2, 2, 2, 2));
        return squareSprite;
    }
}
