

namespace NoskGodMod;

partial class NoskPhaseCheck : CSFsm<NoskPhaseCheck>
{
    private PlayMakerFSM pm => FsmComponent!;
    private HealthManager hm = null!;
    public NoskFsm mainFsm = null!;
    [FsmState]
    private IEnumerator Init()
    {
        DefineEvent(FsmEvent.Finished, nameof(Idle));
        yield return StartActionContent;
        hm = pm.GetComponent<HealthManager>();
        mainFsm = pm.gameObject.GetComponent<NoskFsm>();
    }
    [FsmState]
    private IEnumerator Idle()
    {
        DefineEvent("TOOK DAMAGE", nameof(TookDamage));
        DefineEvent("EXTRA DAMAGED", nameof(TookDamage));
        yield return StartActionContent;
        yield return null;
    }
    [FsmState]
    private IEnumerator LastIdle()
    {
        DefineEvent("NAIL HIT", nameof(NailHit));
        yield return StartActionContent;
        hm.hp = 9999;
        yield return null;
    }
    [FsmState]
    private IEnumerator NailHit()
    {
        DefineEvent("CANCEL", nameof(LastIdle));
        yield return StartActionContent;
        var b = mainFsm.col.bounds;
        if(b.min.y < 9f || HeroControllerR.instance.col2d.bounds.min.y < 9f || !mainFsm.isPhaseLast)
        {
            yield return "CANCEL";
        }
        FSMUtility.SendEventToGameObject(gameObject, "NOSK DEATH");
    }
    [FsmState]
    private IEnumerator TookDamage()
    {
        DefineEvent(FsmEvent.Finished, nameof(Idle));
        DefineEvent("LAST", nameof(LastIdle));
        yield return StartActionContent;
        if(mainFsm.IsTranPhase) yield return FsmEvent.Finished;

        if (hm.hp < mainFsm.Phase1HP && mainFsm.nextEnterP2 == -2)
        {
            mainFsm.IsTranPhase = true;
            mainFsm.nextEnterP2 = -1;
            hm.hp = mainFsm.Phase1HP;
            FSMUtility.SendEventToGameObject(pm.gameObject, "NOSK START TRAN -1");
            yield return FsmEvent.Finished;
        }
        if (hm.hp < mainFsm.Phase2HP && !mainFsm.isVoid.Value && mainFsm.nextEnterP2 == -1)
        {
            mainFsm.IsTranPhase = true;
            mainFsm.nextEnterP2 = 0;
            hm.hp = mainFsm.Phase2HP;
            FSMUtility.SendEventToGameObject(pm.gameObject, "NOSK START TRAN");
            yield return FsmEvent.Finished;
        }
        if(hm.hp <= mainFsm.Phase3HP && !mainFsm.isPhase3)
        {
            mainFsm.IsTranPhase = true;
            hm.hp = mainFsm.Phase3HP;
            FSMUtility.SendEventToGameObject(pm.gameObject, "NOSK START TRAN 3");
            yield return FsmEvent.Finished;
        }
        if (hm.hp < mainFsm.LastPhaseHP && !mainFsm.isPhaseLast)
        {
            FSMUtility.SendEventToGameObject(pm.gameObject, "INTRO LAST PHASE");
            yield return FsmEvent.Finished;
        }
        if (hm.hp < mainFsm.DeathHP && mainFsm.isPhaseLast)
        {
            yield return "LAST";
        }
    }
}
