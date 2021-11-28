using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControler : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    private float speed=0.006f;
    public void setTarget(Transform t)
    {
        target=t;
    }

    void LateUpdate()
    {
        var pos=new Vector3(target.position.x,target.position.y, -40f);
        transform.position = Vector3.Lerp(transform.position, pos, speed);
    }
}
