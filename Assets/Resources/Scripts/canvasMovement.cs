using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canvasMovement : MonoBehaviour
{
    EasingFunction.Function function;
    string direction;
    float xPosOld;
    float yPosOld;
    float xPos;
    float yPos;
    float timer = 0;
    bool isMoving = false;
    bool onBomb = false;
    [SerializeField] RectTransform sections;
    [SerializeField] float accelSpeed;

    // Start is called before the first frame update
    void Start()
    {
        EasingFunction.Ease movement = EasingFunction.Ease.EaseOutBack;
        function = EasingFunction.GetEasingFunction(movement);
        xPosOld = sections.anchoredPosition.x;
        yPosOld = sections.anchoredPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!isMoving)
        {
            if(!onBomb && (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)))
            {
                isMoving = true;
                onBomb = true;
                xPos = 0;
                yPos = 900;
            }
            else if(onBomb && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)))
            {
                isMoving = true;
                onBomb = false;
                xPos = 0;
                yPos = 0;
            }else if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!onBomb)
                {
                    isMoving = true;
                    onBomb = true;
                    xPos = 0;
                    yPos = 900;
                }
                else
                {
                    isMoving = true;
                    onBomb = false;
                    xPos = 0;
                    yPos = 0;
                }
            }
        }


        if (isMoving)
        {
            timer += Time.deltaTime;
            sections.anchoredPosition = new Vector2(function(xPosOld, xPos, timer * accelSpeed), function(yPosOld, yPos, timer * accelSpeed));
            if (timer >= (1f/accelSpeed))
            {
                isMoving = false;
                xPosOld = xPos;
                yPosOld = yPos;
                timer = 0;
                sections.anchoredPosition = new Vector2(xPos, yPos);
            }
        }
    }
}
