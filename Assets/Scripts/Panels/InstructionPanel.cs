using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InstructionPanel : Panel_Base
{
    private static InstructionPanel instance;
    TextMeshProUGUI textComponent;
    public static InstructionPanel Instace { get { return instance; } }

    protected override void Start()
    {
        if (instance == null)
            instance = this;
        else
            return;

        textComponent = transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        base.Start();
        
    }

    public void SetText(string text)
    {
        textComponent.text = text;
        textComponent.ForceMeshUpdate();
        //Adjust the panel width with respect to the text
        Bounds bounds = textComponent.bounds;
        //Debug.Log("Text rendered bounds: " + max + "     " + min);
        Debug.Log("Text rendered bounds: " + textComponent.bounds.ToString());

        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, bounds.extents.y * 2 + 25);
    }

}
