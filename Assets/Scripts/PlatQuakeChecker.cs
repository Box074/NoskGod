using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatQuakeChecker : MonoBehaviour
{
    private VoidWaveMesh wave;
    // Start is called before the first frame update
    void Awake()
    {
        wave = FindObjectOfType<VoidWaveMesh>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger) return;
        
        var trig = collision.GetComponent<IPlatQuakeTrigger>();
        if(trig != null)
        {
            wave.DoFall(trig.Width, trig.Height, wave.GetOffsetX(transform.position.x), 3);
            return;
        }
        var rb2d = collision.attachedRigidbody;
        if(rb2d != null)
        {
            var limitVel = rb2d.GetComponent<NoVelLimit>() == null;
            var layer = rb2d.gameObject.layer;
            if(layer == 9 || layer == 11)
            {
                var d = rb2d.velocity.y;
                if(limitVel)
                {
                    d = Mathf.Max(d, -50);
                }
                if(d < -12)
                {
                    wave.DoFall(1, d / 100, wave.GetOffsetX(transform.position.x), 3);
                }
            }
        }
    }
}
