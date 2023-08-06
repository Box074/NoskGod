
namespace NoskGodMod;

partial class NoskFsm : CSFsm<NoskFsm>
{
    protected override void OnAfterBindPlayMakerFSM()
    {
        Init();
    }
    [FsmState("Land 2")]
    private IEnumerator Land2()
    {
        var orig = OriginalActions;
        DefineEvent("ORIG FINISHED", "Roof Jump?");
        DefineEvent("JUMP CHECK", nameof(JumpCheck));
        yield return StartActionContent;
        if (nextStateAfterLand != null)
        {
            pm.Fsm.SetState(nextStateAfterLand);
            Debug.Log($"NextStateAfterLand: {nextStateAfterLand}");
            nextStateAfterLand = null;
            yield break;
        }
        col.isTrigger = false;
        if (spawnVesselOnLand) PlayMakerFSM.BroadcastEvent("SPAWN RND");

        yield return orig;
        InvokeActions(orig);
        if(isPhase2)
        {
            yield return "JUMP CHECK";
        }
        yield return "ORIG FINISHED";
    }
    protected override void OnBindPlayMakerFSM(PlayMakerFSM pm)
    {
        hm = pm.GetComponent<HealthManagerR>();
        base.OnBindPlayMakerFSM(pm);

        

        IEnumerator SetBGM()
        {
            yield return new WaitForSeconds(0.5f);
            GameManager.instance.AudioManager.ApplyMusicCue(NoskGod.GGSadMusicCue, 0, 0, true);
            MusicUtils.TransitionTo(MusicMask.MainAndSub);
        }

        SetBGM().StartCoroutine();

        hm.hp = 2255;
        pm.Fsm.GetState("Land 2").InsertFsmStateAction<InvokeAction>(new(() =>
        {

        }), 0);
        pm.Fsm.GetState("Launch").AppendFsmStateAction<InvokeAction>(new(() =>
        {
            rig.isKinematic = false;
        }));
        pm.Fsm.GetState("Aim Jump").ForEachFsmStateAction<RandomFloat>(x => new InvokeAction(() =>
        {
            if (nextEnterP2 == 0)
            {
                TargetX.Value = 115.35f;
            }
            var hpos = HeroController.instance.transform.position.x;
            while (true)
            {
                var tx = UnityEngine.Random.Range(JumpMinX.Value, JumpMaxX.Value);
                if (farawayHero && Mathf.Abs(tx - hpos) < 4) continue;
                if (farawayPlatform && (tx > PlatformLeftX && tx < PlatformRightX)) continue;
                TargetX.Value = tx;
                break;
            }
        }));
        pm.Fsm.GetState("Roof Jump?").InsertFsmStateAction<InvokeAction>(new(() =>
        {
            if (nextEnterP2 == 1)
            {
                pm.Fsm.SetState(nameof(EnterP2));
                nextEnterP2 = 2;
            }
        }), 0);
        pm.Fsm.GetState("Falling").InsertFsmStateAction<InvokeAction>(new(() =>
        {
            pm.transform.SetPositionZ(0.04f);
        }), 0);

        bones.transform.parent = pm.transform;
        bones.transform.localPosition = Vector3.zero;
        for (int i = 0; i < 5; i++)
        {
            var b = Instantiate(NoskGod.Bone, bones.transform);
            b.SetActive(true);
            b.transform.localPosition = Vector3.zero;
        }
        bones.SetActive(false);
    }



