using System.Reflection;
using UnityEngine;

namespace SubLibrary.Utilities;

internal class ApplySNLayer : MonoBehaviour
{
    [SerializeField] private LayerName layerName;

    private void Start()
    {
        FieldInfo field = typeof(LayerID).GetField(layerName.ToString(), BindingFlags.Static | BindingFlags.Public);
        gameObject.layer = (int)field.GetValue(null);
    }

    public enum LayerName
    {
        Default,
        Useable,
        NotUseable,
        Player,
        TerrainCollider,
        UI,
        Trigger,
        BaseClipProxy,
        OnlyVehicle,
        Vehicle,
        SubRigidbodyExclude,
        DefaultCollisionMask
    }
}
