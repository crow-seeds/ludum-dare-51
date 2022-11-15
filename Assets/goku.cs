using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class goku : MonoBehaviour
{
    achivementHandler aHandler;
    [SerializeField] RawImage gokuImage;
    bool gone = false;


    // Start is called before the first frame update
    void Start()
    {
        aHandler = GameObject.FindObjectOfType<achivementHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onClick()
    {
        if (!gone)
        {
            Debug.Log("goku");
            colorFader2 c1 = Instantiate(Resources.Load<GameObject>("Prefabs/Image Fader")).GetComponent<colorFader2>();
            c1.set(gokuImage, new Color(1, 1, 1, 0), .5f);
            aHandler.unlockAchievement(7);
            gone = true;
        }
        
    }
}
