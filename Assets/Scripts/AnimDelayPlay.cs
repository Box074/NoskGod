using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimDelayPlay : MonoBehaviour
{
    public Animator anim;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(Random.Range(0, 0.4f));
        anim.Play("Waterfall");
    }

}
