using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class FilesPanel : UserPanel
{
    [Header("Files Panel Stuff")]
    //TODO: Change this
    
    [SerializeField]
    private ImagesPanel imagesPanel;
    [SerializeField]
    private VideosPanel videosPanel;
    
    protected override void Start()
    {
        base.Start();

        //imagesPanel = GetComponentInChildren<ImagesPanel>();
        //videosPanel = GetComponentInChildren<VideosPanel>();
    }

    public void ChangeImage(Sprite image)
    {
        if (imagesPanel)
        {
            imagesPanel.GetImageComponent().sprite = image;
            //Debug.Log("Chane=ged");
        }
    }
    public void ChangeVideo(VideoClip clip)
    {
        //Play the specified video
        VideoManager.Instance.PlayVideo(clip, videosPanel.GetVideoRawTexture());
    }
    public void StopVideo()
    {
        VideoManager.Instance.StopVideo();
    }

}
