using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Rendering.PostProcessing;

public class bomb : MonoBehaviour
{
    // Start is called before the first frame update
    EasingFunction.Function function;
    float xPosOld;
    float yPosOld;
    float xPos;
    float yPos;
    float timer = 0;

    [SerializeField] float bombTimer = 10;
    [SerializeField] float accelSpeed;

    List<bool> wires = new List<bool> { true, true, true, true, true};
    List<bool> correctSolution = new List<bool> { true, true, true, true, true};
    List<int> wireImageNums = new List<int> { 0, 0, 0, 0, 0 };
    [SerializeField] List<int> howManyBombs;
    [SerializeField] List<string> bombTypes;
    [SerializeField] int level = 0;
    bool solved = false;
    int health = 3;



    [SerializeField] TextMeshProUGUI bombTimeLCD;
    [SerializeField] List<RawImage> wireImages;
    [SerializeField] List<RawImage> buttonImages;
    [SerializeField] List<RawImage> symbolImages;
    [SerializeField] Slider mathSlider;


    [SerializeField] List<int> buttonNums = new List<int> { 0, 0, 0, 0 };
    [SerializeField] List<bool> buttonPressed = new List<bool> { false, false, false, false };


    [SerializeField] RectTransform bombLocation;
    bool isMoving = false;
    int howManyLeft = 3;
    bool indicatorLight = false;
    [SerializeField] RawImage indicatorLightTexture;

    string currentType = "wire";
    bool hasBlown = false;

    [SerializeField] AudioSource soundFx;
    [SerializeField] AudioSource explosion;
    [SerializeField] GameObject wireBomb;
    [SerializeField] GameObject buttonBomb;
    [SerializeField] GameObject mathBomb;
    [SerializeField] cameraShake cShake;

    [SerializeField] PostProcessVolume m_Volume;
    [SerializeField] canvasMovement cm;
    [SerializeField] hand hand;
    Bloom bloom;

    float timeBetween = 10;
    float correctAnswer = 0;

    float bloomTimer = 0;
    bool blooming = false;
    bool bloomDown = false;
    bool active = false;

    [SerializeField] public AudioSource ticking;
    [SerializeField] date theDate;

    [SerializeField] GameObject chatter2;
    [SerializeField] GameObject bombStuff;

    bool canStartBomb = false;
    float bloomModifier = 0;

    void Start()
    {
        aHandler = GameObject.FindObjectOfType<achivementHandler>();
        howManyLeft = howManyBombs[level];
        Debug.Log(howManyLeft);
        EasingFunction.Ease movement = EasingFunction.Ease.EaseOutBack;
        function = EasingFunction.GetEasingFunction(movement);
        xPosOld = bombLocation.anchoredPosition.x;
        yPosOld = bombLocation.anchoredPosition.y;
        m_Volume.profile.TryGetSettings(out bloom);
        if(PlayerPrefs.GetInt("skippedIntro", 0) >= 1)
        {
            canMoveOut = true;
            if (level == 0 && theDate.getCurrentStartPos() < 38.309f)
            {
                StartCoroutine(waitForIntro());
            }
            else
            {
                Debug.Log("paoewfoj");
                ticking.mute = false;
                active = true;
                canMoveOut = true;

                float offset = theDate.getCurrentStartPos() - 38.309f;
                float sum = 0;

                for (int i = 0; i < howManyBombs.Count; i++)
                {
                    float oldsum = sum;
                    sum += 10 * howManyBombs[i];
                    Debug.Log(sum);
                    if(sum > offset)
                    {
                        level = i;
                        howManyLeft = howManyBombs[i] - ((int)((offset - oldsum) / 10));
                        bombTimer = 10 - (offset % 10);
                        Debug.Log(bombTimer);
                        break;
                    }
                }

                generateBomb();
                StartCoroutine(resetBomb());
            }

        }
        else
        {
            bombStuff.SetActive(true);
        }
    }

