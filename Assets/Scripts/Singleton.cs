using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T:Singleton<T>
{
    private static T instance = null;
    public static T Instance { get { return instance; }  }

    protected virtual void Start()
    {
        //if (instance == null)
        //{
        //    instance = (T)this;
        //    Debug.Log("Instance name is: " + instance.name);
        //}
        instance = instance == null ? (T)this : instance;
        
    }

}
