using UnityEngine;

[CreateAssetMenu(fileName = "LevelDatabase", menuName = "DrawHero/Level Database")]
public class LevelDatabase : ScriptableObject
{
    public LevelData[] levels;

    public LevelData GetLevel(int levelNumber)
    {
        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i] != null && levels[i].levelNumber == levelNumber)
                return levels[i];
        }
        return null;
    }
}
