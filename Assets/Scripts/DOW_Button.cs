using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

[RequireComponent(typeof(Button))]
public class DOW_Button : MonoBehaviour
{
    //Public fields
    [Header("Optional description. Shows when there's a description panel linked")]
    //public string title;
    [TextArea(1, 30)]
    public string description;

    [HideInInspector]
    public Action<DOW_Button> ButtonPressEvent;

    //Private fields
    Panel_Base panelLinked = null;
    Panel_Base panelCalledFrom = null;

    Button dow_Button;
    TextMeshProUGUI textComponent;
    RectTransform rectTransform;
    Vector3 initialLocalPosition;
    Vector2 initialSizeDelta;
    Color initialColor;
    Sprite initialBlockSprite;

    bool isEnabled = false;

    //Coroutines
    Coroutine scaleCoroutine;
    Coroutine positionCoroutine;

    public void SetText(string text)
    {
        textComponent.text = text;
    }

    protected virtual void Start()
    {
        dow_Button = GetComponent<Button>();
        //On Start, Subscribe the Button Pressed to click event
        dow_Button.onClick.AddListener(ButtonPressed);

        rectTransform = GetComponent<RectTransform>();
        initialLocalPosition = rectTransform.anchoredPosition;
        initialSizeDelta = rectTransform.sizeDelta;
        initialColor = GetComponent<Image>().color;

        if(transform.Find("Image"))
            if(transform.Find("Image").childCount > 0)
                initialBlockSprite = transform.Find("Image").GetChild(0).GetComponent<Image>().sprite;

        Debug.Log("Initial position:  " + rectTransform.anchoredPosition);
    }
    
    /// <summary>
    /// Calls a function which triggers the button pressed event
    /// </summary>
    public void ForceButtonPress()
    {
        ButtonPressed();
    }

    /// <summary>
    /// Called when a DOW button is clicked. Subscription to this is on start
    /// </summary>
    void ButtonPressed()
    {
        Debug.Log("Button Pressed on: " + gameObject.name + "  Parent: " +  gameObject.transform.parent.name);

        if(ButtonPressEvent != null)
            ButtonPressEvent(this);

        //Toggle the enabled State
        isEnabled = !isEnabled;
        
    }

    /// <summary>
    /// This function resets the button to it's initial state. Called when steps are jumped
    /// </summary>
    public virtual void ResetButton()
    {
        GetComponent<Image>().color = initialColor;

        if (transform.Find("Image"))
        {
            transform.Find("Image").GetChild(0).GetComponent<Image>().sprite = initialBlockSprite;
        }
        //TODO: Reset the tick marks
    }
    
    public void SetColor(Color c)
    {
        GetComponent<Image>().color = c;
    }
    public Color GetInitialColor() { return initialColor; }
    

    /// <summary>
    /// Animates the button position
    /// </summary>
    /// <param name="targetPosition">Specify the target position of the animation</param>
    public void AnimatePosition(Vector3 targetPosition)
    {
        if (positionCoroutine != null)
            StopCoroutine(positionCoroutine);

        PanelAnimator.Instance.CreateRectTransformLocalPositionAnimation(rectTransform, rectTransform.localPosition, targetPosition,
                                                            AnimationType.Parallel, 0.55f, 1);
        //positionCoroutine = StartCoroutine(AnimatePositionRoutine(targetPosition));
    }

    IEnumerator AnimatePositionRoutine(Vector3 targetPosition)
    {
        while (Vector3.SqrMagnitude(rectTransform.localPosition - targetPosition) > 0.5f )
        {
            rectTransform.localPosition = Vector3.Lerp(rectTransform.localPosition, targetPosition, 15 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        positionCoroutine = null;
    }


    /// <summary>
    /// Animates the button scale
    /// </summary>
    /// <param name="newSize">Target size of button</param>
    public void AnimateSizeDelta(Vector2? newSize = null)
    {
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);


        if (newSize == null)
            scaleCoroutine = StartCoroutine(AnimateSizeDeltaRoutine(initialSizeDelta));
        else
            scaleCoroutine = StartCoroutine(AnimateSizeDeltaRoutine(newSize.Value));

    }

    IEnumerator AnimateSizeDeltaRoutine(Vector2 newSize)
    {
        while(Vector2.SqrMagnitude(rectTransform.sizeDelta - newSize) > 0.5f)
        {
            rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, newSize, 10.0f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        rectTransform.sizeDelta = newSize;
        scaleCoroutine = null;
    }



    public bool IsEnabled(){ return isEnabled; }
    public RectTransform GetRectTransform() { return rectTransform; }
    public Vector3 GetInitialPosition() { return initialLocalPosition; }

    /// <summary>
    /// Disables the button, but doesn't hide it
    /// </summary>
    public void DisableButton()
    {
        Debug.Log("Disabled");
        isEnabled = false;
        //GetComponent<Image>().color = initialColor * 0.5f;
        dow_Button.interactable = false;
    }
    /// <summary>
    /// Enables the button. If not active, doesn't make it active
    /// </summary>
    public void EnableButton()
    {
        isEnabled = true;
        //GetComponent<Image>().color = initialColor;
        dow_Button.interactable = true;
    }



    public Panel_Base GetLinkedPanel() { return panelLinked; }
    /// <summary>
    /// Link the panel to the button that automatically gets called when the current panel.
    /// Panel activation and deactivation logic is inside the Panel_Base script
    /// </summary>
    /// <param name="panelToLink"></param>
    /// <param name="panelCalledFrom"></param>
    public void LinkPanel(Panel_Base panelToLink, Panel_Base panelCalledFrom)
    {
        panelLinked = panelToLink;
        this.panelCalledFrom = panelCalledFrom;
    }
    public void UnlinkPanel() { panelLinked = null; }
    /// <summary>
    /// Activates the linked panel
    /// </summary>
    /// <param name="panel"></param>
    public void ActivateLinkedPanel(Panel_Base panel)
    {
        //Always unsubscribe
        panelCalledFrom.panelDeactivatedEvent -= ActivateLinkedPanel;

        panelLinked.ActivatePanel(true);
    }
    /// <summary>
    /// Deactivates the linked panel
    /// </summary>
    public void DeactivateLinkedPanel()
    {
        panelLinked.DeactivatePanel(true);
    }

}
