using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tasks_Panel : Panel_Base
{
    [Header("Description Panel")]
    public GameObject descriptionPanel;
    public SimplePanel submitTasksPanel;
    public SimplePanel tasksSubmittedPanel;
    public UnityEngine.UI.Scrollbar scrollBar;

    int currentIndex = 0;
    int highestStepReached = 0;
    int indexBeforeChange = 0;
    TextMeshProUGUI titleTextComponent;
    TextMeshProUGUI descriptionTextComponent;
    bool sequenceChanged = false;
    DOW_Button goButton;

    List<DOW_Tasks_TaskButton> tasksButtons = new List<DOW_Tasks_TaskButton>();
    Dictionary<int, DOW_Tasks_TaskButton> sequenceDictionary;

    int tasksFinished = 0;
    int currentButtonIndex = 0;
    bool tasksSubmitted = false;
    bool tasksCompleted = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        sequenceDictionary = new Dictionary<int, DOW_Tasks_TaskButton>();

        if (descriptionPanel)
        {
            titleTextComponent = descriptionPanel.transform.Find("Title").GetChild(0).GetComponent<TextMeshProUGUI>();
            descriptionTextComponent = descriptionPanel.transform.Find("Description").GetChild(0).GetComponent<TextMeshProUGUI>();
        }
        else
            Debug.Log("Please feed in the description panel");


        int count = 0;
        foreach(DOW_Button button in DOW_Buttons)
        {
            button.ButtonPressEvent += ButtonPressed;

            //Converting to tasks specific button
            DOW_Tasks_TaskButton tb = button as DOW_Tasks_TaskButton;
            tasksButtons.Add(tb);
            tb.SetButtonSequenceIndex(count);

            //if(tb.taskIndex != -1)
            //{
            //}
            sequenceDictionary.Add(tb.taskIndex, tb);

            count++;
        }

        goButton = transform.Find("Go").GetComponent<DOW_Button>();
        goButton.ButtonPressEvent += GoButtonClicked;

        ActivateNextButton();
        ActivatePreviousButton();

        panelActivatedEvent += CheckTasksCompletion;

        //Initially, 0th task is the active task. Get the 0th task button index from the dictionary
        SetActiveTask(sequenceDictionary[0].GetSequenceIndex());


        scrollBar.onValueChanged.AddListener((float f) => { if(f == 0) TaskComplete(-1); });
    }


    void ButtonPressed(DOW_Button button)
    {
        if (button.description != null)
        {
            titleTextComponent.text = button.name;
            descriptionTextComponent.text = button.description;
        }

        //SetActiveTask()
    }

    public void ChangeActiveTask(DOW_Tasks_TaskButton b, int index)
    {
        if (index == -1)
            return;

        //if(index >= 0 && index <= highestStepReached)
        if(b.taskCompleted)
        {
            SetActiveTask(index);

            goButton.EnableButton();
            //if (currentIndex != indexBeforeChange)
            //    goButton.EnableButton();
            //else
            //    goButton.DisableButton();

        }
        else
        {
            if (b.GetSequenceIndex() == indexBeforeChange)
            {
                SetActiveTask(indexBeforeChange);
                goButton.DisableButton();
            }
            else if (b.GetSequenceIndex() == highestStepReached)
            {
                SetActiveTask(highestStepReached);
                goButton.EnableButton();
            }
        }
    }


    protected override void NextButtonClicked(DOW_Button button)
    {
        //base.NextButtonClicked(button);
        if(currentIndex < highestStepReached)
        {
            SetActiveTask(currentIndex + 1);

            if (currentIndex != indexBeforeChange)
                goButton.EnableButton();
            else
                goButton.DisableButton();

        }
    }
    protected override void PreviousButtonClicked(DOW_Button button)
    {
        //base.PreviousButtonClicked(button);
        if(currentIndex > 0)
        {
            SetActiveTask(currentIndex - 1);

            if (currentIndex != indexBeforeChange)
                goButton.EnableButton();
            else
                goButton.DisableButton();
            //if (!sequenceChanged)
            //{
            //    sequenceChanged = true;
            //    PanelManager.Instance.arButton.ButtonPressEvent += SequenceChanged;
            //}
        }
    }


    void GoButtonClicked(DOW_Button button)
    {
        button.DisableButton();
        if(currentIndex != indexBeforeChange)
        {
            //The sequence has been changed
            sequenceChanged = true;

            Debug.Log("Current index:  " + currentIndex);
            //Force the AR button press that closes the current panel
            PanelManager.Instance.arButton.ForceButtonPress();
        }
    }

    public override void ActivatePanel(bool animate = true)
    {
        base.ActivatePanel(animate);

        //Keeping track of whether the index is changed or not
        indexBeforeChange = currentIndex;

        //Disable the Go button initially
        goButton.DisableButton();

        
    }

    void CheckTasksCompletion(Panel_Base panel)
    {
        //submitTasksPanel.ActivatePanel();
        Debug.Log("Tasks completion :   " + tasksCompleted + " count: " + tasksFinished);

        if(tasksCompleted)
        {
            if (!tasksSubmitted)
            {
                submitTasksPanel.ActivatePanel();
                submitTasksPanel.panelDeactivatedEvent += (Panel_Base p) => { tasksSubmitted = true; };
            }
            else
                tasksSubmittedPanel.ActivatePanel();
        }
    }

    public override void DeactivatePanel(bool animate = true)
    {
        base.DeactivatePanel(animate);

        if (sequenceChanged)
        {
            sequenceChanged = false; //Reset sequence change
            
            
            if (tasksButtons[currentIndex].taskIndex != -1)
                SequenceManager.Instance.GoToStep(tasksButtons[currentIndex].taskIndex);
            else
                SetActiveTask(indexBeforeChange);

            indexBeforeChange = currentIndex;
        }
        else
        {
            //currentIndex = indexBeforeChange;
            SetActiveTask(indexBeforeChange);
        }
    }



    void SequenceChanged(DOW_Button button)
    {
        PanelManager.Instance.arButton.ButtonPressEvent -= SequenceChanged;
        
    }


    public void TaskComplete(int taskIndex)
    {
        
        DOW_Tasks_TaskButton b = sequenceDictionary[taskIndex];
        int buttonIndex = b.GetSequenceIndex();
        if (!b.taskCompleted)
        {
            b.taskCompleted = true;
            
            tasksFinished++;

            //Set the color of the right side thingy
            if (b.GetSequenceIndex() < DOW_Buttons.Count)
                DOW_Buttons[b.GetSequenceIndex()].transform.Find("Image").GetComponent<UnityEngine.UI.Image>().color = new Color(0.2f, 0.44f, 0.717f, 1);
            
        }

        //Increment the task
        if (taskIndex >= 0)// && buttonIndex < tasksButtons.Count - 1)
        {
            Debug.Log("Incerementing task from: " + buttonIndex + " to: " + (buttonIndex + 1) + "Button Count: " + tasksButtons.Count + " TASk finish count: " + tasksFinished);
            SetActiveTask(buttonIndex + 1); 
        }
    }


    void SetActiveTask(int index)
    {
        if (index >= 0)
            DOW_Buttons[currentIndex].SetColor(DOW_Buttons[currentIndex].GetInitialColor());

        if (!tasksCompleted && tasksFinished >= DOW_Buttons.Count)
        {
            tasksCompleted = true;
            return;
        }

        if(index < tasksButtons.Count)
        {
            //Set the current index value
            currentIndex = index;
            if (currentIndex > highestStepReached)
                highestStepReached = currentIndex;
            DOW_Buttons[index].SetColor(new Color(0, 216, 255));
        }
    }
    
    
}
