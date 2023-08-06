using Assets.Scripts.Wave;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WAFall : MonoBehaviour
{
    public float width = 1;
    public float height = 1.0f;
    public float offset = 0;
    public float speed = 1;

    

    private void OnEnable()
    {
        enabled = false;

        GetComponent<VoidWaveMesh>().DoFall(width, height, offset, speed);
    }
}
