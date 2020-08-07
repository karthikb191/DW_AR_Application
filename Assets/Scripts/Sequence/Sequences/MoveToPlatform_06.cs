using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPlatform_06 : SequenceBase
{
    public float waitDuration = 5.0f;
    public SimplePanel MoveToPlatformPanel;
    public Animator moveToPlatformAnimator;

    public override void Execute()
    {
        base.Execute();

        InstructionPanel.Instace.SetText("Please scan QR code #4");
        InstructionPanel.Instace.ActivatePanel(true);
        VuforiaManager.Instance.EnableTrackerIndex(3);
        //VuforiaManager.Instance.GetCurrentTrackerEventHandler().ShowObject();

        VuforiaManager.Instance.GetCurrentTrackerEventHandler().RegisterToTrackerTrackedEvent(TrackerTracked);

    }
    void TrackerTracked()
    {
        //unsubscribe
        VuforiaManager.Instance.GetCurrentTrackerEventHandler().UnregisterToTrackerTrackedEvent(TrackerTracked);

        VuforiaManager.Instance.GetCurrentTrackerEventHandler().ShowObject();
        moveToPlatformAnimator.enabled = true;
        moveToPlatformAnimator.Play("ArrowLoop");

        InstructionPanel.Instace.DeactivatePanel(true);
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitDuration);

        //Enable the panel
        MoveToPlatformPanel.ActivatePanel(true);
        MoveToPlatformPanel.panelDeactivatedEvent += PanelClosed;
    }

    public void PanelClosed(Panel_Base panel)
    {
        //Unsubscribe to the panel closing event and go to next sequence
        MoveToPlatformPanel.panelDeactivatedEvent -= PanelClosed;
        VuforiaManager.Instance.DisableTrackerIndex(3);
        
        moveToPlatformAnimator.enabled = false;
        SequenceManager.Instance.GoToNextSequence();
    }

    protected override bool CleanUpStep(int currentIndex, int gotoIndex)
    {
        if (!base.CleanUpStep(currentIndex, gotoIndex))
            return false;

        //Deactivate the instruction panel
        InstructionPanel.Instace.DeactivatePanel(true);

        VuforiaManager.Instance.GetCurrentTrackerEventHandler().ShowObject();
        moveToPlatformAnimator.enabled = true;

        VuforiaManager.Instance.DisableTrackerIndex(3);
        StopAllCoroutines();

        MoveToPlatformPanel.panelDeactivatedEvent -= PanelClosed;
        VuforiaManager.Instance.GetCurrentTrackerEventHandler().UnregisterToTrackerTrackedEvent(TrackerTracked);

        MoveToPlatformPanel.ResetPanel();

        return true;
    }
}
