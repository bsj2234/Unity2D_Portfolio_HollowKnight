using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

//싱글턴
//하나의 인스턴스만 존재하는 클래스
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance { get; set; }

    public static T Instance 
    {   
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(T)) as T;
                if(instance == null)
                {
                     instance = Instantiate(Resources.Load<T>("Prefabs/" + typeof(T).Name)) as T;
                }

            }
            return instance;
        }
    }
}
