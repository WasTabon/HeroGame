using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

public class SetupIteration4
{
    private static readonly Color ColorBg = new Color(0.102f, 0.102f, 0.180f, 1f);
    private static readonly Color ColorPrimary = new Color(0.290f, 0.565f, 0.886f, 1f);
    private static readonly Color ColorAccent = new Color(0.961f, 0.651f, 0.137f, 1f);
    private static readonly Color ColorText = Color.white;

    private const string LEVEL_DIR = "Assets/DrawHero/Levels";
    private const string DB_PATH = "Assets/DrawHero/Levels/LevelDatabase.asset";

    [MenuItem("Tools/DrawHero/Generate Level Data (Iteration 4)")]
    public static void GenerateLevelData()
    {
        if (!System.IO.Directory.Exists(LEVEL_DIR))
            System.IO.Directory.CreateDirectory(LEVEL_DIR);

        List<LevelData> allLevels = new List<LevelData>();

        for (int i = 1; i <= 10; i++)
        {
            string path = LEVEL_DIR + "/Level_" + i.ToString("D2") + ".asset";
            LevelData data = AssetDatabase.LoadAssetAtPath<LevelData>(path);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<LevelData>();
                AssetDatabase.CreateAsset(data, path);
            }
            ConfigureLevel(data, i);
            EditorUtility.SetDirty(data);
            allLevels.Add(data);
        }

