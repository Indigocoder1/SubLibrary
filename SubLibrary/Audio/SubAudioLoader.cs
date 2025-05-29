using Nautilus.Extensions;
using Nautilus.Handlers;
using Nautilus.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace SubLibrary.Audio;

public class SubAudioLoader : MonoBehaviour
{
    [SerializeField] internal List<FMODAsset> fmodAssets;

    /// <summary>
    /// Loads all <see cref="CustomFMODAsset"/>s in the provided Asset Bundle
    /// </summary>
    /// <param name="bundle">The Asset Bundle to load audio from</param>
    public static void LoadAllAudio(AssetBundle bundle)
    {
        var assets = bundle.LoadAllAssets<CustomFMODAsset>();
        foreach (var asset in assets)
        {
            RegisterAssetAudio(asset);
        }
    }

    /// <summary>
    /// Checks all objects in the provided array, to find <see cref="Audio.CustomFMODAsset"/>s. Once found, loads all assets 
    /// </summary>
    /// <param name="allBundleAssets">The assets to search through</param>
    public static void LoadAllAudio(object[] allBundleAssets)
    {
        foreach (var obj in allBundleAssets)
        {
            if (obj is not CustomFMODAsset asset) continue;

            RegisterAssetAudio(asset);
        }
    }

    /// <summary>
    /// Registers the provided CustomFMODAsset with Nautilus, given that <see cref="Audio.CustomFMODAsset.doNotAutoRegister"/> is not set to true
    /// </summary>
    /// <param name="asset">The asset to register</param>
    public static void RegisterAssetAudio(CustomFMODAsset asset)
    {
        if (asset.doNotAutoRegister) return;
            
        var sound = AudioUtils.CreateSound(asset.audioClip, asset.mode);
        if (asset.minDistance3D > 0 || asset.maxDistance3D > 0)
        {
            sound.set3DMinMaxDistance(asset.minDistance3D, asset.maxDistance3D);
        }

        if (asset.fadeOutTime > 0)
        {
            sound.AddFadeOut(asset.fadeOutTime);
        }

        CustomSoundHandler.RegisterCustomSound(asset.path, sound, asset.GetBus());
    }
}