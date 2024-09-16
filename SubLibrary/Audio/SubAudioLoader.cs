using FMOD;
using Nautilus.Handlers;
using Nautilus.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;
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
        foreach (var asset in bundle.LoadAllAssets<CustomFMODAsset>())
        {
            var sound = CustomSoundHandler.RegisterCustomSound(asset.name, asset.audioClip, asset.GetBus(), asset.mode);
            if(asset.minDistance3D > 0 ||  asset.maxDistance3D > 0)
            {
                sound.set3DMinMaxDistance(asset.minDistance3D, asset.maxDistance3D);
            }
        }
    }
}