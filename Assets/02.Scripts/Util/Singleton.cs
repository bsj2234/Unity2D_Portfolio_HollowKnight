using UnityEngine;
using UnityEngine.Assertions;

//�̱���
//�ϳ��� �ν��Ͻ��� �����ϴ� Ŭ����
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance { get; set; }

    public static T Instance 
    {   
        get
        {
            //�ν��Ͻ��� ������ ã�� ���ϸ� ������Ʈ�� ���ҽ�/�����տ��� ã�ƺ���
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