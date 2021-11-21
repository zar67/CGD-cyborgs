using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultChallenge", menuName = "Challenges/KillAmountOf")]
public class KillAmount : Challenge
{
    public Unit.UnitTypes unitType;

    public override string GetDescription()
    {
        return $"Kill {requiredAmount} of {unitType.ToString()}.";
    }
}
