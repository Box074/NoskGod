
namespace NoskGodMod;

partial class NoskFsm : CSFsm<NoskFsm>
{
    [FsmVar]
    private FsmInt jumpCount = new();
    
    [FsmState]
    private IEnumerator P2Idle()
    {
        DefineEvent("ROOF", "RS Jump Antic");
        DefineEvent("JUMP", "Aim Jump");
        FsmBool isFirst = true;
        yield return StartActionContent;
        if(!isFirst.Value) yield return new WaitForSeconds(UnityEngine.Random.Range(0.15f, 1.5f));
        isFirst.Value = false;
        var v = UnityEngine.Random.value;
        if(v <= 0.35f)
        {
            yield return "ROOF";
        }
        else
        {
            jumpCount.Value = UnityEngine.Random.Range(2, 3);
            yield return "JUMP";
        }
    }
    [FsmState]
    private IEnumerator JumpCheck()
    {
        DefineEvent("CONTINUE", "Aim Jump");
        DefineEvent("CANCEL", nameof(P2Idle));
        yield return StartActionContent;
        FSMUtility.SendEventToGameObject(FsmComponent!.gameObject, "SPAWN RNG");
        if(jumpCount.Value-- <= 0) yield return "CANCEL";
        yield return "CONTINUE";
    }
    [FsmState]
    private IEnumerator P2Init()
    {
        DefineEvent("FINISHED", nameof(P2Idle));
        yield return StartActionContent;
        FSMUtility.SendEventToGameObject(FsmComponent!.gameObject, "NOSK VESSEL SPAWN STOP");
        FsmComponent!.Fsm.GetState("Roof Drop").GetFSMStateActionOnState<FlingObjectsFromGlobalPoolTime>().gameObject = NoskGod.VomitGlobNosk;
        FsmComponent!.Fsm.GetState("Land 2").AppendFsmStateAction<InvokeAction>(new(a =>
        {
            a.Fsm.SetState(nameof(JumpCheck));
        }));
        FSMUtility.SetBool(FsmComponent!.gameObject.LocateMyFSM("DropVesselFsm"), "SpawnShade", true);
    }
    [FsmState]
    private IEnumerator NoskDeath()
    {
        DefineGlobalEvent("NOSK DEAD");
        yield return StartActionContent;
        foreach(var v in UnityEngine.Object.FindObjectsOfType<GameObject>().Where(x => x.name == "Nosk Shade")) FSMUtility.SendEventToGameObject(v, "ZERO HP");
    }
}
