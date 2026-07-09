using UnityEngine;

public static class StarEvaluator
{
    public static int Evaluate(float usedInk, float maxInk)
    {
        if (maxInk <= 0f) return 1;

        float ratio = usedInk / maxInk;

        if (ratio <= 0.5f) return 3;
        if (ratio <= 0.8f) return 2;
        return 1;
    }
}
