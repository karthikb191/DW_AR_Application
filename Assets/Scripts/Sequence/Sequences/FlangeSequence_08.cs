using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlangeSequence_08 : SequenceBase
{
    public SimplePanel observeSteamLeakagePanel;
    public OptionsPanel optionsPanel;
    public SimplePanel NotifyOperatorPanel;
    public SimplePanel closeBypassPanel;

    public GameObject highlightFlange;
    public GameObject reboilerObject;

    public override void Execute()
    {
        base.Execute();

        //Show the reboiler object
        highlightFlange.gameObject.SetActive(false);
        reboilerObject.gameObject.SetActive(true);

        VuforiaManager.Instance.GetCurrentTrackerEventHandler().ShowObject();

        //InstructionPanel.Instace.SetText("Please scan QR code #5");
        //InstructionPanel.Instace.ActivatePanel(true);

        //Object should not be shown when tracked
        //VuforiaManager.Instance.EnableTrackerIndex(4);
        //VuforiaManager.Instance.GetCurrentTrackerEventHandler().RegisterToTrackerTrackedEvent(TrackerTracked);

        //Activate the observe steam leakage panel

        observeSteamLeakagePanel.LinkPanelToButton(observeSteamLeakagePanel.GetNextButton(), optionsPanel);

        optionsPanel.LinkPanelToButton(optionsPanel.GetNextButton(), closeBypassPanel);
        optionsPanel.LinkPanelToButton(optionsPanel.GetPreviousButton(), NotifyOperatorPanel);

        closeBypassPanel.LinkPanelToButton(closeBypassPanel.GetPreviousButton(), optionsPanel);

        
        NotifyOperatorPanel.panelDeactivatedEvent += OperatorNotified;
        StartCoroutine(ShowDevice());
    }

    void TrackerTracked()
    {
        //Unsubscribe
        VuforiaManager.Instance.GetCurrentTrackerEventHandler().UnregisterToTrackerTrackedEvent(TrackerTracked);
        InstructionPanel.Instace.DeactivatePanel(true);
        
    }


    IEnumerator ShowDevice()
    {
        yield return new WaitForSeconds(2);
        //Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~Device Shown~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        observeSteamLeakagePanel.ActivatePanel(true);

        //VuforiaManager.Instance.GetCurrentTrackerEventHandler().ShowObject();
    }

    //void SteamLeakagePanelClosed(Panel_Base panel)
    //{
    //    //Unsubscribe
    //    observeSteamLeakagePanel.panelDeactivatedEvent -= SteamLeakagePanelClosed;

    //    StartCoroutine(ShowOptions(5));

    //}

    //bool steamLeaking = false;
    //bool noSteamLeaking = false;

    //IEnumerator ShowOptions(float duration)
    //{
    //    yield return new WaitForSeconds(duration);

    //    //Activate the options panel and enable buttons
    //    optionsPanel.ActivatePanel(true);
    //    optionsPanel.EnableButtons();

    //    //Subscribe to appropriate events
    //    optionsPanel.panelDeactivatedEvent += OptionsPanelDeactivated;

    //    optionsPanel.steamLeakingButton.ButtonPressEvent += SteamLeaking;
    //    optionsPanel.noSteamLeakingButton.ButtonPressEvent += NoSteamLeaking;
    //}
    //void SteamLeaking(DOW_Button button)
    //{
    //    steamLeaking = true; optionsPanel.DisableButtons();
    //}
    //void NoSteamLeaking(DOW_Button button)
    //{
    //    noSteamLeaking = true; optionsPanel.DisableButtons();
    //}

    //void OptionsPanelDeactivated(Panel_Base panel)
    //{
    //    optionsPanel.panelDeactivatedEvent -= OptionsPanelDeactivated;

    //    if (steamLeaking)
    //    {
    //        closeBypassPanel.ActivatePanel(true);
    //        closeBypassPanel.panelDeactivatedEvent += BackToOptions;
    //    }
    //    if (noSteamLeaking)
    //    {
    //        NotifyOperatorPanel.ActivatePanel(true);
    //        NotifyOperatorPanel.panelDeactivatedEvent += OperatorNotified;
    //    }

    //    //Reset the leak variables
    //    steamLeaking = false;
    //    noSteamLeaking = false;
    //}

    
    ////If there's a steam leaking, pressing the next button takes you back to the options
    //void BackToOptions(Panel_Base panel)
    //{
    //    //Unsubscribe
    //    closeBypassPanel.panelDeactivatedEvent -= BackToOptions;
    //    StartCoroutine(ShowOptions(1));
    //}



    void OperatorNotified(Panel_Base panel)
    {
        //Unsubscribe
        NotifyOperatorPanel.panelDeactivatedEvent -= OperatorNotified;

        //Disable the tracker
        VuforiaManager.Instance.DisableTrackerIndex(4);

        SequenceManager.Instance.GoToNextSequence();
        //Once this is done, Force open the tasks panel and show that all the tasks are completed
        PanelManager.Instance.TasksButton.ForceButtonPress();

    }

    protected override bool CleanUpStep(int currentIndex, int gotoIndex)
    {
        if (!base.CleanUpStep(currentIndex, gotoIndex))
            return false;

        StopAllCoroutines();

        //Disable the tracker
        VuforiaManager.Instance.DisableTrackerIndex(4);

        VuforiaManager.Instance.GetCurrentTrackerEventHandler().UnregisterToTrackerTrackedEvent(TrackerTracked);
        //observeSteamLeakagePanel.panelDeactivatedEvent -= SteamLeakagePanelClosed;
        //optionsPanel.panelDeactivatedEvent -= OptionsPanelDeactivated;
        //closeBypassPanel.panelDeactivatedEvent -= BackToOptions;
        NotifyOperatorPanel.panelDeactivatedEvent -= OperatorNotified;
        //optionsPanel.steamLeakingButton.ButtonPressEvent -= SteamLeaking;
        //optionsPanel.noSteamLeakingButton.ButtonPressEvent -= NoSteamLeaking;


        observeSteamLeakagePanel.ResetPanel();
        optionsPanel.ResetPanel();
        NotifyOperatorPanel.ResetPanel();
        closeBypassPanel.ResetPanel();

        return true;
    }
}
