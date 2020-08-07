using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TimerPanel : SimplePanel
{
    public TextMeshProUGUI timerString;
    public Image timerAnimationImage;

    Coroutine timerRoutine;
    bool timerShown = false;
    protected override void Start()
    {
        base.Start();

        //Disable the next button
        if(nextButton)
            nextButton.gameObject.SetActive(false);

        if(previousButton)
            previousButton.ButtonPressEvent += PreviousButtonPressed;

        
        //StartTimer(5);
    }
    /// <summary>
    /// When previous button is pressed, reset the timer in addition to doing whatever weird shit it's doing before
    /// </summary>
    /// <param name="button"></param>
    void PreviousButtonPressed(DOW_Button button)
    {
        System.Action<Panel_Base> r = null;

        r = (Panel_Base panel) => { panelDeactivatedEvent -= r; ResetTimer(); };
        panelDeactivatedEvent += r;
    }
    public override void ActivatePanel(bool animate = true)
    {
        base.ActivatePanel(animate);
        DeactivateNextButton();
        //if (nextButton)
        //{
        //    nextButton.gameObject.SetActive(false);
        //    nextButton.EnableButton();
        //}
        if (previousButton)
        {
            previousButton.gameObject.SetActive(true);
            previousButton.EnableButton();
        }
    }

    public void ResetTimer()
    {
        if (timerShown)
            return;

        //Stop the coroutine timer
        if(timerRoutine != null)
        {
            StopCoroutine(timerRoutine);
            timerRoutine = null;
        }
        timerString.text = "00:00:00";
        timerAnimationImage.fillAmount = 0;
    }
    public void StartTimer(float time)
    {
        timerRoutine = StartCoroutine(TimerRoutine(time));
    }

    IEnumerator TimerRoutine(float time)
    {
        string minutes;
        string seconds;
        float currentTime = 0;
        float totalSeconds = 10 * 60;
        float div = totalSeconds / time;

        while (currentTime < totalSeconds)
        {
            float frameTime = Time.deltaTime;
            currentTime += frameTime * div;
            float t = currentTime / 60;
            minutes = ((int)t).ToString("D2");
            seconds = ((int)((t % 1) * 100)).ToString("D2");

            timerString.text = "00:" + minutes + ":" + seconds;

            timerAnimationImage.fillAmount = currentTime / totalSeconds;

            yield return new WaitForFixedUpdate();
        }
        timerString.text = "00:10:00";

        //Enable the next button
        ActivateNextButton();

        timerRoutine = null;
        yield return null;
    }

    public override void ResetPanel()
    {
        base.ResetPanel();
        timerShown = false;
        ResetTimer();
    }
}
