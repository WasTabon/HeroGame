using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private Vector3 basePosition;

    private void Awake()
    {
        Instance = this;
        basePosition = transform.localPosition;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void Shake(float strength = 0.4f, float duration = 0.25f)
    {
        transform.DOKill();
        transform.localPosition = basePosition;
        transform.DOShakePosition(duration, strength, 20, 90, false, true)
            .OnComplete(() => transform.localPosition = basePosition);
    }
}
