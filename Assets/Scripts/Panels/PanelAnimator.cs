using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public enum AnimationEffect
{
    FadeIn,
    Position,
    ScaleUp
}
public enum AnimationType
{
    Serial,
    Parallel
}
[System.Serializable]
public class AnimationContainer
{
    public GameObject gameObject;
    public AnimationEffect animType;
    [HideInInspector]
    public AnimationBeing animationBeing;

    //For scale and position animations and whatever has initial and target vectors
    //public Vector3 initialVector = new Vector3(0, 0, 0);
    //public Vector3 targetVector = new Vector3(1, 1, 1);
}

public struct AnimationVectorStuct
{
    public AnimationVectorStuct(Vector3 initial, Vector3 target)
    {
        this.initial = initial;
        this.target = target;
    }
    public Vector3 initial;
    public Vector3 target;
}
public class AnimationBeingTargetField
{
    AnimationVectorStuct animationVectorStruct;
    Vector3 vector;
    Color col;
    Material material;

    public void SetMaterial(Material mat)
    {
        material = mat;
    }
    public void SetVector3(Vector3 vec)
    {
        this.vector = vec;
    }
    public void SetColor(Color col)
    {
        this.col = col;
    }
    public void SetAnimationVectorStruct(AnimationVectorStuct animStruct)
    {
        animationVectorStruct = animStruct;
    }
    public Color GetColor()
    {
        return col;
    }
    public Vector3 GetVector3()
    {
        return vector;
    }
    public AnimationVectorStuct GetAnimationVectorStruct()
    {
        return animationVectorStruct;
    }
    public Material getMaterial()
    {
        return material;
    }
    
}
public class AnimationBeing : IEquatable<AnimationBeing>
{
    public AnimationBeing(GameObject objRef, System.Action<GameObject, AnimationBeingTargetField, float> func, float duration = 1, int direction = 1)
    {
        this.duration = duration;
        this.direction = direction;
        //this.func = animFunction;

        if (direction > 0)
            currentDuration = 0;
        else
            currentDuration = duration;

        obj = objRef;
        this.func = func;
    }

    public bool animationFinished = false;
    public bool targetForDestruction = false;
    float duration = 1;
    float currentDuration = 0;
    int direction;
    bool waiting = false;
    bool loop = false;
    public GameObject obj;
    AnimationBeingTargetField targetField;
    
    
    System.Action<GameObject, AnimationBeingTargetField, float> func;

    System.Action animationFinishedEvent;
    
    public void SetDefaults(AnimationContainer container)
    {
        
        switch (container.animType)
        {
            case AnimationEffect.FadeIn:
                Color c = container.gameObject.GetComponent<UnityEngine.UI.Image>().color;
                container.gameObject.GetComponent<UnityEngine.UI.Image>().color = new Color(c.r, c.g, c.b, 0);
                break;
            case AnimationEffect.Position:
                break;
            case AnimationEffect.ScaleUp:
                container.gameObject.transform.localScale = new Vector3(0, 0, 0);
                break;

        }
    }

    public void CreateAnimationBeingTargetAndInitializeDefaultValue<T>(T targetProperty)
    {
        targetField = new AnimationBeingTargetField();
        if(targetProperty is Material)
        {
            Material target = (Material)Convert.ChangeType(targetProperty, typeof(Material));

        }
        if(targetProperty is Vector3)
        {
            Vector3 target = (Vector3)Convert.ChangeType(targetProperty, typeof(Vector3));

            targetField.SetVector3(target);
            return;
        }
        if(targetProperty is Color)
        {
            Color target = (Color)Convert.ChangeType(targetProperty, typeof(Color));

            targetField.SetColor(target);
            return;
        }
        if (targetProperty is Color32)
        {
            Color32 target = (Color32)Convert.ChangeType(targetProperty, typeof(Color32));

            targetField.SetColor(target);
            return;
        }
        if (targetProperty is AnimationVectorStuct)
        {
            AnimationVectorStuct target = (AnimationVectorStuct)Convert.ChangeType(targetProperty, typeof(AnimationVectorStuct));

            Debug.Log("Taaaaaa: " + target.initial + " " + target.target);

            targetField.SetAnimationVectorStruct(target);
            return;
        }
    }

