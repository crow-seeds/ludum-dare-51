using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class bomb : MonoBehaviour
{
    // Start is called before the first frame update
    EasingFunction.Function function;
    float xPosOld;
    float yPosOld;
    float xPos;
    float yPos;
    float timer = 0;
    [SerializeField] float accelSpeed;

    List<bool> wires = new List<bool> { true, true, true, true, true};
    List<bool> correctSolution = new List<bool> { true, true, true, true, true};

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
    }

    public void cutWire(int i)
    {
        if(wires[i] == true)
        {
            wireImages[i].texture = Resources.Load<Texture>("Sprites/wire_broken");
            wires[i] = false;
            soundFx.PlayOneShot(Resources.Load<AudioClip>("Sound/snip" + Random.Range(0, 5).ToString()));
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
        foreach(RawImage r in wireImages){
            r.texture = Resources.Load<Texture>("Sprites/wire");
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
            case 1:
                for (int i = 0; i < 5; i++)
                {
                    if (Random.Range(0f, 1f) < .5f)
                    {
                        wireImages[i].color = Color.green;
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
            case 2:
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
        Debug.Log(howManyLeft);
        yield return new WaitForSeconds(1f / accelSpeed + .1f);
        if(howManyLeft == 0)
        {
            howManyLeft = 3;
            level++;
        }
        generateBomb();
        isMoving = true;
        xPos = 0;
        yPos = 0;
    }
}
