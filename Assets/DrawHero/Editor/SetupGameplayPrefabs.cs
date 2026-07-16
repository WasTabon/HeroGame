using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SetupGameplayPrefabs
{
    private const string PREFAB_DIR = "Assets/DrawHero/Prefabs";

    private static readonly Color ColorEnemy = new Color(0.85f, 0.30f, 0.35f, 1f);
    private static readonly Color ColorBlock = new Color(0.30f, 0.30f, 0.45f, 1f);
    private static readonly Color ColorBox = new Color(0.55f, 0.40f, 0.25f, 1f);

    [MenuItem("Tools/DrawHero/Generate Gameplay Prefabs")]
    public static void GenerateAll()
    {
        if (!System.IO.Directory.Exists(PREFAB_DIR))
            System.IO.Directory.CreateDirectory(PREFAB_DIR);

        GameObject enemyPrefab = BuildEnemyPrefab();
        GameObject blockPrefab = BuildBlockPrefab();
        GameObject boxPrefab = BuildBoxPrefab();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        WireToLevelLoader(enemyPrefab, blockPrefab, boxPrefab);

        Debug.Log("DrawHero: Enemy/Block/Box prefabs created in " + PREFAB_DIR + " and wired to LevelLoader. Replace sprites there.");
    }

    private static GameObject BuildEnemyPrefab()
    {
        GameObject go = new GameObject("Enemy");

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = GetRoundSprite();
        sr.color = ColorEnemy;
        sr.sortingOrder = 3;

        Rigidbody2D body = go.AddComponent<Rigidbody2D>();
        body.mass = 3f;
        body.gravityScale = 1f;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        CircleCollider2D col = go.AddComponent<CircleCollider2D>();
        col.radius = 0.5f;

        Enemy enemy = go.AddComponent<Enemy>();
        enemy.maxHP = 30f;

        string path = PREFAB_DIR + "/Enemy.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return prefab;
    }

    private static GameObject BuildBlockPrefab()
    {
        GameObject go = new GameObject("Block");

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = GetSlicedSprite();
        sr.color = ColorBlock;
        sr.drawMode = SpriteDrawMode.Sliced;
        sr.size = new Vector2(4, 1);
        sr.sortingOrder = 1;

        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.size = new Vector2(4, 1);

        string path = PREFAB_DIR + "/Block.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return prefab;
    }

    private static GameObject BuildBoxPrefab()
    {
        GameObject go = new GameObject("Box");

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = GetSlicedSprite();
        sr.color = ColorBox;
        sr.drawMode = SpriteDrawMode.Sliced;
        sr.size = new Vector2(1.2f, 1.2f);
        sr.sortingOrder = 2;

        Rigidbody2D body = go.AddComponent<Rigidbody2D>();
        body.mass = 2f;
        body.gravityScale = 1f;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.size = new Vector2(1.2f, 1.2f);

        string path = PREFAB_DIR + "/Box.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return prefab;
    }

    private static void WireToLevelLoader(GameObject enemy, GameObject block, GameObject box)
    {
        string scenePath = "Assets/DrawHero/Scenes/Game.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        GameObject loaderGo = GameObject.Find("LevelLoader");
        if (loaderGo == null)
        {
            Debug.LogError("LevelLoader not found in Game scene. Run Iteration 4 setup first.");
            return;
        }

        LevelLoader loader = loaderGo.GetComponent<LevelLoader>();
        if (loader == null)
        {
            Debug.LogError("LevelLoader component missing.");
            return;
        }

        loader.enemyPrefab = enemy;
        loader.blockPrefab = block;
        loader.boxPrefab = box;

        EditorUtility.SetDirty(loader);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static Sprite GetRoundSprite()
    {
        return AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
    }

    private static Sprite GetSlicedSprite()
    {
        return AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
    }
}
