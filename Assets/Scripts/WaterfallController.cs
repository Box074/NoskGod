using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterfallController : MonoBehaviour
{
    public Rigidbody2D headRig;
    public float maxSpeed;
    public GameObject prefab;
    public float distance;
    public GameObject last;

    void Update()
    {
        if(headRig.transform.position.y < 0)
        {
            enabled = false;
            headRig.isKinematic = true;
            return;
        }
        if(headRig.velocity.y < -maxSpeed)
        {
            headRig.velocity = new Vector2(0, -maxSpeed);
        }
        if(last != null)
        {
            if(Mathf.Abs(last.transform.position.y - headRig.transform.position.y) <= distance)
            {
                return;
            }
        }
        last = Instantiate(prefab, transform);
        last.transform.position = headRig.transform.position;
    }
}
