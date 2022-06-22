
namespace NoskGodMod;

partial class NoskGod : ModBase<NoskGod>
{
    public override void Initialize()
    {

    }
    
    [FsmPatcher("GG_Nosk_V", "Mimic Spider", "Mimic Spider")]
    [FsmPatcher("GG_Nosk", "Mimic Spider", "Mimic Spider")]
    private void NoskFsm(PlayMakerFSM pm)
    {
        NoskGodMod.NoskFsm.Apply(pm);
        DropVesselFsm.Attach(pm.gameObject, "Init");
    }
}
