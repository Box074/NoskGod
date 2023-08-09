
namespace NoskGodMod;

partial class NoskFsm : CSFsm<NoskFsm>
{
    [FsmState]
    private IEnumerator PLIntro()
    {
        DefineGlobalEvent("INTRO LAST PHASE");
        DefineEvent("INTRO FINISHED", nameof(PLIdle));
        yield return StartActionContent;

        NoskShade.KillAll(gameObject);

        Music_PL_Start.TransitionTo(0, 2f);
        yield return RoarPrepare();
        ac.PlayOneShot(NoskGod.mimic_spider_scream);
        var roar = DoRoar();

        yield return new WaitForSeconds(1.5f);

        foreach(var v in FindObjectsOfType<CameraLockAreaR>())
        {
            v.cameraYMax += 4;
        }

        var camCtrl = GameCamerasR.instance.cameraController;
        var la = camCtrl.currentLockArea;
        if(la != null)
        {
            camCtrl.ReleaseLock(la);
            camCtrl.LockToArea(la);
        }

        water.GetComponent<AbyssWaterFsm>().raiseUpTo = 9;

        PlayMakerFSM.BroadcastEvent("ABYSS WATER RAISE UP");

        hm.hp = LastPhaseHP;
        hm.isDead = false;

        yield return new WaitForSeconds(3.5f);

        Instantiate(NoskGod.platLagerPrefab, new(94.7f, -5), Quaternion.identity);

        foreach (var v in FindObjectsOfType<SpriteRenderer>())
        {
            var root = v.transform.root.name;
            if (root.Contains("Plat Lager")) continue;
            if (v.bounds.max.y < 10)
            {
                v.enabled = false;
            }
        }

        yield return RoarEnd(roar);

        hm.IsInvincible = false;
        IsTranPhase = false;
        isPhaseLast = true;
        yield return "INTRO FINISHED";
    }



    [FsmState]
    private IEnumerator PLIdle()
    {
        DefineEvent("ATTACK", nameof(PLAttackChoice));
        yield return StartActionContent;
        SetIgnorePlat(false);
        spawnVesselOnLand = false;
        yield return "ATTACK";
    }
    [FsmVar("Will Spit")]
    private FsmBool willSpit = new();

    [FsmState]
    private IEnumerator PL_Charge()
    {
        DefineEvent("NEXT", "Charge Init");
        yield return StartActionContent;
        willSpit.Value = false;
        SetIgnorePlat(true);
        yield return "NEXT";
    }

    [FsmState]
    private IEnumerator PLAttackChoice()
    {
        DefineEvent("ROOF", "RS Jump Antic");
        DefineEvent("CHARGE", nameof(PL_Charge));
        DefineEvent("CANCEL", nameof(PLIdle));
        SendRandomEventV3 eventer = new();
        eventer.events = new[]
        {
            FsmEvent.GetFsmEvent("ROOF"),
            FsmEvent.GetFsmEvent("CHARGE")
        };
        eventer.weights = new FsmFloat[]
        {
            0.15f,
            0.15f
        };
        eventer.trackingInts = new FsmInt[]
        {
            0,
            0,
            
        };
        eventer.eventMax = new FsmInt[]{
            1,
            1,
            
        };
        eventer.trackingIntsMissed = new FsmInt[]
        {
            0,
            0,
            
        };
        eventer.missedMax = new FsmInt[]
        {
            3,
            2,
            
        };
        yield return StartActionContent;
        anim.Play("Idle");
        yield return new WaitForSeconds(Random.Range(0.15f, 0.75f));
        yield return eventer;
        yield return new WaitForSeconds(0.05f);
        yield return "CANCEL";
    }
}
