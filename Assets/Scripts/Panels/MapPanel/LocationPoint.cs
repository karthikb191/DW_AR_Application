using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LocationPoint : MonoBehaviour
{
    public Color32 targetColor;
    Color32 initialColor;
    Image image;

    bool activated = true;
    int direction = 1;
    float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        initialColor = image.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (!activated)
            return;

        //Animate
        float time = Mathf.Abs(Mathf.Sin(timer * 5));
        image.color = time * (Color)initialColor + (1 - time) * (Color)targetColor;

        timer += Time.deltaTime;
        //Debug.Log(time);
    }
    public void Deactivate()
    {
        activated = false;
        timer = 0;
    }
    public void Activate()
    {
        activated = true;
    }
}
