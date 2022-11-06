
namespace NoskGodMod;

partial class NoskFsm : CSFsm<NoskFsm>
{
    public int pl_state = 0;
    public float? pl_lastTime = null;
    public float? pl_waitTime = null;
    public bool pl_last = false;
    public PlayMakerFSM dreamMsgCtrl = null!;
    [FsmState]
    private IEnumerator PLIntro()
    {
        DefineGlobalEvent("INTRO LAST PHASE");
        DefineEvent("IDLE", nameof(PLIdle));
        yield return StartActionContent;
        pl_lastTime = null;
        dreamMsgCtrl = dreamMsg.Value.LocateMyFSM("Display");
        isPhaseLast = true;
        spawnShadeMax = 5;
        spawnShadeMin = 1;
        farawayHero = true;
        waterFsm.maxTotal = 8;
        waterFsm.TendirsAngry();

        //GameManager.instance.AudioManager.ApplyMusicSnapshot(NoskGod.AM_Main, 0, 0.75f);
        Music_PL_Start.TransitionTo();
        yield return "IDLE";
    }
    [FsmState]
    private IEnumerator PLDreamMsg()
    {
        DefineEvent("JUMP", "Aim Jump");
        yield return StartActionContent;
        pl_state++;

        var tk = "NOSK_VOID_DREAM_PL_" + pl_state;
        var r = Language.Language.Get(tk, "UI");
        if(string.IsNullOrWhiteSpace(r) || pl_state > 7)
        {
            pl_waitTime = Time.time;
            yield return "JUMP";
        }
        if(r == "PLACEHOLDER")
        {
            tk = "NOSK_VOID_DREAM_EMPTY";
        }
        dreamMsgCtrl.FsmVariables.GetFsmString("Sheet").Value = "UI";
        dreamMsgCtrl.FsmVariables.GetFsmString("Convo Title").Value = tk;
        dreamMsgCtrl.SendEvent("DISPLAY DREAM MSG ALT");
        yield return "JUMP";
    }

    private static GameObject? pl_pf_deathWave;
    
    [FsmState]
    private IEnumerator PL_LastLand()
    {
        //FIXME: should use shade death effect
        yield return StartActionContent;
        GetComponent<DamageHero>().damageDealt = 0;
        yield return RoarPrepare();
        var roar = DoRoar();
        
        NoskShade.KillAll(gameObject);
        
        PlayMakerFSM.BroadcastEvent("ROAR ENTER");
        NoskShade.onSpawn += shade =>
        {
            var ctrl = shade.LocateMyFSM("Control");
            ctrl.Fsm.GetState("Idle").FindFsmStateTransition("ALERT").ToFsmState = ctrl.Fsm.GetState("Unform");
        };
        yield return new WaitForSeconds(1.5f);
        dropVesselFsm.returnSelf =  true;
        PlayMakerFSM.BroadcastEvent("NOSK VESSEL SPAWN TRAN");
        FSMUtility.SendEventToGameObject(CameraParent.Value, "BigShake");
        FSMUtility.SetBool(GameCameras.instance.cameraShakeFSM, "RumblingBig", true);
        yield return new WaitForSeconds(8);
        dropVesselFsm.returnSelf =  false;
        dropVesselFsm.SpawnShade.Value = false;
        //while(NoskShade.GetShadeCount() > 0) yield return null;
        
        
        PlayMakerFSM.BroadcastEvent("ROAR EXIT");
        FSMUtility.SendEventToGameObject(roar, "END");

        yield return new WaitForSeconds(0.55f);
        foreach(var v in NoskShade.shades)
        {
            if(v != null) Destroy(v);
        }
        NoskShade.shades.Clear();

        dreamMsgCtrl.FsmVariables.GetFsmString("Sheet").Value = "UI";
        dreamMsgCtrl.FsmVariables.GetFsmString("Convo Title").Value = "NOSK_VOID_DREAM_PL_L";
        dreamMsgCtrl.SendEvent("DISPLAY DREAM MSG ALT");
        
        for(int i = 0; i < 3 ; i++) Instantiate(NoskGod.HollowShadeDeathWave, transform.position + new Vector3(0.1145f, 1.8617f), Quaternion.identity).Play();
        Instantiate(NoskGod.DeathExplodeBoss, transform.position + new Vector3(0.1145f, 1.8617f), Quaternion.identity);
        //Instantiate(NoskGod.HollowShadeDepartParticles, transform.position + new Vector3(0.1145f, 1.8617f), Quaternion.identity).Play();
        waterFsm.TendirsCalmDown();
        
        NoskShade.ClearOnSpawn();
        transform.position = new(99999,99999, 0);
        GetComponent<Rigidbody2D>().isKinematic = true;
        PlayMakerFSM.BroadcastEvent("NOSK VESSEL SPAWN STOP");
        HeroController.instance.SetDamageMode(DamageMode.FULL_DAMAGE);
        FSMUtility.SetBool(GameCameras.instance.cameraShakeFSM, "RumblingBig", false);
        Music_PL_Die.TransitionTo(0, 0.45f);
        //GameManager.instance.AudioManager.ApplyMusicCue(NoskGod.AbyssMusic, 0, 2.5f, false);
        yield return new WaitForSeconds(1.5f);
        if(!DebugManager.IsDebug(NoskGod.Instance))
        {
            BossSceneController.Instance.EndBossScene();
        }
    }
    [FsmState]
    private IEnumerator PLIdle()
    {
        DefineEvent("SHOW", nameof(PLDreamMsg));
        DefineEvent("JUMP", "Aim Jump");
        DefineEvent("LAST LAND", nameof(PL_LastLand));
        yield return StartActionContent;
        if(pl_last)
        {
            yield return "LAST LAND";
        }
        jumpCount.Value = UnityEngine.Random.Range(1, 3);
        hm.hp = 9999999;
        if(dreamMsgCtrl.ActiveStateName == "Idle" && (Time.time - (pl_waitTime ?? 0)) > 1)
        {
            pl_waitTime = null;
            if(pl_lastTime is null)
            {
                pl_lastTime = Time.time;
            }
            if(Time.time - pl_lastTime.Value >= 1.75f)
            {
                pl_lastTime = null;
                yield return "SHOW";
            }
        }
        var hpos = HeroController.instance.transform.position;
        if(pl_state == 7 && (hpos.x > PlatformLeftX && hpos.x < PlatformRightX && HeroController.instance.cState.onGround))
        {
            HeroController.instance.SetDamageMode(DamageMode.NO_DAMAGE);
            jumpCount.Value = 0;
            jumpDistance.Value = 97 - transform.position.x;
            pl_last = true;
            if(jumpDistance.Value > 0)
            {
                FsmComponent.SetState("Face R");
            }
            else
            {
                FsmComponent.SetState("Face L");
            }
            
        }
        yield return "JUMP";
    }
}
