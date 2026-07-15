using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PowerUpBar : MonoBehaviour
{
    public Button heavyButton;
    public Button extraButton;
    public Button bombButton;

    public Image heavyIcon;
    public Image extraIcon;
    public Image bombIcon;

    public TextMeshProUGUI heavyCount;
    public TextMeshProUGUI extraCount;
    public TextMeshProUGUI bombCount;

    private bool heavyActive;
    private bool bombActive;

    private void Start()
    {
        heavyButton.onClick.AddListener(OnHeavy);
        extraButton.onClick.AddListener(OnExtra);
        bombButton.onClick.AddListener(OnBomb);

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

    private void Refresh()
    {
        heavyCount.text = "x" + PowerUpManager.GetHeavy();
        extraCount.text = "x" + PowerUpManager.GetExtra();
        bombCount.text = "x" + PowerUpManager.GetBomb();

        SetHighlight(heavyButton.transform, heavyActive);
        SetHighlight(bombButton.transform, bombActive);
    }

    private void SetHighlight(Transform btn, bool active)
    {
        btn.DOKill();
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
}
