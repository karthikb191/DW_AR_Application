using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideosPanel : Panel_Base
{
    [SerializeField]
    private RawImage videoPreview;


    protected override void Start()
    {
        base.Start();

        //videoPreview = GetComponentInChildren<RawImage>();
    }
    public override void DeactivatePanel(bool animate = true)
    {
        base.DeactivatePanel(animate);

        PanelManager.Instance.filesPanel.StopVideo();
    }
    public RawImage GetVideoRawTexture()
    {
        return videoPreview;
    }
}
