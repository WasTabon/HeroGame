using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

public class SetupIteration6
{
    [MenuItem("Tools/DrawHero/Setup Iteration 6 (IAP + PowerUps)")]
    public static void SetupAll()
    {
        string scenePath = "Assets/DrawHero/Scenes/Game.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        GameObject oldBar = GameObject.Find("PowerUpBar");
        if (oldBar != null)
            Object.DestroyImmediate(oldBar);

        GameObject canvasGo = GameObject.Find("GameCanvas");
        if (canvasGo == null)
        {
            Debug.LogError("GameCanvas not found. Run earlier iteration setups first.");
            return;
        }

        Transform safeArea = canvasGo.transform.Find("SafeArea");
        if (safeArea == null)
        {
            Debug.LogError("SafeArea not found under GameCanvas.");
            return;
        }

        Transform existing = safeArea.Find("PowerUpBar");
        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);

        GameObject bar = new GameObject("PowerUpBar", typeof(RectTransform));
        bar.transform.SetParent(safeArea, false);
        RectTransform barRt = bar.GetComponent<RectTransform>();
        barRt.anchorMin = new Vector2(0.5f, 0f);
        barRt.anchorMax = new Vector2(0.5f, 0f);
        barRt.pivot = new Vector2(0.5f, 0f);
        barRt.sizeDelta = new Vector2(700, 200);
        barRt.anchoredPosition = new Vector2(0, 40);

        PowerUpBar script = bar.AddComponent<PowerUpBar>();

        Image heavyIcon, extraIcon, bombIcon;
        TextMeshProUGUI heavyCount, extraCount, bombCount;

        Button heavyBtn = CreatePowerButton(bar.transform, "HeavyButton", "HEAVY",
            new Color(0.5f, 0.5f, 0.6f), new Vector2(-230, 0), out heavyIcon, out heavyCount);
        Button extraBtn = CreatePowerButton(bar.transform, "ExtraButton", "INK+",
            new Color(0.35f, 0.7f, 0.4f), new Vector2(0, 0), out extraIcon, out extraCount);
        Button bombBtn = CreatePowerButton(bar.transform, "BombButton", "BOMB",
            new Color(0.9f, 0.3f, 0.2f), new Vector2(230, 0), out bombIcon, out bombCount);

        script.heavyButton = heavyBtn;
        script.extraButton = extraBtn;
        script.bombButton = bombBtn;
        script.heavyIcon = heavyIcon;
        script.extraIcon = extraIcon;
        script.bombIcon = bombIcon;
        script.heavyCount = heavyCount;
        script.extraCount = extraCount;
        script.bombCount = bombCount;

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("DrawHero Iteration 6: PowerUpBar built in scene under GameCanvas/SafeArea. Replace sprites on the Icon child of each button.");
    }

    private static Button CreatePowerButton(Transform parent, string objectName, string label, Color color, Vector2 pos, out Image icon, out TextMeshProUGUI countLabel)
    {
        GameObject go = new GameObject(objectName, typeof(RectTransform), typeof(Image), typeof(Button), typeof(ButtonPunch));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = color;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(180, 180);
        rt.anchoredPosition = pos;

        GameObject iconGo = new GameObject("Icon", typeof(RectTransform), typeof(Image));
        iconGo.transform.SetParent(go.transform, false);
        icon = iconGo.GetComponent<Image>();
        icon.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        icon.color = new Color(1f, 1f, 1f, 0.9f);
        icon.preserveAspect = true;
        RectTransform iconRt = iconGo.GetComponent<RectTransform>();
        iconRt.anchorMin = new Vector2(0.5f, 1f);
        iconRt.anchorMax = new Vector2(0.5f, 1f);
        iconRt.pivot = new Vector2(0.5f, 1f);
        iconRt.sizeDelta = new Vector2(90, 90);
        iconRt.anchoredPosition = new Vector2(0, -15);

        GameObject nameGo = new GameObject("Name", typeof(RectTransform));
        nameGo.transform.SetParent(go.transform, false);
        TextMeshProUGUI nameTmp = nameGo.AddComponent<TextMeshProUGUI>();
        nameTmp.text = label;
        nameTmp.fontSize = 26;
        nameTmp.color = Color.white;
        nameTmp.alignment = TextAlignmentOptions.Center;
        nameTmp.fontStyle = FontStyles.Bold;
        RectTransform nameRt = nameGo.GetComponent<RectTransform>();
        nameRt.anchorMin = new Vector2(0f, 0.28f);
        nameRt.anchorMax = new Vector2(1f, 0.48f);
        nameRt.offsetMin = Vector2.zero;
        nameRt.offsetMax = Vector2.zero;

        GameObject countGo = new GameObject("Count", typeof(RectTransform));
        countGo.transform.SetParent(go.transform, false);
        countLabel = countGo.AddComponent<TextMeshProUGUI>();
        countLabel.text = "x0";
        countLabel.fontSize = 32;
        countLabel.color = new Color(1f, 0.95f, 0.7f);
        countLabel.alignment = TextAlignmentOptions.Center;
        countLabel.fontStyle = FontStyles.Bold;
        RectTransform countRt = countGo.GetComponent<RectTransform>();
        countRt.anchorMin = new Vector2(0f, 0.04f);
        countRt.anchorMax = new Vector2(1f, 0.28f);
        countRt.offsetMin = Vector2.zero;
        countRt.offsetMax = Vector2.zero;

        return go.GetComponent<Button>();
    }
}
