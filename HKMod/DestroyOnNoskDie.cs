
namespace NoskGodMod;

class DestroyOnNoskDie : MonoBehaviour
{
    public static event Action ev;
    public static void Trig() {
        ev?.Invoke();
        ev = null;
    }
    private void DestroySelf() {
        Destroy(gameObject);
    }
    private void Update() {
        
    }
    private void Awake() {
        ev += DestroySelf;
    }
    private void OnDestroy() {
        ev -= DestroySelf;
    }
}
