using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Globalization;
using System.IO;
using System;
using TMPro;

public class subtitles : MonoBehaviour
{
    [SerializeField] TextAsset subtitleData;
    [SerializeField] TextMeshProUGUI subtitleText;
    [SerializeField] TextMeshProUGUI subtitlesAlt;
    [SerializeField] bool isProtag = false;
    [SerializeField] bool isIntro = false;
    [SerializeField] date theDate;
    [SerializeField] canvasMovement cm;

    [SerializeField] TMP_FontAsset dateFont;
    [SerializeField] TMP_FontAsset protagFont;
    [SerializeField] TMP_FontAsset bombFont;
    [SerializeField] AudioSource audioSync;

    int currentIndex = 0;

    List<subtitleObject> subtitlesList = new List<subtitleObject>();

    IFormatProvider format = new CultureInfo("en-US");

    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.GetInt("skippedIntro", 0) == 0 && !isIntro)
        {
            gameObject.SetActive(false);
        }else if(PlayerPrefs.GetInt("skippedIntro", 0) != 0 && isIntro)
        {
            gameObject.SetActive(false);
        }

        if(PlayerPrefs.GetInt("noSubtitles", 0) == 1)
        {
            gameObject.SetActive(false);
        }

        loadSubtitles();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentIndex < subtitlesList.Count && audioSync.isPlaying)
        {
            timer = audioSync.time;

            if (subtitlesList[currentIndex].time < 0)
            {
                currentIndex++;
            }else if(timer > subtitlesList[currentIndex].time)
            {
                Debug.Log("subtitle choice: " + subtitlesList[currentIndex].choice.ToString() + " date choice: " + theDate.getDialogueChoice().ToString());
                if(subtitlesList[currentIndex].choice == -1 || subtitlesList[currentIndex].choice == theDate.getDialogueChoice())
                {
                    TextMeshProUGUI mainText = subtitleText;

                    if (isProtag)
                    {
                        if (cm.isOnBomb())
                        {
                            mainText = subtitlesAlt;
                        }
                    }

                    mainText.text = subtitlesList[currentIndex].text;

                    
                    int character = subtitlesList[currentIndex].character;
                    switch (character)
                    {
                        case 0:
                            mainText.color = Color.cyan;
                            mainText.font = bombFont;
                            break;
                        case 1:
                            mainText.color = new Color(1, 0.67f, .1f);
                            mainText.font = protagFont;
                            break;
                        case 2:
                            mainText.color = new Color(1, 0.75f, .79f);
                            mainText.font = dateFont;
                            break;
                    }
                }
                currentIndex++;
            }
        }
    }

    void loadSubtitles()
    {
        string data = subtitleData.text;
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(new StringReader(data));

        XmlNodeList dialogueNodeList = xmlDoc.SelectNodes("//data/dialogue");

        foreach (XmlNode infonode in dialogueNodeList)
        {
            if(theDate.getCurrentStartPos() <= (float)Convert.ToDouble(infonode.Attributes["time"].Value, format))
            {
                subtitleObject s = new subtitleObject((float)Convert.ToDouble(infonode.Attributes["time"].Value, format), infonode.Attributes["text"].Value, Convert.ToInt32(infonode.Attributes["option"].Value, format), Convert.ToInt32(infonode.Attributes["character"].Value, format));
                subtitlesList.Add(s);
            }
        }

        subtitlesList.Sort(delegate (subtitleObject x, subtitleObject y) {
            return x.time.CompareTo(y.time);
        });


    }
}
