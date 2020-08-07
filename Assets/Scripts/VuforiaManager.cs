using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class VuforiaManager : Singleton<VuforiaManager>
{
    private int currentTargetScanning;
    public int CurrentTargetScanning { get { return currentTargetScanning; } }

    public DOW_TrackableEventHandler[] trackersInSequence;
    public SimplePanel wrongTrackerPanel;
    int currentTrackerIndex = -1;


    bool vuforiaEnabled = true;
    bool currentTrackerFound = false;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Debug.Log("Vuforia manager is working! " + Instance.name);

        //TODO: Find a better implementation for this
        Invoke("DisableAllTrackers", 0.15f);
        //DisableAllTrackers();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            VuforiaBehaviour.Instance.enabled = !vuforiaEnabled;
            vuforiaEnabled = !vuforiaEnabled;
        }
    }

    #region Tracker Functions
    public void DisableAllTrackers()
    {
        foreach(DOW_TrackableEventHandler tracker in trackersInSequence)
        {
            tracker.DisableTracking();
        }
    }

    public void EnableTrackerIndex(int index)
    {
        //Disable the current tracker
        if (currentTrackerIndex != -1)
            DisableCurrentTracker();

        currentTrackerFound = false;
        currentTrackerIndex = index;
        trackersInSequence[index].EnableTracking();
    }
    public void DisableTrackerIndex(int index)
    {
        trackersInSequence[index].DisableTracking();
    }
    public void EnableNextTracker(bool enableObject = true)
    {
        
        if (currentTrackerIndex != -1)
        {
            //Disable the current tracker
            DisableCurrentTracker();

            //Increment index and enable the next tracker if it is present
            if (currentTrackerIndex < trackersInSequence.Length - 1)
            {
                currentTrackerFound = false;
                currentTrackerIndex++;
                trackersInSequence[currentTrackerIndex].EnableTracking(enableObject);
            }
        }
        else
        {
            if (currentTrackerIndex < trackersInSequence.Length - 1)
            {
                currentTrackerFound = false;
                currentTrackerIndex++;
                trackersInSequence[currentTrackerIndex].EnableTracking(enableObject);
            }
        }
    }
    public void DisableCurrentTracker()
    {
        if (currentTrackerIndex == -1)
            return;
        trackersInSequence[currentTrackerIndex].DisableTracking();
    }
    public DOW_TrackableEventHandler GetCurrentTrackerEventHandler()
    {
        if (currentTrackerIndex == -1)
            return null;
        return trackersInSequence[currentTrackerIndex];
    }
    #endregion


    public void EnableVuforia()
    {
        //Debug.Log("Vuforia enabled!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        VuforiaBehaviour.Instance.enabled = true;
        vuforiaEnabled = true;
    }

    public void DisableVuforia()
    {
        VuforiaBehaviour.Instance.enabled = false;
        vuforiaEnabled = false;
    }


    public void CheckTracker(DOW_TrackableEventHandler tracker)
    {
        if (currentTrackerFound || currentTrackerIndex == -1)
        {
            Debug.Log("Tracker Found but not returning without doing anything");
            return;
        }

        Debug.Log("Check: Current: " + GetCurrentTrackerEventHandler().gameObject.name + " tracked: " + tracker.gameObject.name);
        //If the tracker is not yet found, put forth a proper message
        if(tracker != GetCurrentTrackerEventHandler())
        {
            if (!wrongTrackerPanel.gameObject.activeSelf)
            {
                StopAllCoroutines();
                StartCoroutine(WrongTrackerPanelRoutine(10));
                
            }
        }
        else
        {
            StopAllCoroutines();
            if(wrongTrackerPanel.gameObject.activeSelf)
                wrongTrackerPanel.DeactivatePanel();
            currentTrackerFound = true;
        }
    }
    IEnumerator WrongTrackerPanelRoutine(int duration)
    {
        wrongTrackerPanel.ActivatePanel();
        yield return new WaitForSeconds(duration);
        wrongTrackerPanel.DeactivatePanel();
    }
}
