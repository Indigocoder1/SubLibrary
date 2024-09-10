using Nautilus.Utility;
using UnityEngine;

namespace SubLibrary.Materials;

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
