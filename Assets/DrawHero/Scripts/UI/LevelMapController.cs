using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class LevelMapController : MonoBehaviour
{
    public Transform levelButtonContainer;
    public Button levelButtonPrefab;
    public Button backButton;
    public TextMeshProUGUI totalStarsText;

    private void Start()
    {
        BuildLevelButtons();

        backButton.onClick.AddListener(OnBackClicked);

        if (totalStarsText != null)
            totalStarsText.text = "Stars: " + ProgressManager.GetTotalStars();
    }

    private void BuildLevelButtons()
    {
        for (int i = 1; i <= GameSession.TotalLevels; i++)
        {
            int levelIndex = i;
            Button btn = Instantiate(levelButtonPrefab, levelButtonContainer);
            btn.gameObject.SetActive(true);

            bool unlocked = ProgressManager.IsUnlocked(levelIndex);
            int stars = ProgressManager.GetStars(levelIndex);

            TextMeshProUGUI label = FindLabel(btn.transform, "Label");
            if (label != null)
                label.text = unlocked ? levelIndex.ToString() : "?";

            TextMeshProUGUI starsLabel = FindLabel(btn.transform, "Stars");
            if (starsLabel != null)
                starsLabel.text = unlocked ? BuildStarString(stars) : LockCost(levelIndex);

            Image img = btn.GetComponent<Image>();
            if (!unlocked)
                img.color = new Color(0.3f, 0.3f, 0.4f, 1f);

            btn.interactable = unlocked;

            if (unlocked)
                btn.onClick.AddListener(() => OnLevelClicked(levelIndex));

            RectTransform rt = btn.GetComponent<RectTransform>();
            rt.localScale = Vector3.zero;
            rt.DOScale(1f, 0.35f).SetEase(Ease.OutBack).SetDelay(0.03f * i);
        }
    }

    private string BuildStarString(int stars)
    {
        string s = "";
        for (int i = 0; i < 3; i++)
            s += i < stars ? "*" : "-";
        return s;
    }

    private string LockCost(int level)
    {
        return ProgressManager.GetUnlockCost(level) + "*";
    }

    private TextMeshProUGUI FindLabel(Transform root, string childName)
    {
        Transform t = root.Find(childName);
        if (t != null) return t.GetComponent<TextMeshProUGUI>();
        return null;
    }

    private void OnLevelClicked(int levelIndex)
    {
        GameSession.SelectedLevel = levelIndex;
        TransitionManager.Instance.LoadScene("Game");
    }

    private void OnBackClicked()
    {
        TransitionManager.Instance.LoadScene("MainMenu");
    }
}
