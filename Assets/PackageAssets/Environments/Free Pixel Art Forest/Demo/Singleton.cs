using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance { get; set; }

    public static T Instance 
    {   
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(T)) as T;
                if (instance == null)
                    Assert.IsNotNull(instance, "Wrong singleton type");

            }
            return instance;
        }
    }
}
