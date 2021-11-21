using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultChallenge", menuName = "Challenges/ColoniseRuins")]
public class ColoniseRuins : Challenge
{
    public override string GetDescription()
    {
        return $"Colonise {requiredAmount} ruins.";
    }
}
