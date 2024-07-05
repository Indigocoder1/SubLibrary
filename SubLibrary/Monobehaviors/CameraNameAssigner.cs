using UnityEngine;

namespace SubLibrary.Monobehaviors;

internal class CameraNameAssigner : MonoBehaviour
{
    [Tooltip("The names of the camera positions for the cyclops cameras. Should be localized keys")] public string[] cameraNames;
}
