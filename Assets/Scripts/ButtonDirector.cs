using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonDirector : Singleton<ButtonDirector>
{
    [Header("Start Button")]
    public DOW_Button startButton;

    [Header("UI Buttons")]
    public DOW_Button ARButton;
    public DOW_Button TasksButton;
    public DOW_Button FilesButton;
    public DOW_Button MapButton;
    
}
