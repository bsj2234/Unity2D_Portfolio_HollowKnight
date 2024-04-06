using UnityEngine;





public class BossKnightController : MonoBehaviour
{

    bool fighting = false;
    public Transform target;

    private void Update()
    {
    }


    private void RandomFightSkill()
    {

        int a = Random.Range(0, 5);
        switch (a)
        {
            case 0:
                Follow();
                break;
            default:
                break;
        }
    }

    void Follow()
    {
        Vector2 toTarget = target.position - transform.position;

        float xDir = toTarget.x;
        xDir = Mathf.Sign(xDir);
    }

    void Teleport()
    {

    }

}