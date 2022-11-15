using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NGHelper : MonoBehaviour
{
    public io.newgrounds.core ngio_core;
    public bool hasNewgrounds;
    float timer = 0;
    bool loggedIn = false;

    achivementHandler aHandler;

    // Start is called before the first frame update
    void Start()
    {
        aHandler = gameObject.GetComponent<achivementHandler>();

        if(/*Application.platform == RuntimePlatform.WebGLPlayer &&*/ hasNewgrounds)
        {
            ngio_core.onReady(() =>
            {
                ngio_core.checkLogin((bool logged_in) => {
                    if (logged_in)
                    {
                        onLoggedIn();
                    }
                    else
                    {
                        requestLogIn();
                    }

                });
            });
        }
        else
        {
            ngio_core.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (hasNewgrounds)
        {
            timer += Time.deltaTime;
            if(timer > 10)
            {
                timer = 0;
                io.newgrounds.components.Gateway.ping p = new io.newgrounds.components.Gateway.ping();
                p.callWith(ngio_core, ping);
            }
        }
    }

    void ping(io.newgrounds.results.Gateway.ping result)
    {
        Debug.Log(result.pong);
    }

    void onLoggedIn()
    {
        loggedIn = true;
        aHandler.runThroughAchievements();
    }

    void requestLogIn()
    {
        ngio_core.requestLogin(onLoggedIn);
    }

    public void unlockMedal(int medal_id)
    {
        if (hasNewgrounds && loggedIn)
        {
            io.newgrounds.components.Medal.unlock medal_unlock = new io.newgrounds.components.Medal.unlock();

            medal_unlock.id = medal_id;

            medal_unlock.callWith(ngio_core, onMedalUnlocked);
        }
    }

    void onMedalUnlocked(io.newgrounds.results.Medal.unlock result)
    {
        io.newgrounds.objects.medal medal = result.medal;
        Debug.Log("Medal Unlocked: " + medal.name + " (" + medal.value + " points)");
    }

    public void submitScore(int score_id, int score)
    {
        io.newgrounds.components.ScoreBoard.postScore submit_score = new io.newgrounds.components.ScoreBoard.postScore();
        submit_score.id = score_id;
        submit_score.value = score;
        submit_score.callWith(ngio_core);
    }
}
