using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoManager : Singleton<VideoManager>
{
    VideoPlayer videoPlayer;
    RawImage imageToRenderTextureOn;
    VideoClip clipToPlay;

    protected override void Start()
    {
        base.Start();
        videoPlayer = GetComponent<VideoPlayer>();
        //videoPlayer.clip = clip;
    }

    private void Update()
    {
        if (imageToRenderTextureOn && videoPlayer.isPrepared)
        {
            imageToRenderTextureOn.texture = videoPlayer.texture;
        }
    }

    public void PlayVideo(VideoClip clip, RawImage image)
    {
        clipToPlay = clip;
        SetRawImage(image);

        videoPlayer.Stop();
        videoPlayer.clip = clipToPlay;

        //if (videoPlayer.isPlaying)

        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += PlayClip;
    }
    void PlayClip(VideoPlayer source)
    {
        //Play the new video once the preparation is complete
        videoPlayer.prepareCompleted -= PlayClip;
        
        
        videoPlayer.Play();
    }
    public void ToggleClip()
    {
        if (videoPlayer.isPaused)
            videoPlayer.Play();
        else
            videoPlayer.Pause();
    }
    void PauseClip()
    {
        if(videoPlayer)
            videoPlayer.Pause();
    }
    void ResumeClip()
    {
        if (videoPlayer)
            videoPlayer.Play();
    }


    public void StopVideo()
    {
        videoPlayer.Stop();
        RemoveRawImage();
    }

    void SetRawImage(RawImage image)
    {
        imageToRenderTextureOn = image;
    }
    void RemoveRawImage()
    {
        imageToRenderTextureOn = null;
    }
}
