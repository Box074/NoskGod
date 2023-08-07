using Assets.Scripts.Wave;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections;
using UnityEngine;
using Debug = UnityEngine.Debug;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(PolygonCollider2D))]
public class VoidWaveMesh : MonoBehaviour
{
    private MeshFilter filter;
    private PolygonCollider2D col;
    private Mesh mesh;

    public List<WavePointProvider> points;

    public float depth;
    public float width;

    public int pointsPerUnit;

    public DamageHero damageHero;

    // Start is called before the first frame update
    void Start()
    {
        points ??= new List<WavePointProvider>();
        filter = GetComponent<MeshFilter>();
        col = GetComponent<PolygonCollider2D>();

        GetComponent<MeshRenderer>().sortingOrder = 1000;
    }

    private List<Vector2> CalcPoints(int pointsPerUnit, float? minX = null, float? maxX = null)
    {
        var min = Mathf.Max(minX ?? 0, 0);
        var max = Mathf.Min(maxX ?? width, width);
        var points = new List<Vector2>();
        var distance = 1f / pointsPerUnit;

        this.points ??= new List<WavePointProvider>();
        for (float x = distance + min; x < max; x += distance)
        {
            float y = 0;
            for (int i = 0; i < this.points.Count; i++)
            {
                var p = this.points[i];
                if (points.Count == 0)
                {
                    p.DrawCount = 0;
                }
                if (!p.HasPoint(x, width)) continue;
                p.DrawCount++;
                y += p.GetY(x, width);
            }
            points.Add(new Vector2(x, y));
        }
        return points;
    }

    private bool[] importantBlocks = null;

    private void UpdateCol()
    {

        var blockCount = Mathf.CeilToInt(width / 1f);
        if(importantBlocks == null || importantBlocks.Length <= blockCount)
        {
            importantBlocks = new bool[blockCount];
        }
        Array.Clear(importantBlocks, 0, importantBlocks.Length);
#if UNITY_EDITOR
        var importantColliders = FindObjectsOfType<ImportantCollider>();
#else
        var importantColliders = ImportantCollider.colliders;
#endif
        foreach (var col in importantColliders)
        {
            if (col == null) continue;
            var (min, max) = col.GetRange();
            var minX = Mathf.Max(0, Mathf.FloorToInt(GetOffsetX(min)));
            var maxX = Mathf.Min(blockCount - 1, Mathf.CeilToInt(GetOffsetX(max)));
            if (minX >= maxX) continue;
            for(int i = minX; i <= maxX; i++)
            {
                importantBlocks[i] = true;
            }
        }
        List<Vector2> points = new List<Vector2>();
        for(int i = 0; i < blockCount;)
        {
            int beg = i;
            bool s = importantBlocks[i++];
            for(; i < blockCount; i++)
            {
                if (importantBlocks[i] != s) break;
            }
            points.AddRange(CalcPoints(s ? Mathf.Min(pointsPerUnit, 8) : 1, beg, i));
        }

        var path = new Vector2[points.Count + 2];
        path[0] = new Vector2(-width / 2f, -depth);
        for (int i = 0; i < points.Count; i++)
        {
            var p = points[i];
            if (p.x == 0) continue;
            p.x -= width / 2f;
            path[i + 1] = p;
        }
        path[points.Count + 1] = new Vector2(width / 2f, -depth);
        col.SetPath(0, path);
    }

    private Bounds GetMeshBounds()
    {
        var cam = Camera.main;
        var d = Mathf.Abs(cam.transform.position.z - transform.position.z);
        var lb = cam.ViewportToWorldPoint(new Vector3(0, 0, d));
        var rt = cam.ViewportToWorldPoint(new Vector3(1, 1, d));
        var bounds = new Bounds();
        bounds.min = lb; 
        bounds.max = rt;
        return bounds;
    }

