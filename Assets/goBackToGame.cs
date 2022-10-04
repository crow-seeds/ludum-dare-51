using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class goBackToGame : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject subtitle;
    float timer = 0;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 2)
        {
            subtitle.SetActive(true);
        }
        if (timer > 5)
        {
            PlayerPrefs.SetInt("skippedIntro", 1);
            SceneManager.LoadScene("SampleScene");
        }
    }
}
