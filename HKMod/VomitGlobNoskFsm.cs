
namespace NoskGodMod;

class VomitGlobNoskFsm : CSFsm<VomitGlobNoskFsm>
{
    private static Texture2D tex = NoskGod.assetBundle.LoadAsset<Texture2D>("Assets/Textures/Static/void-vomit.png");
    protected override void OnBindPlayMakerFSM(PlayMakerFSM pm)
    {
        base.OnBindPlayMakerFSM(pm);
        var propBlock = new MaterialPropertyBlock();

        propBlock.SetTexture("_MainTex", tex);
        pm.gameObject.AddComponent<DestroyOnNoskDie>();
        pm.GetComponent<MeshRenderer>().SetPropertyBlock(propBlock);
        pm.transform.SetPositionZ(0);
        foreach(var v in pm.GetComponentsInChildren<ParticleSystem>())
        {
            var r = v.main;
            r.startColor = Color.gray;
        }
        Destroy(pm.GetComponent<DamageHero>());
    }
    private NoskFsm nosk;
    [FsmState]
    private IEnumerator Land()
    {
        DefineEvent("TIME OUT", "Shrink");
        var original = OriginalActions;
        yield return StartActionContent;
        InvokeActions(original);
        nosk = FindObjectOfType<NoskFsm>();
        if(UnityEngine.Random.value <= 0.65f && NoskShade.GetShadeCount() <= 5)
        {
            yield return new WaitForSeconds(1.5f * (1f + UnityEngine.Random.value));
            if (nosk.isPhase3 && Random.value < 0.45f)
            {
                Instantiate(NoskGod.SR_QuakePrefab, transform.position + new Vector3(0, -4, 0), Quaternion.identity);
            }
            else
            {
                NoskShade.Spawn(transform.position);
            }
            
        }
        Destroy(FsmComponent!.gameObject, 3f);
        yield return "TIME OUT";
    }
}
