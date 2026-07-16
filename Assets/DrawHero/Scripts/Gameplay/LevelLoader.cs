using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public LevelDatabase database;

    public GameObject enemyPrefab;
    public GameObject blockPrefab;
    public GameObject boxPrefab;

    private LevelData activeLevel;

    private void Awake()
    {
        LoadLevel(GameSession.SelectedLevel);
    }

    public LevelData ActiveLevel => activeLevel;

    private void LoadLevel(int levelNumber)
    {
        LevelData raw = database.GetLevel(levelNumber);
        if (raw == null)
        {
            Debug.LogWarning("LevelData not found for level " + levelNumber);
            return;
        }

        activeLevel = raw.Resolve();

        SpawnBlocks();
        SpawnBoxes();
        SpawnEnemies();

        if (InkManager.Instance != null)
            InkManager.Instance.SetMaxInk(activeLevel.inkLimit);

        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
            gm.defaultInk = activeLevel.inkLimit;
    }

    private void SpawnEnemies()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("LevelLoader: enemyPrefab is not assigned!");
            return;
        }

        GameObject root = new GameObject("Enemies");

        for (int i = 0; i < activeLevel.enemies.Count; i++)
        {
            EnemySpawn spawn = activeLevel.enemies[i];

            GameObject instance = Instantiate(enemyPrefab, spawn.position, Quaternion.identity, root.transform);
            instance.name = "Enemy_" + i;

            Enemy enemy = instance.GetComponent<Enemy>();
            if (enemy != null)
                enemy.maxHP = spawn.hp;
        }
    }

    private void SpawnBlocks()
    {
        if (blockPrefab == null)
        {
            Debug.LogError("LevelLoader: blockPrefab is not assigned!");
            return;
        }

        GameObject root = new GameObject("Blocks");

        for (int i = 0; i < activeLevel.blocks.Count; i++)
        {
            BlockSpawn spawn = activeLevel.blocks[i];

            GameObject instance = Instantiate(blockPrefab, spawn.position,
                Quaternion.Euler(0, 0, spawn.rotation), root.transform);
            instance.name = "Block_" + i;

            ApplySize(instance, spawn.size);
        }
    }

    private void SpawnBoxes()
    {
        if (boxPrefab == null)
        {
            Debug.LogError("LevelLoader: boxPrefab is not assigned!");
            return;
        }

        GameObject root = new GameObject("Boxes");

        for (int i = 0; i < activeLevel.boxes.Count; i++)
        {
            BoxSpawn spawn = activeLevel.boxes[i];

            GameObject instance = Instantiate(boxPrefab, spawn.position, Quaternion.identity, root.transform);
            instance.name = "Box_" + i;

            ApplySize(instance, spawn.size);

            Rigidbody2D body = instance.GetComponent<Rigidbody2D>();
            if (body != null)
                body.mass = spawn.mass;
        }
    }

    private void ApplySize(GameObject instance, Vector2 size)
    {
        SpriteRenderer sr = instance.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            if (sr.drawMode == SpriteDrawMode.Simple)
                sr.drawMode = SpriteDrawMode.Sliced;
            sr.size = size;
        }

        BoxCollider2D col = instance.GetComponent<BoxCollider2D>();
        if (col != null)
            col.size = size;
    }
}
