using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazards_03 : SequenceBase
{
    
    public PPE_Panel ppe_Panel;
    public HazardsPanel hazardsPanel;

    public override void Execute()
    {
        hazardsPanel.ActivatePanel();

        ppe_Panel.LinkPanelToButton(ppe_Panel.GetNextButton(), hazardsPanel);
        hazardsPanel.LinkPanelToButton(hazardsPanel.GetPreviousButton(), ppe_Panel);

        //Subscribe to the button press event of the next button of hazards panel
        hazardsPanel.GetNextButton().ButtonPressEvent += HazardsNextButtonClicked;
    }
    
    void HazardsNextButtonClicked(DOW_Button button)
    {
        hazardsPanel.GetNextButton().ButtonPressEvent -= HazardsNextButtonClicked;
        
        hazardsPanel.panelDeactivatedEvent += PanelDeactivated;
    }
    void PanelDeactivated(Panel_Base panel)
    {
        hazardsPanel.panelDeactivatedEvent -= PanelDeactivated;
        SequenceManager.Instance.GoToNextSequence();
    }

    protected override bool CleanUpStep(int currentIndex, int gotoIndex)
    {
        if (!base.CleanUpStep(currentIndex, gotoIndex))
            return false;

        //Unsubscribe to safe
        hazardsPanel.GetNextButton().ButtonPressEvent -= HazardsNextButtonClicked;
        hazardsPanel.panelDeactivatedEvent -= PanelDeactivated;

        //Reset panels
        //userPanel.ResetPanel();
        hazardsPanel.ResetPanel();

        return true;
    }

}
