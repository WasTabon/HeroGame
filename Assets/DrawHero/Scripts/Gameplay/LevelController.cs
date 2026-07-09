using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class LevelController : MonoBehaviour
{
    public ResultPopup resultPopup;

    private List<Enemy> enemies = new List<Enemy>();
    private bool levelEnded;

    private float loseCheckTimer;
    private const float SETTLE_TIME = 1.5f;

    private void Start()
    {
        RegisterExistingEnemies();
    }

    private void RegisterExistingEnemies()
    {
        enemies.Clear();
        Enemy[] found = FindObjectsOfType<Enemy>();
        for (int i = 0; i < found.Length; i++)
            RegisterEnemy(found[i]);
    }

    public void RegisterEnemy(Enemy enemy)
    {
        if (enemies.Contains(enemy)) return;
        enemies.Add(enemy);
        enemy.OnDeath -= HandleEnemyDeath;
        enemy.OnDeath += HandleEnemyDeath;
    }

    private void HandleEnemyDeath(Enemy enemy)
    {
        enemies.Remove(enemy);

        if (levelEnded) return;

        if (enemies.Count == 0)
            Win();
    }

    private void Update()
    {
        if (levelEnded) return;
        if (enemies.Count == 0) return;

        if (InkManager.Instance == null) return;

        if (InkManager.Instance.HasInkLeft(1f))
        {
            loseCheckTimer = 0f;
            return;
        }

        if (IsSceneSettled())
        {
            loseCheckTimer += Time.deltaTime;
            if (loseCheckTimer >= SETTLE_TIME)
                Lose();
        }
        else
        {
            loseCheckTimer = 0f;
        }
    }

    private bool IsSceneSettled()
    {
        Rigidbody2D[] bodies = FindObjectsOfType<Rigidbody2D>();
        for (int i = 0; i < bodies.Length; i++)
        {
            if (bodies[i].bodyType != RigidbodyType2D.Dynamic) continue;
            if (bodies[i].velocity.magnitude > 0.3f)
                return false;
        }
        return true;
    }

    private void Win()
    {
        if (levelEnded) return;
        levelEnded = true;

        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
            gm.SetInteractable(false);

        int stars = 1;
        if (InkManager.Instance != null)
            stars = StarEvaluator.Evaluate(InkManager.Instance.UsedInk, InkManager.Instance.MaxInk);

        ProgressManager.SetStars(GameSession.SelectedLevel, stars);

        BuildAndShowPopup(true, stars);
    }

    private void Lose()
    {
        if (levelEnded) return;
        levelEnded = true;

        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
            gm.SetInteractable(false);

        BuildAndShowPopup(false, 0);
    }

    public void ResetLevel()
    {
        levelEnded = false;
        loseCheckTimer = 0f;
    }

    private void BuildAndShowPopup(bool win, int stars)
    {
        GameObject canvasGo = new GameObject("ResultPopupCanvas");
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32767;
        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();

        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            new GameObject("EventSystem",
                typeof(UnityEngine.EventSystems.EventSystem),
                typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }

        GameObject backdrop = new GameObject("Backdrop", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
        backdrop.transform.SetParent(canvasGo.transform, false);
        backdrop.GetComponent<Image>().color = Color.black;
        RectTransform bdRt = backdrop.GetComponent<RectTransform>();
        bdRt.anchorMin = Vector2.zero;
        bdRt.anchorMax = Vector2.one;
        bdRt.offsetMin = Vector2.zero;
        bdRt.offsetMax = Vector2.zero;
        CanvasGroup bdGroup = backdrop.GetComponent<CanvasGroup>();
        bdGroup.alpha = 0f;
        bdGroup.DOFade(0.6f, 0.3f);

        GameObject panel = new GameObject("Panel", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(canvasGo.transform, false);
        panel.GetComponent<Image>().color = new Color(0.14f, 0.14f, 0.24f, 1f);
        RectTransform panelRt = panel.GetComponent<RectTransform>();
        panelRt.anchorMin = new Vector2(0.5f, 0.5f);
        panelRt.anchorMax = new Vector2(0.5f, 0.5f);
        panelRt.pivot = new Vector2(0.5f, 0.5f);
        panelRt.sizeDelta = new Vector2(820, 1000);
        panelRt.anchoredPosition = Vector2.zero;

        CreateLabel(panel.transform, win ? "LEVEL COMPLETE" : "OUT OF INK", 60,
            win ? new Color(0.4f, 0.85f, 0.35f) : new Color(0.9f, 0.4f, 0.35f),
            new Vector2(0, -80), new Vector2(760, 140));

        if (win)
            BuildStars(panel.transform, stars);

        float btnY = win ? 60f : 120f;

        if (win)
        {
            CreateButton(panel.transform, "NEXT", new Color(0.4f, 0.75f, 0.4f),
                new Vector2(0, btnY), OnNext);
            btnY -= 200f;
        }

        CreateButton(panel.transform, "RETRY", new Color(0.290f, 0.565f, 0.886f),
            new Vector2(0, btnY), OnRetry);
        btnY -= 200f;

        CreateButton(panel.transform, "MENU", new Color(0.961f, 0.651f, 0.137f),
            new Vector2(0, btnY), OnMenu);

        panelRt.localScale = Vector3.zero;
        panelRt.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.1f);

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySfx("pop");
    }

    private void BuildStars(Transform panel, int stars)
    {
        Color earned = new Color(0.98f, 0.78f, 0.20f);
        Color empty = new Color(1f, 1f, 1f, 0.15f);
        float spacing = 200f;

        for (int i = 0; i < 3; i++)
        {
            GameObject starGo = new GameObject("Star" + i, typeof(RectTransform), typeof(Image));
            starGo.transform.SetParent(panel, false);
            Image img = starGo.GetComponent<Image>();
            img.sprite = PrimitiveSprites.Circle();
            img.color = empty;
            RectTransform rt = starGo.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.sizeDelta = new Vector2(150, 150);
            rt.anchoredPosition = new Vector2((i - 1) * spacing, -280 + (i == 1 ? 20 : 0));
            rt.localScale = Vector3.zero;

            int index = i;
            bool won = index < stars;
            rt.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.5f + index * 0.2f)
                .OnStart(() =>
                {
                    if (won)
                    {
                        img.color = earned;
                        if (SoundManager.Instance != null)
                            SoundManager.Instance.PlaySfx("pop", 1f + index * 0.15f);
                    }
                });
        }
    }

    private void CreateLabel(Transform parent, string text, float fontSize, Color color, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject("Label", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.sizeDelta = size;
        rt.anchoredPosition = pos;
    }

    private void CreateButton(Transform parent, string text, Color color, Vector2 pos, UnityEngine.Events.UnityAction onClick)
    {
        GameObject go = new GameObject(text + "Button", typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = color;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(520, 150);
        rt.anchoredPosition = pos;

        Button btn = go.GetComponent<Button>();
        btn.onClick.AddListener(onClick);

        GameObject textGo = new GameObject("Label", typeof(RectTransform));
        textGo.transform.SetParent(go.transform, false);
        TextMeshProUGUI tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 46;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        RectTransform textRt = textGo.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;
    }

    private void OnNext()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySfx("click");
        int next = GameSession.SelectedLevel + 1;
        if (next > GameSession.TotalLevels)
            next = GameSession.TotalLevels;
        GameSession.SelectedLevel = next;
        TransitionManager.Instance.LoadScene("Game");
    }

    private void OnRetry()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySfx("click");
        TransitionManager.Instance.LoadScene("Game");
    }

    private void OnMenu()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySfx("click");
        TransitionManager.Instance.LoadScene("LevelMap");
    }
}
