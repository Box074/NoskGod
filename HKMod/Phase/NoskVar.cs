
namespace NoskGodMod;

partial class NoskFsm : CSFsm<NoskFsm>
{
    private GameObject bones = Instantiate(NoskGod.Bone);
    [FsmVar]
    private FsmObject fsmSelf = new();
    [FsmVar]
    private FsmGameObject CameraParent = new();
    [FsmVar]
    internal FsmBool isVoid = new();
    [FsmVar("Roar Point")]
    private FsmGameObject roarPoint = new();
    [FsmVar("Target X")]
    private FsmFloat TargetX = new();
    [FsmVar("Jump Max X")]
    private FsmFloat JumpMaxX = new();
    [FsmVar("Jump Min X")]
    private FsmFloat JumpMinX = new();
    [FsmVar]
    private FsmGameObject Head = new();
    private Vector3 origPos;
    internal int nextEnterP2 = -2;
    [FsmVar]
    private FsmInt jumpCount = new();
    [FsmVar]
    private FsmBool isTranPhase = false;
    [FsmVar("Enemy Dream Msg")]
    private FsmGameObject dreamMsg = new();
    [FsmVar("Jump Distance")]
    private FsmFloat jumpDistance = new();
    internal bool isPhase3 = false;
    internal bool isPhase2 = false;
    internal bool isPhaseLast = false;

    public HealthManagerR hm = null!;
    public BoxCollider2D col = null!;
    public Rigidbody2D rig = null!;
    public MeshRenderer rend = null!;
    public DropVesselFsm dropVesselFsm = null!;
    public NoskPhaseCheck phaseCheckFsm = null!;
    public tk2dSpriteAnimator anim = null!;
    public AudioSource ac = null!;
    public int Phase1HP = 2500 + GetWithLevel(0, 200, 400);
    public int Phase2HP = 2000 + GetWithLevel(0, 200, 400);
    public int Phase3HP = 1950;
    public int LastPhaseHP = 1000;
    public float TimeP2 = GetWithLevel(30, 50, 50);

    public bool spawnVesselOnLand = false;
    public bool spawnShadeOnLand = false;

    public int spawnShadeMin = 1;
    public int spawnShadeMax = 3;
    public bool farawayHero = false;
    public bool farawayPlatform = false;
    public string nextStateAfterLand = null;
    public const MusicMask Music_FakeDie = MusicMask.MainAndSub;
    public const MusicMask Music_FakeDie_Dark = MusicMask.MainOnly;
    public const MusicMask Music_FakeDie_P2 = MusicMask.MainAndSub;
    public const MusicMask Music_P2_Roar = MusicMask.NoneExtraOrMainAltOrTension;
    public const MusicMask Music_P3_Intro = MusicMask.ActionAndSub;
    public const MusicMask Music_PL_Start = MusicMask.SubOnly;
    public const MusicMask Music_PL_Die = MusicMask.MainAndSub;
    public const float PlatformLeftX = 88;
    public const float PlatformRightX = 103.8f;
}
