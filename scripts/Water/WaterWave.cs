
namespace NoskGodMod;

class WaterWave : MonoBehaviour
{
    public MeshRenderer mr = null!;
    public MeshFilter mf = null!;
    public WaveMesh wave = new();
    public PolygonCollider2D col = null!;
    public List<MeshVertex> points = null!;
    public DamageHero dh = null!;
    private List<Vector2> col_points = new();
    public float xMin = 0;
    public float xMax = 120;
    public float speed = 0;
    public bool loop = false;
    public bool loopThrough = false;
    public bool IsFinished => points.Count <= 2;
    private void Awake()
    {
        mf = gameObject.AddComponent<MeshFilter>();
        mr = gameObject.AddComponent<MeshRenderer>();
        col = gameObject.AddComponent<PolygonCollider2D>();
        dh = gameObject.AddComponent<DamageHero>();
        gameObject.AddComponent<NonBouncer>().active = true;
        dh.damageDealt = 1;
        dh.hazardType = (int)HazardType.NON_HAZARD + 1;
        col.isTrigger = true;
        gameObject.layer = (int)PhysLayers.ENEMIES;
        wave.points = new()
        {
            new(new(xMin, 1), new(0, 1)),
            new(new(-3, 2), new(0,0)),
            new(new(xMax, 1), new(1,1))
        };
        points = wave.points;
        mf.sharedMesh = wave.UnityObject;
        mr.sharedMaterial = new(GraphicsLibrary.shaders["NoLightShader"]);
        mr.sharedMaterial.mainTexture = Texture2D.blackTexture;
        unchecked
        {
            mr.sortingLayerID = (int)3945752401; //Over
        }
        mr.enabled = false;
        col.enabled = false;
    }
    public void SortPoints()
    {
        if (points.Count == 1)
        {
            if (points[0].position.x == xMin) points.Add(new(new(xMax, 0), new(1, 1)));
            else points.Insert(0, new(new(xMin, 0), new(0, 1)));
        }
        else if (points.Count == 0)
        {
            points.Add(new(new(xMin, 0), new(0, 1)));
            points.Add(new(new(xMax, 0), new(1, 1)));
        }
        points.Sort((a, b) => a.position.x.CompareTo(b.position.x));
    }
    public Vector2 GetPointPositionInWorld(int index)
    {
        index += 1;
        if (index >= points.Count) throw new ArgumentOutOfRangeException(nameof(index));
        var p = points[index].position;
        return p * (Vector2)transform.localScale + (Vector2)transform.position;
    }
    public void Move(float offset = 0.1f)
    {
        if (offset == 0) return;

        for (int i = 1; i < points.Count - 1; i++)
        {
            var p = points[i];
            p.position += new Vector2(offset, 0);
            points[i] = p;
        }
        SortPoints();
        for (int i = 0; i < points.Count; i++)
        {
            var p = points[i].position;
            if (p.x < xMin || (p.x == xMin && i > 0) || p.x > xMax || (p.x == xMax && i < (points.Count - 1)))
            {
                if (!loop)
                {
                    points.RemoveAt(i);
                    i--;
                }
                else
                {
                    if (p.x <= xMin)
                    {
                        p.x = xMax - (xMin - p.x);
                    }
                    else
                    {
                        p.x = xMin + (p.x - xMax);
                    }
                    loopThrough = true;
                }
            }
        }
        UpdateMesh();
    }
    public IEnumerator WaitFinish()
    {
        while (!IsFinished) yield return null;
    }
    public void Insert(float y, float uvx, PointDirection dir = PointDirection.Left)
    {
        if (dir != PointDirection.Left && dir != PointDirection.Right) throw new ArgumentOutOfRangeException(nameof(dir));
        y += 1;
        var x = (dir == PointDirection.Left) ? (xMin + 0.5f) : (xMax - 0.5f);
        var p = (dir == PointDirection.Left) ? 1 : (points.Count - 2);
        points.Insert(p, new(new(x, y), new(uvx, 0)));
        UpdateMesh();
    }
    private void Update()
    {
        if (points.Count == 2)
        {
            mr.enabled = false;
            col.enabled = false;
            return;
        }
        if (speed != 0)
        {
            Move(speed * Time.deltaTime);
        }

    }
    public void UpdateMesh()
    {
        if (points.Count == 2)
        {
            mr.enabled = false;
            col.enabled = false;
            return;
        }
        col.enabled = true;
        mr.enabled = true;
        SortPoints();
        points[0] = new(new(xMin, 1), new(0, 1));
        points[points.Count - 1] = new(new(xMax, 1), new(0, 1));

        col_points.Clear();
        col_points.AddRange(points.Select(x => x.position));
        col.SetPath(0, col_points);

        wave.Update();
    }
}
