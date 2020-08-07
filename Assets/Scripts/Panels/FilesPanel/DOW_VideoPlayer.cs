using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DOW_VideoPlayer : MonoBehaviour, IPointerClickHandler
{
    VideoManager videoManager;

    public void OnPointerClick(PointerEventData eventData)
    {
        videoManager.ToggleClip();
    }

    // Start is called before the first frame update
    void Start()
    {
        videoManager = VideoManager.Instance;
    }
    
}
