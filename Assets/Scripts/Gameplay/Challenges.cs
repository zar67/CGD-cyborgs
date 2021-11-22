using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Challenge
{
    string description;
    bool isCompleted = false;
    int requiredAmount = 0;

    int currentAmount = 0;

    void Evaulate()
    {
        if (currentAmount >= requiredAmount)
        {
            isCompleted = true;
        }
    }
}

public class GlobalStats
{
    //Could update stuff here to be viewed on a stats page
    //Just some example stats for now
    int level = 1;
    int kills = 0;
    int ruins = 0;
}