using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DOW_Tasks_ImagesButton : DOW_Button
{
    FilesPanel filesPanel;
    public Sprite imageToShow;

    protected override void Start()
    {
        base.Start();

        
        filesPanel = PanelManager.Instance.filesPanel;
        ButtonPressEvent += ButtonClicked;
    }

    void ButtonClicked(DOW_Button button)
    {
        if (imageToShow)
            filesPanel.ChangeImage(imageToShow);
    }

}
