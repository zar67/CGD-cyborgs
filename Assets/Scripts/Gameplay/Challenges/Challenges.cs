using UnityEngine;

[System.Serializable]
public abstract class Challenge : ScriptableObject
{
    [System.Serializable]
    public struct Reward
    {
        public uint score;
    }

    [Header("Rewards")]
    public Reward reward = new Reward { score = 10 };

    [Header("Requirements")]
    public int requiredAmount = 1;
    public int currentAmount
    {
        get; protected set;
    }
    public bool completed
    {
        get; protected set;
    }

    private bool rewardClaimed = false;

    private string description;

    public virtual string GetDescription()
    {
        return description;
    }

    public void IncreaseCurrentAmount()
    {
        if(!completed)
        {
            currentAmount++;
        }
    }

    public void ClaimReward()
    {
        if(!rewardClaimed && completed)
        {
            SaveReadWrite.data.score += reward.score;
            rewardClaimed = true;
            ServerManager sm = new ServerManager();
            sm.SetScore(SaveReadWrite.data.name, SaveReadWrite.data.score.ToString());
        }
    }

    protected void Evaluate()
    {
        if (currentAmount >= requiredAmount)
        {
            completed = true;
        }
    }
}
