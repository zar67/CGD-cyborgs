using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "DefaultChallenge", menuName = "Challenges/ColoniseRuins")]
public class ColoniseRuins : Challenge
{
    public override string GetDescription()
    {
        return $"Colonise ruins.";
    }
}
