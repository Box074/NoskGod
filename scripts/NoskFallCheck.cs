
namespace NoskGodMod;

class NoskFallCheck : MonoBehaviour
{
    public PlayMakerFSM pm = null!;
    public Rigidbody2D rig = null!;
    private void Update() {
        if(pm.ActiveStateName == "Falling")
        {
            if((Mathf.Abs(rig.velocity.y) + Mathf.Abs(rig.velocity.x)) < 0.1f)
            {
                pm.SendEvent("LAND");
            }
        }
    }
}
