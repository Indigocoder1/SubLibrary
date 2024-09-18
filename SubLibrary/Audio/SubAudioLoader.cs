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
        foreach (var asset in bundle.LoadAllAssets<CustomFMODAsset>())
        {
            if (asset.doNotAutoRegister) continue;

            string id = asset.id == string.Empty ? asset.path : asset.id;
            var sound = AudioUtils.CreateSound(asset.audioClip, asset.mode);
            if (asset.minDistance3D > 0 || asset.maxDistance3D > 0)
            {
                sound.set3DMinMaxDistance(asset.minDistance3D, asset.maxDistance3D);
            }
            CustomSoundHandler.RegisterCustomSound(asset.path, sound, asset.GetBus());
        }
    }
}