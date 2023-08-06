
using Modding;
using System.Text;

namespace NoskGodMod;

partial class NoskGod : ModBase<NoskGod>
{
    public static tk2dSpriteCollectionData oldKnight = null!;
    public static List<tk2dSpriteAnimationClip> backdashClips = new();
    private void OldKnightAnimLoad()
    {
        var tgo = new GameObject("Old Knight");
        DontDestroyOnLoad(tgo);
        oldKnight = tgo.AddComponent<tk2dSpriteCollectionData>();

        oldKnight.textures = new[] {
            assetBundle.LoadAsset<Texture2D>("Assets/Textures/Static/knight-atlas.png")
        };
        
        JToken spriteInfo = JToken.Parse(Encoding.UTF8.GetString(ModResources.KNIGHT_SPRITES));
        var sdef = spriteInfo["spriteDefinitions"]!;
        foreach (var v in sdef)
        {
            v["material"] = null;
        }
        oldKnight.spriteDefinitions = sdef.ToObject<tk2dSpriteDefinition[]>();

        var mat = Instantiate(newKnightColData.materials[0]);
        mat.mainTexture = oldKnight.textures[0];
        oldKnight.materials = new[] { mat };
        oldKnight.material = mat;
        foreach (var v in oldKnight.spriteDefinitions!)
        {
            v.material = mat;
        }

        oldKnight.invOrthoSize = 2;
        oldKnight.halfTargetHeight = 32;
        oldKnight.textureFilterMode = FilterMode.Bilinear;

        string[] requireClips = new[] { "Back Dash", "Backdash Land 1", "Backdash Land 2", "Back Dash 2", "Back Dash 3", "Back Dash Pause" };
        string[] origSpeed = new[] { "Backdash Land 1", "Backdash Land 2" };
        JToken animData = JToken.Parse(Encoding.UTF8.GetString(ModResources.KNIGHT_ANIM));
        foreach (var v in animData["clips"]!)
        {
            var name = (string)v["name"]!;
            if (!requireClips.Contains(name)) continue;
            var clip = new tk2dSpriteAnimationClip();
            clip.name = name;
            clip.fps = (float)v["fps"]!;
            if (!origSpeed.Contains(name)) clip.fps *= 2;
            clip.loopStart = (int)v["loopStart"]!;
            clip.wrapMode = (tk2dSpriteAnimationClip.WrapMode)((int)v["wrapMode"]!);
            List<tk2dSpriteAnimationFrame> frames = new();

            foreach (var f in v["frames"]!)
            {
                var frame = new tk2dSpriteAnimationFrame();
                frame.spriteCollection = oldKnight;
                frame.spriteId = (int)f["spriteId"]!;
                frame.eventFloat = (float)f["eventFloat"]!;
                frame.eventInfo = (string)f["eventInfo"]!;
                frame.eventInt = (int)f["eventInt"]!;
                frame.triggerEvent = (bool)f["triggerEvent"]!;
                frames.Add(frame);
            }

            clip.frames = frames.ToArray();
            backdashClips.Add(clip);
        }
        newKnightAnim.clips = newKnightAnim.clips.Concat(backdashClips).ToArray();
    }
    protected override List<(SupportedLanguages, string)> LanguagesEx => new()
    {
        (SupportedLanguages.EN, ModResources.RES_ID_EN),
        (SupportedLanguages.ZH, ModResources.RES_ID_ZH)
    };
    private class SpawnShadeOrb : FsmStateAction
    {
        public GameObject spawnPoint = null!;
        public override void OnEnter()
        {
            if(spawnPoint == null) spawnPoint = Owner;
            var count = UnityEngine.Random.Range(10, 16);
            for (int i = 0; i < count; i++)
            {
                var orb = Instantiate(ShadeOrb, spawnPoint.transform.position, Quaternion.identity);
                orb.SetActive(true);
                var rig = orb.GetComponent<Rigidbody2D>();
                rig.velocity = new(UnityEngine.Random.value * 25 + 25, UnityEngine.Random.value * 15 + 15);
            }
            Finish();
        }
    }
    private FsmStateAction ReplaceSpawnBlood(SpawnBlood sb)
    {
        return new SpawnShadeOrb()
        {
            spawnPoint = sb.spawnPoint.Value ?? sb.Owner
        };
    }
    private class StatueFilter : GameObjectFilter
    {
        public override bool Filter(GameObject go) => go.name == "GG_Statue_Nosk";
    }
    public override void Initialize()
    {
        OldKnightAnimLoad();
        I18n.UseLanguageHook = true;

        foreach (var v in DeathExplodeBoss.GetComponentsInChildren<tk2dSprite>())
        {
            v.color = Color.black - new Color(0, 0, 0, 0.3f);
        }
        foreach (var v in DeathExplodeBoss.GetComponentsInChildren<SpriteRenderer>())
        {
            v.color = Color.black - new Color(0, 0, 0, 0.5f);
        }
        foreach (var v in DeathExplodeBoss.GetComponentsInChildren<SimpleSpriteFade>())
        {
            v.Reflect().normalColor = Color.black;
            v.fadeInColor = Color.black - new Color(0, 0, 0, 1);
        }
        Destroy(DeathExplodeBoss.FindChild("Splat Explode Orange"));
        DeathExplodeBoss.LocateMyFSM("Additional Effects").Fsm.GetState("State 1").ForEachFsmStateAction<SpawnBlood>(ReplaceSpawnBlood);
        DeathExplodeBoss.LocateMyFSM("Additional Effects").Fsm.GetState("State 1").SaveActions();


        Destroy(AbyssTendrils.LocateMyFSM("Black Charm"));

        new GameObjectWatcher(new StatueFilter(), statue =>
        {
            var bc = statue!.GetComponent<BossStatueR>();
            //bc.bossDetails.descriptionKey = "NOSK_STATUE_NAME";
            //bc.bossDetails.nameKey = "NOSK_STATUE_DESC";
            bc.bossScene.tier2Scene = null;
            bc.bossScene.tier3Scene = null;
        });

        On.HeroController.HazardRespawn += HeroController_HazardRespawn;
    }

