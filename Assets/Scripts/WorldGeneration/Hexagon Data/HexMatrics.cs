using System;

[Serializable]
public struct HexMatrics
{
    public float OuterRadius;
    public float InnerRadius;

    private const float INNER_RADIUS_MULTIPLIER = 0.866025404f;

    public HexMatrics(float outerRadius)
    {
        OuterRadius = outerRadius;
        InnerRadius = outerRadius * INNER_RADIUS_MULTIPLIER;
    }
}