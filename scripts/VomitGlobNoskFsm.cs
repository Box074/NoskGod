
namespace NoskGodMod;

class VomitGlobNoskFsm : CSFsm<VomitGlobNoskFsm>
{
    protected override void OnBindPlayMakerFSM(PlayMakerFSM pm)
    {
        base.OnBindPlayMakerFSM(pm);
        var propBlock = new MaterialPropertyBlock();
        propBlock.SetTexture("_MainTex", ModRes.VOMIT_VOID_TEX);
        pm.GetComponent<MeshRenderer>().SetPropertyBlock(propBlock);
        pm.transform.SetPositionZ(0);
        foreach(var v in pm.GetComponentsInChildren<ParticleSystem>())
        {
            var r = v.main;
            r.startColor = Color.gray;
        }
        UnityEngine.Object.Destroy(pm.GetComponent<DamageHero>());
    }
    [FsmState]
    private IEnumerator Land()
    {
        DefineEvent("TIME OUT", "Shrink");
        var original = OriginalActions;
        yield return StartActionContent;
        InvokeActions(original);
        if(UnityEngine.Random.value <= 0.65f && NoskShade.GetShadeCount() <= 5)
        {
            yield return new WaitForSeconds(1.5f * (1f + UnityEngine.Random.value));
            NoskShade.Spawn(transform.position);
        }
        UnityEngine.Object.Destroy(FsmComponent!.gameObject, 3f);
        yield return "TIME OUT";
    }
}
