
namespace NoskGodMod;

class DelayAttachFsm<T> : MonoBehaviour where T : CSFsm<T>, new()
{
    public string fsmName = "";
    private void OnEnable()
    {
        CSFsm<T>.Apply(gameObject.LocateMyFSM(fsmName));
        Destroy(this);
    }
}

class DelayAttachVomit : DelayAttachFsm<VomitGlobNoskFsm>
{
    public DelayAttachVomit()
    {
        fsmName = "Vomit Glob";
    }
}
