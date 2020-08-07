using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPanelButton : DOW_Button
{
    [Header("Panel Linked to press")]
    public Panel_Base linkedPanel;
    bool panelActivated = false;


    protected override void Start()
    {
        base.Start();
        

        if (linkedPanel)
        {
            linkedPanel.panelActivatedEvent += (Panel_Base panel) => { panelActivated = true; };
            linkedPanel.panelDeactivatedEvent += (Panel_Base panel) => { panelActivated = false; };
        }
    }

    public bool IsPanelActive()
    {
        return panelActivated;
    }
    public void ShowPanel()
    {
        if (linkedPanel)
        {
            Debug.Log("Activating the Panel@&$%#$%&^$^$$&^$^&$&$");
            linkedPanel.ActivatePanel(true);
        }
    }
    public void HidePanel()
    {
        if(linkedPanel)
            linkedPanel.DeactivatePanel(true);
    }

    Coroutine blinkRoutine;
    AnimationBeing being;
    public void Blink()
    {
        //blinkRoutine = StartCoroutine(BlinkRoutine());
        being = PanelAnimator.Instance.CreateScaleAnim(gameObject, new Vector3(0.8f, 0.8f, 0.8f), Vector3.one, AnimationType.Parallel, 0.8f, 1, true);
    }


    public void StopBlinking()
    {
        //if(blinkRoutine!=null)
        //    StopCoroutine(blinkRoutine);
        //blinkRoutine = null;
        if(being != null)
            being.Stop();
        //gameObject.transform.localScale = Vector3.one;
    }

    IEnumerator BlinkRoutine()
    {
        while (true)
        {
            AnimationBeing b = PanelAnimator.Instance.CreateParallelAnimationBeing(gameObject, ScaleAnim, 
                new AnimationVectorStuct(new Vector3(0.8f, 0.8f, 0.8f), Vector3.one), 0.5f, 1);

            while (!b.animationFinished)
                yield return null;

            b = PanelAnimator.Instance.CreateParallelAnimationBeing(gameObject, ScaleAnim,
                new AnimationVectorStuct(new Vector3(0.8f, 0.8f, 0.8f), Vector3.one), 0.5f, -1);

            while (!b.animationFinished)
                yield return null;
        }
        
    }

    void ScaleAnim(GameObject g, AnimationBeingTargetField field, float alpha)
    {
        Vector3 res = field.GetAnimationVectorStruct().initial * (1 - alpha) + field.GetAnimationVectorStruct().target * alpha;
        //Debug.Log(res);
        g.transform.localScale = res;
    }
}
