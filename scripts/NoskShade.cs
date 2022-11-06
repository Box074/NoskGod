
namespace NoskGodMod;

static class NoskShade
{
    public static List<GameObject> shades = new();
    public static int GetShadeCount()
    {
        shades.RemoveAll(x => x == null || !x.activeInHierarchy);
        return shades.Count;
    }
    public static event Action<GameObject>? onSpawn = null;
    public static void ClearOnSpawn()
    {
        onSpawn = null;
    }
    public static void KillAll(GameObject? dieTarget = null)
    {
        GetShadeCount();
        foreach (var v in shades)
        {
            if (dieTarget != null)
            {
                var ctrl = v.LocateMyFSM("Control");
                var move = ctrl.Fsm.GetState("Retreat").GetFSMStateActionOnState<iTweenMoveTo>();
                if (move == null)
                {
                    move = new iTweenMoveTo()
                    {
                        gameObject = new()
                        {
                            OwnerOption = OwnerDefaultOption.UseOwner
                        },
                        transformPosition = dieTarget,
                        time = 2,
                        space = Space.World,
                        easeType = iTween.EaseType.easeInCubic,
                        loopType = iTween.LoopType.none,
                        axis = iTweenFsmAction.AxisRestriction.z,
                        finishEvent = FsmEvent.Finished,
                        stopOnExit = true,
                        loopDontFinish = true
                    };
                    ctrl.Fsm.GetState("Retreat").With(state => state.ForEachFsmStateAction<iTweenMoveBy>(x => move))
                    .AppendFsmStateAction<InvokeAction>(new(() =>
                    {
                        foreach (var v in ctrl.GetComponentsInChildren<Collider2D>()) v.enabled = false;
                    }));
                }
                else
                {
                    move.transformPosition = dieTarget;
                }
            }
            FSMUtility.SendEventToGameObject(v, "ZERO HP");
        }
        shades.Clear();
    }
    private class ShadeParticlesCtrl : MonoBehaviour
    {
        public GameObject shade = null!;
        public bool willStop = false;
        private bool waitStop = false;
        private ParticleSystem ps = null!;
        private void Update()
        {
            if (!willStop)
            {
                if(shade == null)
                {
                    Destroy(gameObject);
                    return;
                }
                transform.position = shade.transform.position + new Vector3(0.06f, -0.22f, 0.01f);
            }
            else
            {
                if(ps == null) ps = GetComponent<ParticleSystem>();
                if(!waitStop)
                {
                    ps.Stop();
                    waitStop = true;
                }
                else
                {
                    if(ps.isStopped)
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
    public static GameObject Spawn(Vector3 pos, int hp = 12, GameObject? dieTarget = null)
    {
        var sibling = UnityEngine.Object.Instantiate(NoskGod.Sibling);
        sibling.transform.position = pos;
        sibling.name = "Nosk Shade";
        UnityEngine.Object.Destroy(sibling.FindChild("Alert Range"));
        var sp = sibling.FindChildWithPath("Shade Particles")!;
        var sp_ctrl = sp.AddComponent<ShadeParticlesCtrl>();
        sp_ctrl.shade = sibling;
        sp.transform.parent = null;

        var ctrl = sibling.LocateMyFSM("Control");
        foreach (var v in ctrl.GetComponentsInChildren<Collider2D>()) v.isTrigger = true;
        ctrl.Fsm.GetState("Friendly?").RemoveAllFsmStateActions<SetDamageHeroAmount>();
        ctrl.Fsm.GetState("Friendly?").RemoveAllFsmStateActions<IntCompare>();
        ctrl.Fsm.GetState("Friendly?").AppendFsmStateAction<InvokeAction>(new(() =>
        {
            ctrl.Fsm.SetState("Init");
            ctrl.GetComponent<HealthManager>().hp = hp;

            FSMUtility.SetBool(ctrl, "Friendly", false);
        }));
        ctrl.Fsm.GetState("Recycle").AppendFsmStateAction<InvokeAction>(new(() =>
        {
            sp_ctrl.willStop = true;
            UnityEngine.Object.Destroy(sibling);
        }));
        ctrl.Fsm.GetState("Idle").AppendFsmStateAction<InvokeAction>(new(() =>
        {
            ctrl.Fsm.GetFsmBool("Alert Range").Value = true;
        }));
        if (dieTarget != null)
        {
            ctrl.Fsm.GetState("Retreat").With(state => state.ForEachFsmStateAction<iTweenMoveBy>(x => new iTweenMoveTo()
            {
                gameObject = new()
                {
                    OwnerOption = OwnerDefaultOption.UseOwner
                },
                transformPosition = dieTarget,
                time = 2,
                space = Space.World,
                easeType = iTween.EaseType.easeInCubic,
                loopType = iTween.LoopType.none,
                axis = iTweenFsmAction.AxisRestriction.z,
                finishEvent = FsmEvent.Finished,
                stopOnExit = true,
                loopDontFinish = true
            })).AppendFsmStateAction<InvokeAction>(new(() =>
            {
                foreach (var v in ctrl.GetComponentsInChildren<Collider2D>()) v.enabled = false;
            }));
        }
        sibling.GetComponent<DamageHero>().damageDealt = 1;
        UnityEngine.Object.Destroy(sibling.GetComponent<LimitBehaviour>());
        FSMUtility.SetBool(ctrl, "No Spawn", false);
        sibling.SetActive(true);
        
        shades.Add(sibling);
        //ctrl.SetState("Friendly?");
        onSpawn?.Invoke(sibling);
        return sibling;
    }
}
