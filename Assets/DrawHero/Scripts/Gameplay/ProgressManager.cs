using UnityEngine;

public static class ProgressManager
{
    private const string STAR_KEY = "level_stars_";

    public static int GetStars(int level)
    {
        return PlayerPrefs.GetInt(STAR_KEY + level, 0);
    }

    public static void SetStars(int level, int stars)
    {
        int existing = GetStars(level);
        if (stars > existing)
        {
            PlayerPrefs.SetInt(STAR_KEY + level, stars);
            PlayerPrefs.Save();
        }
    }

    public static int GetTotalStars()
    {
        int total = 0;
        for (int i = 1; i <= GameSession.TotalLevels; i++)
            total += GetStars(i);
        return total;
    }

    public static int GetUnlockCost(int level)
    {
        if (level <= 1) return 0;
        return (level - 1) * 2;
    }

    public static bool IsUnlocked(int level)
    {
        if (level <= 1) return true;
        return GetTotalStars() >= GetUnlockCost(level);
    }

    public static void ResetAllProgress()
    {
        for (int i = 1; i <= GameSession.TotalLevels; i++)
            PlayerPrefs.DeleteKey(STAR_KEY + i);
        PlayerPrefs.Save();
    }
}
