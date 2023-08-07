using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyAnim : MonoBehaviour
{
    public Animator animator;
    
    void OnEnable()
    {
        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        yield return null;
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Blob")) yield return null;
        yield return new WaitForSeconds(0.15f);
        iTween.MoveBy(gameObject, iTween.Hash(
            "amount", new Vector3(0, -14, 0),
            "time", 2,
            "easetype", iTween.EaseType.easeInQuart
            ));
        Destroy(gameObject, 15);
    }
}
