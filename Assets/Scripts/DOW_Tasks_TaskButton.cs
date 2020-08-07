using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DOW_Tasks_TaskButton : DOW_Button
{
    public int taskIndex = -1;
    public bool taskCompleted = false;
    int sequenceIndex = 0;

    public void SetButtonSequenceIndex(int index)
    {
        sequenceIndex = index;
    }
    public int GetSequenceIndex() { return sequenceIndex; }
    protected override void Start()
    {
        base.Start();
        ButtonPressEvent += ChangeCurrentTask;
    }
    void ChangeCurrentTask(DOW_Button button)
    {
        PanelManager.Instance.tasksPanel.ChangeActiveTask(this, sequenceIndex);
    }


}
