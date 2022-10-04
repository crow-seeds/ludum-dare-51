using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class moveAroundCanvas : MonoBehaviour
{
    Vector3 originalPosition;
    RectTransform thisObj;
    [SerializeField] float speed;
    [SerializeField] canvasMovement cm;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = gameObject.GetComponent<RectTransform>().localPosition;
        thisObj = gameObject.GetComponent<RectTransform>();
        Vector3 rand = Random.insideUnitSphere * 45;
        thisObj.localPosition = new Vector3(thisObj.localPosition.x + rand.x, thisObj.localPosition.y + rand.y, 0);
        Debug.Log(originalPosition);
        Debug.Log(thisObj.localPosition);
    }

    // Update is called once per frame
    void Update()
    {
        if (!cm.getIsMoving() && !cm.isOnBomb() && !cm.bombOut)
        {
            thisObj.RotateAround(originalPosition, Vector3.back, Time.deltaTime * speed);
            thisObj.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
