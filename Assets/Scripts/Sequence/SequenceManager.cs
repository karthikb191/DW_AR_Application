using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SequenceManager : Singleton<SequenceManager>
{
    public int startIndex = 0;
    int currentIndex = 0;
    SequenceBase[] sequences;
    Action sequenceFinisedEvent;
    public Func<int, int, bool> StepCleanEvent;

    protected override void Start()
    {
        base.Start();

        currentIndex = startIndex;

        if (transform.childCount > 0)
        {
            sequences = new SequenceBase[transform.childCount];

            for (int i = 0; i < transform.childCount; i++)
            {
                SequenceBase sequence = transform.GetChild(i).GetComponent<SequenceBase>();
                sequence.sequenceIndex = i;

                if (sequence)
                    sequences[i] = sequence;
            }
        }
        else
            Debug.Log("Place the sequences as children of the sequence manager object");


        //After a certain delay, start the sequence. Remove the delay at the release
        Invoke("TempStepActivation", 0.5f);

        sequenceFinisedEvent += FinishSequence;

    }

    public void TempStepActivation()
    {
        //Start the sequence
        StartSequence();

        //Enable the vuforia engine initially
        VuforiaManager.Instance.EnableVuforia();

    }

    public void StartSequence()
    {
        //Start the first sequence
        if (sequences.Length > 0)
            sequences[currentIndex].Execute();
    }
    public void GoToNextSequence()
    {
        Debug.Log("Current Index : " + currentIndex);
        //Notify panel manager that a task is complete
        PanelManager.Instance.tasksPanel.TaskComplete(currentIndex);

        sequenceFinisedEvent();
    }

    public void GoToStep(int index)
    {
        if (index < 0 || index >= sequences.Length)
            return;

        int current = currentIndex;
        currentIndex = index;

        //Cleanup the current step and execute the specified step
        StartCoroutine(CleanUp(current, index));
    }
    IEnumerator CleanUp(int current, int goToIndex)
    {
        StepCleanEvent(current, goToIndex);
        yield return new WaitForSeconds(2);

        //Execute from first sequence
        sequences[currentIndex].Execute();
    }

    private void FinishSequence()
    {
        currentIndex += 1;
        Debug.Log("HAZARDS! NEXT! BUTTON!!!!");
        if (currentIndex < sequences.Length)
            sequences[currentIndex].Execute();

        //TODO: Add the else condition that checks if there's no more steps
    }

}
