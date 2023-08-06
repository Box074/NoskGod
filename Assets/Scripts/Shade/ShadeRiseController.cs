using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ShadeRiseController : MonoBehaviour
{
    public float maxY = 0;
    public float minY = 0;
    private float targetY = 0;
    public float time = 1;
    public GameObject next;
    public Animator anim;
    private float a;
    private float vel;

    [Header("Audio")]
    public AudioSource ad;
    public AudioClip Startle;
    private AnimatorStateInfo StateInfo => anim.GetCurrentAnimatorStateInfo(0);
    IEnumerator Start()
    {
        targetY = Random.Range(minY, maxY);

        var dis = targetY - transform.position.y;
        a = -dis * 2 / time / time;
        vel = Mathf.Sqrt(Mathf.Abs(4 * dis * dis / time / time));

        anim.Play("Blob");

        ad.PlayOneShot(Startle);
        while(transform.position.y < targetY && vel > 0) yield return null;

        anim.Play("Form");
        vel = a = 0;
        while (!StateInfo.IsName("EmptyClip")) yield return null;

        next.SetActive(true);
        next.transform.DetachChildren();
        Destroy(gameObject);
    }
    private void Update()
    {
        if (vel > 0)
        {
            var ny = transform.position;
            var t = Time.deltaTime;
            var x = vel * t + a * t * t / 2;
            vel += a * t;
            ny.y += x;
            transform.position = ny;
        }
    }
}
