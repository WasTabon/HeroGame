using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

public class SetupUIPrefabs
{
    private const string RESOURCES_DIR = "Assets/DrawHero/Resources";

    private static readonly Color ColorPanel = new Color(0.14f, 0.14f, 0.24f, 1f);
    private static readonly Color ColorPrimary = new Color(0.290f, 0.565f, 0.886f, 1f);
    private static readonly Color ColorAccent = new Color(0.961f, 0.651f, 0.137f, 1f);
    private static readonly Color ColorGreen = new Color(0.4f, 0.75f, 0.4f);
    private static readonly Color ColorRed = new Color(0.7f, 0.3f, 0.3f);

    [MenuItem("Tools/DrawHero/Generate All UI Prefabs")]
    public static void GenerateAll()
    {
        if (!System.IO.Directory.Exists(RESOURCES_DIR))
            System.IO.Directory.CreateDirectory(RESOURCES_DIR);

        BuildResultPopupPrefab();
        BuildSettingsPrefab();
        RemoveDeadScenePopup();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("DrawHero: ResultPopup + SettingsPanel prefabs generated in Assets/DrawHero/Resources. Edit sprites there.");
    }

    private static void BuildResultPopupPrefab()
    {
        GameObject root = CreateCanvasRoot("ResultPopup", 32767);
        ResultPopup script = root.AddComponent<ResultPopup>();

        CanvasGroup backdrop;
        Button backdropBtn;
        CreateBackdrop(root.transform, out backdrop, out backdropBtn);

        RectTransform panel = CreatePanel(root.transform, new Vector2(820, 1000));

        RectTransform title = CreateText(panel, "Title", "LEVEL COMPLETE", 58, Color.white,
            new Vector2(0, -60), new Vector2(760, 130));

        Image[] stars = new Image[3];
        for (int i = 0; i < 3; i++)
        {
            GameObject starGo = new GameObject("Star" + i, typeof(RectTransform), typeof(Image));
            starGo.transform.SetParent(panel, false);
            Image img = starGo.GetComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            img.color = new Color(0.98f, 0.78f, 0.20f);
            img.preserveAspect = true;
            RectTransform rt = starGo.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.sizeDelta = new Vector2(150, 150);
            rt.anchoredPosition = new Vector2((i - 1) * 190f, -230 + (i == 1 ? 25 : 0));
            stars[i] = img;
        }

        Button nextBtn = CreateButton(panel, "NextButton", "NEXT", ColorGreen, new Vector2(0, -40), new Vector2(520, 140));
        Button retryBtn = CreateButton(panel, "RetryButton", "RETRY", ColorPrimary, new Vector2(0, -210), new Vector2(520, 140));
        Button menuBtn = CreateButton(panel, "MenuButton", "MENU", ColorAccent, new Vector2(0, -380), new Vector2(520, 140));

        script.backdrop = backdrop;
        script.panel = panel;
        script.titleText = title.GetComponent<TextMeshProUGUI>();
        script.nextButton = nextBtn;
        script.retryButton = retryBtn;
        script.menuButton = menuBtn;
        script.stars = stars;

        SavePrefab(root, "ResultPopup");
    }

    private static void BuildSettingsPrefab()
    {
        GameObject root = CreateCanvasRoot("SettingsPanel", 32760);
        SettingsPanel script = root.AddComponent<SettingsPanel>();

        CanvasGroup backdrop;
        Button backdropBtn;
        CreateBackdrop(root.transform, out backdrop, out backdropBtn);

        RectTransform panel = CreatePanel(root.transform, new Vector2(820, 1200));

        CreateText(panel, "Title", "SETTINGS", 58, Color.white, new Vector2(0, -60), new Vector2(760, 120));

        Button sfxBtn = CreateButton(panel, "SfxButton", "SFX: ON", ColorPrimary, new Vector2(0, 160), new Vector2(600, 120));
        Button musicBtn = CreateButton(panel, "MusicButton", "MUSIC: ON", ColorPrimary, new Vector2(0, 10), new Vector2(600, 120));
        Button hapticBtn = CreateButton(panel, "HapticButton", "HAPTICS: ON", ColorPrimary, new Vector2(0, -140), new Vector2(600, 120));
        Button shopBtn = CreateButton(panel, "ShopButton", "SHOP", ColorGreen, new Vector2(0, -300), new Vector2(600, 120));
        Button resetBtn = CreateButton(panel, "ResetButton", "RESET PROGRESS", ColorRed, new Vector2(0, -440), new Vector2(600, 120));
        Button closeBtn = CreateButton(panel, "CloseButton", "CLOSE", ColorAccent, new Vector2(0, -580), new Vector2(600, 120));

        script.backdrop = backdrop;
        script.backdropButton = backdropBtn;
        script.panel = panel;
        script.sfxButton = sfxBtn;
        script.musicButton = musicBtn;
        script.hapticButton = hapticBtn;
        script.shopButton = shopBtn;
        script.resetButton = resetBtn;
        script.closeButton = closeBtn;
        script.sfxLabel = sfxBtn.GetComponentInChildren<TextMeshProUGUI>();
        script.musicLabel = musicBtn.GetComponentInChildren<TextMeshProUGUI>();
        script.hapticLabel = hapticBtn.GetComponentInChildren<TextMeshProUGUI>();

        SavePrefab(root, "SettingsPanel");
    }

    private static void RemoveDeadScenePopup()
    {
        string scenePath = "Assets/DrawHero/Scenes/Game.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        GameObject canvas = GameObject.Find("GameCanvas");
        if (canvas != null)
        {
            Transform dead = canvas.transform.Find("ResultPopup");
            if (dead != null)
                Object.DestroyImmediate(dead.gameObject);
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static GameObject CreateCanvasRoot(string name, int sortingOrder)
    {
        GameObject root = new GameObject(name);
        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortingOrder;
        CanvasScaler scaler = root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;
        root.AddComponent<GraphicRaycaster>();
        return root;
    }

    private static void CreateBackdrop(Transform parent, out CanvasGroup group, out Button button)
    {
        GameObject go = new GameObject("Backdrop", typeof(RectTransform), typeof(Image), typeof(CanvasGroup), typeof(Button));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = Color.black;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        group = go.GetComponent<CanvasGroup>();
        button = go.GetComponent<Button>();
    }

    private static RectTransform CreatePanel(Transform parent, Vector2 size)
    {
        GameObject go = new GameObject("Panel", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = ColorPanel;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = size;
        rt.anchoredPosition = Vector2.zero;
        return rt;
    }

    private static RectTransform CreateText(Transform parent, string name, string text, float fontSize, Color color, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.sizeDelta = size;
        rt.anchoredPosition = pos;
        return rt;
    }

    private static Button CreateButton(Transform parent, string name, string text, Color color, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button), typeof(ButtonPunch));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = color;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = size;
        rt.anchoredPosition = pos;

        GameObject textGo = new GameObject("Label", typeof(RectTransform));
        textGo.transform.SetParent(go.transform, false);
        TextMeshProUGUI tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 42;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        RectTransform textRt = textGo.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;

        return go.GetComponent<Button>();
    }

    private static void SavePrefab(GameObject root, string name)
    {
        string path = RESOURCES_DIR + "/" + name + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
    }
}
