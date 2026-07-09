using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LevelMapController : MonoBehaviour
{
    public Transform levelButtonContainer;
    public Button levelButtonPrefab;
    public Button backButton;

    private void Start()
    {
        BuildLevelButtons();

        backButton.onClick.AddListener(OnBackClicked);
    }

    private void BuildLevelButtons()
    {
        for (int i = 1; i <= GameSession.TotalLevels; i++)
        {
            int levelIndex = i;
            Button btn = Instantiate(levelButtonPrefab, levelButtonContainer);
            btn.gameObject.SetActive(true);

            TMPro.TextMeshProUGUI label = btn.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (label != null)
                label.text = levelIndex.ToString();

            btn.onClick.AddListener(() => OnLevelClicked(levelIndex));

            RectTransform rt = btn.GetComponent<RectTransform>();
            rt.localScale = Vector3.zero;
            rt.DOScale(1f, 0.35f).SetEase(Ease.OutBack).SetDelay(0.03f * i);
        }
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