    public float Update()
    {
        
        if (animationFinished || waiting)
        {
            
            Debug.Log("Anim Finished " + obj.gameObject.name);
            return -float.MinValue;
        }
        
        currentDuration += Time.deltaTime * direction;

        float normalizedTime = currentDuration / duration; // Normalizes time to [0 1] range
        normalizedTime = Mathf.Clamp(normalizedTime, 0, 1);

        float alpha;
        if (loop)
        {
            alpha = PanelAnimator.Instance.animCurve_Loop.Evaluate(normalizedTime);
        }
        else
        {
            alpha = PanelAnimator.Instance.animCurve.Evaluate(normalizedTime);
        }

        //Debug.Log("Object:" + obj.gameObject.name + " Current duration: " + currentDuration + " Normalized Time: " + alpha + " alpha: " + alpha);
        func?.Invoke(obj, targetField, alpha); //Run the function with the normalized time so that the animations happen in that range

        //Forward animation
        if (currentDuration > duration)
        {
            if (loop)
            {
                currentDuration = 0;
            }
            else
            {
                currentDuration = duration;
                animationFinished = true;
                if(animationFinishedEvent != null)
                    animationFinishedEvent();
            }
        }

        //Reverse animation
        if (currentDuration < 0)
        {
            if (loop)
            {
                currentDuration = duration;
            }
            else
            {
                currentDuration = 0;
                animationFinished = true;
                if (animationFinishedEvent != null)
                    animationFinishedEvent();
            }
        }

        return currentDuration; // Return the current duration for the exit condition checks
    }
    public void Stop()
    {
        loop = false;
    }
    public void StopImmediately()
    {
        loop = false;
        animationFinished = true;
        

        if (animationFinishedEvent != null)
            animationFinishedEvent();
    }
    float waitTime = 0;
    public void SetLoop(bool flag) { loop = flag; }
    public void Wait()
    {
        waiting = true;
    }
    public void Play()
    {
        waiting = false;
        //if (targetForDestruction)
        //    targetForDestruction = false;
        Debug.Log("Playing:  " + obj.gameObject.name);
    }


    public bool Equals(AnimationBeing other)
    {
        return (other.obj == this.obj);
    }
    
}

public class PanelAnimator : Singleton<PanelAnimator>
{

    public List<System.Func<IEnumerator>> animationFunctionsStack = new List<System.Func<IEnumerator>>();
    public AnimationCurve animCurve;
    public AnimationCurve animCurve_Loop;
    
