
namespace NoskGodMod;

class NoskFallCheck : MonoBehaviour
{
    public PlayMakerFSM pm = null!;
    public Rigidbody2D rig = null!;
    private float timer = 0;
    private const float FALL_CHECK = 1.75f;
    private void Update() {
        if(pm.ActiveStateName == "Falling")
        {
            if(Mathf.Abs(rig.velocity.y) < 0.1f)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                timer = FALL_CHECK;
            }
            if(transform.position.y <= 3 || timer <= 0)
            {
                pm.SendEvent("LAND");
                timer = FALL_CHECK;
            }
        }
        else
        {
            timer = FALL_CHECK;
        }
    }
}
