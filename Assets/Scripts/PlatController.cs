using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatController : MonoBehaviour
{
    public static bool forceFlat = false;
    public VoidWaveMesh wave;
    public PolygonCollider2D extraCol;
    public AudioClip intoWaterAudio;
    public Rigidbody2D rig;

    public GameObject ColliderPointL;
    public GameObject ColliderPointR;
    public bool skipAnim;
    private bool first = false;
    // Start is called before the first frame update
    void Start()
    {
        if(wave == null)
        {
            wave = FindObjectOfType<VoidWaveMesh>();
        }
        first = false;
        SetSortingOrder(-100);
        transform.localEulerAngles = new Vector3(0, 0, Random.value < 0.5f ? -90 : 90);
    }
    void SetSortingOrder(int order)
    {
        foreach(var v in GetComponentsInChildren<Renderer>())
        {
            v.sortingOrder = order;
        }
    }
    void UpdateAnim()
    {
        if (!skipAnim)
        {
            if (transform.position.y <= wave.GetWorldY(transform.position.x))
            {
                if (!first)
                {
                    rig.velocity = new Vector2(0, 20);
                    return;
                }
            }
            else
            {
                SetSortingOrder(100);
                if (rig.velocity.y <= 0)
                {
                    first = true;
                }
                return;
            }
            wave.DoFall(0.4f, -0.75f, wave.GetOffsetX(transform.position.x), 4);
            AudioSource.PlayClipAtPoint(intoWaterAudio, transform.position, 1);
        }
        extraCol.enabled = true;
        GetComponent<Collider2D>().enabled = true;
        SetSortingOrder(100);
        Destroy(rig);
        return;
    }

    // Update is called once per frame
    void Update()
    {
        if(rig != null)
        {
            UpdateAnim();
            return;
        }
        var op = (transform.position - wave.transform.position).x * 
            wave.transform.localScale.x + 
            wave.width / 2f;

        var p0 = new Vector2(op, wave.GetY(op));
        var p1 = new Vector2(op + 0.1f, wave.GetY(op + 0.1f));
        var of = p0 - p1;
        var angle = forceFlat ? 0 : (Mathf.Atan2(of.y, of.x) * Mathf.Rad2Deg);
        transform.localEulerAngles =
            forceFlat ? Vector3.zero : 
            new Vector3(0, 0, Mathf.LerpAngle(transform.localEulerAngles.z - 180, angle, Time.deltaTime * 2)
            + 180)
            ;
        var pos = transform.position;
        pos.y = wave.transform.position.y + p0.y * wave.transform.localScale.y;
        transform.position = pos;

        var points = new Vector2[4];
        points[0] = ColliderPointL.transform.position - pos;
        points[1] = ColliderPointR.transform.position - pos;
        points[2] = points[1] + new Vector2(0, -100);
        points[3] = points[0] + new Vector2(0, -100);

        extraCol.SetPath(0, points);
        extraCol.transform.localEulerAngles = new Vector3(0, 0, 0 - transform.localEulerAngles.z);
    }
}
