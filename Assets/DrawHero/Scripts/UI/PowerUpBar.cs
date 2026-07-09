using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PowerUpBar : MonoBehaviour
{
    private TextMeshProUGUI heavyCount;
    private TextMeshProUGUI extraCount;
    private TextMeshProUGUI bombCount;

    private RectTransform heavyBtn;
    private RectTransform extraBtn;
    private RectTransform bombBtn;

    private bool heavyActive;
    private bool bombActive;

    private void Start()
    {
        Build();
        Refresh();

        if (IAPManager.Instance != null)
        {
            IAPManager.Instance.OnPowerPackPurchased -= HandlePurchased;
            IAPManager.Instance.OnPowerPackPurchased += HandlePurchased;
        }
    }

    private void OnDestroy()
    {
        if (IAPManager.Instance != null)
            IAPManager.Instance.OnPowerPackPurchased -= HandlePurchased;
    }

    private void HandlePurchased()
    {
        Refresh();
    }

    private void Build()
    {
        GameObject canvasGo = new GameObject("PowerUpCanvas");
        canvasGo.transform.SetParent(transform);
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 400;
        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();

        GameObject row = new GameObject("Row", typeof(RectTransform));
        row.transform.SetParent(canvasGo.transform, false);
        RectTransform rowRt = row.GetComponent<RectTransform>();
        rowRt.anchorMin = new Vector2(0.5f, 0f);
        rowRt.anchorMax = new Vector2(0.5f, 0f);
        rowRt.pivot = new Vector2(0.5f, 0f);
        rowRt.sizeDelta = new Vector2(700, 200);
        rowRt.anchoredPosition = new Vector2(0, 40);

        heavyBtn = CreateButton(row.transform, "HEAVY", new Color(0.5f, 0.5f, 0.6f), new Vector2(-230, 0), OnHeavy, out heavyCount);
        extraBtn = CreateButton(row.transform, "INK+", new Color(0.35f, 0.7f, 0.4f), new Vector2(0, 0), OnExtra, out extraCount);
        bombBtn = CreateButton(row.transform, "BOMB", new Color(0.9f, 0.3f, 0.2f), new Vector2(230, 0), OnBomb, out bombCount);
    }

    private void Refresh()
    {
        heavyCount.text = "x" + PowerUpManager.GetHeavy();
        extraCount.text = "x" + PowerUpManager.GetExtra();
        bombCount.text = "x" + PowerUpManager.GetBomb();

        SetHighlight(heavyBtn, heavyActive);
        SetHighlight(bombBtn, bombActive);
    }

    private void SetHighlight(RectTransform btn, bool active)
    {
        btn.DOScale(active ? 1.15f : 1f, 0.2f).SetEase(Ease.OutBack);
    }

    private void OnHeavy()
    {
        if (heavyActive)
        {
            heavyActive = false;
            PowerUpManager.HeavyNextDraw = false;
            PowerUpManager.AddHeavy(1);
            Refresh();
            return;
        }

        if (PowerUpManager.UseHeavy())
        {
            heavyActive = true;
            PlayClick();
            Refresh();
        }
        else
        {
            ShopPanel.Open();
        }
    }

    private void OnBomb()
    {
        if (bombActive)
        {
            bombActive = false;
            PowerUpManager.BombNextDraw = false;
            PowerUpManager.AddBomb(1);
            Refresh();
            return;
        }

        if (PowerUpManager.UseBomb())
        {
            bombActive = true;
            PlayClick();
            Refresh();
        }
        else
        {
            ShopPanel.Open();
        }
    }

    private void OnExtra()
    {
        if (PowerUpManager.UseExtra())
        {
            if (InkManager.Instance != null)
                InkManager.Instance.SetMaxInk(InkManager.Instance.MaxInk * 1.5f);
            PlayClick();
            Refresh();
        }
        else
        {
            ShopPanel.Open();
        }
    }

    private void PlayClick()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySfx("click");
    }

    private void Update()
    {
        if (heavyActive && !PowerUpManager.HeavyNextDraw)
        {
            heavyActive = false;
            Refresh();
        }
        if (bombActive && !PowerUpManager.BombNextDraw)
        {
            bombActive = false;
            Refresh();
        }
    }

    private RectTransform CreateButton(Transform parent, string label, Color color, Vector2 pos, UnityEngine.Events.UnityAction onClick, out TextMeshProUGUI countLabel)
    {
        GameObject go = new GameObject(label + "PU", typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = color;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(180, 180);
        rt.anchoredPosition = pos;

        go.GetComponent<Button>().onClick.AddListener(onClick);

        GameObject nameGo = new GameObject("Name", typeof(RectTransform));
        nameGo.transform.SetParent(go.transform, false);
        TextMeshProUGUI nameTmp = nameGo.AddComponent<TextMeshProUGUI>();
        nameTmp.text = label;
        nameTmp.fontSize = 30;
        nameTmp.color = Color.white;
        nameTmp.alignment = TextAlignmentOptions.Center;
        nameTmp.fontStyle = FontStyles.Bold;
        RectTransform nameRt = nameGo.GetComponent<RectTransform>();
        nameRt.anchorMin = new Vector2(0, 0.4f);
        nameRt.anchorMax = new Vector2(1, 1f);
        nameRt.offsetMin = Vector2.zero;
        nameRt.offsetMax = Vector2.zero;

        GameObject countGo = new GameObject("Count", typeof(RectTransform));
        countGo.transform.SetParent(go.transform, false);
        countLabel = countGo.AddComponent<TextMeshProUGUI>();
        countLabel.text = "x0";
        countLabel.fontSize = 34;
        countLabel.color = new Color(1f, 0.95f, 0.7f);
        countLabel.alignment = TextAlignmentOptions.Center;
        countLabel.fontStyle = FontStyles.Bold;
        RectTransform countRt = countGo.GetComponent<RectTransform>();
        countRt.anchorMin = new Vector2(0, 0f);
        countRt.anchorMax = new Vector2(1, 0.4f);
        countRt.offsetMin = Vector2.zero;
        countRt.offsetMax = Vector2.zero;

        return rt;
    }
}