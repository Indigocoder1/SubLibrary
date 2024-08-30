using FMOD;
using Nautilus.Handlers;
using Nautilus.Utility;
using SubLibrary.SaveData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SubLibrary.Audio;

public class SubAudioLoader : MonoBehaviour
{
    private const MODE k3DSoundModes = MODE.DEFAULT | MODE._3D | MODE.ACCURATETIME | MODE._3D_LINEARSQUAREROLLOFF;
    private const MODE k2DSoundModes = MODE.DEFAULT | MODE._2D | MODE.ACCURATETIME;
    private const MODE kStreamSoundModes = k2DSoundModes | MODE.CREATESTREAM;
    private const MODE kWorldSoundModes = k3DSoundModes | MODE.CREATESTREAM;

    private static Dictionary<string, string> busPaths = new();
    private static bool busPathsInitialized;

    [SerializeField] internal SerializableList<AudioData> audioDatas = new();

    private string[] names;

    /// <summary>
    /// Loads all <see cref="AudioData"/>s in the provided Asset Bundle
    /// </summary>
    /// <param name="bundle">The Asset Bundle to load audio from</param>
    public static void LoadAllAudio(AssetBundle bundle)
    {
        EnsureBusPaths();

        object[] allGOs = bundle.LoadAllAssets(typeof(GameObject));
        Plugin.Logger.LogInfo($"Bundle = {bundle} | Gameobjects = {allGOs} (Length = {allGOs?.Length})");
        foreach(var gameObject in allGOs)
        {
            if (gameObject is not GameObject) continue;

            var loader = (gameObject as GameObject).GetComponentInChildren<SubAudioLoader>(true);
            if (loader == null) continue;

            foreach (var data in loader.audioDatas)
            {
                Plugin.Logger.LogInfo($"Loading data ({data}) | Path = {data.path} | Clip = {data.audioClip} | Type = {data.audioType}");
                LoadAudioData(data);
            }
        }
    }

    private static void LoadAudioData(AudioData audioData)
    {
        if (audioData.audioType == AudioType.OnlyLoadInBundle) return;

        MODE mode = audioData.audioType switch
        {
            AudioType.StreamSound => kStreamSoundModes,
            AudioType.WorldSound => kWorldSoundModes,
            _ => MODE.DEFAULT
        };

        string busPath = busPaths[audioData.busPath.ToString()];
        var sound = AudioUtils.CreateSound(audioData.audioClip, mode);

        CustomSoundHandler.RegisterCustomSound(audioData.path, sound, busPath);
    }

    private static void EnsureBusPaths()
    {
        if (busPathsInitialized) return;

        //There's a lot of assumptions going on here. If a const field other than a string is added to the BusPaths class everything breaks lol
        var fields = typeof(AudioUtils.BusPaths).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        foreach (var field in fields)
        {
            if (!field.IsLiteral) continue;

            busPaths.Add(field.Name, (string)field.GetValue(null));
        }
    }
}

[Serializable]
internal class AudioData
{
    public string name;
    public string path;
    public AudioClip audioClip;
    public AudioType audioType;
    public BusPath busPath;
}

internal enum AudioType
{
    WorldSound,
    StreamSound,
    OnlyLoadInBundle
}

internal enum BusPath
{
    UnderwaterCreatures,
    SurfaceCreatures,
    PDAVoice,
    VoiceOvers,
    Music,
    EnvironmentalMusic,
    UnderwaterAmbient,
    SurfaceAmbient,
    PlayerSFXs
}