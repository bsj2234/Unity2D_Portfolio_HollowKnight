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
            //인스턴스를 씬에서 찾지 못하면 프로젝트의 리소스/프리팹에서 찾아본다
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(T)) as T;
                if(instance == null)
                {
                     instance = Instantiate(Resources.Load<T>("Prefabs/" + typeof(T).Name));
                }
            }
            Assert.IsNotNull(instance, "There is no instancable object");
            return instance;
        }
    }
}