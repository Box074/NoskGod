
namespace NoskGodMod;

class AbyssWaterFsm : CSFsm<AbyssWaterFsm>
{
    public float raiseUpTo = 13.5f;
    public float raiseUpTime = 3;
    public FsmInt spawnCount = 1;
    public FsmInt maxTotal = 100;
    private bool _init = false;
    public List<GameObject> tendirs = new();
    private void SpawnTendirs(float x)
    {
        var tendir = Instantiate(NoskGod.AbyssTendrils, transform);
        tendir.transform.localPosition = new(x, -6.0919f, 8.904f + UnityEngine.Random.value * 0.5f);
        Destroy(tendir.GetComponent<AudioSource>());

        tendir.SetActive(true);
        tendirs.Add(tendir);
    }
    public void TendirsCalmDown()
    {
        foreach (var v in tendirs)
        {
            v.FindChildWithPath("Alert Range")?.SetActive(false);
            v.LocateMyFSM("Control").FsmVariables.FindFsmBool("Alert Range").Value = false;
        }
    }
    public void TendirsAngry()
    {
        foreach (var v in tendirs)
        {
            v.FindChildWithPath("Alert Range")?.SetActive(false);
            v.LocateMyFSM("Control").FsmVariables.FindFsmBool("Alert Range").Value = true;
        }
    }
    public void TendirsNormal()
    {
        foreach (var v in tendirs)
        {
            v.FindChildWithPath("Alert Range")?.SetActive(true);
        }
    }
    [FsmState]
    private IEnumerator Init()
    {
        DefineEvent(FsmEvent.Finished, nameof(Idle));
        yield return StartActionContent;
        if (_init) yield return FsmEvent.Finished;
        gameObject.layer = (int)GlobalEnums.PhysLayers.ENEMIES;
        transform.position = new(169.7977f, 9.5326f, -8.9f);
        var box = gameObject.AddComponent<BoxCollider2D>();
        box.isTrigger = true;
        box.size = new(132.3585f, 9.9681f);
        box.offset = new(-38.8488f, -13.2063f);
        gameObject.AddComponent<NonBouncer>().active = true;
        var dh = gameObject.AddComponent<DamageHero>();
        dh.damageDealt = 1;
        dh.hazardType = 5;

        var mat = new Material(Shader.Find("Sprites/Default"));
        foreach (var v in GetComponentsInChildren<Renderer>())
        {
            v.sharedMaterial = mat;
            unchecked
            {
                v.sortingLayerID = (int)3945752401; //Over
            }
        }

        SpawnTendirs(-95.8986f);
        SpawnTendirs(-91.455f);
        SpawnTendirs(-87.4296f);
        SpawnTendirs(-85.5513f);
        SpawnTendirs(-62.1828f);
        SpawnTendirs(-58.4808f);
        SpawnTendirs(-54.5397f);
    }
    [FsmState]
    private IEnumerator Idle()
    {
        DefineEvent("ABYSS WATER RAISE UP", nameof(RaiseUp));
        DefineEvent("ABYSS WATER SPAWN", nameof(SpawnShade));
        yield return StartActionContent;
    }
    [FsmState]
    private IEnumerator SpawnShade()
    {
        DefineEvent(FsmEvent.Finished, nameof(Idle));
        yield return StartActionContent;
        for (int i = 0; i < spawnCount.Value; i++)
        {
            var p = new Vector3(UnityEngine.Random.Range(74, 114), GetComponent<BoxCollider2D>().bounds.max.y - 1, 0);
            if (NoskShade.GetShadeCount() > maxTotal.Value)
            {
                foreach (var v in NoskShade.shades.ToArray().Where(x => !x.GetComponent<MeshRenderer>().isVisible))
                {
                    UnityEngine.Object.Destroy(v);
                    NoskShade.shades.Remove(v);
                    if (NoskShade.shades.Count < maxTotal.Value) break;
                }
                if (NoskShade.GetShadeCount() > maxTotal.Value)
                {
                    foreach (var v in NoskShade.shades.Take(NoskShade.GetShadeCount() - maxTotal.Value))
                    {
                        FSMUtility.SendEventToGameObject(v, "ZERO HP");
                    }
                }
            }
            NoskShade.Spawn(p, 12);
        }
    }
    [FsmState]
    private IEnumerator RaiseUp()
    {
        DefineEvent(FsmEvent.Finished, nameof(Idle));
        yield return StartActionContent;
        TendirsCalmDown();
        var pos = transform.position;
        var hpos = pos;
        hpos.y = raiseUpTo;
        var esec = 1 / raiseUpTime;
        var p = 0f;
        while (p < 1)
        {
            yield return null;
            p += esec * Time.deltaTime;
            p = Mathf.Clamp01(p);
            transform.position = Vector3.Lerp(pos, hpos, p);
        }
        TendirsNormal();
    }
}
