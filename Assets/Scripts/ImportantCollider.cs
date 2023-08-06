using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ImportantCollider : MonoBehaviour
{
    public static List<ImportantCollider> colliders = new List<ImportantCollider>();
    private Rigidbody2D rb2d;
    private List<Collider2D> cols;
    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        colliders.Add(this);
    }

    private void OnDisable()
    {
        colliders.Remove(this);
    }

    public (float min, float max) GetRange()
    {
        cols ??= new List<Collider2D>();
        var count = rb2d.GetAttachedColliders(cols);
        var max = cols.Take(count).Max(x => x.bounds.max.x) + 3;
        var min = cols.Take(count).Min(x => x.bounds.min.x) - 3;
        return (min, max);
    }
    private void Update()
    {
        
    }
}
