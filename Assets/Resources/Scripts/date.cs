using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Xml;
using System.Globalization;
using System.IO;
using System;
using TMPro;

public class date : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] canvasMovement c;
    float affection = 100;
    [SerializeField] Image affectionBar;
    int dialogueIndex = 0;
    [SerializeField] TextAsset dialogueData;
    IFormatProvider format = new CultureInfo("en-US");
    List<List<dialogue>> dialogue = new List<List<dialogue>> {new List<dialogue>(), new List<dialogue>(), new List<dialogue>(), new List<dialogue>(), new List<dialogue>(), new List<dialogue>(), new List<dialogue>(), new List<dialogue>(), };
    bool canSelect = false;
    [SerializeField] List<TextMeshProUGUI> optionTexts;

    void Start()
    {
        loadDialogue();
    }

    void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    void loadDialogue()
    {
        string data = dialogueData.text;
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(new StringReader(data));

        XmlNodeList dialogueNodeList = xmlDoc.SelectNodes("//data/dialogue");

        foreach (XmlNode infonode in dialogueNodeList)
        {
            dialogue d = new dialogue(Convert.ToInt32(infonode.Attributes["id"].Value, format), infonode.Attributes["options"].Value, 
                Convert.ToInt32(infonode.Attributes["order"].Value, format), infonode.Attributes["busy"].Value,
                (float)Convert.ToDouble(infonode.Attributes["affectionMult"].Value, format), (float)Convert.ToDouble(infonode.Attributes["timeUntilAnswer"].Value, format),
                (float)Convert.ToDouble(infonode.Attributes["timeToAnswer"].Value, format));
            dialogue[d.getOrder()].Add(d);
        }

        foreach(List<dialogue> l in dialogue)
        {
            Shuffle(l);
        }

    }

    IEnumerator doDialogue()
    {
        yield return new WaitForSeconds(dialogue[dialogueIndex][0].getTimeUntilAnswer());


    }

    // Update is called once per frame
    void Update()
    {
        if (c.isOnBomb())
        {
            affection -= Time.deltaTime * 5;
        }
        else
        {
            affection += Time.deltaTime * 8;
        }
        affection = Mathf.Min(100, affection);
        affectionBar.fillAmount = affection / 100f;

    }
}
