using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

public class SetupIteration1
{
    private static readonly Color ColorBg = new Color(0.102f, 0.102f, 0.180f, 1f);
    private static readonly Color ColorPrimary = new Color(0.290f, 0.565f, 0.886f, 1f);
    private static readonly Color ColorAccent = new Color(0.961f, 0.651f, 0.137f, 1f);
    private static readonly Color ColorText = Color.white;

    [MenuItem("Tools/DrawHero/Setup All Scenes (Iteration 1)")]
    public static void SetupAll()
    {
        string dir = "Assets/DrawHero/Scenes";
        if (!System.IO.Directory.Exists(dir))
            System.IO.Directory.CreateDirectory(dir);

        BuildBootScene(dir);
        BuildMainMenuScene(dir);
        BuildLevelMapScene(dir);
        BuildGameScene(dir);

        AddScenesToBuildSettings(dir);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorSceneManager.OpenScene(dir + "/Boot.unity");

        Debug.Log("DrawHero Iteration 1: all scenes created. Press Play from Boot scene.");
    }

    private static void BuildBootScene(string dir)
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject bootstrap = new GameObject("GameBootstrap");
        bootstrap.AddComponent<GameBootstrap>();

        EditorSceneManager.SaveScene(scene, dir + "/Boot.unity");
    }

    private static void BuildMainMenuScene(string dir)
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        Canvas canvas = CreateCanvas("MainMenuCanvas");
        CreateEventSystem();

        GameObject bg = CreateFullScreenImage(canvas.transform, "Background", ColorBg);

        GameObject safeArea = CreateSafeAreaRoot(canvas.transform);

        RectTransform title = CreateText(safeArea.transform, "Title", "DRAW HERO", 72, ColorAccent);
        title.anchorMin = new Vector2(0.5f, 0.75f);
        title.anchorMax = new Vector2(0.5f, 0.75f);
        title.anchoredPosition = Vector2.zero;
        title.sizeDelta = new Vector2(700, 160);

        Button playBtn = CreateButton(safeArea.transform, "PlayButton", "PLAY", ColorPrimary);
        RectTransform playRt = playBtn.GetComponent<RectTransform>();
        playRt.anchorMin = new Vector2(0.5f, 0.4f);
        playRt.anchorMax = new Vector2(0.5f, 0.4f);
        playRt.anchoredPosition = Vector2.zero;
        playRt.sizeDelta = new Vector2(360, 120);

        GameObject controllerGo = new GameObject("MainMenuController");
        MainMenuController controller = controllerGo.AddComponent<MainMenuController>();
        controller.playButton = playBtn;
        controller.titleTransform = title;

        EditorSceneManager.SaveScene(scene, dir + "/MainMenu.unity");
    }

    private static void BuildLevelMapScene(string dir)
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        Canvas canvas = CreateCanvas("LevelMapCanvas");
        CreateEventSystem();

        CreateFullScreenImage(canvas.transform, "Background", ColorBg);

        GameObject safeArea = CreateSafeAreaRoot(canvas.transform);

        RectTransform header = CreateText(safeArea.transform, "Header", "SELECT LEVEL", 48, ColorText);
        header.anchorMin = new Vector2(0.5f, 1f);
        header.anchorMax = new Vector2(0.5f, 1f);
        header.pivot = new Vector2(0.5f, 1f);
        header.anchoredPosition = new Vector2(0, -40);
        header.sizeDelta = new Vector2(600, 100);

        GameObject scrollGo = new GameObject("ScrollView", typeof(RectTransform), typeof(Image), typeof(ScrollRect));
        scrollGo.transform.SetParent(safeArea.transform, false);
        RectTransform scrollRt = scrollGo.GetComponent<RectTransform>();
        scrollRt.anchorMin = new Vector2(0.05f, 0.08f);
        scrollRt.anchorMax = new Vector2(0.95f, 0.82f);
        scrollRt.offsetMin = Vector2.zero;
        scrollRt.offsetMax = Vector2.zero;
        Image scrollBg = scrollGo.GetComponent<Image>();
        scrollBg.color = new Color(1f, 1f, 1f, 0.03f);

        GameObject viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Image), typeof(Mask));
        viewport.transform.SetParent(scrollGo.transform, false);
        RectTransform vpRt = viewport.GetComponent<RectTransform>();
        vpRt.anchorMin = Vector2.zero;
        vpRt.anchorMax = Vector2.one;
        vpRt.offsetMin = Vector2.zero;
        vpRt.offsetMax = Vector2.zero;
        viewport.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.01f);
        viewport.GetComponent<Mask>().showMaskGraphic = false;

        GameObject content = new GameObject("Content", typeof(RectTransform), typeof(GridLayoutGroup), typeof(ContentSizeFitter));
        content.transform.SetParent(viewport.transform, false);
        RectTransform contentRt = content.GetComponent<RectTransform>();
        contentRt.anchorMin = new Vector2(0f, 1f);
        contentRt.anchorMax = new Vector2(1f, 1f);
        contentRt.pivot = new Vector2(0.5f, 1f);
        contentRt.offsetMin = Vector2.zero;
        contentRt.offsetMax = Vector2.zero;

        GridLayoutGroup grid = content.GetComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(160, 160);
        grid.spacing = new Vector2(20, 20);
        grid.padding = new RectOffset(20, 20, 20, 20);
        grid.childAlignment = TextAnchor.UpperCenter;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 3;

        ContentSizeFitter fitter = content.GetComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        ScrollRect scrollRect = scrollGo.GetComponent<ScrollRect>();
        scrollRect.content = contentRt;
        scrollRect.viewport = vpRt;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Elastic;

        Button levelBtnPrefab = CreateLevelButtonTemplate();
        levelBtnPrefab.gameObject.SetActive(false);
        levelBtnPrefab.transform.SetParent(content.transform, false);

        Button backBtn = CreateButton(safeArea.transform, "BackButton", "BACK", ColorAccent);
        RectTransform backRt = backBtn.GetComponent<RectTransform>();
        backRt.anchorMin = new Vector2(0.5f, 0f);
        backRt.anchorMax = new Vector2(0.5f, 0f);
        backRt.pivot = new Vector2(0.5f, 0f);
        backRt.anchoredPosition = new Vector2(0, 30);
        backRt.sizeDelta = new Vector2(240, 90);

        GameObject controllerGo = new GameObject("LevelMapController");
        LevelMapController controller = controllerGo.AddComponent<LevelMapController>();
        controller.levelButtonContainer = content.transform;
        controller.levelButtonPrefab = levelBtnPrefab;
        controller.backButton = backBtn;

        EditorSceneManager.SaveScene(scene, dir + "/LevelMap.unity");
    }

    private static void BuildGameScene(string dir)
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        Canvas canvas = CreateCanvas("GameCanvas");
        CreateEventSystem();

        CreateFullScreenImage(canvas.transform, "Background", ColorBg);

        GameObject safeArea = CreateSafeAreaRoot(canvas.transform);

        RectTransform levelLabel = CreateText(safeArea.transform, "LevelLabel", "Level 1", 48, ColorText);
        levelLabel.anchorMin = new Vector2(0.5f, 1f);
        levelLabel.anchorMax = new Vector2(0.5f, 1f);
        levelLabel.pivot = new Vector2(0.5f, 1f);
        levelLabel.anchoredPosition = new Vector2(0, -40);
        levelLabel.sizeDelta = new Vector2(400, 100);

        RectTransform placeholder = CreateText(safeArea.transform, "Placeholder", "Gameplay coming\nin Iteration 2", 32, new Color(1f, 1f, 1f, 0.4f));
        placeholder.anchorMin = new Vector2(0.5f, 0.5f);
        placeholder.anchorMax = new Vector2(0.5f, 0.5f);
        placeholder.anchoredPosition = Vector2.zero;
        placeholder.sizeDelta = new Vector2(600, 200);

        Button backBtn = CreateButton(safeArea.transform, "BackButton", "BACK", ColorAccent);
        RectTransform backRt = backBtn.GetComponent<RectTransform>();
        backRt.anchorMin = new Vector2(0.5f, 0f);
        backRt.anchorMax = new Vector2(0.5f, 0f);
        backRt.pivot = new Vector2(0.5f, 0f);
        backRt.anchoredPosition = new Vector2(0, 30);
        backRt.sizeDelta = new Vector2(240, 90);

        GameObject controllerGo = new GameObject("GameSceneController");
        GameSceneController controller = controllerGo.AddComponent<GameSceneController>();
        controller.backButton = backBtn;
        controller.levelLabel = levelLabel.GetComponent<TextMeshProUGUI>();

        EditorSceneManager.SaveScene(scene, dir + "/Game.unity");
    }

    private static Canvas CreateCanvas(string name)
    {
        GameObject go = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;

        return canvas;
    }

    private static void CreateEventSystem()
    {
        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() != null) return;
        GameObject go = new GameObject("EventSystem",
            typeof(UnityEngine.EventSystems.EventSystem),
            typeof(UnityEngine.EventSystems.StandaloneInputModule));
    }

    private static GameObject CreateSafeAreaRoot(Transform parent)
    {
        GameObject go = new GameObject("SafeArea", typeof(RectTransform), typeof(SafeAreaFitter));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return go;
    }

    private static GameObject CreateFullScreenImage(Transform parent, string name, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        Image img = go.GetComponent<Image>();
        img.color = color;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
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
        tmp.enableWordWrapping = true;

        return go.GetComponent<RectTransform>();
    }

    private static Button CreateButton(Transform parent, string name, string text, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button), typeof(ButtonPunch));
        go.transform.SetParent(parent, false);

        Image img = go.GetComponent<Image>();
        img.color = color;

        Button btn = go.GetComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.highlightedColor = Color.white;
        cb.pressedColor = new Color(color.r * 0.85f, color.g * 0.85f, color.b * 0.85f, 1f);
        cb.disabledColor = new Color(color.r, color.g, color.b, 0.5f);
        btn.colors = cb;

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

        return btn;
    }

    private static Button CreateLevelButtonTemplate()
    {
        GameObject go = new GameObject("LevelButton", typeof(RectTransform), typeof(Image), typeof(Button), typeof(ButtonPunch));

        Image img = go.GetComponent<Image>();
        img.color = ColorPrimary;

        Button btn = go.GetComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.pressedColor = new Color(ColorPrimary.r * 0.85f, ColorPrimary.g * 0.85f, ColorPrimary.b * 0.85f, 1f);
        cb.disabledColor = new Color(ColorPrimary.r, ColorPrimary.g, ColorPrimary.b, 0.5f);
        btn.colors = cb;

        GameObject textGo = new GameObject("Label", typeof(RectTransform));
        textGo.transform.SetParent(go.transform, false);
        TextMeshProUGUI tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.text = "1";
        tmp.fontSize = 56;
        tmp.color = ColorText;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        RectTransform textRt = textGo.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;

        return btn;
    }

    private static void AddScenesToBuildSettings(string dir)
    {
        string[] sceneNames = { "Boot", "MainMenu", "LevelMap", "Game" };
        var buildScenes = new System.Collections.Generic.List<EditorBuildSettingsScene>();

        foreach (string sceneName in sceneNames)
        {
            string path = dir + "/" + sceneName + ".unity";
            buildScenes.Add(new EditorBuildSettingsScene(path, true));
        }

        EditorBuildSettings.scenes = buildScenes.ToArray();
    }
}
