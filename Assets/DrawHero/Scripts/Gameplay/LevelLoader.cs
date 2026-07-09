using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public LevelDatabase database;

    public Color enemyColor = new Color(0.85f, 0.30f, 0.35f, 1f);
    public Color blockColor = new Color(0.30f, 0.30f, 0.45f, 1f);
    public Color boxColor = new Color(0.55f, 0.40f, 0.25f, 1f);

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
        GameObject root = new GameObject("Enemies");

        for (int i = 0; i < activeLevel.enemies.Count; i++)
        {
            EnemySpawn spawn = activeLevel.enemies[i];

            GameObject enemy = new GameObject("Enemy");
            enemy.transform.SetParent(root.transform);
            enemy.transform.position = spawn.position;

            SpriteRenderer sr = enemy.AddComponent<SpriteRenderer>();
            sr.sprite = PrimitiveSprites.Circle();
            sr.color = enemyColor;
            sr.sortingOrder = 3;

            Rigidbody2D body = enemy.AddComponent<Rigidbody2D>();
            body.mass = 3f;
            body.gravityScale = 1f;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            CircleCollider2D col = enemy.AddComponent<CircleCollider2D>();
            col.radius = 0.5f;

            Enemy e = enemy.AddComponent<Enemy>();
            e.maxHP = spawn.hp;
        }
    }

    private void SpawnBlocks()
    {
        GameObject root = new GameObject("Blocks");

        for (int i = 0; i < activeLevel.blocks.Count; i++)
        {
            BlockSpawn spawn = activeLevel.blocks[i];

            GameObject block = new GameObject("Block");
            block.transform.SetParent(root.transform);
            block.transform.position = spawn.position;
            block.transform.rotation = Quaternion.Euler(0, 0, spawn.rotation);

            SpriteRenderer sr = block.AddComponent<SpriteRenderer>();
            sr.sprite = PrimitiveSprites.Square();
            sr.color = blockColor;
            sr.drawMode = SpriteDrawMode.Sliced;
            sr.size = spawn.size;
            sr.sortingOrder = 1;

            BoxCollider2D col = block.AddComponent<BoxCollider2D>();
            col.size = spawn.size;
        }
    }

    private void SpawnBoxes()
    {
        GameObject root = new GameObject("Boxes");

        for (int i = 0; i < activeLevel.boxes.Count; i++)
        {
            BoxSpawn spawn = activeLevel.boxes[i];

            GameObject box = new GameObject("Box");
            box.transform.SetParent(root.transform);
            box.transform.position = spawn.position;

            SpriteRenderer sr = box.AddComponent<SpriteRenderer>();
            sr.sprite = PrimitiveSprites.Square();
            sr.color = boxColor;
            sr.drawMode = SpriteDrawMode.Sliced;
            sr.size = spawn.size;
            sr.sortingOrder = 2;

            Rigidbody2D body = box.AddComponent<Rigidbody2D>();
            body.mass = spawn.mass;
            body.gravityScale = 1f;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            BoxCollider2D col = box.AddComponent<BoxCollider2D>();
            col.size = spawn.size;
        }
    }
}
