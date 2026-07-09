using UnityEngine;

public static class ManagerBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (SoundManager.Instance == null)
        {
            GameObject go = new GameObject("SoundManager");
            go.AddComponent<SoundManager>();
        }

        if (HapticManager.Instance == null)
        {
            GameObject go = new GameObject("HapticManager");
            go.AddComponent<HapticManager>();
        }

        if (TransitionManager.Instance == null)
        {
            GameObject go = new GameObject("TransitionManager");
            go.AddComponent<TransitionManager>();
        }

        if (IAPManager.Instance == null)
        {
            GameObject go = new GameObject("IAPManager");
            go.AddComponent<IAPManager>();
            Object.DontDestroyOnLoad(go);
        }

        Application.targetFrameRate = 60;
    }
}
