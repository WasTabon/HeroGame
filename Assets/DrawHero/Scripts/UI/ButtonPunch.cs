using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonPunch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public string clickSound = "click";

    private Vector3 baseScale;
    private Tween scaleTween;

    private void Awake()
    {
        baseScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        scaleTween?.Kill();
        scaleTween = transform.DOScale(baseScale * 0.95f, 0.08f).SetEase(Ease.OutQuad);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        scaleTween?.Kill();
        scaleTween = transform.DOScale(baseScale, 0.12f).SetEase(Ease.OutBack);

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySfx(clickSound);

        if (HapticManager.Instance != null)
            HapticManager.Instance.Light();
    }
}
