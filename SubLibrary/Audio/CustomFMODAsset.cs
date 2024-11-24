using FMOD;
using Nautilus.Utility;
using UnityEngine;

namespace SubLibrary.Audio;

[CreateAssetMenu(fileName = "CustomFMODAsset", menuName = "Subnautica/Create FMOD Asset")]
public class CustomFMODAsset : FMODAsset
{
    private void OnEnable()
    {
        path = name;
    }

    public AudioClip audioClip;
    public bool doNotAutoRegister;
    public MODE mode = AudioUtils.StandardSoundModes_3D;
    public float maxDistance3D;
    public float minDistance3D;
    [Tooltip("Leave at -1 to not use fading")]
    public float fadeOutTime = -1;

    [SerializeField] private SoundBus bus;
    [SerializeField] private string customBusPath;

    public string GetBus() => bus switch
    {
        SoundBus.Custom => customBusPath,
        SoundBus.Sfx => "bus:/master/SFX_for_pause/PDA_pause/all/SFX",
        SoundBus.Pda => AudioUtils.BusPaths.PDAVoice,
        SoundBus.VoiceLine => AudioUtils.BusPaths.VoiceOvers,
        SoundBus.Music => AudioUtils.BusPaths.Music,
        SoundBus.UnderwaterCreature => AudioUtils.BusPaths.UnderwaterCreatures,
        SoundBus.SurfaceCreature => AudioUtils.BusPaths.SurfaceCreatures,
        SoundBus.Reverb => AudioUtils.BusPaths.PlayerSFXs,
        _ => string.Empty,
    };
}

internal enum SoundBus
{
    Custom = -1,
    Sfx,
    Pda,
    VoiceLine,
    Music,
    UnderwaterCreature,
    SurfaceCreature,
    Reverb
}