
namespace NoskGodMod;

partial class NoskFsm : CSFsm<NoskFsm>
{
    private PlayMakerFSM pm => FsmComponent!;

    public bool IsTranPhase
    {
        get => isTranPhase.Value;
        set {
            if(value)
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
            isTranPhase.Value = value;
        }
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
        if(!noSound) pm.GetComponent<AudioSource>().PlayOneShot(NoskGod.mimic_spider_scream);
        var roar = UnityEngine.Object.Instantiate(NoskGod.RoarEmitter, roarPoint.Value.transform);
        roar.transform.localPosition = Vector3.zero;
        HeroController.instance.gameObject.LocateMyFSM("Roar Lock").Fsm.GetFsmGameObject("Roar Object").Value = gameObject;
        return roar;
    }
    private IEnumerator Roar(bool noHard = false, bool noSound = false)
    {
        yield return RoarPrepare();
        
        var roar = DoRoar(noSound);
        if(!noHard) PlayMakerFSM.BroadcastEvent("ROAR ENTER");

        yield return new WaitForSeconds(3.5f);

        yield return RoarEnd(roar);
    }
    private void Init()
    {
        DropVesselFsm.Attach(gameObject, out dropVesselFsm ,"Init");
        NoskPhaseCheck.Attach(gameObject, out phaseCheckFsm ,"Init");

        effect_color_add = gameObject.AddComponent<ColorAddEffect>();
        effect_color_scale = gameObject.AddComponent<ColorScaleEffect>();

        anim = GetComponent<tk2dSpriteAnimator>();

        var fc = gameObject.AddComponent<NoskFallCheck>();
        fc.pm = FsmComponent;
        fc.rig = GetComponent<Rigidbody2D>();
    }
}