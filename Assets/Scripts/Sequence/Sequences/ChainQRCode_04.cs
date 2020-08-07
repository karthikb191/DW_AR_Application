using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class ChainQRCode_04 : SequenceBase
{

    public int chainArrowAnimationDuration = 5;
    public SimplePanel chainInstructionPanel;
    public Animator arrowAnimator;

   
    public override void Execute()
    {
        base.Execute();

        InstructionPanel.Instace.SetText("Please scan the QR code #2");
        InstructionPanel.Instace.ActivatePanel(true);

        VuforiaManager.Instance.EnableTrackerIndex(1);
        //Subscribe to tracker found event
        VuforiaManager.Instance.GetCurrentTrackerEventHandler().RegisterToTrackerTrackedEvent(ChainTrackerFound);
    }

    public void ChainTrackerFound()
    {
        //Play the arrow animation
        arrowAnimator.enabled = true;
        arrowAnimator.Play("ArrowLoop");

        InstructionPanel.Instace.DeactivatePanel(true);
        //Should NOT disable tracker; Unsubscribe and start the timer
        VuforiaManager.Instance.GetCurrentTrackerEventHandler().UnregisterToTrackerTrackedEvent(ChainTrackerFound);

        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        //Wait for s specified amount of time
        yield return new WaitForSeconds(chainArrowAnimationDuration);

        chainInstructionPanel.ActivatePanel(true);
        //subscribe to the panel close event
        chainInstructionPanel.panelDeactivatedEvent += ChainInstructionPanelClosed;
    }

    void ChainInstructionPanelClosed(Panel_Base panel)
    {
        //Stop the animation
        arrowAnimator.enabled = false;
        
        //unsubscribe and move on to the next sequence

        SequenceManager.Instance.GoToNextSequence();

        //TODO: Change the target map location point
    }

    protected override bool CleanUpStep(int currentIndex, int gotoIndex)
    {
        arrowAnimator.enabled = false;

        if (!base.CleanUpStep(currentIndex, gotoIndex))
            return false;
        InstructionPanel.Instace.DeactivatePanel(true);
        StopAllCoroutines();
        VuforiaManager.Instance.GetCurrentTrackerEventHandler().UnregisterToTrackerTrackedEvent(ChainTrackerFound);
        chainInstructionPanel.panelDeactivatedEvent -= ChainInstructionPanelClosed;

        chainInstructionPanel.ResetPanel();

        return true;
    }
}
