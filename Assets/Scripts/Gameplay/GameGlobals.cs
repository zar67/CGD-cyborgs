using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalStats
{
    //Could update stuff here to be viewed on a stats page
    //Just some example stats for now
    int xp = 0;
    int level = 1;
    int kills = 0;
    int ruins = 0;
}

public class GlobalData
{
    public static List<Challenge> dailyChallenges = new List<Challenge>();
}