using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QRCode1_02 : SequenceBase
{
    public InstructionPanel instructionPanel;
    public UserPanel userPanel;

    public PPE_Panel ppe_Panel;
    public HazardsPanel hazardsPanel;

    public override void Execute()
    {
        if(!userPanel.gameObject.activeSelf)
            userPanel.ActivatePanel(true);

        InstructionPanel.Instace.ActivatePanel(true);

        //Set the text
        InstructionPanel.Instace.SetText("Please scan the QR code #1");

        //Enable the next QR code tracker
        VuforiaManager.Instance.EnableTrackerIndex(0);

        //subscribe to the tracked event
        VuforiaManager.Instance.GetCurrentTrackerEventHandler().RegisterToTrackerTrackedEvent(TrackerOneFound);
    }

    public void TrackerOneFound()
    {
        
        //Once the tracker is found, disable the current tracker and enable the PPE panel
        ppe_Panel.ActivatePanel();
        VuforiaManager.Instance.GetCurrentTrackerEventHandler().UnregisterToTrackerTrackedEvent(TrackerOneFound);
        if (ppe_Panel.gameObject.activeSelf)
        {
            //Unsubscribe from the tracker event

            VuforiaManager.Instance.DisableCurrentTracker();
            InstructionPanel.Instace.DeactivatePanel(true);


            //link the next button of ppe panel to hazards
            //ppe_Panel.LinkPanelToButton(ppe_Panel.GetNextButton(), hazardsPanel);
            //hazardsPanel.LinkPanelToButton(hazardsPanel.GetPreviousButton(), ppe_Panel);


            //Subscribe to the button press event of the next button of hazards panel
            ppe_Panel.GetNextButton().ButtonPressEvent += PPENextButtonClicked;

            //ppe_Panel.panelDeactivatedEvent += PPEDeactivated;
        }
    }
    void PPENextButtonClicked(DOW_Button button)
    {
        ppe_Panel.GetNextButton().ButtonPressEvent -= PPENextButtonClicked;

        ppe_Panel.panelDeactivatedEvent += PanelDeactivated;
    }
    void PanelDeactivated(Panel_Base panel)
    {
        ppe_Panel.panelDeactivatedEvent -= PanelDeactivated;
        SequenceManager.Instance.GoToNextSequence();
    }

    protected override bool CleanUpStep(int currentIndex, int gotoIndex)
    {
        if (!base.CleanUpStep(currentIndex, gotoIndex))
            return false;

        InstructionPanel.Instace.DeactivatePanel(true);
        //Unsubscribe to safe
        VuforiaManager.Instance.GetCurrentTrackerEventHandler().UnregisterToTrackerTrackedEvent(TrackerOneFound);
        ppe_Panel.GetNextButton().ButtonPressEvent -= PPENextButtonClicked;
        
        //Reset panels
        //userPanel.ResetPanel();
        ppe_Panel.ResetPanel();

        return true;
    }

}
