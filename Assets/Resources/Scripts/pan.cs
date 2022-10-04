using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pan : MonoBehaviour
{
    // Start is called before the first frame update

    float timer = 0;
    [SerializeField] RectTransform thisToMove;
    [SerializeField] float yValue;
    [SerializeField] AudioSource ticking;
    [SerializeField] Transform phone;
    bool isMoving = false;
    [SerializeField] bomb theBomb;
    [SerializeField] RectTransform table;

    float xPosOldTable;
    float yPosOldTable;
    float xPosTable;
    float yPosTable;



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            timer += Time.deltaTime / 5f;
            //table.anchoredPosition = new Vector3(0, Mathf.Lerp(-677, -322, Mathf.Min(timer * 2,1)), 0);
            thisToMove.localPosition = new Vector3(0, Mathf.Lerp(0, 900, timer), 0);
            if (thisToMove.localPosition.y >= yValue)
            {
                thisToMove.localPosition = new Vector3(0, yValue, 0);
                isMoving = false;
            }

        }
    }

    public void makeMove()
    {
        isMoving = true;
        ticking.mute = false;
        ticking.volume *= 5;
        phone.localScale = new Vector3(4,4,1);
        theBomb.generateBomb();
        theBomb.makeActive();
    }

}
