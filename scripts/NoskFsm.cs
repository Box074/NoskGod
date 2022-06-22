
namespace NoskGodMod;

partial class NoskFsm : CSFsm<NoskFsm>
{
    protected override void OnBindPlayMakerFSM(PlayMakerFSM pm)
    {
        base.OnBindPlayMakerFSM(pm);
        pm.gameObject.GetComponent<HealthManager>().hp = CompileInfo.TEST_MODE ? 2550 : 3500;
        pm.Fsm.GetState("Land 2").InsertFsmStateAction<InvokeAction>(new(() =>
        {
            if (nextEnterP2 > -2) PlayMakerFSM.BroadcastEvent("SPAWN RND");
        }), 0);
        pm.Fsm.GetState("Idle").InsertFsmStateAction<InvokeAction>(new(() =>
        {
            if (pm.GetComponent<HealthManager>().hp <= 3000 && nextEnterP2 == -2)
            {
                nextEnterP2 = -1;
                FSMUtility.SendEventToGameObject(pm.gameObject, "NOSK START TRAN -1");
                return;
            }
            if (pm.GetComponent<HealthManager>().hp <= 2500 && !isVoid.Value && nextEnterP2 == -1)
            {
                nextEnterP2 = 0;
                FSMUtility.SendEventToGameObject(pm.gameObject, "NOSK START TRAN");
            }
        }), 0);
        pm.Fsm.GetState("Aim Jump").InsertFsmStateAction<InvokeAction>(new(() =>
        {
            if (nextEnterP2 == 0)
            {
                TargetX.Value = 115.35f;
            }
        }), 2);
        pm.Fsm.GetState("Roof Jump?").InsertFsmStateAction<InvokeAction>(new(() =>
        {
            if (nextEnterP2 == 1)
            {
                pm.Fsm.SetState("EnterP2");
                nextEnterP2 = 2;
            }
        }), 0);
        pm.Fsm.GetState("Falling").InsertFsmStateAction<InvokeAction>(new(() =>
        {
            FsmComponent!.transform.SetPositionZ(0.04f);
        }), 0);

        bones.transform.parent = pm.transform;
        bones.transform.localPosition = Vector3.zero;
        for (int i = 0; i < 5; i++)
        {
            var b = UnityEngine.Object.Instantiate(NoskGod.Bone, bones.transform);
            b.transform.localPosition = Vector3.zero;
        }
    }
    private GameObject bones = UnityEngine.Object.Instantiate(NoskGod.Bone);
    [FsmVar]
    private FsmGameObject CameraParent = new();
    [FsmVar]
    private FsmBool isVoid = new();
    [FsmVar("Roar Point")]
    private FsmGameObject roarPoint = new();
    [FsmVar("Target X")]
    private FsmFloat TargetX = new();
    [FsmVar]
    private FsmGameObject Head = new();
    private Vector3 origPos;
    private int nextEnterP2 = -2;
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
    }
    [FsmState]
    private IEnumerator EnterP2()
    {
        DefineEvent("END", nameof(P2Init));
        yield return StartActionContent;

        var anim = FsmComponent!.GetComponent<tk2dSpriteAnimator>();
        yield return anim.PlayAnimWait("Roar Init");
        anim.Play("Roar Loop");
        FsmComponent!.GetComponent<AudioSource>().PlayOneShot(NoskGod.mimic_spider_scream);
        var roar = UnityEngine.Object.Instantiate(NoskGod.RoarEmitter, roarPoint.Value.transform);
        roar.transform.localPosition = Vector3.zero;
        HeroController.instance.gameObject.LocateMyFSM("Roar Lock").Fsm.GetFsmGameObject("Roar Object").Value = FsmComponent!.gameObject;
        PlayMakerFSM.BroadcastEvent("ROAR ENTER");

        yield return new WaitForSeconds(3.5f);

        FSMUtility.SendEventToGameObject(roar, "END");
        PlayMakerFSM.BroadcastEvent("ROAR EXIT");
        yield return anim.PlayAnimWait("Roar Finish");
        anim.Play("Idle");

        yield return "END";
    }
    [FsmState]
    private IEnumerator TranP1()
    {
        DefineGlobalEvent("NOSK START TRAN -1");
        DefineEvent("TRAN FINISHED", "Idle");
        yield return StartActionContent;
        nextEnterP2 = -1;
        var anim = FsmComponent!.GetComponent<tk2dSpriteAnimator>();
        yield return anim.PlayAnimWait("Roar Init");
        anim.Play("Roar Loop");
        FsmComponent!.GetComponent<AudioSource>().PlayOneShot(NoskGod.mimic_spider_scream);
        var roar = UnityEngine.Object.Instantiate(NoskGod.RoarEmitter, roarPoint.Value.transform);
        roar.transform.localPosition = Vector3.zero;
        HeroController.instance.gameObject.LocateMyFSM("Roar Lock").Fsm.GetFsmGameObject("Roar Object").Value = FsmComponent!.gameObject;
        PlayMakerFSM.BroadcastEvent("ROAR ENTER");
        FSMUtility.SendEventToGameObject(CameraParent.Value, "BigShake");
        FSMUtility.SetBool(GameCameras.instance.cameraShakeFSM, "RumblingBig", true);
        yield return new WaitForSeconds(0.75f);
        PlayMakerFSM.BroadcastEvent("SPAWN RND");
        yield return new WaitForSeconds(2.5f);
        FSMUtility.SetBool(GameCameras.instance.cameraShakeFSM, "RumblingBig", false);
        FSMUtility.SendEventToGameObject(roar, "END");
        PlayMakerFSM.BroadcastEvent("ROAR EXIT");
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
        var deathEffect = FsmComponent!.GetComponent<EnemyDeathEffects>();
        deathEffect.EmitCorpse(null, false, false);
        deathEffect.EmitLargeInfectedEffects();

        Head = deathEffect.private_corpse().FindChild("Head");
        UnityEngine.Object.Destroy(Head.Value.LocateMyFSM("Head Control"));
        var hm = FsmComponent!.GetComponent<HealthManager>();
        hm.private_enemyDeathEffects() = null;
        hm.OnDeath += () => FsmComponent!.Fsm.ProcessEvent(FsmEvent.GetFsmEvent("NOSK DEAD"));
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
            var sibling = UnityEngine.Object.Instantiate(NoskGod.Sibling);
            sibling.name = "Nosk Shade";
            sibling.transform.position = v.transform.position;
            UnityEngine.Object.Destroy(sibling.FindChild("Alert Range"));
            var ctrl = sibling.LocateMyFSM("Control");
            foreach (var v2 in ctrl.GetComponentsInChildren<Collider2D>()) v2.isTrigger = true;
            ctrl.Fsm.GetState("Friendly?").AppendFsmStateAction<InvokeAction>(new(() =>
            {
                ctrl.Fsm.SetState("Init");
                ctrl.GetComponent<HealthManager>().hp = 30;
                FSMUtility.SetBool(ctrl, "Friendly", false);
            }));
            ctrl.Fsm.GetState("Recycle").AppendFsmStateAction<InvokeAction>(new(() =>
            {
                UnityEngine.Object.Destroy(sibling);
            }));
            ctrl.Fsm.GetState("Idle").AppendFsmStateAction<InvokeAction>(new(() =>
            {
                ctrl.Fsm.GetFsmBool("Alert Range").Value = true;
            }));
            UnityEngine.Object.Destroy(sibling.GetComponent<LimitBehaviour>());
            FSMUtility.SetBool(ctrl, "No Spawn", false);
            ctrl.Fsm.GetState("Retreat").With(state => state.ForEachFsmStateAction<iTweenMoveBy>(x => new iTweenMoveTo()
            {
                gameObject = new()
                {
                    OwnerOption = OwnerDefaultOption.UseOwner
                },
                transformPosition = Head,
                time = 2,
                space = Space.World,
                easeType = iTween.EaseType.easeInCubic,
                loopType = iTween.LoopType.none,
                axis = iTweenFsmAction.AxisRestriction.xy,
                finishEvent = FsmEvent.Finished,
                stopOnExit = true,
                loopDontFinish = true
            })).AppendFsmStateAction<InvokeAction>(new(() =>
            {
                foreach (var v in ctrl.GetComponentsInChildren<Collider2D>()) v.enabled = false;
            }));
            sibling.SetActive(true);
            //ctrl.Fsm.SetState("Pause");
        }
        GameManager.instance.AudioManager.ApplyMusicCue(NoskGod.MimicSpiderMusic, 0, 0.5f, true);
        while (GameObject.Find("Nosk Shade") != null) yield return null;
        yield return new WaitForSeconds(1.5f);
        FSMUtility.SendEventToGameObject(CameraParent.Value, "BigShake");
        FSMUtility.SetBool(GameCameras.instance.cameraShakeFSM, "RumblingBig", true);
        PlayMakerFSM.BroadcastEvent("ROAR ENTER");

        yield return new WaitForSeconds(5.5f);
        FSMUtility.SetBool(GameCameras.instance.cameraShakeFSM, "RumblingBig", false);

        var mesh = FsmComponent!.GetComponent<MeshRenderer>();
        var rig = FsmComponent!.GetComponent<Rigidbody2D>();
        var anim = FsmComponent!.GetComponent<tk2dSpriteAnimator>();
        var dream = FsmComponent!.GetComponent<EnemyDreamnailReaction>();
        var col = FsmComponent!.GetComponent<Collider2D>();
        col.enabled = true;
        FsmComponent!.gameObject.AddComponent<SpriteTweenColorNeutral>();
        UnityEngine.Object.Destroy(FsmComponent!.GetComponent<InfectedEnemyEffects>());
        var hitEffect = FsmComponent!.gameObject.AddComponent<EnemyHitEffectsShade>();
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

        FsmComponent!.GetComponent<HealthManager>().private_hitEffectReceiver() = hitEffect;
        dream.private_convoTitle() = "INFECTED_KNIGHT_D";
        dream.private_convoAmount() = 3;

        var propBlock = new MaterialPropertyBlock();
        propBlock.SetTexture("_MainTex", NoskGod.voidNoskTex);
        FsmComponent!.GetComponent<MeshRenderer>().SetPropertyBlock(propBlock);

        FsmComponent!.transform.position = origPos;
        FsmComponent!.transform.SetPositionZ(5.2f);
        mesh.enabled = true;
        anim.Play("Jump");
        rig.isKinematic = false;
        rig.velocity = new Vector2(0, 55);

        Head.Value.SetActive(false);

        bones.transform.SetPositionZ(0);
        bones.GetComponent<ParticleSystem>().Play(true);
        yield return "JUMP";
    }
    [FsmState]
    private IEnumerator TranP21()
    {
        DefineEvent("TRAN FINISH", nameof(TranP22));
        yield return StartActionContent;
        nextEnterP2 = 1;
        origPos = FsmComponent!.transform.position;
        var rig = FsmComponent!.GetComponent<Rigidbody2D>();
        rig.velocity = Vector2.zero;
        rig.isKinematic = true;
        rig.position = new Vector2(9999, 9999);

        while (!Head.Value.activeInHierarchy) yield return null;

        PlayerData.instance.disablePause = true;
        GameManager.instance.AudioManager.ApplyMusicCue(NoskGod.AbyssMusic, 0, 2.5f, false);
        HeroController.instance.gameObject.LocateMyFSM("Roar Lock").Fsm.GetFsmGameObject("Roar Object").Value =  Head.Value;
        PlayMakerFSM.BroadcastEvent("ROAR ENTER");

        FSMUtility.SendEventToGameObject(CameraParent.Value, "BigShake");
        FSMUtility.SetBool(GameCameras.instance.cameraShakeFSM, "RumblingBig", true);
        yield return new WaitForSeconds(3.5f);
        PlayMakerFSM.BroadcastEvent("NOSK VESSEL SPAWN TRAN");

        yield return new WaitForSeconds(5.5f);

        var blanker = FsmVariables.GlobalVariables.GetFsmGameObject("HUD Blanker").Value.LocateMyFSM("Blanker Control");
        FSMUtility.SetFloat(blanker, "Fade Time", 0.3f);
        blanker.SendEvent("FADE IN");

        yield return new WaitForSeconds(1.5f);

        HeroController.instance.gameObject.LocateMyFSM("Roar Lock").Fsm.GetFsmGameObject("Roar Object").Value = Head.Value;

        MakeScene();

        FSMUtility.SetBool(GameCameras.instance.cameraShakeFSM, "RumblingBig", false);

        yield return new WaitForSeconds(2.5f);
        while (!HeroController.instance.cState.onGround) yield return null;
        
        PlayMakerFSM.BroadcastEvent("NOSK VESSEL SPAWN STOP");
        PlayMakerFSM.BroadcastEvent("ROAR EXIT");
        var darkness = FSMHelper.FindFsm("Darkness Control")!;
        darkness.Variables.FindFsmInt("Darkness Level").Value = 1;
        darkness.ProcessEvent(FsmEvent.GetFsmEvent("RESET"));
        HeroController.instance.RelinquishControlNotVelocity();
        HeroController.instance.StopAnimationControl();
        HeroController.instance.gameObject.LocateMyFSM("Dream Return").SetState("Land");
        foreach(var v in GameCameras.instance.sceneParticles.sceneParticles)
        {
            v.particleObject.SetActive(v.mapZone == GlobalEnums.MapZone.ABYSS);
        }
        GameCameras.instance.sceneParticles.defaultParticles.particleObject.SetActive(false);

        yield return null;

        blanker.SendEvent("FADE OUT");

        while (HeroController.instance.controlReqlinquished) yield return null;
        yield return new WaitForSeconds(1.25f);

        yield return "TRAN FINISH";
    }

}

