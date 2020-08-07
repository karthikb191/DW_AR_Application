using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsPanel : SimplePanel
{
    [HideInInspector]
    public DOW_Button steamLeakingButton;
    [HideInInspector]
    public DOW_Button noSteamLeakingButton;

    protected override void Start()
    {
        base.Start();

        steamLeakingButton = nextButton;
        noSteamLeakingButton = previousButton;

        steamLeakingButton.gameObject.SetActive(true);
        noSteamLeakingButton.gameObject.SetActive(true);
    }

    public void DisableButtons()
    {
        steamLeakingButton.DisableButton();
        noSteamLeakingButton.DisableButton();
    }
    public void EnableButtons()
    {
        steamLeakingButton.EnableButton();
        noSteamLeakingButton.EnableButton();
    }
}