    [FsmState]
    private IEnumerator TranP1()
    {
        DefineGlobalEvent("NOSK START TRAN -1");
        DefineEvent("TRAN FINISHED", "Idle");
        yield return StartActionContent;
        nextEnterP2 = -1;
        spawnVesselOnLand = true;
        var anim = pm.GetComponent<tk2dSpriteAnimator>();
        yield return anim.PlayAnimWait("Roar Init");
        anim.Play("Roar Loop");
        pm.GetComponent<AudioSource>().PlayOneShot(NoskGod.mimic_spider_scream);
        var roar = Instantiate(NoskGod.RoarEmitter, roarPoint.Value.transform);
        roar.transform.localPosition = Vector3.zero;
        HeroController.instance.gameObject.LocateMyFSM("Roar Lock").Fsm.GetFsmGameObject("Roar Object").Value = pm.gameObject;
        PlayMakerFSM.BroadcastEvent("ROAR ENTER");
        FSMUtility.SendEventToGameObject(CameraParent.Value, "BigShake");
        FSMUtility.SetBool(GameCameras.instance.cameraShakeFSM, "RumblingBig", true);
        yield return new WaitForSeconds(0.75f);
        PlayMakerFSM.BroadcastEvent("SPAWN RND");
        yield return new WaitForSeconds(2.5f);
        FSMUtility.SetBool(GameCameras.instance.cameraShakeFSM, "RumblingBig", false);
        FSMUtility.SendEventToGameObject(roar, "END");
        PlayMakerFSM.BroadcastEvent("ROAR EXIT");
        IsTranPhase = false;
        yield return "TRAN FINISHED";
    }
    [FsmState]
    private IEnumerator FakeDie()
    {
        DefineGlobalEvent("NOSK START TRAN");
        DefineEvent("FINISHED", nameof(TranP21));
        yield return StartActionContent;
        nextEnterP2 = 0;
        PlayerData.instance.disablePause = true;
        var deathEffect = pm.GetComponent<EnemyDeathEffectsR>();
        deathEffect.EmitCorpse(null, false, false);
        deathEffect.EmitLargeInfectedEffects();
        var corpse = deathEffect.corpse;
        Head = corpse.FindChild("Head");
        deathEffect.corpse = null;
        deathEffect.PreInstantiate();
        deathEffect.enemyDeathType = EnemyDeathTypesR.Shade;
        Destroy(Head.Value.LocateMyFSM("Head Control"));
        hm.enemyDeathEffects = null;
        //hm.OnDeath += () => pm.Fsm.ProcessEvent(FsmEvent.GetFsmEvent("NOSK DEAD"));
    }
    [FsmState]
    private IEnumerator TranP22()
    {
        DefineEvent("JUMP", "Rising");
        yield return StartActionContent;
        PlayerData.instance.disablePause = false;

        var arr = FindObjectsOfType<GameObject>(false).Where(x => x.name == "Abyss Drop Corpse").Take(8);
        foreach (var v in arr)
        {
            var sibling = NoskShade.Spawn(v.transform.position, 50, Head.Value);
            Destroy(sibling.FindChild("Alert Range"));
            sibling.SetActive(true);
            //ctrl.Fsm.SetState("Pause");
        }
        //GameManager.instance.AudioManager.ApplyMusicSnapshot(NoskGod.AM_Main, 0, 0.5f);
        Music_FakeDie_P2.TransitionTo(0, 1.75f);
        while (NoskShade.GetShadeCount() > 0) yield return null;


        yield return new WaitForSeconds(1.5f);

        FSMUtility.SendEventToGameObject(CameraParent.Value, "BigShake");
        FSMUtility.SetBool(GameCameras.instance.cameraShakeFSM, "RumblingBig", true);
        PlayMakerFSM.BroadcastEvent("ROAR ENTER");
        yield return new WaitForSeconds(3.5f);


        FSMUtility.SetBool(GameCameras.instance.cameraShakeFSM, "RumblingBig", false);

        

        var mesh = pm.GetComponent<MeshRenderer>();
        var rig = pm.GetComponent<Rigidbody2D>();
        var anim = pm.GetComponent<tk2dSpriteAnimator>();
        var dream = pm.GetComponent<EnemyDreamnailReactionR>();
        var col = pm.GetComponent<Collider2D>();
        DestroyImmediate(pm.GetComponent<SpriteFlash>());
        col.enabled = true;
        pm.gameObject.AddComponent<SpriteTweenColorNeutral>();
        Destroy(pm.GetComponent<InfectedEnemyEffects>());
        var hitEffect = pm.gameObject.AddComponent<EnemyHitEffectsShadeR>();
        hitEffect.audioPlayerPrefab = NoskGod.AudioPlayerActor;
        hitEffect.heroDamage = new()
        {
            PitchMax = 1,
            PitchMin = 1,
            Volume = 1,
            Clip = NoskGod.hero_damage
        };
        hitEffect.hollowShadeStartled = new()
        {
            PitchMax = 1,
            PitchMin = 1,
            Volume = 1,
            Clip = NoskGod.hollow_shade_startle
        };
        hitEffect.hitShade = NoskGod.HitShade;
        hitEffect.hitFlashBlack = NoskGod.HitFlashBlack;
        hitEffect.slashEffectGhostDark1 = NoskGod.SlashEffectGhostDark1;
        hitEffect.slashEffectGhostDark2 = NoskGod.SlashEffectGhostDark2;
        hitEffect.slashEffectShade = NoskGod.SlashEffectShade;

        hm.hitEffectReceiver = hitEffect;
        hm.enemyType = 6;

        dream.convoTitle = "NOSK_VOID_DREAM_P2";
        dream.convoAmount = 1; //TODO

        var propBlock = new MaterialPropertyBlock();

        propBlock.SetTexture("_MainTex", NoskGod.voidNoskTex);
        pm.GetComponent<MeshRenderer>().SetPropertyBlock(propBlock);

        pm.transform.position = origPos;
        pm.transform.SetPositionZ(5.2f);
        mesh.enabled = true;
        anim.Play("Jump");
        rig.isKinematic = false;
        rig.velocity = new Vector2(0, 55);

        
        bones.gameObject.SetActive(true);
        bones.transform.SetPositionZ(0);
        bones.GetComponent<ParticleSystem>().Play(true);

        var hpos = HeroController.instance.transform.position;

        HeroController.instance.SetDamageMode(DamageMode.NO_DAMAGE);
        yield return new WaitForFixedUpdate();
        var cpos = GetComponent<BoxCollider2D>().bounds.center;
        if (Mathf.Abs(hpos.x - cpos.x) < 4)
        {
            bool shouldBackdash = (hpos.x > cpos.x) ? !HeroController.instance.cState.facingRight :
                HeroController.instance.cState.facingRight;
            PlayMakerFSM.BroadcastEvent("ROAR EXIT");
            yield return null;
            if (shouldBackdash)
            {
                NoskGod.HeroBackDash();
            }
            else
            {
                HeroControllerR.instance.HeroDash();
            }
        }
        //GameManager.instance.AudioManager.ApplyMusicSnapshot(NoskGod.AM_ActionSub, 0, 0.5f);
        yield return "JUMP";
    }
    [FsmState]
    private IEnumerator TranP21()
    {
        DefineEvent("TRAN FINISH", nameof(TranP22));
        yield return StartActionContent;
        nextEnterP2 = 1;
        origPos = pm.transform.position;
        var rig = pm.GetComponent<Rigidbody2D>();
        rig.velocity = Vector2.zero;
        rig.isKinematic = true;
        rig.position = new Vector2(9999, 9999);

        while (!Head.Value.activeInHierarchy) yield return null;

        PlayerData.instance.disablePause = true;
        Music_FakeDie.TransitionTo();
        //MusicUtils.TransitionTo(MusicMask.MainOnly);
        //GameManager.instance.AudioManager.ApplyMusicSnapshot(NoskGod.AM_Sub, 0, 0.5f);
        HeroController.instance.gameObject.LocateMyFSM("Roar Lock").Fsm.GetFsmGameObject("Roar Object").Value = Head.Value;
        PlayMakerFSM.BroadcastEvent("ROAR ENTER");

        FSMUtility.SendEventToGameObject(CameraParent.Value, "BigShake");
        FSMUtility.SetBool(GameCameras.instance.cameraShakeFSM, "RumblingBig", true);
        yield return new WaitForSeconds(3.5f);
        PlayMakerFSM.BroadcastEvent("NOSK VESSEL SPAWN TRAN");

        yield return new WaitForSeconds(5.5f);

        var blanker = FsmVariables.GlobalVariables.GetFsmGameObject("HUD Blanker").Value.LocateMyFSM("Blanker Control");
        FSMUtility.SetFloat(blanker, "Fade Time", 0.3f);
        blanker.SendEvent("FADE IN");
        PlayMakerFSM.BroadcastEvent("OUT");


        yield return new WaitForSeconds(1.5f);

        HeroController.instance.gameObject.LocateMyFSM("Roar Lock").Fsm.GetFsmGameObject("Roar Object").Value = Head.Value;

        MakeScene();

        Head.Value.SetActive(false);

        FSMUtility.SetBool(GameCameras.instance.cameraShakeFSM, "RumblingBig", false);
        Music_FakeDie_Dark.TransitionTo(0, 1.5f);
        yield return new WaitForSeconds(3.75f);
        while (!HeroController.instance.cState.onGround) yield return null;

        PlayMakerFSM.BroadcastEvent("NOSK VESSEL SPAWN STOP");
        PlayMakerFSM.BroadcastEvent("ROAR EXIT");
        HeroController.instance.RelinquishControlNotVelocity();
        HeroController.instance.StopAnimationControl();
        HeroController.instance.gameObject.LocateMyFSM("Dream Return").SetState("Land");
        foreach (var v in GameCameras.instance.sceneParticles.sceneParticles)
        {
            v.particleObject.SetActive(v.mapZone == MapZone.ABYSS);
        }
        GameCameras.instance.sceneParticles.defaultParticles.particleObject.SetActive(false);

        foreach(var v in new string[] { "Spit 1", "Spit 2", "Spit 3", "Spit 4", "Spit 5" })
        {
            var s = FsmComponent.Fsm.GetState(v);
            s.ForEachFsmStateAction<FlingObjectsFromGlobalPoolVel>(a =>
            {
                a.gameObject = NoskGod.VomitGlobNosk;
                return a;
            });
            s.ForEachFsmStateAction<FlingObjectsFromGlobalPool>(a =>
            {
                a.gameObject = NoskGod.VomitGlobNosk;
                return a;
            });
        }

        yield return null;

        blanker.SendEvent("FADE OUT");

        while (HeroController.instance.controlReqlinquished) yield return null;
        PlayMakerFSM.BroadcastEvent("IN");
        yield return new WaitForSeconds(1.25f);

        yield return "TRAN FINISH";
    }

}

