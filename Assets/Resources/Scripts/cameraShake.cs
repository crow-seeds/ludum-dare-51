using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class cameraShake : MonoBehaviour
{
    public static cameraShake instance;

    private Vector3 _originalPos;
    private Vector3 _originalPosCanvas;
    private float _timeAtCurrentFrame;
    private float _timeAtLastFrame;
    private float _fakeDelta;
    [SerializeField] RectTransform canvas;
    [SerializeField] canvasMovement canvasMove;

    GameObject[] sprites;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        sprites = GameObject.FindGameObjectsWithTag("sprite");
        
    }

    void Update()
    {
        // Calculate a fake delta time, so we can Shake while game is paused.
        _timeAtCurrentFrame = Time.realtimeSinceStartup;
        _fakeDelta = _timeAtCurrentFrame - _timeAtLastFrame;
        _timeAtLastFrame = _timeAtCurrentFrame;
    }

    public void Shake(float duration, float amount)
    {
        Debug.Log("aoisdjfoiwe");
        instance._originalPos = instance.gameObject.transform.localPosition;
        instance._originalPosCanvas = canvas.localPosition;

        instance.StopAllCoroutines();
        instance.StartCoroutine(instance.cShake(duration, amount));
    }

    public IEnumerator cShake(float duration, float amount)
    {
        float endTime = Time.time + duration;

        while (duration > 0)
        {
            float newAmount = amount;
            if(duration < 1)
            {
                newAmount *= duration; 
            }
            //transform.localPosition = _originalPos + Random.insideUnitSphere * newAmount;
            canvas.localPosition = canvasMove.getCurrentPos() + Random.insideUnitSphere * newAmount * 160;
            canvas.localPosition = new Vector3(canvas.localPosition.x, canvas.localPosition.y, 0);

            Vector3 rand = Random.insideUnitSphere;
            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i].transform.position +=  rand * newAmount * .2f;
            }
            duration -= _fakeDelta;

            yield return null;
        }

        //transform.localPosition = _originalPos;
    }
}