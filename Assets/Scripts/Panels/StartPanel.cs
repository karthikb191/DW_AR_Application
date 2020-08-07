using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPanel : Panel_Base
{
    public DOW_Button startButton;
    public UserPanel userPanel;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //Find the start button
        

        startButton.ButtonPressEvent += StartButtonPressed;
    }
    public override void ActivatePanel(bool animate = true)
    {
        base.ActivatePanel(animate);

        startButton.EnableButton();
    }

    void StartButtonPressed(DOW_Button button)
    {
        //Event that triggers when the current active panel is disabled
        //panelDeactivatedEvent += () => { userPanel.DisableAllButtons(); userPanel.ActivatePanel(true); };

        //Deactivate the start panel
        button.DisableButton();
        DeactivatePanel(true);

    }
    
}
