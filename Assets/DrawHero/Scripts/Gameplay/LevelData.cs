using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawn
{
    public Vector2 position;
    public float hp = 30f;
}

[System.Serializable]
public class BlockSpawn
{
    public Vector2 position;
    public Vector2 size = new Vector2(4, 1);
    public float rotation = 0f;
}

[System.Serializable]
public class BoxSpawn
{
    public Vector2 position;
    public Vector2 size = new Vector2(1.2f, 1.2f);
    public float mass = 2f;
}

[CreateAssetMenu(fileName = "LevelData", menuName = "DrawHero/Level Data")]
public class LevelData : ScriptableObject
{
    public int levelNumber = 1;
    public float inkLimit = 100f;

    public List<EnemySpawn> enemies = new List<EnemySpawn>();
    public List<BlockSpawn> blocks = new List<BlockSpawn>();
    public List<BoxSpawn> boxes = new List<BoxSpawn>();

    public bool isClone = false;
    public LevelData sourceLevel;

    public LevelData Resolve()
    {
        if (isClone && sourceLevel != null)
            return sourceLevel;
        return this;
    }
}
