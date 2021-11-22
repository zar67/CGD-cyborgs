using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedEvents : MonoBehaviour
{
    [Header("Challenges")]
    [SerializeField] private int maxChallenges = 1;
    [SerializeField] public List<Challenge> allChallenges = new List<Challenge>();

    public void CheckCurrentDate()
    {
        int currentDate = WorldTimeAPI.GetCurrentDay();
        int previousDate = currentDate;

        if(SaveReadWrite.DoesFileExist())
        {
            SaveReadWrite.ReadFromJSON();
            previousDate = SaveReadWrite.data.previousLoginDate;
        }

        //If the days are different, then time must have passed
        //Of course, this could go wrong if someone doesn't play for exactly a year
        //but that's really unlikely and not that important right now
        if (currentDate != previousDate) ResetDailyChallenges();

        //Save over old data after checking the date
        SaveReadWrite.data.previousLoginDate = currentDate;
        SaveReadWrite.SaveToJSON();
    }

    private void ResetDailyChallenges()
    {
        GlobalData.dailyChallenges.Clear();

        int min = Mathf.Min(allChallenges.Count, maxChallenges);

        while (GlobalData.dailyChallenges.Count < min)
        {
            int i = Random.Range(0, allChallenges.Count);
            Challenge thisChallenge = allChallenges[i];

            if (!GlobalData.dailyChallenges.Contains(thisChallenge))
            {
                GlobalData.dailyChallenges.Add(thisChallenge);
            }
        }

        Debug.Log("Updated daily challenges!");
    }
}

