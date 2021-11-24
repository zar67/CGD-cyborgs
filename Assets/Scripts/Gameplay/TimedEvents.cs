using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedEvents : MonoBehaviour
{
    [Header("Challenges")]
    [SerializeField] private int maxChallenges = 5;
    [SerializeField] public List<Challenge> allChallenges;

    public void CheckCurrentDate()
    {
        int currentDate = WorldTimeAPI.GetCurrentDay();
        int previousDate = currentDate;

        if (SaveReadWrite.DoesFileExist())
        {
            SaveReadWrite.ReadFromJSON();
            previousDate = SaveReadWrite.data.previousLoginDate;

            if (currentDate != previousDate) ResetDailyChallenges();
            else GlobalData.dailyChallenges = SaveReadWrite.data.dailyChallenges;
        }
        else
        {
            ResetDailyChallenges();
        }

        //Save over old data after checking the date
        SaveReadWrite.data.previousLoginDate = currentDate;
        SaveReadWrite.data.dailyChallenges = GlobalData.dailyChallenges;
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

