using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hand : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 dest = new Vector3(-2.546571f, - 4.220434f, 0);
    bool isMoving = false;
    float timer;
    [SerializeField] AudioSource soundFx;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (isMoving)
        {
            transform.Rotate(Vector3.back * Time.deltaTime * 900);
            transform.position = Vector3.MoveTowards(transform.position, dest, Time.deltaTime * 14);

            if(Vector3.Distance(transform.position, dest) <= .1f)
            {
                soundFx.PlayOneShot(Resources.Load<AudioClip>("Sound/hand_land"));
                transform.rotation = Quaternion.Euler(0, 0, 0);
                isMoving = false;
            }
        }
        
    }

    public void fly()
    {
        isMoving = true;
    }
}
