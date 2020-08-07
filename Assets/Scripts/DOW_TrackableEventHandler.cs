using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using Vuforia.UnityRuntimeCompiled;

public class DOW_TrackableEventHandler : DefaultTrackableEventHandler
{
    
    System.Action<TrackableBehaviour.StatusChangeResult> changeres;

    System.Action trackerTrackedEvent;
    System.Action trackerLostEvent;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        changeres = OnTrackableStateChanged;

        //tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        GetComponent<TrackableBehaviour>().RegisterOnTrackableStatusChanged(changeres);

        //gameObject.SetActive(false);
        //StartCoroutine(Disable());

        base.OnTrackingLost();
        
    }

    public void DisableTracking(bool showObject = true)
    {
        //GetComponent<ImageTargetBehaviour>().enabled = false;

        if (transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
            child.gameObject.SetActive(false);
        }
    }
    public void EnableTracking(bool showObject = true)
    {
        GetComponent<ImageTargetBehaviour>().enabled = true;

        //If the object should be shown when the tracker is tracked, disable the object
        //TODO: Might need to change the activation logic to enabling and disabling mesh renderer
        if (!showObject)
        {
            if (transform.childCount > 0)
            {
                Transform child = transform.GetChild(0);
                child.gameObject.SetActive(false);
            }
        }
        else
        {
            if (transform.childCount > 0)
            {
                Transform child = transform.GetChild(0);
                child.gameObject.SetActive(true);
            }
        }
    }
    public void ShowObject()
    {
        Transform child = transform.GetChild(0);
        if (child)
        {
            child.gameObject.SetActive(true);
        }
    }
    public void HideObject()
    {
        Transform child = transform.GetChild(0);
        if (child)
        {
            child.gameObject.SetActive(false);
        }
    }

    
    protected override void OnTrackingFound()
    {
        Debug.Log("Tracking Found");
        //Check if the tracker being tracked is the correct one
        VuforiaManager.Instance.CheckTracker(this);
        //return;
        base.OnTrackingFound();
        //return;
    }
    protected override void OnTrackingLost()
    {
        Debug.Log("Tracking Lost");
        base.OnTrackingLost();
        //return;
    }

    IEnumerator Disable()
    {
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
    }
    
    public void RegisterToTrackerTrackedEvent(System.Action func)
    {
        trackerTrackedEvent += func;
    }
    public void RegisterToTrackerLostEvent(System.Action func)
    {
        trackerLostEvent += func;
    }
    public void UnregisterToTrackerTrackedEvent(System.Action func)
    {
        trackerTrackedEvent -= func;
    }
    public void UnregisterToTrackerLostEvent(System.Action func)
    {
        trackerLostEvent -= func;
    }


    protected void OnTrackableStateChanged(TrackableBehaviour.StatusChangeResult res)
    {
        Debug.Log("Trackable state:  " + res.NewStatus);
        switch (res.NewStatus)
        {
            case TrackableBehaviour.Status.DETECTED:
                if(trackerTrackedEvent != null)
                    trackerTrackedEvent();
                break;
            case TrackableBehaviour.Status.EXTENDED_TRACKED:
                if (trackerTrackedEvent != null)
                    trackerTrackedEvent();
                break;
            case TrackableBehaviour.Status.TRACKED:
                if (trackerTrackedEvent != null)
                    trackerTrackedEvent();
                break;
            case TrackableBehaviour.Status.NO_POSE:
                if (trackerLostEvent != null)
                    trackerLostEvent();
                break;
        }
    }
}
