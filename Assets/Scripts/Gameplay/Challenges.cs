using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class Challenge : ScriptableObject
{
    public int requiredAmount = 1;

    [System.Serializable]
    public struct Reward
    {
        public int currency;
        public int XP;
    }

    public Reward reward = new Reward { currency = 10, XP = 10 };

    public int currentAmount { get; protected set; }
    public bool completed { get; protected set; }

    private string description;

    public virtual string GetDescription()
    {
        return description;
    }

    protected void Evaluate()
    {
        if (currentAmount >= requiredAmount)
        {
            completed = true;
        }
    }
}

[CreateAssetMenu(fileName = "DefaultChallenge", menuName = "Challenges/KillAmountOf")]
public class KillAmount : Challenge
{
    public string unitType; //Units need to have stuff like names, etc

    public override string GetDescription()
    {
        return $"Kill {requiredAmount} of {unitType}.";
    }

    private void OnAction()
    {
        currentAmount++;
    }
}

[CreateAssetMenu(fileName = "DefaultChallenge", menuName = "Challenges/ColoniseRuins")]
public class ColoniseRuins : Challenge
{
    public override string GetDescription()
    {
        return $"Colonise {requiredAmount} ruins.";
    }

    private void OnAction()
    {
        currentAmount++;
    }
}

public class GlobalStats
{
    //Could update stuff here to be viewed on a stats page
    //Just some example stats for now
    int xp = 0;
    int level = 1;
    int kills = 0;
    int ruins = 0;
}