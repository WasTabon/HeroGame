using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenuController : MonoBehaviour
{
    public Button playButton;
    public RectTransform titleTransform;

    private void Start()
    {
        playButton.onClick.AddListener(OnPlayClicked);

        titleTransform.localScale = Vector3.zero;
        titleTransform.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetDelay(0.2f);
    }

    private void OnPlayClicked()
    {
        TransitionManager.Instance.LoadScene("LevelMap");
    }
}