        System.Random rng = new System.Random(12345);
        for (int i = 11; i <= 20; i++)
        {
            string path = LEVEL_DIR + "/Level_" + i.ToString("D2") + ".asset";
            LevelData data = AssetDatabase.LoadAssetAtPath<LevelData>(path);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<LevelData>();
                AssetDatabase.CreateAsset(data, path);

                int src = rng.Next(1, 11);
                data.levelNumber = i;
                data.isClone = true;
                data.sourceLevel = allLevels[src - 1];
            }
            EditorUtility.SetDirty(data);
            allLevels.Add(data);
        }

        LevelDatabase db = AssetDatabase.LoadAssetAtPath<LevelDatabase>(DB_PATH);
        if (db == null)
        {
            db = ScriptableObject.CreateInstance<LevelDatabase>();
            AssetDatabase.CreateAsset(db, DB_PATH);
        }
        db.levels = allLevels.ToArray();
        EditorUtility.SetDirty(db);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("DrawHero Iteration 4: 20 level assets + database generated (11-20 are fixed random clones of 1-10).");
    }

    private static void ConfigureLevel(LevelData data, int n)
    {
        data.levelNumber = n;
        data.isClone = false;
        data.sourceLevel = null;
        data.enemies = new List<EnemySpawn>();
        data.blocks = new List<BlockSpawn>();
        data.boxes = new List<BoxSpawn>();

        float groundY = -7f;

        if (n <= 3)
        {
            data.inkLimit = 120f;
            data.enemies.Add(new EnemySpawn { position = new Vector2(0, groundY), hp = 25f });
            if (n >= 2)
                data.enemies.Add(new EnemySpawn { position = new Vector2(3f, groundY), hp = 25f });
            if (n == 3)
                data.blocks.Add(new BlockSpawn { position = new Vector2(-2f, groundY + 1f), size = new Vector2(1f, 3f) });
        }
        else if (n <= 7)
        {
            data.inkLimit = 110f;
            data.enemies.Add(new EnemySpawn { position = new Vector2(-3f, groundY), hp = 30f });
            data.enemies.Add(new EnemySpawn { position = new Vector2(3f, groundY), hp = 30f });

            data.blocks.Add(new BlockSpawn { position = new Vector2(0f, groundY + 0.5f), size = new Vector2(2f, 4f) });
            data.blocks.Add(new BlockSpawn { position = new Vector2(-6f, groundY + 1.5f), size = new Vector2(3f, 1f), rotation = 20f });

            if (n >= 6)
                data.boxes.Add(new BoxSpawn { position = new Vector2(0f, groundY + 4f), size = new Vector2(1.5f, 1.5f), mass = 3f });
        }
        else
        {
            data.inkLimit = 100f;
            data.enemies.Add(new EnemySpawn { position = new Vector2(-4f, groundY), hp = 30f });
            data.enemies.Add(new EnemySpawn { position = new Vector2(0f, groundY), hp = 30f });
            data.enemies.Add(new EnemySpawn { position = new Vector2(4f, groundY), hp = 30f });

            data.blocks.Add(new BlockSpawn { position = new Vector2(-2f, groundY + 1f), size = new Vector2(1f, 3f) });
            data.blocks.Add(new BlockSpawn { position = new Vector2(2f, groundY + 1f), size = new Vector2(1f, 3f) });

            data.boxes.Add(new BoxSpawn { position = new Vector2(-4f, groundY + 3f), size = new Vector2(1.3f, 1.3f), mass = 2.5f });
            data.boxes.Add(new BoxSpawn { position = new Vector2(4f, groundY + 3f), size = new Vector2(1.3f, 1.3f), mass = 2.5f });
        }
    }

    [MenuItem("Tools/DrawHero/Setup Game Scene (Iteration 4)")]
    public static void SetupGameScene()
    {
        string scenePath = "Assets/DrawHero/Scenes/Game.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        GameObject testEnemies = GameObject.Find("Enemies");
        if (testEnemies != null)
            Object.DestroyImmediate(testEnemies);

        LevelDatabase db = AssetDatabase.LoadAssetAtPath<LevelDatabase>(DB_PATH);
        if (db == null)
        {
            Debug.LogError("Run 'Generate Level Data (Iteration 4)' first!");
            return;
        }

        GameObject loaderGo = GameObject.Find("LevelLoader");
        if (loaderGo == null) loaderGo = new GameObject("LevelLoader");
        LevelLoader loader = loaderGo.GetComponent<LevelLoader>();
        if (loader == null) loader = loaderGo.AddComponent<LevelLoader>();
        loader.database = db;

        AddStarsToPopup();

        GameObject lcGo = GameObject.Find("LevelController");
        if (lcGo != null)
        {
            LevelController lc = lcGo.GetComponent<LevelController>();
            Transform popupT = GameObject.Find("GameCanvas").transform.Find("ResultPopup");
            if (lc != null && popupT != null)
                lc.resultPopup = popupT.GetComponent<ResultPopup>();
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("DrawHero Iteration 4: Game scene now loads levels from data. Test enemies removed.");
    }

    private static void AddStarsToPopup()
    {
        GameObject canvasGo = GameObject.Find("GameCanvas");
        if (canvasGo == null) return;

        Transform popupT = canvasGo.transform.Find("ResultPopup");
        if (popupT == null) return;

        Transform panelT = popupT.Find("Panel");
        if (panelT == null) return;

        Transform existing = panelT.Find("StarRow");
        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);

        GameObject starRow = new GameObject("StarRow", typeof(RectTransform));
        starRow.transform.SetParent(panelT, false);
        RectTransform rowRt = starRow.GetComponent<RectTransform>();
        rowRt.anchorMin = new Vector2(0.5f, 1f);
        rowRt.anchorMax = new Vector2(0.5f, 1f);
        rowRt.pivot = new Vector2(0.5f, 1f);
        rowRt.sizeDelta = new Vector2(600, 180);
        rowRt.anchoredPosition = new Vector2(0, -200);

        Image[] stars = new Image[3];
        float spacing = 190f;
        for (int i = 0; i < 3; i++)
        {
            GameObject holder = new GameObject("StarHolder" + i, typeof(RectTransform));
            holder.transform.SetParent(starRow.transform, false);
            RectTransform hRt = holder.GetComponent<RectTransform>();
            hRt.anchorMin = new Vector2(0.5f, 0.5f);
            hRt.anchorMax = new Vector2(0.5f, 0.5f);
            hRt.pivot = new Vector2(0.5f, 0.5f);
            hRt.sizeDelta = new Vector2(160, 160);
            hRt.anchoredPosition = new Vector2((i - 1) * spacing, i == 1 ? 20 : 0);

            GameObject starGo = new GameObject("Star", typeof(RectTransform), typeof(Image));
            starGo.transform.SetParent(holder.transform, false);
            Image img = starGo.GetComponent<Image>();
            img.sprite = GetStarSprite();
            img.color = new Color(1f, 1f, 1f, 0.15f);
            RectTransform sRt = starGo.GetComponent<RectTransform>();
            sRt.anchorMin = Vector2.zero;
            sRt.anchorMax = Vector2.one;
            sRt.offsetMin = Vector2.zero;
            sRt.offsetMax = Vector2.zero;
            stars[i] = img;
        }

        ResultPopup popup = popupT.GetComponent<ResultPopup>();
        popup.stars = stars;
    }

    [MenuItem("Tools/DrawHero/Setup Level Map (Iteration 4)")]
    public static void SetupLevelMap()
    {
        string scenePath = "Assets/DrawHero/Scenes/LevelMap.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        GameObject controllerGo = GameObject.Find("LevelMapController");
        LevelMapController controller = controllerGo != null ? controllerGo.GetComponent<LevelMapController>() : null;
        if (controller == null)
        {
            Debug.LogError("LevelMapController not found. Run Iteration 1 setup first.");
            return;
        }

        Transform templateT = controller.levelButtonContainer != null
            ? FindTemplate(controller.levelButtonContainer)
            : null;

        if (templateT != null)
            AddStarsLabelToTemplate(templateT.gameObject);

        Canvas canvas = Object.FindObjectOfType<Canvas>();
        Transform safeArea = canvas.transform.Find("SafeArea");
        Transform totalT = safeArea.Find("TotalStars");
        if (totalT == null)
        {
            RectTransform total = CreateText(safeArea, "TotalStars", "Stars: 0", 40, ColorAccent);
            total.anchorMin = new Vector2(0.5f, 1f);
            total.anchorMax = new Vector2(0.5f, 1f);
            total.pivot = new Vector2(0.5f, 1f);
            total.sizeDelta = new Vector2(500, 70);
            total.anchoredPosition = new Vector2(0, -140);
            controller.totalStarsText = total.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            controller.totalStarsText = totalT.GetComponent<TextMeshProUGUI>();
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("DrawHero Iteration 4: Level map now shows locks and stars.");
    }

    private static Transform FindTemplate(Transform container)
    {
        for (int i = 0; i < container.childCount; i++)
        {
            Transform c = container.GetChild(i);
            if (c.GetComponent<Button>() != null)
                return c;
        }
        return null;
    }

    private static void AddStarsLabelToTemplate(GameObject template)
    {
        Transform existing = template.transform.Find("Stars");
        if (existing != null) return;

        GameObject go = new GameObject("Stars", typeof(RectTransform));
        go.transform.SetParent(template.transform, false);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = "---";
        tmp.fontSize = 26;
        tmp.color = new Color(0.98f, 0.78f, 0.20f);
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0f);
        rt.anchorMax = new Vector2(0.5f, 0f);
        rt.pivot = new Vector2(0.5f, 0f);
        rt.sizeDelta = new Vector2(150, 40);
        rt.anchoredPosition = new Vector2(0, 8);
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

    private static Sprite starSprite;

    private static Sprite GetStarSprite()
    {
        if (starSprite != null) return starSprite;

        int size = 64;
        Texture2D tex = new Texture2D(size, size);
        Vector2 c = new Vector2(size / 2f, size / 2f);
        float outer = size / 2f - 2f;
        float inner = outer * 0.45f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 p = new Vector2(x, y) - c;
                float angle = Mathf.Atan2(p.y, p.x);
                float r = p.magnitude;

                float a = angle + Mathf.PI / 2f;
                float sector = Mathf.Repeat(a, Mathf.PI * 2f / 5f);
                float half = Mathf.PI / 5f;
                float t = Mathf.Abs(sector - half) / half;
                float edge = Mathf.Lerp(inner, outer, t);

                tex.SetPixel(x, y, r <= edge ? Color.white : new Color(1, 1, 1, 0));
            }
        }
        tex.Apply();
        starSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        return starSprite;
    }
}
