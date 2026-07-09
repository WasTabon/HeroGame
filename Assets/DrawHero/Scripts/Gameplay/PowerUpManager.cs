using UnityEngine;

public static class PowerUpManager
{
    private const string HEAVY_KEY = "pu_heavy";
    private const string EXTRA_KEY = "pu_extra";
    private const string BOMB_KEY = "pu_bomb";

    public static bool HeavyNextDraw;
    public static bool BombNextDraw;

    public static int GetHeavy() => PlayerPrefs.GetInt(HEAVY_KEY, 0);
    public static int GetExtra() => PlayerPrefs.GetInt(EXTRA_KEY, 0);
    public static int GetBomb() => PlayerPrefs.GetInt(BOMB_KEY, 0);

    public static void AddHeavy(int n) => SetCount(HEAVY_KEY, GetHeavy() + n);
    public static void AddExtra(int n) => SetCount(EXTRA_KEY, GetExtra() + n);
    public static void AddBomb(int n) => SetCount(BOMB_KEY, GetBomb() + n);

    public static void GrantPowerPack()
    {
        AddHeavy(1);
        AddExtra(1);
        AddBomb(1);
    }

    public static bool UseHeavy()
    {
        if (GetHeavy() <= 0) return false;
        SetCount(HEAVY_KEY, GetHeavy() - 1);
        HeavyNextDraw = true;
        return true;
    }

    public static bool UseBomb()
    {
        if (GetBomb() <= 0) return false;
        SetCount(BOMB_KEY, GetBomb() - 1);
        BombNextDraw = true;
        return true;
    }

    public static bool UseExtra()
    {
        if (GetExtra() <= 0) return false;
        SetCount(EXTRA_KEY, GetExtra() - 1);
        return true;
    }

    public static void ClearPendingDrawFlags()
    {
        HeavyNextDraw = false;
        BombNextDraw = false;
    }

    private static void SetCount(string key, int value)
    {
        PlayerPrefs.SetInt(key, Mathf.Max(0, value));
        PlayerPrefs.Save();
    }
}
