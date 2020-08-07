using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instruction_03 : SequenceBase
{
    public SimplePanel stepbyStepInstructionPanel;

    int counter = 0;

    //Events
    System.Action<Panel_Base> FilesPressed = null;
    System.Action<Panel_Base> MapPressed = null;
    System.Action<Panel_Base> TasksPressed = null;

    public override void Execute()
    {
        stepbyStepInstructionPanel.ActivatePanel(true);


        //PanelManager.Instance.arButton.ButtonPressEvent += AllTabsPressed;
        stepbyStepInstructionPanel.panelDeactivatedEvent += StepbyStepInstructionPanelClosed;

        //Animate all the buttons
        PanelManager.Instance.TasksButton.Blink();
        PanelManager.Instance.MapButton.Blink();
        PanelManager.Instance.FilesButton.Blink();



        //FilesPressed = (Panel_Base panel) =>
        //{
        //    PanelManager.Instance.FilesButton.StopBlinking();
        //    Debug.Log("Files Pressed!!!");
        //    PanelManager.Instance.FilesButton.linkedPanel.panelActivatedEvent -= FilesPressed;
        //    counter++;
        //};
        //MapPressed = (Panel_Base panel) =>
        //{
        //    PanelManager.Instance.MapButton.StopBlinking();
        //    Debug.Log("MAP Pressed!!!");
        //    PanelManager.Instance.MapButton.linkedPanel.panelActivatedEvent -= MapPressed;
        //    counter++;
        //};
        //TasksPressed = (Panel_Base panel) =>
        //{
        //    PanelManager.Instance.TasksButton.StopBlinking();
        //    Debug.Log("Tasks Pressed!!!");
        //    PanelManager.Instance.TasksButton.linkedPanel.panelActivatedEvent -= TasksPressed;
        //    counter++;
        //};

        //PanelManager.Instance.FilesButton.linkedPanel.panelActivatedEvent += FilesPressed;
        //PanelManager.Instance.MapButton.linkedPanel.panelActivatedEvent += MapPressed;
        //PanelManager.Instance.TasksButton.linkedPanel.panelActivatedEvent += TasksPressed;
    }

    void StepbyStepInstructionPanelClosed(Panel_Base panel)
    {
        PanelManager.Instance.TasksButton.StopBlinking();
        PanelManager.Instance.MapButton.StopBlinking();
        PanelManager.Instance.FilesButton.StopBlinking();

        //Debug.Log("Counter:  " + counter);
        //If all the tabs are pressed and explored, go to next sequence
        //if (counter >= 3)
        //{
        //    Debug.Log("All Tabs Pressed!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        //    //PanelManager.Instance.arButton.ButtonPressEvent -= StepbyStepInstructionPanelClosed;
        //
        //}

        SequenceManager.Instance.GoToNextSequence();
    }

    protected override bool CleanUpStep(int currentIndex, int gotoIndex)
    {
        if (!base.CleanUpStep(currentIndex, gotoIndex))
            return false;

        counter = 0;

        //Unsubscribe
        stepbyStepInstructionPanel.panelDeactivatedEvent -= StepbyStepInstructionPanelClosed;
        //PanelManager.Instance.FilesButton.linkedPanel.panelActivatedEvent -= FilesPressed;
        //PanelManager.Instance.MapButton.linkedPanel.panelActivatedEvent -= MapPressed;
        //PanelManager.Instance.TasksButton.linkedPanel.panelActivatedEvent -= TasksPressed;
        //PanelManager.Instance.arButton.ButtonPressEvent -= AllTabsPressed;

        //If panels are blinking, stop them
        PanelManager.Instance.TasksButton.StopBlinking();
        PanelManager.Instance.MapButton.StopBlinking();
        PanelManager.Instance.FilesButton.StopBlinking();

        //Reset panels
        stepbyStepInstructionPanel.ResetPanel();

        return true;
    }

}
