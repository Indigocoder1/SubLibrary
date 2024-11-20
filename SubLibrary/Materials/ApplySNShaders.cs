using Nautilus.Utility;
using System;
using UnityEngine;

namespace SubLibrary.Materials;

[Obsolete("Use Nautilus's ApplySNShaders class instead")]
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