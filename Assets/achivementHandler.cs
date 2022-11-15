using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class achivementHandler : MonoBehaviour
{
    List<int> NGAchievementNums = new List<int> { 70929, 70930, 70931, 70934, 70932, 70933, 70940, 70937, 70935, 70936, 70938, 70939 };
    NGHelper ng;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        ng = gameObject.GetComponent<NGHelper>();
        runThroughAchievements();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //0 is beat wires, 1 is beat buttons, 2 is beat math, 3 is die, 4 is heartbreak ending, 5 is true ending, 6 is complete intro, 7 is goku, 8 is get max affection, 9 is 0 affection
    //10 is complete without checkpoints, 11 is complete without checkpoints with 0 lives lost and best romance ending

    public void unlockAchievement(int i)
    {
        PlayerPrefs.SetInt("achievement" + i.ToString(), 1);

        if (ng.hasNewgrounds)
        {
            ng.unlockMedal(NGAchievementNums[i]);
        }
    }

    public void runThroughAchievements()
    {
        if (ng.hasNewgrounds)
        {
            for(int i = 0; i < NGAchievementNums.Count; i++)
            {
                if(PlayerPrefs.GetInt("achievement" + i.ToString(), 0) == 1)
                {
                    ng.unlockMedal(NGAchievementNums[i]);
                }
            }
        }
    }
}
