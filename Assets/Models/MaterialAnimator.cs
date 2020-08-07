using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Material))]
public class MaterialAnimator : MonoBehaviour
{
    public Material material;
    AnimationBeing being;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("CreateMaterialAnimationFunction", 1);
    }

    void CreateMaterialAnimationFunction()
    {
        being = PanelAnimator.Instance.CreateMaterialAnimation(gameObject, material, "_Transparency", AnimationType.Parallel, 1, 1, true);
        Invoke("Stop", 10);
    }
    void Stop()
    {
        being.Stop();
    }
}
