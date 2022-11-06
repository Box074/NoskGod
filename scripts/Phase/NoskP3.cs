
namespace NoskGodMod;

partial class NoskFsm : CSFsm<NoskFsm>
{
    public GameObject water = null!;
    [FsmState]
    private IEnumerator P3Intro()
    {
        DefineGlobalEvent("NOSK START TRAN 3");
        DefineEvent("INTRO FINISHED", nameof(P3Init));
        yield return StartActionContent;

        NoskShade.KillAll(gameObject);
        
        isPhase3 = true;
        hm.IsInvincible = true;
        water = Instantiate(NoskGod.AbyssWater, Vector3.zero, Quaternion.identity);
        water.SetActive(true);
        AbyssWaterFsm.Attach(water, out waterFsm, "Init");
        yield return null;
        

        yield return RoarPrepare();
        GetComponent<AudioSource>().PlayOneShot(NoskGod.mimic_spider_scream);
        var roar = DoRoar();
        Music_P3_Intro.TransitionTo(0, 0.75f);

        HeroController.instance.SetHazardRespawn(new(94.1745f, 7.4081f, 0.004f), true);
        waterFsm.raiseUpTo = 13.5f;
        PlayMakerFSM.BroadcastEvent("ABYSS WATER RAISE UP");

        yield return new WaitForSeconds(3.5f);

        dropVesselFsm.CleanUp(6.3f);

        yield return RoarEnd(roar);

        hm.IsInvincible = false;
        IsTranPhase = false;
        yield return "INTRO FINISHED";
    }
    [FsmState]
    private IEnumerator P3Init()
    {
        DefineEvent("IDLE", nameof(P3Idle));
        yield return StartActionContent;
        spawnVesselOnLand = false;
        spawnShadeOnLand = true;
        yield return "IDLE";
    }
    [FsmState]
    private IEnumerator P3S_Jump()
    {
        DefineEvent("JUMP", "Aim Jump");
        DefineEvent("CANCEL", nameof(P3Idle));
        yield return StartActionContent;
        spawnVesselOnLand = false;
        spawnShadeOnLand = true;
        if(NoskShade.GetShadeCount() > 4) yield return "CANCEL";
        jumpCount.Value = UnityEngine.Random.Range(1, 2);
        yield return "JUMP";
    }
    [FsmState]
    private IEnumerator P3S_Hide()
    {
        yield return StartActionContent;
        farawayPlatform = true;
    }
    [FsmState]
    private IEnumerator P3Idle()
    {
        DefineEvent("ATTACK", nameof(P3AttackChoice));
        yield return StartActionContent;
        yield return "ATTACK";
    }
    [FsmState]
    private IEnumerator P3AttackChoice()
    {
        DefineEvent("JUMP", nameof(P3S_Jump));
        SendRandomEventV3 eventer = new();
        eventer.events = new[]
        {
            FsmEvent.GetFsmEvent("JUMP")
        };
        eventer.weights = new FsmFloat[]
        {
            0.15f
        };
        eventer.trackingInts = new FsmInt[]
        {
            0
        };
        eventer.eventMax = new FsmInt[]{
            1
        };
        eventer.trackingIntsMissed = new FsmInt[]
        {
            0
        };
        eventer.missedMax = new FsmInt[]
        {
            5
        };
        yield return StartActionContent;
        anim.Play("Idle");
        if(hm.hp < LastPhaseHP)
        {
            yield return "INTRO LAST PHASE";
        }
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.15f, 0.75f));
        yield return eventer;
    }
}
