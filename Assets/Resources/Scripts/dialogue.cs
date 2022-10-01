using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dialogue
{
    List<string> options = new List<string>();
    int id = 0;
    int order = 0;
    string busyOption = "";
    float affectionMult = 1;
    float timeUntilAnswer = 10;
    float timeToAnswer = 3;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public dialogue(int i, string op, int or, string b, float a, float t1, float t2)
    {
        id = i;
        order = or;
        options = new List<string>(op.Split(','));
        affectionMult = a;
        busyOption = b;
        timeUntilAnswer = t1;
        timeToAnswer = t2;
    }

    public int getID()
    {
        return id;
    }

    public int getOrder()
    {
        return order;
    }

    public float getAffectionMult()
    {
        return affectionMult;
    }

    public float getTimeUntilAnswer()
    {
        return timeUntilAnswer;
    }

    public float getTimeToAnswer()
    {
        return timeToAnswer;
    }

    public string getOption(int i)
    {
        if(i == -1)
        {
            return busyOption;
        }
        if(i < options.Count)
        {
            return options[i];
        }
        else
        {
            return "";
        }
    }
}
