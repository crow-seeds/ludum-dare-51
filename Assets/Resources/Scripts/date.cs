using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Xml;
using System.Globalization;
using System.IO;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class date : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] canvasMovement c;
    float affection = 60;
    [SerializeField] Image affectionBar;
    int dialogueIndex = 0;
    [SerializeField] TextAsset dialogueData;
    IFormatProvider format = new CultureInfo("en-US");
    dialogue[] dialogueList = new dialogue[10];
    bool canSelect = false;
    [SerializeField] List<TextMeshProUGUI> optionTexts;
    [SerializeField] List<RawImage> controls;
    [SerializeField] RawImage vignette;
    [SerializeField] RawImage darkBackground;

    [SerializeField] List<float> howLongTheIntroIs;
    [SerializeField] AudioSource chatter1;
    [SerializeField] AudioSource chatter2;
    [SerializeField] bomb theBomb;
    [SerializeField] pan panthing;

    dialogue currentDialogue;

    float dateTimer = 0;
    float dateTotalTime = 0;
    [SerializeField] Image timerIndicator;
    [SerializeField] AudioSource protagAudio;
    [SerializeField] AudioSource dateAudio;
    [SerializeField] AudioSource emmetAudio;
    [SerializeField] AudioSource ticking;

    [SerializeField] RawImage girl;
    int dialogueChoice = 0;
    float timer = 0;

    void Start()
    {
        loadDialogue();
        if (PlayerPrefs.GetInt("skippedIntro", 0) == 0)
        {
            protagAudio.clip = Resources.Load<AudioClip>("Sound/intro");
            protagAudio.Play();
            dateAudio.clip = null;
            //dateAudio.Stop();
            emmetAudio.clip = null;
            //emmetAudio.Stop();
            protag.gameObject.SetActive(true);
            protagBack.gameObject.SetActive(true);
            StartCoroutine(intro());
        }
        else
        {

            StartCoroutine(doDialogue());
        }
        //StartCoroutine(doDialogue());
        randomizeChatter();
        
    }

    [SerializeField] RawImage protag;
    [SerializeField] RawImage protagBack;
    IEnumerator intro()
    {   
        yield return new WaitForSeconds(howLongTheIntroIs[0]);
        //fade to date
        colorFader2 c1 = Instantiate(Resources.Load<GameObject>("Prefabs/Image Fader")).GetComponent<colorFader2>();
        colorFader2 c2 = Instantiate(Resources.Load<GameObject>("Prefabs/Image Fader")).GetComponent<colorFader2>();
        c1.set(protag, new Color(1, 1, 1, 0), .5f);
        c2.set(protagBack, new Color(1, 1, 1, 0), .5f);
        yield return new WaitForSeconds(howLongTheIntroIs[1]);
        StartCoroutine(fadeInSpace());
    }

    IEnumerator goToTitleCard()
    {
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene("titleCard");
    }

    void randomizeChatter()
    {
        chatter1.Stop();
        chatter2.Stop();
        chatter1.clip = Resources.Load<AudioClip>("Sound/chatter_" + UnityEngine.Random.Range(1, 5));
        chatter1.time = UnityEngine.Random.Range(0f, chatter1.clip.length);
        chatter1.loop = true;
        chatter2.clip = Resources.Load<AudioClip>("Sound/chatter_" + UnityEngine.Random.Range(1, 5));
        chatter2.time = UnityEngine.Random.Range(0f, chatter2.clip.length);
        chatter2.loop = true;
        chatter1.Play();
        chatter2.Play();

    }

    void loadDialogue()
    {
        string data = dialogueData.text;
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(new StringReader(data));

        XmlNodeList dialogueNodeList = xmlDoc.SelectNodes("//data/dialogue");

        foreach (XmlNode infonode in dialogueNodeList)
        {
            dialogue d = new dialogue(Convert.ToInt32(infonode.Attributes["id"].Value, format), infonode.Attributes["options"].Value, 
                Convert.ToInt32(infonode.Attributes["order"].Value, format), infonode.Attributes["busy"].Value,
                infonode.Attributes["affections"].Value, (float)Convert.ToDouble(infonode.Attributes["timeUntilAnswer"].Value, format),
                (float)Convert.ToDouble(infonode.Attributes["timeToAnswer"].Value, format));
            dialogueList[d.getOrder()] = d;
        }

    }

    void fadeAllSprites(float time, Color c)
    {
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("sprite"))
        {
            colorFader c1 = Instantiate(Resources.Load<GameObject>("Prefabs/Sprite Fader")).GetComponent<colorFader>();
            c1.set(g.GetComponent<SpriteRenderer>(), c, time);
        }
    }


    float previousTime = 0;
    IEnumerator doDialogue()
    {
        if(dialogueIndex >= dialogueList.Length)
        {
            yield break;
        }
        currentDialogue = dialogueList[dialogueIndex];
        Debug.Log(dialogueList[dialogueIndex].getTimeUntilAnswer());
        yield return new WaitForSeconds(dialogueList[dialogueIndex].getTimeUntilAnswer() - timer);
        dialogueChoice = 0;
        previousTime = dialogueList[dialogueIndex].getTimeUntilAnswer() + 1f + dateTotalTime;
        colorFader2 c1 = Instantiate(Resources.Load<GameObject>("Prefabs/Image Fader")).GetComponent<colorFader2>();
        colorFader2 c2 = Instantiate(Resources.Load<GameObject>("Prefabs/Image Fader")).GetComponent<colorFader2>();
        colorFader4 c4 = Instantiate(Resources.Load<GameObject>("Prefabs/Real Image Fader")).GetComponent<colorFader4>();
        fadeAllSprites(.5f, new Color(.5f, .5f, .5f));
        c1.set(vignette, new Color(1, 1, 1, 1), .5f);
        c2.set(darkBackground, new Color(0, 0, 0, .5f), .5f);
        c4.set(timerIndicator, new Color(1, 1, 1, 1f), .5f);
        timerIndicator.fillAmount = 1;
        for(int i = 0; i < 4; i++)
        {
            if(dialogueList[dialogueIndex].getOption(i) != "")
            {
                colorFader2 c6 = Instantiate(Resources.Load<GameObject>("Prefabs/Image Fader")).GetComponent<colorFader2>();
                c6.set(controls[i], new Color(1,1,1,1f), .5f);
                TextMeshProUGUI t = optionTexts[i];
                t.gameObject.SetActive(true);
                t.text = dialogueList[dialogueIndex].getOption(i);
                colorFader3 c3 = Instantiate(Resources.Load<GameObject>("Prefabs/Text Fader")).GetComponent<colorFader3>();
                c3.set(t, Color.white, .5f);
            }

        }
        c.setBloomModifier(10);
        yield return new WaitForSeconds(.5f);
        canSelect = true;
        dateTimer = dialogueList[dialogueIndex].getTimeToAnswer();
        dateTotalTime = dialogueList[dialogueIndex].getTimeToAnswer();
        yield return new WaitForSeconds(.5f + dateTotalTime);
        c.setBloomModifier(0);
        for (int i = 0; i < 4; i++)
        {
            optionTexts[i].gameObject.SetActive(false);
        }
        
        canSelect = false;
        dialogueIndex++;
        StartCoroutine(doDialogue());
    }

    [SerializeField] RawImage spaceIndicator;
    bool spaceToPan = false;
    IEnumerator fadeInSpace()
    {
        colorFader2 c1 = Instantiate(Resources.Load<GameObject>("Prefabs/Image Fader")).GetComponent<colorFader2>();
        colorFader2 c2 = Instantiate(Resources.Load<GameObject>("Prefabs/Image Fader")).GetComponent<colorFader2>();
        c1.set(vignette, new Color(1, 1, 1, 1), .5f);
        c2.set(darkBackground, new Color(0, 0, 0, .5f), .5f);
        fadeAllSprites(.5f, new Color(.5f, .5f, .5f));
        colorFader2 c3 = Instantiate(Resources.Load<GameObject>("Prefabs/Image Fader")).GetComponent<colorFader2>();
        c3.set(spaceIndicator, Color.white, .5f);
        yield return new WaitForSeconds(.5f);
        c.setBloomModifier(10);
        spaceToPan = true;
    }

    public void exitTutorial()
    {
        dialogueIndex = 1;
        StartCoroutine(doDialogue());
        colorFader2 c1 = Instantiate(Resources.Load<GameObject>("Prefabs/Image Fader")).GetComponent<colorFader2>();
        c1.set(spaceIndicator, new Color(1,1,1,0), .5f);
    }

    public void selectAnswer(int i)
    {
        if (canSelect)
        {
            colorFader2 c1 = Instantiate(Resources.Load<GameObject>("Prefabs/Image Fader")).GetComponent<colorFader2>();
            
            colorFader3 c2 = Instantiate(Resources.Load<GameObject>("Prefabs/Text Fader")).GetComponent<colorFader3>();
            
            
            
            dialogueChoice = i+1;
            canSelect = false;

            if (currentDialogue.getAFfectionValue(dialogueChoice) > 0)
            {
                c1.set(controls[i], new Color(.6f, 1f, .6f), .3f);
                c2.set(optionTexts[i], new Color(.5f, 1f, .5f), .3f);
            }
            else
            {
                c1.set(controls[i], Color.red, .3f);
                c2.set(optionTexts[i], Color.red, .3f);
            }
        }
        
    }


    // Update is called once per frame
    void Update()
    {
        if (canSelect)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                selectAnswer(0);
            }else if (Input.GetKeyDown(KeyCode.D) && currentDialogue.getOption(1) != "")
            {
                selectAnswer(1);
            }else if (Input.GetKeyDown(KeyCode.S) && currentDialogue.getOption(2) != "")
            {
                selectAnswer(2);
            }
            else if (Input.GetKeyDown(KeyCode.W) && currentDialogue.getOption(3) != "")
            {
                selectAnswer(3);
            }
        }

        timer += Time.deltaTime;

        if(spaceToPan && Input.GetKeyDown(KeyCode.Space))
        {
            c.setBloomModifier(0);
            colorFader2 c1 = Instantiate(Resources.Load<GameObject>("Prefabs/Image Fader")).GetComponent<colorFader2>();
            colorFader2 c2 = Instantiate(Resources.Load<GameObject>("Prefabs/Image Fader")).GetComponent<colorFader2>();
            colorFader2 c3 = Instantiate(Resources.Load<GameObject>("Prefabs/Image Fader")).GetComponent<colorFader2>();
            c1.set(vignette, new Color(1, 1, 1, 0), .5f);
            c2.set(darkBackground, new Color(0, 0, 0, 0f), .5f);
            c3.set(spaceIndicator, new Color(0, 0, 0, 0f), .5f);
            fadeAllSprites(.5f, new Color(1f, 1f, 1f));
            StartCoroutine(goToTitleCard());
            panthing.makeMove();
        }

        if(dateTotalTime != 0)
        {
            dateTimer -= Time.deltaTime;
            timerIndicator.fillAmount = dateTimer / dateTotalTime;
            if(dateTimer <= 0)
            {
                colorFader2 c1 = Instantiate(Resources.Load<GameObject>("Prefabs/Image Fader")).GetComponent<colorFader2>();
                colorFader2 c2 = Instantiate(Resources.Load<GameObject>("Prefabs/Image Fader")).GetComponent<colorFader2>();
                c1.set(vignette, new Color(1, 1, 1, 0), .5f);
                c2.set(darkBackground, new Color(0, 0, 0, 0f), .5f);
                fadeAllSprites(.5f, new Color(1f, 1f, 1f));
                timerIndicator.color = new Color(1, 1, 1, 0);

                for (int i = 0; i < 4; i++)
                {
                    TextMeshProUGUI t = optionTexts[i];
                    t.gameObject.SetActive(true);
                    colorFader3 c3 = Instantiate(Resources.Load<GameObject>("Prefabs/Text Fader")).GetComponent<colorFader3>();
                    c3.set(t, new Color(1,1,1,0), .5f);

                    colorFader2 c4 = Instantiate(Resources.Load<GameObject>("Prefabs/Image Fader")).GetComponent<colorFader2>();
                    c4.set(controls[i], new Color(1, 1, 1, 0), .5f);
                }
                Debug.Log(affection);
                Debug.Log(currentDialogue.getAFfectionValue(dialogueChoice));
                affection += currentDialogue.getAFfectionValue(dialogueChoice);
                Debug.Log(affection);

                if (dialogueChoice > 0)
                {
                    protagAudio.PlayOneShot(Resources.Load<AudioClip>("Sound/Dialogue/protag_" + dialogueIndex.ToString() + "_" + (dialogueChoice - 1).ToString()));
                    dateAudio.PlayOneShot(Resources.Load<AudioClip>("Sound/Dialogue/date_" + dialogueIndex.ToString() + "_" + (dialogueChoice - 1).ToString()));
                    emmetAudio.PlayOneShot(Resources.Load<AudioClip>("Sound/Dialogue/emmet_" + dialogueIndex.ToString() + "_" + (dialogueChoice - 1).ToString()));
                }else if(dialogueChoice == 0)
                {
                    protagAudio.PlayOneShot(Resources.Load<AudioClip>("Sound/Dialogue/protag_" + dialogueIndex.ToString() + "_busy"));
                    dateAudio.PlayOneShot(Resources.Load<AudioClip>("Sound/Dialogue/date_" + dialogueIndex.ToString() + "_busy"));
                    emmetAudio.PlayOneShot(Resources.Load<AudioClip>("Sound/Dialogue/emmet_" + dialogueIndex.ToString() + "_busy"));
                }

                changeExpression("meh");
                if (currentDialogue.getAFfectionValue(dialogueChoice) < 0)
                {
                    dateAudio.PlayOneShot(Resources.Load<AudioClip>("Sound/date_bad"));
                    changeExpression("sad");
                    if(currentDialogue.getAFfectionValue(dialogueChoice) < 20)
                    {
                        changeExpression("angry");
                    }
                }
                if(currentDialogue.getAFfectionValue(dialogueChoice) >= 10)
                {
                    if(currentDialogue.getAFfectionValue(dialogueChoice) >= 20)
                    {
                        changeExpression("laugh");
                    }
                    else
                    {
                        changeExpression("happy");
                    }

                    dateAudio.PlayOneShot(Resources.Load<AudioClip>("Sound/date_good"));
                }

                

                affection = Mathf.Min(100, affection);
                dateTotalTime = 0;
            }
        }

        if (c.isOnBomb())
        {
            affection -= 2.5f * Time.deltaTime;
        }
        else
        {
            affection += 2f * Time.deltaTime;
        }
        affection = Mathf.Min(120, affection);
        affection = Mathf.Max(0, affection);

        if(affection == 0 && !hasReached0)
        {
            dateAudio.PlayOneShot(Resources.Load<AudioClip>("Sound/shatter"));
            hasReached0 = true;
        }

        if (hasReached0)
        {
            affection = 0;
            changeExpression("angry");
        }

        if (affection >= 100)
        {
            changeExpression("laugh");
        }



        affectionBar.fillAmount = Mathf.Min(affection / 100f, 1);
    }

    bool hasReached0 = false;

    public float getAffection()
    {
        return affection;
    }


    public void changeExpression(string s)
    {
        girl.texture = Resources.Load<Texture>("Sprites/girl_" + s);
    }
}
