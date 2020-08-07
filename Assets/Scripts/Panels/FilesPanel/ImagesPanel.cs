using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImagesPanel : Panel_Base
{
    [SerializeField]
    private Image imagePreview;

    
    protected override void Start()
    {
        base.Start();

        //imagePreview = GetComponentInChildren<Image>();
    }
    public override void DeactivatePanel(bool animate = true)
    {
        base.DeactivatePanel(animate);

        PanelManager.Instance.filesPanel.ChangeImage(null);
    }
    public Image GetImageComponent()
    {
        return imagePreview;
    }
}
