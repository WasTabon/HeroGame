using UnityEngine;

public class HapticManager : MonoBehaviour
{
    public static HapticManager Instance;

    private const string HAPTIC_KEY = "haptic_enabled";

    public bool HapticEnabled { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        HapticEnabled = PlayerPrefs.GetInt(HAPTIC_KEY, 1) == 1;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void Light()
    {
        if (!HapticEnabled) return;
#if UNITY_IOS || UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }

    public void Medium()
    {
        if (!HapticEnabled) return;
#if UNITY_IOS || UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }

    public void Heavy()
    {
        if (!HapticEnabled) return;
#if UNITY_IOS || UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }

    public void ToggleHaptic()
    {
        HapticEnabled = !HapticEnabled;
        PlayerPrefs.SetInt(HAPTIC_KEY, HapticEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }
}
