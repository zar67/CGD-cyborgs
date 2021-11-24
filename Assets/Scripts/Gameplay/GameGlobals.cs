using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalStats
{
    int xp = 0;
    int rank = 1;
}

public class GlobalData
{
    public static List<Challenge> dailyChallenges = new List<Challenge>();

    public static void UpdateDailyChallenge<T>()
    {
        foreach (Challenge c in dailyChallenges)
        {
            if(c is T)
            {
                c.IncreaseCurrentAmount();
            }
        }
    }

    public static void UpdateDailyChallenge(Unit.UnitTypes unitType)
    {
        foreach (Challenge c in dailyChallenges)
        {
            KillAmount k = c as KillAmount;
            if(k != null && k.unitType == unitType)
            {
                c.IncreaseCurrentAmount();
            }
        }
    }
}