using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "DefaultChallenge", menuName = "Challenges/KillAmountOf")]
public class KillAmount : Challenge
{
    private Dictionary<Unit.EUnitType, string> formattedNames = new Dictionary<Unit.EUnitType, string>()
    {
        {Unit.EUnitType.SOLDIER, "Soldier" },
        {Unit.EUnitType.TANK, "Tank" },
        {Unit.EUnitType.PLANE, "Plane" },
    };

    public Unit.EUnitType unitType;

    public override string GetDescription()
    {
        return $"Destroy {formattedNames[unitType]}s.";
    }
}
