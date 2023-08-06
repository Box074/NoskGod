using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HeroInPositionTooLong : MonoBehaviour
{
    public GameObject Prefab;
    public Vector2 Offset;
    public float MinTime;
    public float MaxTime;
    private float time = -100;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 9 || collision.gameObject.layer == 20)
        {
            time = Random.Range(MinTime, MaxTime);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9 || collision.gameObject.layer == 20)
        {
            time = -9999;
        }
    }

    private void Update()
    {
        if (time < -50) return;
        time -= Time.deltaTime;
        if(time <= 0)
        {
            var pos = transform.position;
            pos += (Vector3)Offset;
            Instantiate(Prefab, pos, Quaternion.identity);
            time = -500;
        }
    }
}
