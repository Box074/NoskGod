using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatQuake : MonoBehaviour
{
    private VoidWaveMesh wave;
    // Start is called before the first frame update
    void Awake()
    {
        wave = FindObjectOfType<VoidWaveMesh>();
    }

    private void OnEnable()
    {
        wave.DoFall(1, -1.5f, wave.GetOffsetX(transform.position.x), 3);
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
