using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardsPanel : Panel_Base
{
    public Sprite tickMark;
    protected override void Start()
    {
        base.Start();
        //allButtonsClickedEvent += AllButtonsClicked;
        //Enable the previous button
        //previousButton.gameObject.SetActive(true);
    }

    protected override void ButtonClicked(DOW_Button button)
    {
        base.ButtonClicked(button);
        //Change the button color and disable it
        button.transform.Find("Image").GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = tickMark;
        button.DisableButton();
    }

    void AllButtonsClicked()
    {
        //When all the buttons are clicked, deactivate the panel
        DeactivatePanel();
    }
}
