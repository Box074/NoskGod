using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ShadeQuakeController : MonoBehaviour
{
    public Collider2D col;
    public Rigidbody2D rb2d;
    public AudioSource ad;

    public Animator anim;
    public float speed;

    [Header("Audio")]
    public AudioClip Prepare;
    public AudioClip Dash;
    public AudioClip Impact;

    private AnimatorStateInfo StateInfo => anim.GetCurrentAnimatorStateInfo(0);
    // Start is called before the first frame update
    IEnumerator Start()
    {
        col.enabled = false;
        Destroy(gameObject, 10);
        anim.Play("Quake Antic");
        ad.PlayOneShot(Prepare);
        while (!StateInfo.IsName("Quake")) yield return null;
        ad.PlayOneShot(Dash);
        rb2d.isKinematic = false;
        col.enabled = true;
        yield return new WaitForSeconds(0.15f);
        while (true)
        {
            rb2d.velocity = new Vector2(0, -speed);
            yield return null;
            if (rb2d.velocity.y > -1)
            {
                break;
            }
        }
        anim.Play("Quake Land");
        col.enabled = false;
        rb2d.velocity = Vector2.zero;
        rb2d.isKinematic = true;
        ad.PlayOneShot(Impact);
        while (!StateInfo.IsName("EmptyClip")) yield return null;
        Destroy(gameObject, 2.5f);
    }
}
