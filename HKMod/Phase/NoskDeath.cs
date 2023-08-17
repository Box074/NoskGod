
namespace NoskGodMod;

partial class NoskFsm : CSFsm<NoskFsm>
{
    [FsmState]
    private IEnumerator Death()
    {
        DefineGlobalEvent("NOSK DEATH");
        DefineEvent("NOSK DEATH ANIM FINISH", nameof(DeathFinish));
        yield return StartActionContent;

        this.anim.Play("Idle");
        NoskShade.KillAll();

        FSMUtility.SendEventToGameObject(HUDCanvas.Value, "OUT");

        FSMUtility.SendEventToGameObject(CameraParent.Value, "RumblingMed");

        Music_PL_Die.TransitionTo(0.15f, 0.45f);
        var split = Instantiate(NoskGod.KnightSplit);
        split.transform.position = HeroController.instance.transform.position;

        var hc = HeroControllerR.instance;

        hc.renderer.enabled = false;
        hc.StopAnimationControl();
        hc.RelinquishControl();
        hc.AffectedByGravity(false);
        hc.rb2d.velocity = Vector2.zero;
       
        foreach(var v in FindObjectsOfType<DamageHero>())
        {
            if(v.gameObject.scene.name == "GG_Nosk")
            {
                v.damageDealt = 0;
                v.hazardType = 1;
            }
        }

        split.SetActive(true);
        var anim = split.GetComponent<tk2dSpriteAnimator>();

        yield return anim.PlayAnimWait("Knight Split Antic");
        FSMUtility.SendEventToGameObject(split, "SPLIT");

        var ball = split.FindChild("Knight Ball");
        ball.SetActive(true);
        ball.transform.SetParent(null, true);

        yield return new WaitForSeconds(1.25f);


        var d = Instantiate(NoskGod.Death_Anim);
        d.transform.position = transform.position;
        transform.position = new(10000, 10000, 0);
        rig.isKinematic = true;
        rig.velocity = Vector2.zero;
    }

    [FsmState]
    private IEnumerator DeathFinish()
    {
        yield return StartActionContent;

        NoskGod.Instance.Log("Finish Boss ----------------------!!!!!");

        BossSceneController.Instance.bossesDeadWaitTime = 0.35f;
        BossSceneController.Instance.EndBossScene();
    }
}
