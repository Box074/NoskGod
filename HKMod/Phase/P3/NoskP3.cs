
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
        water = Instantiate(NoskGod.voidWaterPrefab, Vector3.zero, Quaternion.identity);
        water.SetActive(true);
        water.transform.position = new(94, 2, 0);
        water.GetComponent<VoidWaveMesh>().width = 100;

        AbyssWaterFsm.Attach(water, "Init");
        yield return null;
        

        yield return RoarPrepare();
        ac.PlayOneShot(NoskGod.mimic_spider_scream);
        var roar = DoRoar();
        Music_P3_Intro.TransitionTo(0, 0.75f);

        HeroController.instance.SetHazardRespawn(new(94.1745f, 7.4081f, 0.004f), true);

        PlayMakerFSM.BroadcastEvent("ABYSS WATER RAISE UP");

        

        yield return new WaitForSeconds(3.5f);
        
        Instantiate(NoskGod.platLagerPrefab, new(110.5f, -5), Quaternion.identity);
        Instantiate(NoskGod.platLagerPrefab, new(79.4f, -5), Quaternion.identity);

        dropVesselFsm.CleanUp(6.3f);

        foreach(var v in FindObjectsOfType<SpriteRenderer>())
        {
            var root = v.transform.root.name;
            if (root.Contains("Plat Lager")) continue;
            if(v.bounds.max.y < 6)
            {
                v.enabled = false;
            }
        }

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
        SetIgnorePlat(false);
        yield return "IDLE";
    }
    [FsmState]
    private IEnumerator P3S_Jump()
    {
        DefineEvent("JUMP", "Aim Jump");
        DefineEvent("CANCEL", nameof(P3Idle));
        yield return StartActionContent;
        spawnVesselOnLand = true;
        spawnShadeOnLand = false;
        farawayPlatform = false;
        rig.isKinematic = false;
        col.isTrigger = false;
        if(NoskShade.GetShadeCount() > 4) yield return "CANCEL";
        jumpCount.Value = UnityEngine.Random.Range(GetWithLevel(1, 2, 2), GetWithLevel(2, 4, 4));
        yield return "JUMP";
    }

    [FsmState]
    private IEnumerator P3_Charge()
    {
        DefineEvent("NEXT", "Charge Init");
        yield return StartActionContent;
        SetIgnorePlat(true);
        yield return "NEXT";
    }

    private void SetIgnorePlat(bool ignore)
    {
        var self = GetComponent<Collider2D>();
        foreach (var plat in FindObjectsOfType<PlatController>())
        {
            var col = plat.GetComponent<Collider2D>();

            Physics2D.IgnoreCollision(col, self, ignore);
        }
    }

    [FsmState]
    private IEnumerator P3Idle()
    {
        DefineEvent("ATTACK", nameof(P3AttackChoice));
        DefineEvent("LAST", nameof(PLIdle));
        yield return StartActionContent;
        if (isPhaseLast) yield return "LAST";
        yield return  "ATTACK";
    }
    
    
    [FsmState]
    private IEnumerator P3AttackChoice()
    {
        DefineEvent("JUMP", nameof(P3S_Jump));
        //DefineEvent("HIDE", nameof(P3S_Hide));
        DefineEvent("ROOF", "RS Jump Antic");
        DefineEvent("CHARGE", nameof(P3_Charge));
        DefineEvent("HIDE", nameof(P3S_Jump));
        DefineEvent("LAST", nameof(PLIdle));
        SendRandomEventV3 eventer = new();
        eventer.events = new[]
        {
            FsmEvent.GetFsmEvent("JUMP"),
            FsmEvent.GetFsmEvent("ROOF"),
            FsmEvent.GetFsmEvent("CHARGE")
        };
        eventer.weights = new FsmFloat[]
        {
            0.35f,
            0.15f,
            0.15f
        };
        eventer.trackingInts = new FsmInt[]
        {
            0,
            0,
            0
        };
        eventer.eventMax = new FsmInt[]{
            1,
            1,
            2
        };
        eventer.trackingIntsMissed = new FsmInt[]
        {
            0,
            0,
            0
        };
        eventer.missedMax = new FsmInt[]
        {
            5,
            8,
            3
        };
        yield return StartActionContent;
        if(isLastPhase)
        {
            yield return "LAST";
        }
        anim.Play("Idle");
        if(hm.hp < LastPhaseHP)
        {
            yield return "INTRO LAST PHASE";
        }
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.15f, 0.75f));
        yield return eventer;
    }
}
