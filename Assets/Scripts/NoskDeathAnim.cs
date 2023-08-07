using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoskDeathAnim : MonoBehaviour
{
    public AudioSource audio;
    public VoidWaveMesh waveMesh;
    [Header("Audio")]
    public AudioClip scream;

    [Header("Phase 1")]
    public GameObject p1;
    public SpriteRenderer p1_T_Highest;
    public GameObject p1_TG;

    [Header("Phase 2")]
    public GameObject p2;
    public GameObject p2_TG;
    public GameObject p2_TG_2;

    void OnEnable()
    {
        waveMesh = FindObjectOfType<VoidWaveMesh>();
        StartCoroutine(Play());
    }
    IEnumerator SetActive(GameObject go)
    {
        yield return new WaitForSeconds(Random.value * 0.25f);
        go.SetActive(true);
    }
    void SpawnTG(GameObject root)
    {
        root.SetActive(true);
        foreach(Transform v in root.transform)
        {
            v.gameObject.SetActive(false);
            StartCoroutine(SetActive(v.gameObject));
        }
    }
    IEnumerator Play()
    {
        audio.PlayOneShot(scream);
        yield return new WaitForSeconds(0.15f);
        SpawnTG(p1_TG);
        yield return new WaitForSeconds(0.45f);
        
        p2.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        SpawnTG(p2_TG);
        yield return new WaitForSeconds(0.45f);
        float st = Time.time;
        while ((transform.position.y > -10) && p1_T_Highest.isVisible)
        {
            if(Time.time - st > 0.85f)
            {
                SpawnTG(p2_TG_2);
                st = float.MaxValue;
            }
            transform.Translate(0, -2 * Time.fixedDeltaTime, 0, Space.Self);
            yield return new WaitForFixedUpdate();
        }
#if HKMOD
        PlayMakerFSM.BroadcastEvent("NOSK DEATH ANIM FINISH");
#endif
    }
}
