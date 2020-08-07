using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceBase : MonoBehaviour
{
    [HideInInspector]
    public int sequenceIndex;

    protected virtual void Start()
    {
        //Subscribe to the cleanup event
        SequenceManager.Instance.StepCleanEvent += CleanUpStep;
    }
    public virtual void Execute()
    {
        //Executes the sequence
    }
    protected virtual bool CleanUpStep(int currentIndex, int goToIndex)
    {
        if(currentIndex > goToIndex)
        {
            if (sequenceIndex <= currentIndex && sequenceIndex >= goToIndex)
                return true;

            return false;
        }
        else
        {
            if (sequenceIndex >= currentIndex && sequenceIndex <= goToIndex)
                return true;

            return false;
        }
    }
    
}
