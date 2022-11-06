
namespace NoskGodMod;

partial class NoskGod : ModBase<NoskGod>
{
    public static Texture2D voidNoskTex = ModRes.NOSK_VOID_TEX;
    public static Sprite ground01 = ModRes.SPRITE_GROUND_01;
    public static Sprite bg01 = ModRes.SPRITE_BG_01;
    [PreloadSharedAssets(455, "GG Sad")]
    public static MusicCue GGSadMusicCue = null!;
    [PreloadSharedAssets(290, "Corpse Mimic Spider", true)]
    public static GameObject CorpseMimicSpider = null!;
    [PreloadSharedAssets("hero_damage")]
    public static AudioClip hero_damage = null!;
    [PreloadSharedAssets("hollow_shade_startle")]
    public static AudioClip hollow_shade_startle = null!;
    [PreloadSharedAssets("Audio Player Actor")]
    public static AudioSource AudioPlayerActor = null!;
    [PreloadSharedAssets("Hit Flash Black")]
    public static GameObject HitFlashBlack = null!;
    [PreloadSharedAssets("Slash Effect GhostDark1")]
    public static GameObject SlashEffectGhostDark1 = null!;
    [PreloadSharedAssets("Slash Effect GhostDark2")]
    public static GameObject SlashEffectGhostDark2 = null!;
    [PreloadSharedAssets("Slash Effect Shade")]
    public static GameObject SlashEffectShade = null!;
    [PreloadSharedAssets("Hit Shade")]
    public static GameObject HitShade = null!;
    [PreloadSharedAssets(109, "mimic_spider_scream")]
    public static AudioClip mimic_spider_scream = null!;
    [Preload("Dream_Abyss", "follow_particles/falling_knights")]
    public static GameObject falling_knights = null!;
    [Preload("Dream_Abyss", "Corpse Spawn (15)/Abyss Drop Corpse")]
    public static GameObject CorpseSpawn = null!;
    [Preload("Dream_Abyss", "Corpse Spawn (15)/Abyss Drop Corpse/Bone")]
    public static GameObject Bone = null!;
    [PreloadSharedAssets("Roar Wave Emitter")]
    public static GameObject RoarEmitter = null!;
    [PreloadSharedAssets(334, "Abyss")]
    public static MusicCue AbyssMusic = null!;
    [PreloadSharedAssets("Shade Orb")]
    public static GameObject ShadeOrb = null!;
    [Preload("Abyss_15", "Shade Sibling (14)")]
    public static GameObject Sibling = null!;
    [PreloadSharedAssets("Gas Explosion Recycle L")]
    public static GameObject Explosion = null!;
    [Preload("Abyss_16", "Abyss Tendrils")]
    public static GameObject AbyssTendrils = null!;
    [Preload("Abyss_09", "abyss_black-water")]
    public static GameObject AbyssWater = null!;
    [PreloadSharedAssets(32, "Death Explode Boss", true)]
    public static GameObject DeathExplodeBoss = null!;
    public static GameObject VomitGlobNosk = null!;
    [PreloadSharedAssets(0, "Hollow Shade Death")]
    public static GameObject HollowShadeDeath = null!;
    [PreloadSharedAssets(0, "Hollow Shade Death/Particle Wave", true)]
    public static ParticleSystem HollowShadeDeathWave = null!;
    [PreloadSharedAssets(0, "Hollow Shade Death/Depart Particles", true)]
    public static ParticleSystem HollowShadeDepartParticles = null!;
    
    [PreloadSharedAssets(290, "Vomit Glob Nosk")]
    private void PreloadVomitGlobNosk(GameObject go)
    {
        VomitGlobNosk = UnityEngine.Object.Instantiate(go, GameObjectHelper.PrefabHolder.transform);
        UnityEngine.Object.Destroy(VomitGlobNosk.GetComponent<DamageHero>());
        VomitGlobNosk.AddComponent<DelayAttachVomit>();
    }

    [PreloadSharedAssets(290, "Knight")]
    public static tk2dSpriteCollectionData newKnightColData = null!;
    [PreloadSharedAssets(290, "Knight")]
    public static tk2dSpriteAnimation newKnightAnim = null!;

}
