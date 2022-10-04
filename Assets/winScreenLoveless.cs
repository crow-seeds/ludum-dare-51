using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class winScreenLoveless : MonoBehaviour
{
    [SerializeField] AudioSource wobble;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(heartBreak());
    }

    IEnumerator heartBreak()
    {
        yield return new WaitForSeconds(3.6f);
        wobble.Stop();
        wobble.clip = Resources.Load<AudioClip>("Sound/heartbreak_snap");
        wobble.time = 0;
        wobble.Play();
        wobble.PlayOneShot(Resources.Load<AudioClip>("Sound/heartbreak_emmet"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void backToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
