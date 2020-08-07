using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureGauge_05 : SequenceBase
{
    public SimplePanel psigCheckPanel;
    public SimplePanel boardOperatorPanel;
    public TimerPanel timerPanel;
    public MeshRenderer gaugeMaterial;
    public float timerDuration = 10;

    bool trackedFirstTime = false;
    AnimationBeing gaugeBeing;

    public override void Execute()
    {
        base.Execute();
        

        InstructionPanel.Instace.SetText("Please scan QR code #3");
        InstructionPanel.Instace.ActivatePanel(true);

        //Enable the appropriate tracker and dont show the object
        VuforiaManager.Instance.EnableTrackerIndex(2);
        VuforiaManager.Instance.GetCurrentTrackerEventHandler().RegisterToTrackerTrackedEvent(DisplayPsigChekPanel);

        //Animate the gauge material
        gaugeBeing = PanelAnimator.Instance.CreateMaterialAnimation(gaugeMaterial.gameObject, gaugeMaterial.sharedMaterial, "_Transparency", 
                                                        AnimationType.Parallel, 1, 1, true);

        //LinkPanels
        psigCheckPanel.LinkPanelToButton(psigCheckPanel.GetNextButton(), boardOperatorPanel);
        boardOperatorPanel.LinkPanelToButton(boardOperatorPanel.GetPreviousButton(), psigCheckPanel);
        boardOperatorPanel.LinkPanelToButton(boardOperatorPanel.GetNextButton(), timerPanel);
        timerPanel.LinkPanelToButton(timerPanel.GetPreviousButton(), boardOperatorPanel);

        //Timer panel activation event
        timerPanel.panelActivatedEvent += TimerPanelActivated;

        //Timer PanelNextClick
        timerPanel.GetNextButton().ButtonPressEvent += GoToNextSequence;

    }
    void TimerPanelActivated(Panel_Base panel)
    {
        timerPanel.StartTimer(10);
    }

    void DisplayPsigChekPanel()
    {
        //Show the highlight gauge
        VuforiaManager.Instance.GetCurrentTrackerEventHandler().ShowObject();

        //Play the arrow animation

        //Deactivate the instruction panel
        InstructionPanel.Instace.DeactivatePanel(true);

        //Disable current tracker and Unsubscribe
        //VuforiaManager.Instance.DisableCurrentTracker();
        VuforiaManager.Instance.GetCurrentTrackerEventHandler().UnregisterToTrackerTrackedEvent(DisplayPsigChekPanel);

        psigCheckPanel.ActivatePanel(true);

        //Next panel shows when current panel is deactivated
        //psigCheckPanel.panelDeactivatedEvent += BoardOperatorPanelDisplay;
    }
    void GoToNextSequence(DOW_Button panel)
    {
        VuforiaManager.Instance.DisableTrackerIndex(2);

        if (gaugeBeing != null)
            gaugeBeing.Stop();
        timerPanel.GetNextButton().ButtonPressEvent -= GoToNextSequence;
        //Sequence complete
        SequenceManager.Instance.GoToNextSequence();
        //TODO: The next seqeuence might need to be merged into this one
    }

    void BoardOperatorPanelDisplay(Panel_Base panel)
    {
        //Unsubscribe
        psigCheckPanel.panelDeactivatedEvent -= BoardOperatorPanelDisplay;

        //Activate the board operator panel and subscribe to it's closing event
        boardOperatorPanel.ActivatePanel(true);
        boardOperatorPanel.panelDeactivatedEvent += TimerPanelDisplay;
    }

    void TimerPanelDisplay(Panel_Base panel)
    {
        //Unsubscribe
        //boardOperatorPanel.panelDeactivatedEvent -= TimerPanelDisplay;

        //Activate timer panel and subscribe to it's deactivation event
        timerPanel.ActivatePanel(true);
        timerPanel.StartTimer(timerDuration);
        //timerPanel.panelDeactivatedEvent += TimerPanelDeactivated;
    }

    void TimerPanelDeactivated(Panel_Base panel)
    {
        //Sequence complete
        SequenceManager.Instance.GoToNextSequence(); 
        //TODO: The next seqeuence might need to be merged into this one
    }

    protected override bool CleanUpStep(int currentIndex, int gotoIndex)
    {
        if (!base.CleanUpStep(currentIndex, gotoIndex))
            return false;


        if (gaugeBeing != null)
            gaugeBeing.Stop();
        

        trackedFirstTime = false;
        VuforiaManager.Instance.DisableTrackerIndex(2);

        VuforiaManager.Instance.GetCurrentTrackerEventHandler().UnregisterToTrackerTrackedEvent(DisplayPsigChekPanel);
        psigCheckPanel.panelDeactivatedEvent -= BoardOperatorPanelDisplay;
        boardOperatorPanel.panelDeactivatedEvent -= TimerPanelDisplay;
        timerPanel.panelDeactivatedEvent -= TimerPanelDeactivated;

        timerPanel.panelActivatedEvent -= TimerPanelActivated;
        timerPanel.GetNextButton().ButtonPressEvent -= GoToNextSequence;
        
        //timerPanel.ResetTimer();
        timerPanel.ResetPanel();
        psigCheckPanel.ResetPanel();
        boardOperatorPanel.ResetPanel();

        return true;
    }
}
