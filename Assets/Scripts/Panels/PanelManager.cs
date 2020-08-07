using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : Singleton<PanelManager>
{
    bool canswitch = true;

    [Header("Panels")]
    public Tasks_Panel tasksPanel;
    public FilesPanel filesPanel;

    [Header("Buttons representing respective panels")]
    public UserPanelButton arButton;
    public UserPanelButton TasksButton;
    public UserPanelButton FilesButton;
    public UserPanelButton MapButton;

    protected override void Start()
    {
        base.Start();

        tasksPanel = TasksButton.linkedPanel as Tasks_Panel;
        filesPanel = FilesButton.linkedPanel as FilesPanel;
    }

    UserPanelButton previousActiveButton;
    UserPanelButton activeButton;
    void ChangePanel(UserPanelButton previousButton, UserPanelButton currentButton)
    {
        previousActiveButton = previousButton;  activeButton = currentButton;
        canswitch = false;

        if (previousActiveButton != null && previousActiveButton.linkedPanel)
        {
            if (previousActiveButton.linkedPanel)
            {
                previousActiveButton.HidePanel();
                previousActiveButton.linkedPanel.panelDeactivatedEvent += Deactivated;
            }
            else
            {
                canswitch = true;
            }
        }
        else
        {
            if (activeButton.linkedPanel)
            {
                activeButton.ShowPanel();
                activeButton.linkedPanel.panelActivatedEvent += ActiveButtonShown;
            }
            else
            {
                canswitch = true;
            }
        }

    }
    void Deactivated(Panel_Base panel)
    {
        if (activeButton.linkedPanel)
        {
            if (activeButton.linkedPanel)
            {
                activeButton.ShowPanel();
                activeButton.linkedPanel.panelActivatedEvent += ActiveButtonShown;
            }
            else
            {
                canswitch = true;
            }
        }
        else
        {
            canswitch = true;
        }

        //Remove from subscription
        if (previousActiveButton.linkedPanel)
            previousActiveButton.linkedPanel.panelDeactivatedEvent -= Deactivated;

    }
    void ActiveButtonShown(Panel_Base panel)
    {
        //Remove from subscription
        if (activeButton.linkedPanel)
            activeButton.linkedPanel.panelActivatedEvent -= ActiveButtonShown;
        //can only switch after the active button is shown
        canswitch = true;
    }
}
