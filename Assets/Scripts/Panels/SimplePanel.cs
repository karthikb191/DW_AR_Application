using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePanel : Panel_Base
{
    // Start is called before the first frame update

    protected override void Start()
    {
        base.Start();

        
    }

    public override void ActivatePanel(bool animate = true)
    {
        base.ActivatePanel(animate);
        if (nextButton)
        {
            nextButton.gameObject.SetActive(true);
            nextButton.EnableButton();
        }
        if (previousButton)
        {
            previousButton.gameObject.SetActive(true);
            previousButton.EnableButton();
        }
    }

}
