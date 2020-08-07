using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;


public class DOW_Tasks_VideosButton : DOW_Button
{
    FilesPanel filesPanel;
    public VideoClip clip;
    protected override void Start()
    {
        base.Start();

        filesPanel = PanelManager.Instance.filesPanel;
        ButtonPressEvent += ButtonClicked;
    }

    void ButtonClicked(DOW_Button button)
    {
        //Change the video clip playing to the current one
        filesPanel.ChangeVideo(clip);
    }

}
