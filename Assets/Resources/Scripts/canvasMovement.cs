using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class canvasMovement : MonoBehaviour
{
    EasingFunction.Function function;
    string direction;
    float xPosOld;
    float yPosOld;
    float xPos;
    float yPos;

    float xPosOldCam;
    float yPosOldCam;
    float xPosCam;
    float yPosCam;

    Vector3 currentPos;
    Vector3 currentPosCam;

    float timer = 0;
    float timerB = 0;

    bool isMoving = false;
    bool onBomb = false;
    bool bombZoom = false;
    public bool bombOut = false;


    [SerializeField] RectTransform sections;
    [SerializeField] RectTransform sections2;
    [SerializeField] Transform camPosition;
    [SerializeField] RectTransform table;
    [SerializeField] float accelSpeed;

    [SerializeField] GameObject bombAudio;
    [SerializeField] GameObject dateAudio;
    [SerializeField] RectTransform phone;
    [SerializeField] GameObject phoneInside;

    [SerializeField] AudioSource bombBGM;
    [SerializeField] AudioSource dateBGM;
    [SerializeField] PostProcessVolume m_Volume;
    [SerializeField] bomb theBomb;
    [SerializeField] date theDate;
    [SerializeField] AudioSource ticking;
    [SerializeField] AudioSource protag;
    [SerializeField] AudioSource bombVoice;
    [SerializeField] AudioSource dateVoice;
    ColorGrading cg;
    Bloom bloom;

    public float bloomModifier;

    // Start is called before the first frame update
    void Start()
    {
        EasingFunction.Ease movement = EasingFunction.Ease.EaseOutBack;
        function = EasingFunction.GetEasingFunction(movement);
        xPosOld = sections.anchoredPosition.x;
        yPosOld = sections.anchoredPosition.y;
        xPosOldCam = camPosition.position.x;
        yPosOldCam = camPosition.position.y;
        currentPos = new Vector2(xPosOld, yPosOld);
        currentPosCam = new Vector3(xPosOldCam, yPosOldCam);
        m_Volume.profile.TryGetSettings(out cg);
        m_Volume.profile.TryGetSettings(out bloom);

        bombVoice.GetComponent<AudioSource>().time = theDate.getCurrentStartPos();
        dateVoice.GetComponent<AudioSource>().time = theDate.getCurrentStartPos();
        protag.time = theDate.getCurrentStartPos();
    }

    public void setBloomModifier(float m)
    {
        float oldBloom = bloomModifier;
        bloomModifier = m;
        if (!onBomb)
        {
            if(oldBloom == bloom.intensity.value)
            {
                bloom.intensity.value = m;
            }
            else
            {
                bloom.intensity.value = Mathf.Max(bloom.intensity.value, bloomModifier);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
       
        if (!isMoving)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("asdf");
                switchView();
            }
        }

        sections2.position = sections.position;


        if (isMoving)
        {
            timer += Time.deltaTime;
            currentPos = new Vector2(function(xPosOld, xPos, timer * accelSpeed), function(yPosOld, yPos, timer * accelSpeed));
            currentPosCam = new Vector3(function(xPosOldCam, xPosCam, timer * accelSpeed), function(yPosOldCam, yPosCam, timer * accelSpeed), -10);
            sections.anchoredPosition = currentPos;
            camPosition.position = currentPosCam;
           
            if (timer >= (1f/accelSpeed))
            {
                isMoving = false;
                xPosOld = xPos;
                yPosOld = yPos;
                xPosOldCam = xPosCam;
                yPosOldCam = yPosCam;
                timer = 0;
                sections.anchoredPosition = new Vector2(xPos, yPos);
                camPosition.position = new Vector2(xPosCam, yPosCam);
            }
        }

        if (bombZoom && !bombOut)
        {
            timerB += Time.deltaTime * 2f;
            if(timerB > .5f && timerB <= 1.5f)
            {
                phone.localScale = new Vector3(function(1, 4, timerB - .5f), function(1, 4, timerB - .5f), 1);
                cg.temperature.value = function(25, -25, timerB - .5f);
            }
            if(timerB >= 2f)
            {
                phoneInside.SetActive(true);
                timerB = 0;
                bombZoom = false;
            }
        }

        if (bombOut && !bombZoom)
        {
            timerB += Time.deltaTime * 2;
            if (timerB > .5f && timerB <= 1.5f)
            {
                cg.temperature.value = function(-25, 25, timerB - .5f);
                phone.localScale = new Vector3(function(4, 1, timerB - .5f), function(4, 1, timerB - .5f), 1);
            }
            if (timerB >= 1.5f)
            {
                timerB = 0;
                isMoving = true;
                bombOut = false;
            }
        }

        
    }


    public void switchView()
    {
        theDate.spaceAction();
        Debug.Log("pee");

        if (theBomb.getCanStartBomb() && !isMoving)
        {
            if (!onBomb && bombOut == false && bombZoom == false)
            {
                protag.PlayOneShot(Resources.Load<AudioClip>("Sound/swish0"));
                isMoving = true;
                onBomb = true;
                bombZoom = true;
                xPos = 0;
                yPos = 900;
                xPosCam = 0;
                yPosCam = -10;
                bombBGM.mute = false;
                dateBGM.mute = true;
                if (bloom.intensity.value == bloomModifier)
                {
                    bloom.intensity.value = 0;
                }

                foreach (AudioSource a in bombAudio.GetComponentsInChildren<AudioSource>())
                {
                    a.volume = a.volume * 3f;
                }

                foreach (AudioSource a in dateAudio.GetComponentsInChildren<AudioSource>())
                {
                    a.volume = a.volume / 6f;
                }
            }
            else if (bombOut == false && bombZoom == false)
            {
                protag.PlayOneShot(Resources.Load<AudioClip>("Sound/swish1"));
                bloom.intensity.value = Mathf.Max(bloom.intensity.value, bloomModifier);
                onBomb = false;
                //isMoving = true;
                bombOut = true;
                phoneInside.SetActive(false);
                xPos = 0;
                yPos = 0;
                xPosCam = 0;
                yPosCam = 0;
                bombBGM.mute = true;
                dateBGM.mute = false;
                goku.SetActive(false);

                foreach (AudioSource a in bombAudio.GetComponentsInChildren<AudioSource>())
                {
                    a.volume = a.volume / 3f;
                }

                foreach (AudioSource a in dateAudio.GetComponentsInChildren<AudioSource>())
                {
                    a.volume = a.volume * 6f;
                }

                if (Random.Range(0f, 1f) < .01f)
                {
                    goku.SetActive(true);
                }
            }
        }
    }


    public Vector3 getCurrentPos()
    {
        return new Vector3(currentPos.x, currentPos.y, 0);
    }
    [SerializeField] GameObject goku;
    public bool isOnBomb()
    {
        return onBomb;
    }

    public bool getIsMoving()
    {
        return isMoving;
    }

    public void panToBomb()
    {
        
    }
}
