using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class locking : MonoBehaviour
{
    [SerializeField] int achievementRequirement;

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.GetInt("achievement" + achievementRequirement.ToString(), 0) == 0)
        {
            gameObject.GetComponent<Button>().interactable = false;
            gameObject.GetComponentInChildren<RawImage>().texture = Resources.Load<Texture>("Sprites/previews/locked");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