    private IEnumerator HeroController_HazardRespawn(On.HeroController.orig_HazardRespawn orig, 
        HeroController self)
    {
        IEnumerator Wait()
        {
            PlatController.forceFlat = true;
            yield return new WaitForSeconds(1.5f);
            PlatController.forceFlat = false;
        }
        Wait().StartCoroutine();
        NoskShade.KillAll();
        return orig(self);
    }

    public static void HeroBackDash()
    {
        var hc = HeroControllerR.instance;
        hc.StopAnimationControl();
        hc.RelinquishControlNotVelocity();
        hc.ResetAttacks();
        hc.CancelBounce();
        hc.ResetLook();
        var ac = hc.audioCtrl;
        ac.StopSound(HeroSoundsR.FOOTSTEPS_RUN);
        ac.StopSound(HeroSoundsR.FOOTSTEPS_WALK);
        ac.PlaySound(HeroSoundsR.DASH);
        var ih = hc.inputHandler;
        if (hc.cState.wallSliding)
        {
            hc.FlipSprite();
        }
        else if (ih.inputActions.right.IsPressed)
        {
            hc.FaceRight();
        }
        else if (ih.inputActions.left.IsPressed)
        {
            hc.FaceLeft();
        }
        DoBackDash().StartCoroutine();
    }
    private static IEnumerator DoBackDash()
    {
        var hc = HeroControllerR.instance;
        var anim = hc.GetComponent<tk2dSpriteAnimator>();
        var move = DoBackDashMove().StartCoroutine();
        yield return null;
        yield return anim.PlayAnimWait("Back Dash");
        yield return anim.PlayAnimWait("Back Dash 2");
        yield return anim.PlayAnimWait("Back Dash 3");

        move.Stop();
        var rig = hc.rb2d;
        rig.velocity = Vector2.zero;
        yield return anim.PlayAnimWait("Backdash Land 1");
        yield return anim.PlayAnimWait("Backdash Land 2");
        hc.AffectedByGravity(true);
        hc.RegainControl();
        hc.StartAnimationControl();
    }
    private static IEnumerator DoBackDashMove()
    {
        var hc = HeroControllerR.instance;
        var rig = hc.rb2d;
        var num = hc.DASH_SPEED;
        while (true)
        {
            hc.AffectedByGravity(false);
            hc.ResetHardLandingTimer();
            var result = new Vector2();
            if (hc.cState.facingRight)
            {
                if (hc.ToOriginal().CheckForBump(CollisionSide.right))
                {
                    result = new Vector2(-num, (!hc.cState.onGround) ? 5f : 4f);
                }
                else
                {
                    result = new Vector2(-num, 0f);
                }
            }
            else if (hc.ToOriginal().CheckForBump(CollisionSide.left))
            {
                result = new Vector2(num, (!hc.cState.onGround) ? 5f : 4f);
            }
            else
            {
                result = new Vector2(num, 0f);
            }
            rig.velocity = Modding.ModHooksR.DashVelocityChange(result);
            yield return null;
        }
    }

    [FsmPatcher("GG_Nosk", "Mimic Spider", "Mimic Spider")]
    private void NoskFsm(PlayMakerFSM pm)
    {
        NoskGodMod.NoskFsm.Apply(pm);
    }

   
}
