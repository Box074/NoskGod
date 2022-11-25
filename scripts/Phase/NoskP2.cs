
namespace NoskGodMod;

partial class NoskFsm : CSFsm<NoskFsm>
{
    private float enterP2Time;
    [FsmState]
    private IEnumerator P2Idle()
    {
        DefineEvent("ROOF", "RS Jump Antic");
        DefineEvent("JUMP", "Aim Jump");
        DefineEvent("IS P3", nameof(P3Idle));
        
        FsmBool isFirst = true;
        yield return StartActionContent;
        if (isPhase3) yield return "IS P3";
        if (Time.time - enterP2Time > TimeP2)
        {
            IsTranPhase = true;
            yield return "NOSK START TRAN 3";
        }
        //if(!isFirst.Value) yield return new WaitForSeconds(UnityEngine.Random.Range(0.15f, 1.5f));
        isFirst.Value = false;
        var v = UnityEngine.Random.value;
        if (v <= 0.35f)
        {
            yield return "ROOF";
        }
        else
        {
            jumpCount.Value = UnityEngine.Random.Range(4, 5);
            yield return "JUMP";
        }
    }
    [FsmState]
    private IEnumerator JumpCheck()
    {
        DefineEvent("CONTINUE", "Aim Jump");
        DefineEvent("CANCEL", nameof(P2Idle));
        DefineEvent("LAST CANCEL", nameof(PLIdle));
        yield return StartActionContent;
        if (spawnVesselOnLand) PlayMakerFSM.BroadcastEvent("SPAWN RND");
        if (spawnShadeOnLand && waterFsm != null)
        {
            int c = UnityEngine.Random.Range(1, 3);
            waterFsm.spawnCount = c;
            PlayMakerFSM.BroadcastEvent("ABYSS WATER SPAWN");
        }
        if (jumpCount.Value-- <= 0) yield return isPhaseLast ? "LAST CANCEL" : "CANCEL";
        yield return "CONTINUE";
    }
    [FsmState]
    private IEnumerator EnterP2()
    {
        DefineEvent("END", nameof(P2Init));
        yield return StartActionContent;
        HeroController.instance.SetDamageMode(DamageMode.FULL_DAMAGE);
        yield return RoarPrepare();
        
        var roar = DoRoar(false);
        PlayMakerFSM.BroadcastEvent("ROAR ENTER");
        Music_P2_Roar.TransitionTo();
        yield return new WaitForSeconds(3.5f);

        yield return RoarEnd(roar);

        yield return "END";
    }
    [FsmState]
    private IEnumerator P2Init()
    {
        DefineEvent("FINISHED", nameof(P2Idle));
        yield return StartActionContent;
        FSMUtility.SendEventToGameObject(pm.gameObject, "NOSK VESSEL SPAWN STOP");
        pm.Fsm.GetState("Roof Drop").GetFSMStateActionOnState<FlingObjectsFromGlobalPoolTime>().gameObject = NoskGod.VomitGlobNosk;
        dropVesselFsm.SpawnShade.Value = true;
        IsTranPhase = false;
        enterP2Time = Time.time;
        isPhase2 = true;
    }
    [FsmState]
    private IEnumerator NoskDeath()
    {
        DefineGlobalEvent("NOSK DEAD");
        yield return StartActionContent;
        _ = NoskShade.GetShadeCount();
        foreach (var v in NoskShade.shades) FSMUtility.SendEventToGameObject(v, "ZERO HP");
    }
}
