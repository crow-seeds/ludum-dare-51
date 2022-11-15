using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startup : MonoBehaviour
{
    [SerializeField] AudioSource protag;
    [SerializeField] AudioSource date;
    [SerializeField] AudioSource bomb;
    [SerializeField] GameObject everything;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(load());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator load()
    {
        if(PlayerPrefs.GetInt("skippedIntro", 0) == 0)
        {
            yield return new WaitWhile(() => !(protag.clip.loadState.Equals(AudioDataLoadState.Loaded)));
            protag.Play();
        }
        else
        {
            yield return new WaitWhile(() => !(protag.clip.loadState.Equals(AudioDataLoadState.Loaded)));
            yield return new WaitWhile(() => !(date.clip.loadState.Equals(AudioDataLoadState.Loaded)));
            yield return new WaitWhile(() => !(bomb.clip.loadState.Equals(AudioDataLoadState.Loaded)));
            protag.Play();
            date.Play();
            bomb.Play();
        }
        everything.SetActive(true);
        
    }
}
