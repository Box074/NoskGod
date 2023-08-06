
namespace NoskGodMod;

partial class NoskFsm : CSFsm<NoskFsm>
{
    private PlayMakerFSM pm => FsmComponent!;
    public bool IsTranPhase
    {
        get => isTranPhase.Value;
        set
        {
            if (value)
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
            isTranPhase.Value = value;
        }
    }
    public static int GetLevel() => BossSceneController.Instance?.BossLevel ?? 0;
    public static T GetWithLevel<T>(T level1, T level2, T level3) => GetLevel() switch
    {
        0 => level1,
        1 => level2,
        2 => level3,
        _ => level1
    };
    private void PlayOneShot(AudioClip clip, float volume = 1)
    {
        NoskGod.AudioPlayerActor.Spawn(transform.position.With((ref Vector3 r) =>
        {
            r.x = Mathf.Clamp(r.x, 71, 114);
            r.y = Mathf.Clamp(r.y, -10, 20);
        })).PlayOneShot(clip, volume);
    }
    private List<GameObject> activeOnDie = new();
    private void MakeScene()
    {
        var bg = new GameObject("Bg");
        bg.AddComponent<DestroyOnNoskDie>();
        bg.AddComponent<SpriteRenderer>().sprite = NoskGod.bg01;
        bg.transform.position = new Vector3(78.1956f, 3.4769f, 3.8164f);
        bg.transform.localScale = new Vector3(3, 3, 3);

        Instantiate(bg, new Vector3(84.9119f, 4.3642f, 4.1728f), Quaternion.identity);
        Instantiate(bg, new Vector3(89.3991f, 5.5078f, 4.4055f), Quaternion.identity);
        Instantiate(bg, new Vector3(96.3463f, 6.2787f, 4.5873f), Quaternion.identity);
        Instantiate(bg, new Vector3(102.2135f, 5.8351f, 4.7073f), Quaternion.identity);
        Instantiate(bg, new Vector3(109.5589f, 4.0169f, 4.3818f), Quaternion.identity);
        Instantiate(bg, new Vector3(113.9788f, 4.0169f, 4.5073f), Quaternion.identity);
        Instantiate(bg, new Vector3(117.5806f, 5.0605f, 5.1f), Quaternion.identity);

        GameObject.Find("GG_Arena_Prefab/Crowd")?.SetActive(false);
        GameObject.Find("GG_Arena_Prefab/BG")?.SetActive(false);
        GameObject.Find("GG_Arena_Prefab/Godseeker Crowd")?.SetActive(false);
        Destroy(GameObject.Find("GG_Arena_Prefab")?.GetComponent<AudioSource>());

        foreach (var v in FindObjectsOfType<GameObject>().Where(x => x.name.StartsWith("deepnest_glow_mush_") ||
                                                                                        x.name.StartsWith("haze") ||
                                                                                        x.name.StartsWith("GG_gods_ray") ||
                                                                                        //x.name.StartsWith("stalactite_small_set") ||
                                                                                        //x.name.StartsWith("deepnest_black_solid") ||
                                                                                        //x.name.StartsWith("deep_back_mid") ||
                                                                                        x.name.StartsWith("white_mush_Mids")//||
                                                                                        //x.name == "Crowd"||
                                                                                        //x.name == "Godseeker Crowd"
                                                                                        )) {
                                                                                            v.SetActive(false);
                                                                                            activeOnDie.Add(v);
                                                                                        }
        //TODO: Remove platform
    }
    private IEnumerator RoarPrepare()
    {
        yield return anim.PlayAnimWait("Roar Init");
        anim.Play("Roar Loop");
    }
    private IEnumerator RoarEnd(GameObject roar)
    {
        FSMUtility.SendEventToGameObject(roar, "END");
        PlayMakerFSM.BroadcastEvent("ROAR EXIT");
        yield return anim.PlayAnimWait("Roar Finish");
        anim.Play("Idle");
    }
    private GameObject DoRoar(bool noSound = false)
    {
        if (!noSound) pm.GetComponent<AudioSource>().PlayOneShot(NoskGod.mimic_spider_scream);
        var roar = Instantiate(NoskGod.RoarEmitter, roarPoint.Value.transform);
        roar.transform.localPosition = Vector3.zero;
        HeroController.instance.gameObject.LocateMyFSM("Roar Lock").Fsm.GetFsmGameObject("Roar Object").Value = gameObject;
        return roar;
    }
    private IEnumerator Roar(bool noHard = false, bool noSound = false)
    {
        yield return RoarPrepare();

        var roar = DoRoar(noSound);
        if (!noHard) PlayMakerFSM.BroadcastEvent("ROAR ENTER");

        yield return new WaitForSeconds(3.5f);

        yield return RoarEnd(roar);
    }
    private void Init()
    {
        DropVesselFsm.Attach(gameObject, out dropVesselFsm, "Init");
        NoskPhaseCheck.Attach(gameObject, out phaseCheckFsm, "Init");


        anim = GetComponent<tk2dSpriteAnimator>();
        col = GetComponent<BoxCollider2D>();
        rig = GetComponent<Rigidbody2D>();
        rend = GetComponent<MeshRenderer>();
        ac = GetComponent<AudioSource>();

        var fc = gameObject.AddComponent<NoskFallCheck>();
        fc.pm = FsmComponent;
        fc.rig = GetComponent<Rigidbody2D>();
    }
}