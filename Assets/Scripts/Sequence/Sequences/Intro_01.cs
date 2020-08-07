using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro_01 : SequenceBase
{
    public StartPanel startPanel;

    
    //on pressing the start button go to next sequence
    void StartButtonPressed(DOW_Button button)
    {
        //Debug.Log("Start button pressed !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        SequenceManager.Instance.GoToNextSequence();

        //Activate vuforia engine on pressing start button
        VuforiaManager.Instance.EnableVuforia();
    }


    public override void Execute()
    {
        //Disable the vuforia engine initially
        VuforiaManager.Instance.DisableVuforia();

        //Activates the start panel
        startPanel.ActivatePanel(true);

        //Register event to start button press
        startPanel.startButton.ButtonPressEvent += StartButtonPressed;
    }
    protected override bool CleanUpStep(int currentIndex, int gotoIndex)
    {
        if (!base.CleanUpStep(currentIndex, gotoIndex))
            return false;

        //Unsubscribe events
        startPanel.startButton.ButtonPressEvent -= StartButtonPressed;

        //Reset panels
        startPanel.ResetPanel();

        return true;
    }

}
