using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Panel_Base : MonoBehaviour
{
        
    [Header("Panel with layout group containing buttons")]
    public GameObject[] itemNamesPanel;
    [Header("Initial X position to start with. Used to hide the button")]
    public float localX = 0;

    protected List<DOW_Button> DOW_Buttons = new List<DOW_Button>();
    protected DOW_Button nextButton;
    protected DOW_Button previousButton;

    
    public bool manipulatePanelRaycasts = true;
    public bool serialAnimations = false;
    public AnimationContainer[] animationContainers;

    public System.Action AnimationCompleteEvent;
    public System.Action<Panel_Base> panelActivatedEvent;
    public System.Action<Panel_Base> panelDeactivatedEvent;
    public System.Action allButtonsClickedEvent;

    int buttonsClicked = 0;
    bool isActive = false;

    protected virtual void Awake()
    {
        gameObject.SetActive(true);
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
        Debug.Log("PPE Panel");

        if(itemNamesPanel.Length > 0)
        {
            for (int k = 0; k < itemNamesPanel.Length; k++)
            {
                Debug.Log("Child count in the names panel: " + itemNamesPanel[k].transform.childCount);
                for (int i = 0; i < itemNamesPanel[k].transform.childCount; i++)
                {
                    
                    DOW_Button button = itemNamesPanel[k].transform.GetChild(i).GetComponent<DOW_Button>();
                    if (button)
                    {   
                        DOW_Buttons.Add(button);
                        //subsrcibe to the button click event
                        button.ButtonPressEvent += ButtonClicked;
                    }
                }
            }
        }


        #region Next and previous button cache
        //Check for next and previous buttons
        Transform next = gameObject.transform.Find("Next");
        if (next)
        {
            nextButton = next.GetComponent<DOW_Button>();
            if (nextButton)
            {
                nextButton.ButtonPressEvent += NextButtonClicked;
                
            }
        }
        Transform previous = gameObject.transform.Find("Previous");
        if (previous)
        {
            previousButton = previous.GetComponent<DOW_Button>();
            if (previousButton)
            {
                previousButton.ButtonPressEvent += PreviousButtonClicked;

            }
        }

        #endregion

        DeactivatePanel(false);
        DeactivateNextButton();

        //ActivatePanel(true);
        //HideButtons(itemNamesPanel);
        //ShowButtons(itemNamesPanel, true);

    }

    public virtual void ResetPanel()
    {
        if (gameObject.activeSelf)
            DeactivatePanel(true);
        foreach (DOW_Button b in DOW_Buttons)
            b.ResetButton();

        DeactivateNextButton();
        //Previous button need not be deactivated
        //DeactivatePreviousButton();
    }

    /// <summary>
    /// On clicking the "next" button, the panel is disabled by default. Override to extend the behavior
    /// </summary>
    protected virtual void NextButtonClicked(DOW_Button button)
    {
        if (button.GetLinkedPanel())
            panelDeactivatedEvent += button.ActivateLinkedPanel;

        button.DisableButton();
        DeactivatePanel(true);
        //Not disabling previous button causes issues
        if(previousButton)
            previousButton.DisableButton();
        
    }
    protected virtual void PreviousButtonClicked(DOW_Button button)
    {
        if (button.GetLinkedPanel())
            panelDeactivatedEvent += button.ActivateLinkedPanel;

        button.DisableButton();
        DeactivatePanel(true);
        //Not disabling next button causes issues
        if(nextButton)
            nextButton.DisableButton();

    }


    protected virtual void ButtonClicked(DOW_Button button)
    {
        buttonsClicked++;

        if (buttonsClicked == DOW_Buttons.Count)
            allButtonsClickedEvent?.Invoke();

        //If all the buttons are clicked, activate the next button
        if (buttonsClicked >= DOW_Buttons.Count)
        {
            //Reset the buttons clicked
            buttonsClicked = 0;
            ActivateNextButton();
            
        }
    }


    void StopCoroutinesOnButtonsAndPanel()
    {
        StopAllCoroutines();
        foreach (DOW_Button b in DOW_Buttons)
            b.StopAllCoroutines();
    }
    
    public virtual void ActivatePanel(bool animate = true)
    {
        Debug.Log("PANEL ACTIVATE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        this.gameObject.SetActive(true);
        //Enable the raycaster also
        if(manipulatePanelRaycasts)
            GetComponent<Image>().raycastTarget = true;


        if (animate)
        {
            //Create and assign proper Animation Containers
            foreach (AnimationContainer container in animationContainers)
            {
                System.Action<GameObject, AnimationBeingTargetField, float> func = null;
                switch (container.animType)
                {
                    case AnimationEffect.FadeIn:
                        func = ObjectFadeAnimation;

                        if(!serialAnimations)
                            container.animationBeing = PanelAnimator.Instance.CreateParallelAnimationBeing(container.gameObject, 
                                func, new Color(1, 1, 1, 1), 0.5f, 1);
                        else
                            container.animationBeing = PanelAnimator.Instance.CreateSequenceAnimationBeing(container.gameObject,
                                func, new Color(1, 1, 1, 1), 1.0f, 1);

                        break;
                    case AnimationEffect.Position:
                        break;
                    case AnimationEffect.ScaleUp:
                        if(!serialAnimations)
                            container.animationBeing = PanelAnimator.Instance.CreateScaleAnim(container.gameObject, Vector3.zero, Vector3.one, AnimationType.Parallel, 0.5f, 1);
                        else
                            container.animationBeing = PanelAnimator.Instance.CreateScaleAnim(container.gameObject, Vector3.zero, Vector3.one, AnimationType.Serial, 0.5f, 1);
                        //func = PanelScaleAnimation;
                        //
                        //container.animationBeing = PanelAnimator.Instance.CreateParallelAnimationBeing(container.gameObject,
                        //    func, new AnimationVectorStuct(Vector3.zero, Vector3.one), 0.5f, 1);
                        break;
                }
                container.animationBeing.Wait();

                //Set the default values
                container.animationBeing.SetDefaults(container);
            }
        }
        
        StopCoroutinesOnButtonsAndPanel();
        
        Debug.Log("Game object is set to true!!!!!!!!!!!!!!!!!!!!");

        if (animate)
            StartCoroutine(StartActivationAnimations(animate));
        else
            ShowButtons(null, false);

        //ShowButtons(null, animate);
    }

    public virtual void DeactivatePanel(bool animate = true)
    {
        if (!gameObject.activeSelf)
            return;

        StopCoroutinesOnButtonsAndPanel();

        if (animate)
        {
            //Create and assign proper Animation Containers
            foreach (AnimationContainer container in animationContainers)
            {
                System.Action<GameObject, AnimationBeingTargetField, float> func = null;
                switch (container.animType)
                {
                    case AnimationEffect.FadeIn:
                        func = ObjectFadeAnimation;

                        container.animationBeing = PanelAnimator.Instance.CreateParallelAnimationBeing(container.gameObject,
                            func, new Color(1, 1, 1, 1), 0.5f, -1);
                        break;
                    case AnimationEffect.Position:
                        break;
                    case AnimationEffect.ScaleUp:
                        container.animationBeing = PanelAnimator.Instance.CreateScaleAnim(container.gameObject, Vector3.zero, Vector3.one, AnimationType.Parallel, 0.5f, -1);
                        
                        Debug.Log("Created for: " + container.gameObject.name);
                        //func = PanelScaleAnimation;
                        //
                        //container.animationBeing = PanelAnimator.Instance.CreateParallelAnimationBeing(container.gameObject,
                        //    func, new AnimationVectorStuct(Vector3.zero, Vector3.one), 0.5f, -1);
                        break;
                }
                container.animationBeing.Wait();
                //b.SetDefaults(container);
            }
        }


        if (animate)
            StartCoroutine(StartDeactivationAnimations(animate));
        else
        {
            HideButtons(null, false);
            gameObject.SetActive(false);

            if(manipulatePanelRaycasts)
                GetComponent<Image>().raycastTarget = false;
            //If a panel has a previous button, it doesnt have to be deactivated.
            //Only the next button should be deactivated
            //if(nextButton)
            //    nextButton.gameObject.SetActive(false);
            //if (previousButton)
            //    previousButton.gameObject.SetActive(false);
        }
    }
    public DOW_Button GetNextButton() { return nextButton; }
    public DOW_Button GetPreviousButton() { return previousButton; }

    int prevIndex = -1;
    protected void PanelScaleAnimation(GameObject obj, AnimationBeingTargetField target, float alpha)
    {
        Vector3 res = target.GetAnimationVectorStruct().initial * (1 - alpha) + target.GetAnimationVectorStruct().target * alpha;
        //Debug.Log(res);
        obj.transform.localScale = res;
    }
    protected void ObjectFadeAnimation(GameObject obj, AnimationBeingTargetField target, float alpha)
    {
        //Debug.Log("Alpha: " + alpha);
        float a = target.GetColor().a * alpha;
        Color c = obj.GetComponent<Image>().color;
        Image image = obj.GetComponent<Image>();
        obj.GetComponent<Image>().color = new Color(c.r, c.g, c.b, a);
    }

    
    //Make these functions better
    virtual protected IEnumerator StartActivationAnimations(bool animate = true)
    {
        AnimationBeing b = null;
        foreach(AnimationContainer container in animationContainers)
        {
            container.animationBeing.Play();
            
            b = container.animationBeing;
            yield return new WaitForSeconds(0.1f);
        }
        
        if (b != null)
            while (!b.animationFinished)
                yield return null;


        ShowButtons(null, animate);

        //Wait for the button animation to play out
        yield return new WaitForSeconds(0.75f);
        //while (PanelAnimator.Instance.Contains(ShowButtonsAnimated))
        //    yield return null;


        isActive = true;
        panelActivatedEvent?.Invoke(this);


        //If the next and previous buttons are already active, enable them
        if (nextButton)
            if (nextButton.gameObject.activeSelf)
                ActivateNextButton();
        if (previousButton)
            if (previousButton.gameObject.activeSelf)
                ActivatePreviousButton();

        yield return null;
    }
    virtual protected IEnumerator StartDeactivationAnimations(bool animate = true)
    {
        //Hide the buttons first
        HideButtons(null, animate);

        //Wait for a while for button animations to play out
        yield return new WaitForSeconds(0.75f);

        //while (PanelAnimator.Instance.Contains(HideButtonsAnimated))
        //    yield return null;

        AnimationBeing b = null;
        foreach (AnimationContainer container in animationContainers)
        {
            Debug.Log("Deactivation container playing : " + gameObject.name);
            container.animationBeing.Play();
            b = container.animationBeing;
            yield return new WaitForSeconds(0.1f);
        }
        //if (b)
        //    Debug.Log("ANimation being:  " + b.name);
        //else
        //    Debug.Log("No B");
        //while (!b.animationFinished)
        if(b != null)
            while (!b.animationFinished)
                yield return null;

        //yield return new WaitForSeconds(1);

        gameObject.SetActive(false);
        if(manipulatePanelRaycasts)
            GetComponent<Image>().raycastTarget = false;


        isActive = false;
        panelDeactivatedEvent?.Invoke(this);


        //TODO: Need to add an animation to this
        //if(nextButton)
        //    nextButton.gameObject.SetActive(false);
        //if (previousButton)
        //    previousButton.gameObject.SetActive(false);
        yield return null;

    }

    public void LinkPanelToButton(DOW_Button button, Panel_Base panel)
    {
        if (button)
        {
            if (!button.GetLinkedPanel())
            {
                button.LinkPanel(panel, this);
                //panelDeactivatedEvent += button.ActivateLinkedPanel;
            }
            else
            {
                Debug.LogWarning("Panel is already linked to this button. Ignoring!");
            }
        }
        else
        {
            Debug.LogError("No button lo link");
        }
    }
    public void UnlinkPanelOnButton(DOW_Button button)
    {
        if (button)
        {
            if (button.GetLinkedPanel())
            {
                button.UnlinkPanel();
                panelDeactivatedEvent -= button.ActivateLinkedPanel;
            }
        }
    }


    #region Buttons Animations in a panel


    public void ActivateNextButton()
    {
        if (nextButton)
        {
            nextButton.gameObject.SetActive(true);
            nextButton.EnableButton();
        }
    }
    public void DeactivateNextButton()
    {
        if (nextButton)
        {
            nextButton.gameObject.SetActive(false);
            nextButton.DisableButton();
        }
    }
    public void ActivatePreviousButton()
    {
        if (previousButton)
        {
            previousButton.gameObject.SetActive(true);
            previousButton.EnableButton();
        }
    }
    public void DeactivatePreviousButton()
    {
        if (previousButton)
        {
            previousButton.gameObject.SetActive(false);
            previousButton.DisableButton();
        }
    }


    public void HideButtons(GameObject baseLayoutPanel = null, bool animate = false)
    {
        //Enabling and disabling layout elements for seamless animation
        for(int i = 0; i < itemNamesPanel.Length; i++)
        {
            if (itemNamesPanel[i].GetComponent<VerticalLayoutGroup>())
                itemNamesPanel[i].GetComponent<VerticalLayoutGroup>().enabled = false;

            if (itemNamesPanel[i].transform.parent.GetComponent<ScrollRect>())
                itemNamesPanel[i].transform.parent.GetComponent<ScrollRect>().enabled = false;
        }
        

        if (!animate)
        {
            foreach(DOW_Button button in DOW_Buttons)
            {
                Vector3 initialPos = button.GetInitialPosition();
                Debug.Log("initial position : " + initialPos);
                button.GetRectTransform().localPosition = new Vector3(localX, initialPos.y, initialPos.z);

                //Disable the button Interaction
                button.DisableButton();
            }
        }
        else
        {
            PanelAnimator.Instance.PlayAnimation(HideButtonsAnimated);
        }
    }

    public void ShowButtons(GameObject baseLayoutPanel = null, bool animate = false)
    {
        if (!animate)
        {
            foreach (DOW_Button button in DOW_Buttons)
            {
                button.GetRectTransform().localPosition = button.GetInitialPosition();

                //Enable interaction on button
                button.EnableButton();
            }

            for (int i = 0; i < itemNamesPanel.Length; i++)
            {
                //Enabling and disabling layout elements for seamless animation
                if(itemNamesPanel[i].GetComponent<VerticalLayoutGroup>())
                    itemNamesPanel[i].GetComponent<VerticalLayoutGroup>().enabled = true;


                if (itemNamesPanel[i].transform.parent.GetComponent<ScrollRect>())
                    itemNamesPanel[i].transform.parent.GetComponent<ScrollRect>().enabled = true;
            }

        }
        else
        {
            PanelAnimator.Instance.PlayAnimation(ShowButtonsAnimated);
             
        }
    }

    IEnumerator HideButtonsAnimated()
    {
        //for(int i = 0; i < Mathf.Min(6, DOW_Buttons.Count); i++)
        for(int i = 0; i < DOW_Buttons.Count; i++)
        {
            DOW_Button button = DOW_Buttons[i];

            Vector3 initialPos = button.GetInitialPosition();
            button.AnimatePosition(new Vector3(localX, initialPos.y, initialPos.z));

            
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.5f);

        //Disable button interaction
        foreach (DOW_Button button in DOW_Buttons)
        {
            button.DisableButton();
        }

        //Remove animation from the panel animator list
        //if (PanelAnimator.Instance.animationFunctionsStack.Contains(HideButtonsAnimated))
        //{
        //    PanelAnimator.Instance.PopAnimation(HideButtonsAnimated);
        //}


        yield return null;
    }

    IEnumerator ShowButtonsAnimated()
    {
        //for (int i = 0; i < Mathf.Min(6, DOW_Buttons.Count); i++)
        for (int i = 0; i < DOW_Buttons.Count; i++)
        {
            DOW_Button button = DOW_Buttons[i];

            button.AnimatePosition(button.GetInitialPosition());

            //Debug.Log("Initial POsition: " + button.GetInitialPosition());
            yield return new WaitForSeconds(0.1f);
        }

        //Enable button interaction
        foreach (DOW_Button button in DOW_Buttons)
        {
            button.EnableButton();
        }

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < itemNamesPanel.Length; i++)
        {
            if (itemNamesPanel[i].GetComponent<VerticalLayoutGroup>())
                itemNamesPanel[i].GetComponent<VerticalLayoutGroup>().enabled = true;

            if (itemNamesPanel[i].transform.parent.GetComponent<ScrollRect>())
                itemNamesPanel[i].transform.parent.GetComponent<ScrollRect>().enabled = true;
        }


        //Remove animation from the panel animator list
        //if (PanelAnimator.Instance.animationFunctionsStack.Contains(ShowButtonsAnimated))
        //{
        //    PanelAnimator.Instance.PopAnimation(ShowButtonsAnimated);
        //}
        
        yield return null;
    }

    #endregion

    public bool IsActive()
    {
        return isActive;
    }
}
