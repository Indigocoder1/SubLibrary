using Nautilus.Utility;
using UnityEngine;

namespace SubLibrary.Materials;

// Deprecated whenever Nautilus decides to release Pre.33
internal class ApplySNShaders : MonoBehaviour
{
    [SerializeField] private GameObject applyTo;

    private void OnValidate()
    {
        if (applyTo == null) applyTo = gameObject;
    }

    private void Start()
    {
        MaterialUtils.ApplySNShaders(applyTo);
    }
}