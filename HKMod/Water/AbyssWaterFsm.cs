
namespace NoskGodMod;

class AbyssWaterFsm : CSFsm<AbyssWaterFsm>
{
    public float raiseUpTo = 5f;
    public float raiseUpTime = 3;
    public FsmInt spawnCount = 1;
    public FsmInt maxTotal = 100;
    private bool _init = false;

    [FsmState]
    private IEnumerator Init()
    {
        DefineEvent(FsmEvent.Finished, nameof(Idle));
        yield return StartActionContent;
        if (_init) yield return FsmEvent.Finished;
        gameObject.AddComponent<DestroyOnNoskDie>();
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
    }
}
