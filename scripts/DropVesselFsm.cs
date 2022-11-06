
namespace NoskGodMod;

class DropVesselFsm : CSFsm<DropVesselFsm>
{
    [FsmVar]
    private FsmFloat RoofY = new();
    [FsmVar]
    public FsmInt SpawnCount = new();
    [FsmVar]
    public FsmInt FallType = new();
    [FsmVar]
    public FsmBool SpawnShade = new();
    [FsmVar]
    public FsmInt SpawnMaxCount = 4;
    public bool returnSelf = false;
    public List<GameObject> dropCorpses = new();
    public void CleanUp(float minY)
    {
        foreach(var v in dropCorpses)
        {
            if(v == null) continue;
            if(v.GetComponent<MeshRenderer>().bounds.max.y < minY)
            {
                Destroy(v);
            }
        }
    }
    [FsmState]
    private IEnumerator Spawn()
    {
        DefineEvent("FINISHED", nameof(CheckCount));
        yield return StartActionContent;
        var x = UnityEngine.Random.Range(74, 114);
        var drop = UnityEngine.Object.Instantiate(NoskGod.CorpseSpawn, new Vector3(x, RoofY.Value, 0.0038f), Quaternion.identity);
        drop.name = "Abyss Drop Corpse";
        dropCorpses.Add(drop);
        if (SpawnShade.Value)
        {
            drop.LocateMyFSM("Control").Fsm.GetState("Land").AppendFsmStateAction<InvokeAction>(new(a =>
            {
                if((UnityEngine.Random.value >= 0.5f || NoskShade.GetShadeCount() > SpawnMaxCount.Value) && !returnSelf) return;
                NoskShade.Spawn(drop.transform.position, dieTarget: returnSelf ? gameObject : null);
            }));

        }
        drop.LocateMyFSM("Control").Fsm.GetState("Land").With(x => x.RemoveAllFsmStateActions<SendEventByName>());
        drop.transform.SetScaleX(UnityEngine.Random.Range(0, 100) <= 50 ? -1 : 1);
        drop.SetActive(true);
    }
    [FsmState]
    private IEnumerator SpawnTran()
    {
        DefineGlobalEvent("NOSK VESSEL SPAWN TRAN");
        DefineEvent("FINISHED", nameof(CheckCount));
        yield return StartActionContent;
        SpawnCount.Value = 99999;
        FallType.Value = 3;
    }
    [FsmState]
    private IEnumerator CheckCount()
    {
        DefineEvent("CANCEL", nameof(Idle));
        DefineEvent("SPAWN", nameof(Spawn));
        yield return StartActionContent;
        if (SpawnCount.Value >= 0)
        {
            SpawnCount.Value--;
            var waitTime = FallType.Value switch
            {
                0 => UnityEngine.Random.Range(0.01f, 0.25f),
                1 => UnityEngine.Random.Range(0.5f, 0.8f),
                2 => UnityEngine.Random.Range(1.5f, 2.8f),
                3 => UnityEngine.Random.Range(0.01f, 0.1f),
                _ => 0
            };
            if (waitTime > 0) yield return new WaitForSeconds(waitTime);
            yield return "SPAWN";
        }
        yield return "CANCEL";
    }
    [FsmState]
    private IEnumerator RndCount()
    {
        DefineEvent("FINISHED", nameof(CheckCount));
        yield return StartActionContent;
        SpawnCount.Value = UnityEngine.Random.Range(1, 3);
    }
    [FsmState]
    private IEnumerator Idle()
    {
        DefineGlobalEvent("NOSK VESSEL SPAWN STOP");
        DefineEvent("SPAWN RND", nameof(RndCount));
        DefineEvent("SPAWN", nameof(CheckCount));
        yield return StartActionContent;
        SpawnCount.Value = 0;
        FallType.Value = 0;
        //yield return "SPAWN";
    }
    [FsmState]
    private IEnumerator Init()
    {
        DefineEvent("FINISHED", nameof(Idle));
        yield return StartActionContent;
        RoofY.Value = 21.3f;
    }
}

