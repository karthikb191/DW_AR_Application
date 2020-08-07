using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PPE_Panel : Panel_Base
{
    public Sprite tickMark;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }
    protected override void ButtonClicked(DOW_Button button)
    {
        base.ButtonClicked(button);
        //Change the button color and disable it
        //button.GetComponent<Image>().color = Color.blue;
        button.transform.Find("Image").GetChild(0).GetComponent<Image>().sprite = tickMark;
        button.DisableButton();
    }
}
