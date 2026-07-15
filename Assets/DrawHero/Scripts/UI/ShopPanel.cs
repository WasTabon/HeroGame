using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ShopPanel : MonoBehaviour
{
    public CanvasGroup backdrop;
    public RectTransform panel;
    public Button buyButton;
    public Button closeButton;
    public Button backdropButton;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI contentsText;
    public Image packIcon;

    public static void Open()
    {
        GameObject prefab = Resources.Load<GameObject>("ShopPanel");
        if (prefab == null)
        {
            Debug.LogError("ShopPanel prefab not found in Resources folder!");
            return;
        }

        Instantiate(prefab);
    }

    private void Start()
    {
        buyButton.onClick.AddListener(OnBuyClicked);
        closeButton.onClick.AddListener(Close);

        if (backdropButton != null)
            backdropButton.onClick.AddListener(Close);

        if (IAPManager.Instance != null)
        {
            IAPManager.Instance.OnPowerPackPurchased -= HandlePurchased;
            IAPManager.Instance.OnPowerPackPurchased += HandlePurchased;
        }

        backdrop.alpha = 0f;
        backdrop.DOFade(0.6f, 0.3f);

        panel.localScale = Vector3.zero;
        panel.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.05f);

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySfx("pop");
    }

    private void OnDestroy()
    {
        if (IAPManager.Instance != null)
            IAPManager.Instance.OnPowerPackPurchased -= HandlePurchased;
    }

    private void OnBuyClicked()
    {
        if (IAPManager.Instance != null)
            IAPManager.Instance.BuyPowerPack();
    }

    private void HandlePurchased()
    {
        Close();
    }

    private void Close()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySfx("click");

        panel.DOScale(0f, 0.25f).SetEase(Ease.InBack)
            .OnComplete(() => Destroy(gameObject));
    }
}
