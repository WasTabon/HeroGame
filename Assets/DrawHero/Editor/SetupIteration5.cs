using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

public class SetupIteration5
{
    [MenuItem("Tools/DrawHero/Setup Iteration 5 (Polish)")]
    public static void SetupAll()
    {
        SetupGameScene();
        SetupLevelMap();

        Debug.Log("DrawHero Iteration 5: camera shake, tutorial, settings gear added.");
    }

    private static void SetupGameScene()
    {
        string scenePath = "Assets/DrawHero/Scenes/Game.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        Camera cam = Camera.main;
        if (cam != null && cam.GetComponent<CameraShake>() == null)
            cam.gameObject.AddComponent<CameraShake>();

        if (GameObject.Find("TutorialHint") == null)
        {
            GameObject tut = new GameObject("TutorialHint");
            tut.AddComponent<TutorialHint>();
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static void SetupLevelMap()
    {
        string scenePath = "Assets/DrawHero/Scenes/LevelMap.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null) return;

        Transform safeArea = canvas.transform.Find("SafeArea");
        if (safeArea == null) return;

        if (safeArea.Find("SettingsGear") != null)
        {
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            return;
        }

        GameObject gear = new GameObject("SettingsGear", typeof(RectTransform), typeof(Image), typeof(Button), typeof(ButtonPunch), typeof(SettingsGearButton));
        gear.transform.SetParent(safeArea, false);
        gear.GetComponent<Image>().color = new Color(0.290f, 0.565f, 0.886f);
        RectTransform rt = gear.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(1f, 1f);
        rt.sizeDelta = new Vector2(100, 100);
        rt.anchoredPosition = new Vector2(-40, -40);

        GameObject textGo = new GameObject("Label", typeof(RectTransform));
        textGo.transform.SetParent(gear.transform, false);
        TextMeshProUGUI tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.text = "=";
        tmp.fontSize = 50;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        RectTransform textRt = textGo.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }
}