    private void UpdateMesh()
    {
        
        if(mesh == null)
        {
            mesh = new Mesh();
            mesh.MarkDynamic();
            filter.sharedMesh = mesh;
        }

        var b = GetMeshBounds();

        var points = CalcPoints(pointsPerUnit, GetOffsetX(b.min.x) - 5, GetOffsetX(b.max.x) + 5);

        var total = points.Count;

        var blocks = new List<List<Vector2>>();
        {
            List<Vector2> curBlock = null;
            
            var root = Vector2.zero;
            var prev = Vector2.zero;
            var isRise = false;
            for (int i = 0; i < total; i++)
            {
                var p = points[i];

                if (curBlock == null)
                {
                    curBlock = new List<Vector2>();
                    if(blocks.Count > 0)
                    {
                        curBlock.Add(prev);
                    }
                }
                try
                {
                    if (root.x == 0)
                    {
                        root = p;
                        prev = p;
                        continue;
                    }
                    if (root == prev)
                    {
                        isRise = p.y > prev.y;
                    }
                    curBlock.Add(p);
                    if ((p.y > prev.y && !isRise) || (p.y <= prev.y && isRise))
                    {
                        if (curBlock.Count >= 2) blocks.Add(curBlock);
                        curBlock = null;
                        root = p;
                    }
                }
                finally
                {
                    prev = p;
                }
            }
            if (curBlock != null && curBlock.Count >= 2) blocks.Add(curBlock);
        }
        List<Vector3> vertexs = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        float midX = width / 2f;
        int PutVertex(Vector3 vertex, Vector2 uv)
        {
            vertex.x -= midX;
            vertexs.Add(vertex);
            uvs.Add(uv);
            return vertexs.Count - 1;
        }
        foreach (var block in blocks)
        {
            var isUp = block[0].y <= block[1].y;
            var max = isUp ? block[block.Count - 1] : block[0];
            var min = isUp ? block[0] : block[block.Count - 1];

            var root = new Vector2(max.x, -depth);

            int rootId = PutVertex(root, Vector2.zero);

            if(isUp)
            { 
                PutVertex(new Vector2(min.x, -depth), Vector2.zero);
            }

            foreach(var p in block)
            {
                if (isUp)
                {
                    triangles.Add(vertexs.Count - 1);
                    triangles.Add(vertexs.Count);
                }
                else
                {
                    triangles.Add(vertexs.Count);
                    triangles.Add(vertexs.Count + 1);
                }
                triangles.Add(rootId);

                PutVertex(p, Vector2.zero);
            }
            
            if(!isUp)
            {
                PutVertex(new Vector2(min.x, -depth), Vector2.zero);
            }
        }

        mesh.Clear();
        mesh.SetVertices(vertexs);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(triangles, 0);
    }

    public float GetY(float x)
    {
        float y = 0;
        for (int i = 0; i < this.points.Count; i++)
        {
            var p = points[i];
            if (!p.HasPoint(x, width)) continue;
            y += p.GetY(x, width);
        }
        return y;
    }
    public float GetWorldY(float worldX)
    {
        return GetY(GetOffsetX(worldX)) * transform.localScale.y + transform.position.y;
    }
    public float GetOffsetX(float worldX)
    {
        return (worldX - transform.position.x) *
            transform.localScale.x +
            width / 2f;
    }
    public float GetMinY(float startX, float endX)
    {
        return CalcPoints(2, startX, endX).Select(x => x.y).Min();
    }
    public bool InRange(float offset)
    {
        return offset >= 0 && offset <= width;
    }
    float nextUpdate = 0;
    // Update is called once per frame
    void Update()
    {
        if(PlatController.currentPlat != null)
        {
            damageHero.damageDealt = 0;
        }
        else
        {
            damageHero.damageDealt = 1;
        }
        nextUpdate -= Time.deltaTime;
        if(nextUpdate <= 0)
        {
            nextUpdate = 0.05f;
        }
        else
        {
            return;
        }
        pointsPerUnit = pointsPerUnit <= 0 ? 1 : pointsPerUnit;
        //Stopwatch stopwatch = new Stopwatch();
        //stopwatch.Start();
        UpdateMesh();
        UpdateCol();
        //stopwatch.Stop();
        //Debug.Log($"Time({pointsPerUnit}): {stopwatch.Elapsed.TotalSeconds} ({1f / stopwatch.Elapsed.TotalSeconds}fps)");
    }


    public void DoFall(float width, float height, float offset, float speed)
    {
        var left = gameObject.AddComponent<WPPSin>();
        left.MoveSpeed = -speed;
        left.LeftRange = left.RightRange = 1;
        left.Width = width;
        left.Height = height;
        left.Center = offset;
        var right = gameObject.AddComponent<WPPSin>();
        right.MoveSpeed = speed;
        right.LeftRange = right.RightRange = 1;
        right.Width = width;
        right.Height = -height;
        right.Center = offset;

        left.isTemp = right.isTemp = true;
    }

    
}
