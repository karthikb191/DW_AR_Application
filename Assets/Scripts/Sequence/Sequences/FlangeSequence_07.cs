using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlangeSequence_07 : SequenceBase
{
    public SimplePanel nextButtonPanel;

    public GameObject highlightFlange;
    public GameObject reboilerObject;
    public MeshRenderer flangeRenderer;

    AnimationBeing flangeBeing;
    public override void Execute()
    {
        base.Execute();

        highlightFlange.gameObject.SetActive(true);
        reboilerObject.gameObject.SetActive(false);

        InstructionPanel.Instace.SetText("Please scan QR code #5");
        InstructionPanel.Instace.ActivatePanel(true);

        //Object should not be shown when tracked
        VuforiaManager.Instance.EnableTrackerIndex(4);
        VuforiaManager.Instance.GetCurrentTrackerEventHandler().RegisterToTrackerTrackedEvent(TrackerTracked);
    }

    void TrackerTracked()
    {
        //Unsubscribe
        VuforiaManager.Instance.GetCurrentTrackerEventHandler().UnregisterToTrackerTrackedEvent(TrackerTracked);
        InstructionPanel.Instace.DeactivatePanel(true);


        //Activate the panel
        StartCoroutine(ShowDevice());

        //Show the highlight flange
        VuforiaManager.Instance.GetCurrentTrackerEventHandler().ShowObject();


        flangeBeing = PanelAnimator.Instance.CreateMaterialAnimation(flangeRenderer.gameObject, flangeRenderer.sharedMaterial, "_Transparency",
                                                        AnimationType.Parallel, 1, 1, true);
    }
    IEnumerator ShowDevice()
    {
        yield return new WaitForSeconds(5);
        Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~Device Shown~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        nextButtonPanel.ActivatePanel(true);
        nextButtonPanel.panelDeactivatedEvent += NextButtonPanelClosed;
        //VuforiaManager.Instance.GetCurrentTrackerEventHandler().ShowObject();
    }
    void NextButtonPanelClosed(Panel_Base panel)
    {
        //Unsubscribe
        flangeBeing.Stop();
        nextButtonPanel.panelDeactivatedEvent -= NextButtonPanelClosed;
        VuforiaManager.Instance.DisableTrackerIndex(4);
        SequenceManager.Instance.GoToNextSequence();
    }
    
    protected override bool CleanUpStep(int currentIndex, int gotoIndex)
    {
        if (!base.CleanUpStep(currentIndex, gotoIndex))
            return false;

        VuforiaManager.Instance.DisableTrackerIndex(4);
        flangeBeing.Stop();
        StopAllCoroutines();

        VuforiaManager.Instance.GetCurrentTrackerEventHandler().UnregisterToTrackerTrackedEvent(TrackerTracked);
        nextButtonPanel.panelDeactivatedEvent -= NextButtonPanelClosed;


        nextButtonPanel.ResetPanel();
        
        return true;
    }
}
