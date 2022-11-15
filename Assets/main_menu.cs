using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class main_menu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI fullscreen;
    [SerializeField] TextMeshProUGUI subtitles;
    [SerializeField] float accelSpeed;
    EasingFunction.Function function;
    Vector2 currentPos;
    float xPosOld;
    float yPosOld;
    float xPos;
    bool isMoving = false;
    float timer;
    float yPos;
    [SerializeField] RectTransform sections;


    // Start is called before the first frame update
    void Start()
    {
        EasingFunction.Ease movement = EasingFunction.Ease.EaseOutBack;
        function = EasingFunction.GetEasingFunction(movement);
        xPosOld = sections.anchoredPosition.x;
        yPosOld = sections.anchoredPosition.y;
        currentPos = new Vector2(xPosOld, yPosOld);

        if(PlayerPrefs.GetInt("noSubtitles", 0) == 1)
        {
            subtitles.text = "Subtitles: Off";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            timer += Time.deltaTime;
            currentPos = new Vector2(function(xPosOld, xPos, timer * accelSpeed), function(yPosOld, yPos, timer * accelSpeed));
            sections.anchoredPosition = currentPos;
            if (timer >= (1f / accelSpeed))
            {
                isMoving = false;
                xPosOld = xPos;
                yPosOld = yPos;
                timer = 0;
                sections.anchoredPosition = new Vector2(xPos, yPos);
            }
        }

        if (Screen.fullScreen)
        {
            fullscreen.text = "Window";
        }
        else
        {
            fullscreen.text = "Fullscreen";
        }
    }

    public void playFromChapter(int i)
    {
        PlayerPrefs.SetInt("skippedIntro", i);
        SceneManager.LoadScene("SampleScene");
    }

    public void intro()
    {
        PlayerPrefs.SetInt("skippedIntro", 0);
        SceneManager.LoadScene("SampleScene");
    }

    public void toggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
        if (Screen.fullScreen)
        {
            fullscreen.text = "Window";
        }
        else
        {
            fullscreen.text = "Fullscreen";
        }
    }

    public void toggleSubtitles()
    {
        if(PlayerPrefs.GetInt("noSubtitles", 0) == 0)
        {
            PlayerPrefs.SetInt("noSubtitles", 1);
            subtitles.text = "Subtitles: Off";
        }
        else
        {
            PlayerPrefs.SetInt("noSubtitles", 0);
            subtitles.text = "Subtitles: On";
        }
    }

    public void moveToSettings()
    {
        isMoving = true;
        xPos = 0;
        yPos = 900;
    }

    public void moveToChapterSelect()
    {
        isMoving = true;
        xPos = -1600;
        yPos = 0;
    }

    public void backToMenu()
    {
        isMoving = true;
        xPos = 0;
        yPos = 0;
    }



}
