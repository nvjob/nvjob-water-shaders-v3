using System.Collections;
using UnityEngine;


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


public class Dragon : MonoBehaviour
{
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public Transform dragonIn;

    //--------------

    Transform tr;
    Animator dragonA;
    float vertical, horizontal, incline;
    bool free;


    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void Start()
    {
        //--------------

        tr = transform;
        dragonA = dragonIn.GetComponent<Animator>();
        StartCoroutine(RandomAngle());

        //--------------
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void LateUpdate()
    {
        //--------------

        tr.Translate(Vector3.forward * Time.deltaTime * 5);

        if (tr.position.y > 46) vertical += Time.deltaTime * 5;
        else vertical -= Time.deltaTime * 5;

        vertical = Mathf.Clamp(vertical, -15.5f, 15.5f);
        tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.Euler(vertical, horizontal, incline), 0.085f * Time.deltaTime);
        dragonIn.localEulerAngles = new Vector3(15 - Mathf.PingPong(Time.time * 3.5f, 30), 10 - Mathf.PingPong(Time.time * 1.5f, 20), 15 - Mathf.PingPong(Time.time * 4, 30));

        float chas = 0.1f - Mathf.PingPong(Time.time * 0.02f, 0.2f);
        if (tr.eulerAngles.x > 180) dragonA.speed = 1.05f + chas;
        else dragonA.speed = 0.95f + chas;

        //--------------
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    IEnumerator RandomAngle()
    {
        //--------------

        while (true)
        {
            float dis = Vector3.Distance(Vector3.zero, new Vector3(tr.position.x, 0, tr.position.z));

            if (free == true)
            {
                if (dis > 120) horizontal = Quaternion.LookRotation(Vector3.zero - tr.position).eulerAngles.y;
                else if (dis < 50) horizontal = Random.Range(-180, 180);
            }
            else
            {
                horizontal = Quaternion.LookRotation(Vector3.zero - tr.position).eulerAngles.y;
                if (dis < 40) free = true;
            }

            if (horizontal >= 0) incline = -20;
            else incline = 20;
            yield return new WaitForSeconds(5);
        }

        //--------------
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
