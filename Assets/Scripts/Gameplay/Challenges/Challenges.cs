using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Challenge : ScriptableObject
{
    [System.Serializable]
    public struct Reward
    {
        public int currency;
        public int XP;
    }

    [Header("Rewards")]
    public Reward reward = new Reward { currency = 10, XP = 10 };

    [Header("Requirements")]
    public int requiredAmount = 1;
    public int currentAmount { get; protected set; }
    public bool completed { get; protected set; }

    private string description;

    public virtual string GetDescription()
    {
        return description;
    }

    public void OnAction()
    {
        currentAmount++;
    }

    protected void Evaluate()
    {
        if (currentAmount >= requiredAmount)
        {
            completed = true;
        }
    }
}
