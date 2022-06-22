
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
        if(UnityEngine.Random.value <= 0.15f && UnityEngine.Object.FindObjectsOfType<GameObject>().Count(x => x.name == "Nosk Shade") <= 5)
        {
            yield return new WaitForSeconds(1.5f * (1f + UnityEngine.Random.value));
            var sibling = UnityEngine.Object.Instantiate(NoskGod.Sibling);
            sibling.name = "Nosk Shade";
            sibling.transform.position = FsmComponent!.transform.position;
            UnityEngine.Object.Destroy(sibling.FindChild("Alert Range"));
            var ctrl = sibling.LocateMyFSM("Control");
            ctrl.Fsm.GetState("Friendly?").AppendFsmStateAction<InvokeAction>(new(() =>
            {
                ctrl.Fsm.SetState("Init");
                ctrl.GetComponent<HealthManager>().hp = 13;
                foreach(var v in ctrl.GetComponentsInChildren<Collider2D>()) v.isTrigger = true;
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
            sibling.SetActive(true);
        }
        else
        {
            yield return new WaitForSeconds(3.5f);
        }
        UnityEngine.Object.Destroy(FsmComponent!.gameObject, 3f);
        yield return "TIME OUT";
    }
}
