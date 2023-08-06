
namespace NoskGodMod;

partial class NoskGod : ModBase<NoskGod>
{
    public static readonly AssetBundle assetBundle = AssetBundle.LoadFromMemory(ModResources.VOIDNOSK);

    public static Texture2D voidNoskTex = assetBundle.LoadAsset<Texture2D>("Assets/Textures/Static/void-nosk.png");
    
    public static Sprite bg01 = assetBundle.LoadAsset<Sprite>("Assets/Textures/Static/skull_corpses_layered_0003_1_set.png");
    
    public static GameObject voidWaterPrefab = assetBundle.LoadAsset<GameObject>("Assets/VoidWave.prefab");

    public static GameObject platLagerPrefab = assetBundle.LoadAsset<GameObject>("Assets/Plat Lager.prefab");

    public static GameObject SR_QuakePrefab = assetBundle.LoadAsset<GameObject>("Assets/Shade/SR_Quake.prefab");

    public static GameObject S_QuakePrefab = assetBundle.LoadAsset<GameObject>("Assets/Shade/Quake Shade.prefab");

    public static GameObject waterFallPrefab = assetBundle.LoadAsset<GameObject>("Assets/Waterfall.prefab");

    [PreloadSharedAssets(455, "GG Sad")]
    public static MusicCue GGSadMusicCue = null!;
    [PreloadSharedAssets(290, "Corpse Mimic Spider", true)]
    public static GameObject CorpseMimicSpider = null!;
    [PreloadSharedAssets("hero_damage")]
    public static AudioClip hero_damage = null!;
    [PreloadSharedAssets("hollow_shade_startle")]
    public static AudioClip hollow_shade_startle = null!;
    [PreloadSharedAssets("mimic_spider_land")]
    public static AudioClip mimic_spider_land = null!;
    [PreloadSharedAssets("mimic_spider_jump")]
    public static AudioClip mimic_spider_jump = null!;
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
        VomitGlobNosk = Instantiate(go, GameObjectHelper.PrefabHolder.transform);
        Destroy(VomitGlobNosk.GetComponent<DamageHero>());
        VomitGlobNosk.AddComponent<DelayAttachVomit>();
    }

    [PreloadSharedAssets(290, "Knight")]
    public static tk2dSpriteCollectionData newKnightColData = null!;
    [PreloadSharedAssets(290, "Knight")]
    public static tk2dSpriteAnimation newKnightAnim = null!;

}
