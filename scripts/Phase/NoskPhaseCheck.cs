

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
        yield return StartActionContent;
        yield return null;
    }
    [FsmState]
    private IEnumerator TookDamage()
    {
        DefineEvent(FsmEvent.Finished, nameof(Idle));
        yield return StartActionContent;
        if(mainFsm.IsTranPhase) yield return FsmEvent.Finished;

        if (hm.hp <= mainFsm.Phase1HP && mainFsm.nextEnterP2 == -2)
        {
            mainFsm.IsTranPhase = true;
            mainFsm.nextEnterP2 = -1;
            FSMUtility.SendEventToGameObject(pm.gameObject, "NOSK START TRAN -1");
        }
        if (hm.hp <= mainFsm.Phase2HP && !mainFsm.isVoid.Value && mainFsm.nextEnterP2 == -1)
        {
            mainFsm.IsTranPhase = true;
            mainFsm.nextEnterP2 = 0;
            FSMUtility.SendEventToGameObject(pm.gameObject, "NOSK START TRAN");
        }
        if(hm.hp <= mainFsm.Phase3HP && !mainFsm.isPhase3)
        {
            mainFsm.IsTranPhase = true;
            FSMUtility.SendEventToGameObject(pm.gameObject, "NOSK START TRAN 3");
        }
    }
}
