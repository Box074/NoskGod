
namespace NoskGodMod;

partial class NoskFsm : CSFsm<NoskFsm>
{
    protected override void OnAfterBindPlayMakerFSM()
    {
        Init();
    }
    protected override void OnBindPlayMakerFSM(PlayMakerFSM pm)
    {
        hm = pm.GetComponent<HealthManager>();
        base.OnBindPlayMakerFSM(pm);

        GameManager.instance.AudioManager.ApplyMusicCue(NoskGod.GGSadMusicCue, 0, 0, true);
        MusicUtils.TransitionTo(MusicMask.NoneExtraOrMainAltOrTension);

        hm.hp = CompileInfo.TEST_MODE ? 2550 : 3500;
        pm.Fsm.GetState("Land 2").InsertFsmStateAction<InvokeAction>(new(() =>
        {
            if (spawnVesselOnLand) PlayMakerFSM.BroadcastEvent("SPAWN RND");
            if (spawnShadeOnLand && waterFsm != null)
            {
                int c = UnityEngine.Random.Range(spawnShadeMin, spawnShadeMax);
                waterFsm.spawnCount = c;
                PlayMakerFSM.BroadcastEvent("ABYSS WATER SPAWN");
            }
        }), 0);
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
            var b = UnityEngine.Object.Instantiate(NoskGod.Bone, bones.transform);
            b.SetActive(true);
            b.transform.localPosition = Vector3.zero;
        }
        bones.SetActive(false);
    }

    private void MakeScene()
    {
        var bg = new GameObject("Bg");
        bg.AddComponent<SpriteRenderer>().sprite = NoskGod.bg01;
        bg.transform.position = new Vector3(78.1956f, 3.4769f, 3.8164f);
        bg.transform.localScale = new Vector3(3, 3, 3);

        UnityEngine.Object.Instantiate(bg, new Vector3(84.9119f, 4.3642f, 4.1728f), Quaternion.identity);
        UnityEngine.Object.Instantiate(bg, new Vector3(89.3991f, 5.5078f, 4.4055f), Quaternion.identity);
        UnityEngine.Object.Instantiate(bg, new Vector3(96.3463f, 6.2787f, 4.5873f), Quaternion.identity);
        UnityEngine.Object.Instantiate(bg, new Vector3(102.2135f, 5.8351f, 4.7073f), Quaternion.identity);
        UnityEngine.Object.Instantiate(bg, new Vector3(109.5589f, 4.0169f, 4.3818f), Quaternion.identity);
        UnityEngine.Object.Instantiate(bg, new Vector3(113.9788f, 4.0169f, 4.5073f), Quaternion.identity);
        UnityEngine.Object.Instantiate(bg, new Vector3(117.5806f, 5.0605f, 5.1f), Quaternion.identity);

        GameObject.Find("GG_Arena_Prefab/Crowd")?.SetActive(false);
        //GameObject.Find("GG_Arena_Prefab/BG")?.SetActive(false);
        GameObject.Find("GG_Arena_Prefab/Godseeker Crowd")?.SetActive(false);
        Destroy(GameObject.Find("GG_Arena_Prefab")?.GetComponent<AudioSource>());
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
        var roar = UnityEngine.Object.Instantiate(NoskGod.RoarEmitter, roarPoint.Value.transform);
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
        var deathEffect = pm.GetComponent<EnemyDeathEffects>();
        deathEffect.EmitCorpse(null, false, false);
        deathEffect.EmitLargeInfectedEffects();
        var corpse = Ref<GameObject>.From(ref deathEffect.private_corpse());
        Head = corpse.Value.FindChild("Head");
        corpse.Value = null!;
        deathEffect.PreInstantiate();
        deathEffect.private_enemyDeathType() = EnemyDeathTypes.Shade;
        UnityEngine.Object.Destroy(Head.Value.LocateMyFSM("Head Control"));
        hm.private_enemyDeathEffects() = null;
        //hm.OnDeath += () => pm.Fsm.ProcessEvent(FsmEvent.GetFsmEvent("NOSK DEAD"));
    }
    [FsmState]
    private IEnumerator TranP22()
    {
        DefineEvent("JUMP", "Rising");
        yield return StartActionContent;
        PlayerData.instance.disablePause = false;

        var arr = UnityEngine.Object.FindObjectsOfType<GameObject>(false).Where(x => x.name == "Abyss Drop Corpse").Take(8);
        foreach (var v in arr)
        {
            var sibling = NoskShade.Spawn(v.transform.position, 50, Head.Value);
            UnityEngine.Object.Destroy(sibling.FindChild("Alert Range"));
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
        var dream = pm.GetComponent<EnemyDreamnailReaction>();
        var col = pm.GetComponent<Collider2D>();
        DestroyImmediate(pm.GetComponent<SpriteFlash>());
        col.enabled = true;
        pm.gameObject.AddComponent<SpriteTweenColorNeutral>();
        Destroy(pm.GetComponent<InfectedEnemyEffects>());
        var hitEffect = pm.gameObject.AddComponent<EnemyHitEffectsShade>();
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

        hm.private_hitEffectReceiver() = hitEffect;
        hm.private_enemyType() = 6;

        dream.private_convoTitle() = "NOSK_VOID_DREAM_P2";
        dream.private_convoAmount() = 1; //TODO

        var propBlock = new MaterialPropertyBlock();
        propBlock.SetTexture("_MainTex", NoskGod.voidNoskTex);
        pm.GetComponent<MeshRenderer>().SetPropertyBlock(propBlock);

        pm.transform.position = origPos;
        pm.transform.SetPositionZ(5.2f);
        mesh.enabled = true;
        anim.Play("Jump");
        rig.isKinematic = false;
        rig.velocity = new Vector2(0, 55);

        Head.Value.SetActive(false);
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
                HeroController.instance.HeroDash();
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

        /*var blanker = FsmVariables.GlobalVariables.GetFsmGameObject("HUD Blanker").Value.LocateMyFSM("Blanker Control");
        FSMUtility.SetFloat(blanker, "Fade Time", 0.3f);
        blanker.SendEvent("FADE IN");*/
        PlayMakerFSM.BroadcastEvent("OUT");

        effect_color_add.Lerp(new(-1, -1, -1), 0.35f);

        yield return new WaitForSeconds(1.5f);

        HeroController.instance.gameObject.LocateMyFSM("Roar Lock").Fsm.GetFsmGameObject("Roar Object").Value = Head.Value;

        MakeScene();

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
            v.particleObject.SetActive(v.mapZone == GlobalEnums.MapZone.ABYSS);
        }
        GameCameras.instance.sceneParticles.defaultParticles.particleObject.SetActive(false);

        yield return null;

        //blanker.SendEvent("FADE OUT");
        effect_color_scale.Vector = new(0.9f, 0.9f, 0.9f, 1);
        effect_color_add.Lerp(Vector4.zero, 0.55f);


        while (HeroController.instance.controlReqlinquished) yield return null;
        PlayMakerFSM.BroadcastEvent("IN");
        yield return new WaitForSeconds(1.25f);

        yield return "TRAN FINISH";
    }

}

