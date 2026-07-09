using System;
using UnityEngine;

public class InkManager : MonoBehaviour
{
    public static InkManager Instance;

    public event Action<float, float> OnInkChanged;

    private float maxInk = 100f;
    private float usedInk = 0f;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public float MaxInk => maxInk;
    public float UsedInk => usedInk;
    public float RemainingInk => maxInk - usedInk;

    public void SetMaxInk(float value)
    {
        maxInk = value;
        usedInk = 0f;
        OnInkChanged?.Invoke(usedInk, maxInk);
    }

    public bool CanDraw(float length)
    {
        return usedInk + length <= maxInk;
    }

    public bool HasInkLeft(float minLength)
    {
        return RemainingInk >= minLength;
    }

    public void SpendInk(float length)
    {
        usedInk = Mathf.Min(usedInk + length, maxInk);
        OnInkChanged?.Invoke(usedInk, maxInk);
    }

    public void ResetInk()
    {
        usedInk = 0f;
        OnInkChanged?.Invoke(usedInk, maxInk);
    }
}
