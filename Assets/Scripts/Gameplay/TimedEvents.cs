using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedEvents : MonoBehaviour
{
    [Header("Challenges")]
    [SerializeField] private int maxChallenges = 1;
    [SerializeField] private List<Challenge> allChallenges;
    private List<Challenge> dailyChallenges = new List<Challenge>();

    private void ResetDailyChallenges()
    {
        dailyChallenges.Clear();

        int min = Mathf.Min(allChallenges.Count, maxChallenges);

        while (dailyChallenges.Count < min)
        {
            int i = Random.Range(0, allChallenges.Count);
            Challenge thisChallenge = allChallenges[i];

            if (!dailyChallenges.Contains(thisChallenge))
            {
                dailyChallenges.Add(thisChallenge);
            }
        }

        Debug.Log("Updated daily challenges!");
    }
}