    List<AnimationBeing> animationBeingsSequence = new List<AnimationBeing>();
    List<AnimationBeing> animationBeingsParallel = new List<AnimationBeing>();
    List<AnimationBeing> targetedForDestruction = new List<AnimationBeing>();
    
    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        //Play Sequence animations
        if(animationBeingsSequence.Count > 0)
        {
            AnimationBeing b = animationBeingsSequence[0];
            b.Update();
            if (b.animationFinished)
            {
                b.targetForDestruction = true;
                animationBeingsSequence.RemoveAt(0);
                
                DeleteAnimationBeing(b);
            }
        }
        //Debug.Log(animationBeingsParallel.Count);
        //Play Parallel animations
        foreach(AnimationBeing being in animationBeingsParallel)
        {
            Debug.Log("Being:  " + being.obj.name);
            being.Update();
            //This might end up causing some issues. Check this later
            if (being.animationFinished)
            {
                //Debug.Log("Animation done for: " + being.name);
                being.targetForDestruction = true;
                //animationBeingsParallel.Remove(being);

                targetedForDestruction.Add(being);
                //DeleteAnimationBeing(being);
            }
        }

    }
    private void LateUpdate()
    {
        int index = 0;
        foreach (AnimationBeing being in targetedForDestruction)
        {

            if (animationBeingsParallel.Contains(being))
            {
                Debug.Log("Removing " + being.obj.name);
                animationBeingsParallel.Remove(being);

                //Debug.Log("Deleted Being: " + being.obj.name);
                DeleteAnimationBeing(being);
            }
            index++;
        }
        targetedForDestruction.Clear();
    }


    public AnimationBeing CreateSequenceAnimationBeing<T>(GameObject obj, System.Action<GameObject, AnimationBeingTargetField, float> func, 
        T targetComponent, float duration = 1, int direction = 1)
    {
        AnimationBeing being = new AnimationBeing(obj, func, duration, direction);
        //If the target component is specified, always create an animation target being
        if(targetComponent != null)
            being.CreateAnimationBeingTargetAndInitializeDefaultValue(targetComponent);
        
        animationBeingsSequence.Add(being);

        return being;
    }
    public AnimationBeing CreateParallelAnimationBeing<T>(GameObject obj, System.Action<GameObject, AnimationBeingTargetField, float> func, 
        T targetComponent, float duration = 1, int direction = 1)
    {
        AnimationBeing being = new AnimationBeing(obj, func, duration, direction);
        //If the target component is specified, always create an animation target being
        if(targetComponent != null)
            being.CreateAnimationBeingTargetAndInitializeDefaultValue(targetComponent);

        //Debug.Log("Created");
        animationBeingsParallel.Add(being);

        return being;
    }
    public AnimationBeing CreateScaleAnim(GameObject objRef, Vector3 initial, Vector3 target, AnimationType animType, float duration, int direction, bool loop = false)
    {
        AnimationBeingTargetField t = new AnimationBeingTargetField();
        t.SetAnimationVectorStruct(new AnimationVectorStuct(initial, target));

        System.Action<GameObject, AnimationBeingTargetField, float> func = null;
        func = (GameObject obj, AnimationBeingTargetField field, float alpha) =>
        {
            Vector3 res = field.GetAnimationVectorStruct().initial * (1 - alpha) + field.GetAnimationVectorStruct().target * alpha;
            //Debug.Log(res);
            obj.transform.localScale = res;
        };

        //animation being creation
        AnimationBeing being = new AnimationBeing(objRef, func, duration, direction);
        being.SetLoop(loop);
        being.CreateAnimationBeingTargetAndInitializeDefaultValue(t.GetAnimationVectorStruct());

        switch (animType)
        {
            case AnimationType.Parallel:
                Debug.Log("Created");
                animationBeingsParallel.Add(being);
                break;
            case AnimationType.Serial:
                animationBeingsSequence.Add(being);
                break;
        }
        return being;
    }

    /// <summary>
    /// Animate the specified property value between 0 and 1
    /// </summary>
    /// <param name="objref"></param>
    /// <param name="material"></param>
    /// <param name="property"></param>
    /// <param name="animType"></param>
    /// <param name="duration"></param>
    /// <param name="direction"></param>
    /// <param name="loop"></param>
    /// <returns></returns>
    public AnimationBeing CreateMaterialAnimation(GameObject objRef, Material material, string property, AnimationType animType, float duration, int direction = 1, bool loop = false)
    {
        AnimationBeingTargetField t = new AnimationBeingTargetField();
        t.SetMaterial(material);

        System.Action<GameObject, AnimationBeingTargetField, float> func = null;
        func = (GameObject obj, AnimationBeingTargetField field, float alpha) =>
        {
            //Vector3 res = t.GetAnimationVectorStruct().initial * (1 - alpha) + t.GetAnimationVectorStruct().target * alpha;
            material.SetFloat(property, alpha);
            //Debug.Log(res);
            
        };

        //animation being creation
        AnimationBeing being = new AnimationBeing(objRef, func, duration, direction);
        
        being.SetLoop(loop);
        being.CreateAnimationBeingTargetAndInitializeDefaultValue(t.getMaterial());

        switch (animType)
        {
            case AnimationType.Parallel:
                Debug.Log("Created");
                animationBeingsParallel.Add(being);
                break;
            case AnimationType.Serial:
                animationBeingsSequence.Add(being);
                break;
        }
        return being;
    }
    public AnimationBeing CreateRectTransformLocalPositionAnimation(RectTransform rectTransform,
                Vector3 initial, Vector3 target, AnimationType animType, float duration, int direction = 1, bool loop = false)
    {
        AnimationBeingTargetField t = new AnimationBeingTargetField();
        t.SetAnimationVectorStruct(new AnimationVectorStuct(initial, target));

        System.Action<GameObject, AnimationBeingTargetField, float> func = null;
        func = (GameObject obj, AnimationBeingTargetField field, float alpha) =>
        {
            //Vector3 res = t.GetAnimationVectorStruct().initial * (1 - alpha) + t.GetAnimationVectorStruct().target * alpha;
            obj.GetComponent<RectTransform>().localPosition = field.GetAnimationVectorStruct().initial * (1 - alpha) + 
                                                field.GetAnimationVectorStruct().target * alpha;

            //material.SetFloat(property, alpha);
        };

        //animation being creation
        AnimationBeing being = new AnimationBeing(rectTransform.gameObject, func, duration, direction);

        being.SetLoop(loop);
        being.CreateAnimationBeingTargetAndInitializeDefaultValue(t.GetAnimationVectorStruct());

        switch (animType)
        {
            case AnimationType.Parallel:
                Debug.Log("Created");
                animationBeingsParallel.Add(being);
                break;
            case AnimationType.Serial:
                animationBeingsSequence.Add(being);
                break;
        }
        return being;
    }


    //TODO: Implement dispose functionality
    void DeleteAnimationBeing(AnimationBeing being)
    {
        
        //Destroy(being);
    }


    //The coroutine that gets pushed in to the list must contain the functionality that pops the respective function from the list
    public void PushAnimation(System.Func<IEnumerator> func)
    {
        animationFunctionsStack.Add(func);
    }

    public void PopAnimation(System.Func<IEnumerator> func)
    {
        animationFunctionsStack.Remove(func);
    }
    public void PlayAnimation(System.Func<IEnumerator> func)
    {
        if (!animationFunctionsStack.Contains(func))
        {
            PushAnimation(func);
        }
        StartCoroutine(func());
    }
    public bool Contains(System.Func<IEnumerator> func)
    {
        return animationFunctionsStack.Contains(func);
    }

    IEnumerator Anim()
    {
        yield return new WaitForSeconds(1);
    }
    
}
