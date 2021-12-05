using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "DefaultChallenge", menuName = "Challenges/KillAmountOf")]
public class KillAmount : Challenge
{
    private Dictionary<Unit.UnitTypes, string> formattedNames = new Dictionary<Unit.UnitTypes, string>()
    {
        {Unit.UnitTypes.SOLDIER, "Soldier" },
        {Unit.UnitTypes.TANK, "Tank" },
        {Unit.UnitTypes.PLANE, "Plane" },
    };

    public Unit.UnitTypes unitType;

    public override string GetDescription()
    {
        return $"Destroy {formattedNames[unitType]}s.";
    }
}
