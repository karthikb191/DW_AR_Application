using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPanel : Panel_Base
{
    public UserPanelButton defaultButton;
    UserPanelButton activeButton = null;
    UserPanelButton previousActiveButton = null;

    Coroutine panelChangeCoroutine;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //ActivatePanel(true);

        foreach(DOW_Button button in DOW_Buttons)
        {
            button.ButtonPressEvent += ButtonPressed;
        }

        if(defaultButton)
            panelActivatedEvent += (Panel_Base panel) => {
                defaultButton.ForceButtonPress();
                activeButton = defaultButton;
            };
    }

    public void DisableAllButtons()
    {
        foreach (DOW_Button button in DOW_Buttons)
            button.DisableButton();
    }

    public override void ActivatePanel(bool animate = true)
    {
        base.ActivatePanel(animate);
    }
    int waitIndex = 0;
    //public override void DeactivatePanel(bool animate = true)
    //{
    //    base.DeactivatePanel(animate);
    //    
    //}
    //protected override IEnumerator StartActivationAnimations(bool animate = true)
    //{
    //    return base.StartActivationAnimations(animate);
    //}
    protected override IEnumerator StartDeactivationAnimations(bool animate = true)
    {
        if (activeButton)
        {
            activeButton.AnimateSizeDelta(null);
            if (activeButton.linkedPanel)
                if (activeButton.linkedPanel.IsActive())
                {
                    activeButton.linkedPanel.DeactivatePanel(true);
                    while (activeButton.linkedPanel.IsActive())
                        yield return null;
                }
        }
        if (previousActiveButton)
        {
            previousActiveButton.AnimateSizeDelta(null);
            if (previousActiveButton.linkedPanel)
                if (previousActiveButton.linkedPanel.IsActive())
                {
                    previousActiveButton.linkedPanel.DeactivatePanel(true);
                    while (previousActiveButton.linkedPanel.IsActive())
                        yield return null;
                }
        }
        activeButton = null;
        previousActiveButton = null;

        
        //StartCoroutine(base.StartDeactivationAnimations(animate));
        IEnumerator enumerator;
        enumerator = base.StartDeactivationAnimations(animate);
        while (true)
        {
            yield return enumerator.Current;
            if (enumerator.MoveNext() == false)
                yield break;

        }
        //yield break;
        //while (true)
        //{
        //    yield return enumerator.Current;
        //    if (enumerator.MoveNext() == false)
        //        yield break;
        //}
        
    }



    protected void ButtonPressed(DOW_Button button)
    {
        if (!activeButton)
            canswitch = true;

        if (canswitch)
        {
            Debug.Log("Active button: " + activeButton + "BUtton: " + button + "Previous active buttons: " + previousActiveButton);
            if (activeButton != button)
            {
                if (activeButton)
                {
                    activeButton.AnimateSizeDelta(null);
                    //activeButton.EnableButton();

                    button.AnimateSizeDelta(new Vector2(250, 190));
                    //button.DisableButton();
                    activeButton = button as UserPanelButton;
                }
                else
                {
                    activeButton = button as UserPanelButton;
                    activeButton.AnimateSizeDelta(new Vector2(250, 190));

                    //Disable the button so that no clicking is no more possible on this button
                    //activeButton.DisableButton();
                }
                Debug.Log("SDFSDFGSHDH#$Q%$%#$%@#^@%^@#$^@#4");
                ChangePanel();
            }
            else
            {
                //If the button is enabled, disable the button, or else disable the button
                if (button.IsEnabled())
                {
                    button.AnimateSizeDelta(null);
                    DisablePanel();
                }
                else
                {
                    activeButton.AnimateSizeDelta(new Vector2(250, 190));
                    //ActivatePanel();
                }
            }
            previousActiveButton = activeButton;

        }

        //if (previousActiveButton)
        //    previousActiveButton.HidePanel();
        //activeButton.ShowPanel();

    }

    bool canswitch = true;
    protected void DisablePanel()
    {
        if (defaultButton)
        {
            canswitch = true;
            
            //Unsubscribe all the events
            if(previousActiveButton)
                previousActiveButton.linkedPanel.panelDeactivatedEvent -= Deactivated;
            if(activeButton)
                activeButton.linkedPanel.panelActivatedEvent -= ActiveButtonShown;
            

            defaultButton.ForceButtonPress();

            
        }
    }

    //TODO: Consider disabling all the buttons if this goes out of hand
    void ChangePanel()
    {
        canswitch = false;

        //Debug.Log("Trying to change panel");
        if(previousActiveButton != null && previousActiveButton.linkedPanel)
        {
            //Debug.Log("1");
            if (previousActiveButton.linkedPanel)
            {
                Debug.Log("2");
                
                previousActiveButton.HidePanel();
                previousActiveButton.linkedPanel.panelDeactivatedEvent += Deactivated;
                
            }
            else
            {
                canswitch = true;
            }
            //Debug.Log("3");
        }
        else
        {
            if (activeButton.linkedPanel)
            {
                //Debug.Log("4");
                activeButton.ShowPanel();
                activeButton.linkedPanel.panelActivatedEvent += ActiveButtonShown;
            }
            else
            {
                //Debug.Log("5");
                canswitch = true;
            }
            //Debug.Log("6");
        }
        //Debug.Log("7");
    }

    void Deactivated(Panel_Base panel)
    {
        //Remove from subscription
        panel.panelDeactivatedEvent -= Deactivated;

        if (!activeButton)
            return;

        if (activeButton.linkedPanel)
        {
            if (activeButton.linkedPanel)
            {
                Debug.Log(".....................");
                activeButton.ShowPanel();
                activeButton.linkedPanel.panelActivatedEvent += ActiveButtonShown;

            }
            else
            {
                //Debug.Log("################");
                canswitch = true;
            }
        }
        else
        {
            canswitch = true;
        }
        
    }

    void ActiveButtonShown(Panel_Base panel)
    {
        //Debug.Log("Active button has finished showing");
        //Remove from subscription
        panel.panelActivatedEvent -= ActiveButtonShown;
        //if (activeButton.linkedPanel)
        //    activeButton.linkedPanel.panelActivatedEvent -= ActiveButtonShown;
        //can only switch after the active button is shown
        canswitch = true;
    }

    
}
