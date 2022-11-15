using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class subtitleObject
{
    public float time;
    public string text;
    public int choice;
    public int character;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public subtitleObject(float t, string te, int cho, int cha)
    {
        time = t;
        text = te;
        choice = cho;
        character = cha;
    }
}
