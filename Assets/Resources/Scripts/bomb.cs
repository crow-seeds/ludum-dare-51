using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class bomb : MonoBehaviour
{
    // Start is called before the first frame update
    EasingFunction.Function function;
    float xPosOld;
    float yPosOld;
    float xPos;
    float yPos;
    float timer = 0;

    bool above24Sec = true;
    [SerializeField] float bombTimer = 30;
    [SerializeField] float accelSpeed;

    List<bool> wires = new List<bool> { true, true, true, true, true};
    List<bool> correctSolution = new List<bool> { true, true, true, true, true};
    List<int> wireImageNums = new List<int> { 0, 0, 0, 0, 0 };

    [SerializeField] TextMeshProUGUI bombTimeLCD;
    [SerializeField] List<RawImage> wireImages;
    [SerializeField] RectTransform bombLocation;
    bool isMoving = false;
    int level = 0;
    int howManyLeft = 3;

    [SerializeField] AudioSource soundFx;


    void Start()
    {
        generateBomb();
        EasingFunction.Ease movement = EasingFunction.Ease.EaseOutBack;
        function = EasingFunction.GetEasingFunction(movement);
        xPosOld = bombLocation.anchoredPosition.x;
        yPosOld = bombLocation.anchoredPosition.y;
    }

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

        bombTimer -= Time.deltaTime;
        string display = "";
        int roundedTimer = (int)bombTimer;
        int minutesPassed = roundedTimer / 60;
        if(minutesPassed <= 9)
        {
            display += "0";
        }
        display += minutesPassed.ToString() + ":";
        if(roundedTimer % 60 <= 9)
        {
            display += 0;
        }
        display += roundedTimer % 60;
        bombTimeLCD.text = display;


        if(roundedTimer <= 24 && above24Sec)
        {
            soundFx.Stop();
            above24Sec = false;
            soundFx.clip = Resources.Load<AudioClip>("Sound/bomb_panic");
            soundFx.loop = false;
            soundFx.Play();
        }else if(roundedTimer > 24 && !above24Sec)
        {
            soundFx.Stop();
            above24Sec = true;
            soundFx.clip = Resources.Load<AudioClip>("Sound/bomb_regular");
            soundFx.loop = true;
            soundFx.Play();
        }
    }

    public void cutWire(int i)
    {
        if(wires[i] == true)
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
                    SceneManager.LoadScene("SampleScene");
                    break;
            }
        }
        
    }


    int checkOk()
    {
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
            StartCoroutine(resetBomb());
            return 1;
        }
        return 0;
    }

    void generateBomb()
    {
        correctSolution = new List<bool> { true, true, true, true, true };
        wires = new List<bool> { true, true, true, true, true };
        for(int i = 0; i < wireImages.Count; i++){
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
                    if (Random.Range(0f, 1f) < .5f)
                    {
                        wireImages[i].color = Color.white;
                        if(i % 2  == 0)
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
                break;
            case 1:
                for (int i = 0; i < 5; i++)
                {
                    float randNum = Random.Range(0f, 1f);
                    if (randNum < .5f)
                    {
                        wireImages[i].color = Color.magenta;
                    }
                    else if(randNum < .8f)
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
        }

        bool hasCut = false;
        for(int i = 0; i < wires.Count; i++)
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

    IEnumerator resetBomb()
    {
        yield return new WaitForSeconds(.3f);
        isMoving = true;
        xPos = 0;
        yPos = -900;
        howManyLeft--;
        bombTimer += 10;
        if(bombTimer < 24)
        {
            above24Sec = false;
            soundFx.time = 24 - bombTimer;
        }

        Debug.Log(howManyLeft);
        yield return new WaitForSeconds(1f / accelSpeed + .1f);
        if(howManyLeft == 0)
        {
            howManyLeft = 3;
            level++;
            bombTimer = Mathf.Min(30 + (level - 1) * 5, bombTimer);
        }
        generateBomb();
        isMoving = true;
        xPos = 0;
        yPos = 48;
    }
}
