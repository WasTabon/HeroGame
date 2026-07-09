using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TutorialHint : MonoBehaviour
{
    private CanvasGroup group;
    private bool dismissed;

    private void Start()
    {
        if (GameSession.SelectedLevel > 2)
        {
            Destroy(gameObject);
            return;
        }

        Build();
    }

    private void Build()
    {
        GameObject canvasGo = new GameObject("TutorialCanvas");
        canvasGo.transform.SetParent(transform);
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 500;
        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;

        GameObject textGo = new GameObject("HintText", typeof(RectTransform), typeof(CanvasGroup));
        textGo.transform.SetParent(canvasGo.transform, false);
        TextMeshProUGUI tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.text = GameSession.SelectedLevel == 1
            ? "Draw a shape above the enemy\nand let it fall!"
            : "Use heavy shapes to knock\nenemies off the level!";
        tmp.fontSize = 44;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;

        RectTransform rt = textGo.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(900, 300);
        rt.anchoredPosition = new Vector2(0, 300);

        group = textGo.GetComponent<CanvasGroup>();
        group.alpha = 0f;
        group.DOFade(1f, 0.5f);

        rt.DOAnchorPosY(340, 1.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    private void Update()
    {
        if (dismissed) return;
        if (Input.GetMouseButtonDown(0))
        {
            dismissed = true;
            if (group != null)
                group.DOFade(0f, 0.4f).OnComplete(() => Destroy(gameObject));
        }
    }
}