    IEnumerator waitForIntro()
    {
        yield return new WaitForSeconds(38.309f - theDate.getCurrentStartPos());
        explosion.PlayOneShot(Resources.Load<AudioClip>("Sound/notification"), .7f);
        ticking.mute = false;
        bombLocation.localPosition = new Vector3(0, -900, 0);
        xPosOld = bombLocation.anchoredPosition.x;
        yPosOld = bombLocation.anchoredPosition.y;
        isMoving = true;
        xPos = 0;
        yPos = 0;
        active = true;
        generateBomb();
        StartCoroutine(resetBomb());
    }

    [SerializeField] RawImage background;
    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            timer += Time.deltaTime;
            bombLocation.anchoredPosition = new Vector2(function(xPosOld, xPos, timer * accelSpeed), function(yPosOld, yPos, timer * accelSpeed));
            if (timer >= 1f/accelSpeed)
            {
                isMoving = false;
                xPosOld = xPos;
                yPosOld = yPos;
                timer = 0;
                bombLocation.anchoredPosition = new Vector2(xPos, yPos);
            }
        }

        if (active)
        {
            bombTimer -= Time.deltaTime;
            bombTimer = Mathf.Max(0, bombTimer);
            string display = "";
            int roundedTimer = (int)(bombTimer * 100);
            int minutesPassed = roundedTimer / 100;
            if (minutesPassed <= 9)
            {
                display += "0";
            }
            display += minutesPassed.ToString() + "\n";
            if (roundedTimer % 100 <= 9)
            {
                display += 0;
            }
            display += roundedTimer % 100;
            bombTimeLCD.text = display;

            if ((int)(bombTimer) % 2 == 0)
            {
                indicatorLight = false;
                indicatorLightTexture.color = Color.gray;
            }
            else
            {
                indicatorLight = true;
                indicatorLightTexture.color = Color.red;
            }
        }
        

        if (blooming)
        {
            bloomTimer += Time.deltaTime;
            bloom.intensity.value = Mathf.Max(function(0, 15, bloomTimer), bloom.intensity.value);
            if (bloomTimer > 1f)
            {
                bloom.intensity.value = 15;
                blooming = false;
                StartCoroutine(resetBloom());
            }
        }

        if (bloomDown && !blooming)
        {
            bloomTimer += Time.deltaTime * .2f;
            if (!blooming)
            {
                bloom.intensity.value = function(15, cm.bloomModifier, bloomTimer);
            }
            
            if (bloomTimer > 1f)
            {
                bloomDown = false;
                bloomTimer = 0;
            }
        }
    }

    bool canMoveOut = false;

    IEnumerator resetBloom()
    {
        yield return new WaitForSeconds(1f);
        bloomDown = true;
        bloomTimer = 0;
    }

    public bool getCanStartBomb()
    {
        return canMoveOut;
    }

    public void beginBomb()
    {
        canStartBomb = true;
        canMoveOut = true;
        //start emmet voice lines
    }

    public void cutWire(int i)
    {
        if(wires[i] == true && currentType == "wire")
        {
            wireImages[i].texture = Resources.Load<Texture>("Sprites/wire" + wireImageNums[i].ToString() + "_broken");
            wires[i] = false;
            soundFx.PlayOneShot(Resources.Load<AudioClip>("Sound/snip" + Random.Range(0, 4).ToString()));
            switch (checkOk())
            {
                case 0:
                    //nothing happens, cut wire is correct but not completely done
                    break;
                case 1:
                    //bomb completely defused
                    break;
                case 2:
                    //bomb explodes
                    die();
                    break;
            }
        }else if(!buttonPressed[i] && currentType == "button")
        {
            buttonPressed[i] = true;
            soundFx.PlayOneShot(Resources.Load<AudioClip>("Sound/button"), .5f);
            buttonImages[i].texture = Resources.Load<Texture>("Sprites/button" + buttonNums[i].ToString() + "_select");
            switch (buttonNums[i])
            {
                case 0:
                    if (indicatorLight)
                    {
                        wires[i] = false;
                    }
                    else
                    {
                        die();
                    }
                    break;
                case 1:
                    if (!indicatorLight)
                    {
                        wires[i] = false;
                    }
                    else
                    {
                        die();
                    }
                    break;
                case 2:
                    wires[i] = false;
                    break;
                case 3:
                    if (buttonNums[i - 1] == 0)
                    {
                        if (indicatorLight)
                        {
                            wires[i] = false;
                        }
                        else
                        {
                            die();
                        }
                    }
                    else if(buttonNums[i - 1] == 1)
                    {
                        if (!indicatorLight)
                        {
                            wires[i] = false;
                        }
                        else
                        {
                            die();
                        }
                    }
                    else if(buttonNums[i-1] == 2)
                    {
                        wires[i] = false;
                    }
                    else
                    {
                        if (buttonNums[i - 2] == 0)
                        {
                            if (indicatorLight)
                            {
                                wires[i] = false;
                            }
                            else
                            {
                                die();
                            }
                        }
                        else if (buttonNums[i - 2] == 1)
                        {
                            if (!indicatorLight)
                            {
                                wires[i] = false;
                            }
                            else
                            {
                                die();
                            }
                        }
                        else if (buttonNums[i - 2] == 2)
                        {
                            wires[i] = false;
                        }
                        else
                        {
                            if (buttonNums[i - 3] == 0)
                            {
                                if (indicatorLight)
                                {
                                    wires[i] = false;
                                }
                                else
                                {
                                    die();
                                }
                            }
                            else if (buttonNums[i - 3] == 1)
                            {
                                if (!indicatorLight)
                                {
                                    wires[i] = false;
                                }
                                else
                                {
                                    die();
                                }
                            }
                            else if (buttonNums[i - 3] == 2)
                            {
                                wires[i] = false;
                            }
                        }
                    }
                    break;
            }
            if(checkOk() == 2)
            {
                die();
            }
        }
        
    }


    int checkOk()
    {
        if(currentType == "math")
        {
            if(Mathf.Abs(mathSlider.value - correctAnswer) < .1f)
            {
                return 1;
            }
            return 0;
        }

        bool allCorrect = true;
        bool blowingUp = false;

        for(int i = 0; i < wires.Count; i++)
        {
            if(!correctSolution[i] && wires[i])
            {
                allCorrect = false;
            }
            if(correctSolution[i] && !wires[i])
            {
                blowingUp = true;
            }
        }

        if (blowingUp)
        { 
            return 2;
        }
        if (allCorrect)
        {
            return 1;
        }
        return 0;
    }
    [SerializeField] GameObject otherComponents;
    public void generateBomb()
    {
        correctSolution = new List<bool> { true, true, true, true, true };
        wires = new List<bool> { true, true, true, true, true };
        buttonPressed = new List<bool> { false, false, false, false };
        correctAnswer = 0;
        Debug.Log("oiashdfoijweaifiowejf");

        for (int i = 0; i < wireImages.Count; i++){
            int randInt = Random.Range(0, 4);
            wireImageNums[i] = randInt;
            wireImages[i].texture = Resources.Load<Texture>("Sprites/wire" + randInt);
            if(Random.Range(0f, 1f) < .5)
            {
                wireImages[i].rectTransform.rotation = Quaternion.Euler(0, 0, 180);
            }
            else
            {
                wireImages[i].rectTransform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }

        for (int i = 0; i < buttonImages.Count; i++)
        {
            buttonImages[i].color = Color.white;
        }

        for (int i = 0; i < symbolImages.Count; i++)
        {
            symbolImages[i].rectTransform.rotation = Quaternion.Euler(0, 0, 0);
        }

        wireBomb.SetActive(false);
        buttonBomb.SetActive(false);
        otherComponents.SetActive(true);

        currentType = bombTypes[level];
        switch (bombTypes[level])
        {
            case "wire":
                wireBomb.SetActive(true);
                break;
            case "button":
                buttonBomb.SetActive(true);
                break;
            case "math":
                mathBomb.SetActive(true);
                break;
        }
        Debug.Log(level);

        switch (level)
        {
            case 0:
                for(int i = 0; i < 5; i++)
                {
                    if(Random.Range(0f, 1f) < .5f)
                    {
                        wireImages[i].color = Color.cyan;
                    }
                    else
                    {
                        wireImages[i].color = Color.red;
                        correctSolution[i] = false;
                    }
                }
                wireImages[4].color = Color.red;
                correctSolution[4] = false;
                break;
            case 2:
                for (int i = 0; i < 5; i++)
                {
                    if (Random.Range(0f, 1f) < .6f)
                    {
                        if(Random.Range(0f, 1f) < .5)
                        {
                            wireImages[i].color = Color.white;
                            if (i % 2 == 0)
                            {
                                correctSolution[i] = false;
                            }
                        }
                        else
                        {
                            wireImages[i].color = Color.yellow;
                            if (i % 2 == 1)
                            {
                                correctSolution[i] = false;
                            }
                        }
                    }
                    else if(Random.Range(0f, 1f) < .85f)
                    {
                        if (Random.Range(0f, 1f) < .5f)
                        {
                            wireImages[i].color = Color.magenta;
                        }
                        else
                        {
                            wireImages[i].color = Color.blue;
                        }
                    }
                    else
                    { 
                        wireImages[i].color = Color.red;
                        correctSolution[i] = false;
                    }
                }

                for (int i = 0; i < 5; i++)
                {
                    if (wireImages[i].color == Color.magenta)
                    {
                        if (i != 0 && wireImages[i - 1].color == Color.blue)
                        {
                            correctSolution[i] = false;
                        }
                        if (i != wireImages.Count - 1 && wireImages[i + 1].color == Color.blue)
                        {
                            correctSolution[i] = false;
                        }
                    }
                }

                break;
            case 1:
                for (int i = 0; i < 5; i++)
                {
                    float randNum = Random.Range(0f, 1f);
                    if(randNum < .1f)
                    {
                        wireImages[i].color = Color.red;
                        correctSolution[i] = false;
                    }
                    else if (randNum < .6f)
                    {
                        wireImages[i].color = Color.magenta;
                    }
                    else if(randNum < .85f)
                    {
                        wireImages[i].color = Color.blue;
                    }
                    else
                    {
                        wireImages[i].color = Color.gray;
                    }
                }

                for (int i = 0; i < 5; i++)
                {
                    if(wireImages[i].color == Color.magenta)
                    {
                        if(i != 0 && wireImages[i-1].color == Color.blue)
                        {
                            correctSolution[i] = false;
                        }
                        if (i != wireImages.Count - 1 && wireImages[i + 1].color == Color.blue)
                        {
                            correctSolution[i] = false;
                        }
                    }
                }
                break;
            case 3:
                for (int i = 0; i < 4; i++)
                {
                    if (Random.Range(0f, 1f) < .5f)
                    {
                        correctSolution[i] = false;
                        buttonImages[i].texture = Resources.Load<Texture>("Sprites/button0");
                        buttonNums[i] = 0;
                    }
                    else
                    {
                        buttonImages[i].texture = Resources.Load<Texture>("Sprites/button2");
                        buttonNums[i] = 2;
                    }
                }
                break;
            case 4:
                for (int i = 0; i < 4; i++)
                {
                    float rand = Random.Range(0f, 1f);
                    if (rand < .25f)
                    {
                        correctSolution[i] = false;
                        buttonImages[i].texture = Resources.Load<Texture>("Sprites/button0");
                        buttonNums[i] = 0;
                    }else if(rand < .75f)
                    {
                        buttonImages[i].texture = Resources.Load<Texture>("Sprites/button1");
                        buttonNums[i] = 1;
                        correctSolution[i] = false;
                    }
                    else
                    {
                        buttonImages[i].texture = Resources.Load<Texture>("Sprites/button2");
                        buttonNums[i] = 2;
                    }
                }
                break;
            case 5:
                for (int i = 0; i < 4; i++)
                {
                    float rand = Random.Range(0f, 1f);
                    if (rand < .2f)
                    {
                        correctSolution[i] = false;
                        buttonImages[i].texture = Resources.Load<Texture>("Sprites/button0");
                        buttonNums[i] = 0;
                    }
                    else if (rand < .4f)
                    {
                        buttonImages[i].texture = Resources.Load<Texture>("Sprites/button1");
                        buttonNums[i] = 1;
                        correctSolution[i] = false;
                    }
                    else if (rand < .6f)
                    {
                        buttonImages[i].texture = Resources.Load<Texture>("Sprites/button2");
                        buttonNums[i] = 2;
                    }
                    else
                    {
                        if(i == 0)
                        {
                            correctSolution[i] = false;
                            buttonImages[i].texture = Resources.Load<Texture>("Sprites/button0");
                            buttonNums[i] = 0;
                        }
                        else
                        {
                            buttonImages[i].texture = Resources.Load<Texture>("Sprites/button3");
                            buttonNums[i] = 3;
                            correctSolution[i] = correctSolution[i - 1];
                        }
                    }
                }
                break;
            case 6:
                for(int i = 0; i < symbolImages.Count; i++)
                {
                    if(Random.Range(0f, 1f) < .5f)
                    {
                        correctAnswer += .25f;
                        symbolImages[i].texture = Resources.Load<Texture>("Sprites/triangle0");
                    }
                    else
                    {
                        symbolImages[i].texture = Resources.Load<Texture>("Sprites/triangle2");
                    }
                }
                break;
            case 7:
                for (int i = 0; i < symbolImages.Count; i++)
                {
                    if (Random.Range(0f, 1f) < .7f)
                    {
                        
                        symbolImages[i].texture = Resources.Load<Texture>("Sprites/triangle0");
                        float rand = Random.Range(0f, 1f);
                        if (rand < .5f)
                        {
                            correctAnswer += .25f;
                        }
                        else if(rand < .75f)
                        {
                            symbolImages[i].rectTransform.rotation = Quaternion.Euler(0, 0, 180);
                            correctAnswer -= .25f;
                        }
                        else
                        {
                            symbolImages[i].rectTransform.rotation = Quaternion.Euler(0, 0, 90);
                            correctAnswer += .125f;
                        }
                    }
                    else
                    {
                        symbolImages[i].texture = Resources.Load<Texture>("Sprites/triangle2");
                        float rand = Random.Range(0f, 1f);
                        if (rand < .5f)
                        {
                            
                        }
                        else if (rand < .75f)
                        {
                            symbolImages[i].rectTransform.rotation = Quaternion.Euler(0, 0, 180);
                        }
                        else
                        {
                            symbolImages[i].rectTransform.rotation = Quaternion.Euler(0, 0, 90);
                        }
                    }
                }
                correctAnswer = Mathf.Max(correctAnswer, 0);
                break;
            case 8:
                for (int i = 0; i < symbolImages.Count; i++)
                {
                    float rand = Random.Range(0f, 1f);
                    if (rand < .4f)
                    {
                        symbolImages[i].texture = Resources.Load<Texture>("Sprites/triangle0");
                        float rand2 = Random.Range(0f, 1f);
                        if (rand2 < .5f)
                        {
                            correctAnswer += .25f;
                        }
                        else if (rand2 < .75f)
                        {
                            symbolImages[i].rectTransform.rotation = Quaternion.Euler(0, 0, 180);
                            correctAnswer -= .25f;
                        }
                        else
                        {
                            symbolImages[i].rectTransform.rotation = Quaternion.Euler(0, 0, 90);
                            correctAnswer += .125f;
                        }
                    }else if(rand < .8f)
                    {
                        symbolImages[i].texture = Resources.Load<Texture>("Sprites/triangle1");
                        float rand2 = Random.Range(0f, 1f);
                        if (rand2 < .5f)
                        {
                            correctAnswer += .3333f;
                        }
                        else if (rand2 < .75f)
                        {
                            symbolImages[i].rectTransform.rotation = Quaternion.Euler(0, 0, 180);
                            correctAnswer -= .3333f;
                        }
                        else
                        {
                            symbolImages[i].rectTransform.rotation = Quaternion.Euler(0, 0, 90);
                            correctAnswer += .166666f;
                        }
                    }
                    else
                    {
                        symbolImages[i].texture = Resources.Load<Texture>("Sprites/triangle2");
                        float rand2 = Random.Range(0f, 1f);
                        if (rand < .5f)
                        {

                        }
                        else if (rand < .75f)
                        {
                            symbolImages[i].rectTransform.rotation = Quaternion.Euler(0, 0, 180);
                        }
                        else
                        {
                            symbolImages[i].rectTransform.rotation = Quaternion.Euler(0, 0, 90);
                        }
                    }
                }
                correctAnswer = Mathf.Max(correctAnswer, 0);
                break;

        }

        if(currentType != "math")
        {
            bool hasCut = false;
            for (int i = 0; i < wires.Count; i++)
            {
                if (!correctSolution[i])
                {
                    hasCut = true;
                }
            }

            if (!hasCut)
            {
                generateBomb();
            }
        }
    }
    

    [SerializeField] AudioSource dateAudio;
    void die()
    {
        if (!hasBlown)
        { 
            
            health--;
            if(health <= 0)
            {
                theDate.changeExpression("surprised");
                StartCoroutine(gameOver());
            }else if(health == 2)
            {
                background.texture = Resources.Load<Texture>("Sprites/background_new_2");
                chatter2.SetActive(false);
                explosion.PlayOneShot(Resources.Load<AudioClip>("Sound/screams2"),.3f);
                hand.fly();
            }else if(health == 1)
            {
                background.texture = Resources.Load<Texture>("Sprites/background_new_3");
                float curTime = dateAudio.time;
                dateAudio.Stop();
                dateAudio.clip = Resources.Load<AudioClip>("Sound/datePianoless");
                dateAudio.time = curTime;
                dateAudio.Play();
                explosion.PlayOneShot(Resources.Load<AudioClip>("Sound/screams1"),.3f);
            }
            blooming = true;
            cShake.Shake(5, .1f);
            Debug.Log("diee!!!!");
            explosion.PlayOneShot(Resources.Load<AudioClip>("Sound/explosion"));
            hasBlown = true;
        }
        
    }

    [SerializeField] SpriteRenderer redScreen;
    IEnumerator gameOver()
    {
        aHandler.unlockAchievement(3);
        explosion.PlayOneShot(Resources.Load<AudioClip>("Sound/Dialogue/protag_0_0"));
        explosion.PlayOneShot(Resources.Load<AudioClip>("Sound/date_scream"));
        colorFader c1 = Instantiate(Resources.Load<GameObject>("Prefabs/Sprite Fader")).GetComponent<colorFader>();
        c1.set(redScreen, new Color(1, 0, 0, 1), 1.5f);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("gameOver");
    }

    void win()
    {
        if(health > 0)
        {
            if(aHandler != null)
            {
                aHandler.unlockAchievement(2);
            }
            
            if(PlayerPrefs.GetInt("skippedIntro", 0) <= 2 && aHandler != null)
            {
                aHandler.unlockAchievement(10);
                if(health == 3 && theDate.getAffection() > 5 )
                {
                    aHandler.unlockAchievement(11);
                }
            }


            Debug.Log("you win!!!");
            PlayerPrefs.SetInt("finalHealth", health);
            PlayerPrefs.SetFloat("finalAffection", theDate.getAffection());
            if (theDate.getAffection() > 5)
            {
                if(aHandler != null)
                {
                    aHandler.unlockAchievement(5);
                }
                
                SceneManager.LoadScene("win");
            }
            else
            {
                if (aHandler != null)
                {
                    aHandler.unlockAchievement(4);
                }
                SceneManager.LoadScene("winLoveless");
            }
        } 
    }
    [SerializeField] achivementHandler aHandler;
    IEnumerator resetBomb()
    {
        soundFx.clip = Resources.Load<AudioClip>("Sound/bomb_timer");
        soundFx.time = 0;
        soundFx.Play();

        yield return new WaitForSeconds(timeBetween);
        if(checkOk() == 0)
        {
            die();
        }
        if (checkOk() == 1)
        {
            soundFx.PlayOneShot(Resources.Load<AudioClip>("Sound/puzzle_solve"));
        }
        hasBlown = false;
        isMoving = true;
        xPos = 0;
        yPos = -900;
        howManyLeft--;
        yield return new WaitForSeconds(1f / accelSpeed + .1f);
        generateBomb();
        isMoving = true;
        xPos = 0;
        yPos = 0;
        if (howManyLeft == 0)
        {
            level++;
            if(level == 9)
            {
                win();
                yield break;
            }
            howManyLeft = howManyBombs[level];

            if(level == 3 && aHandler != null)
            {
                aHandler.unlockAchievement(0);
            }else if(level == 6 && aHandler != null)
            {
                aHandler.unlockAchievement(1);
            }
        }
       
        bombTimer = timeBetween;
        StartCoroutine(resetBomb());
        
    }

    public void makeActive()
    {
        active = true;
    }
}
