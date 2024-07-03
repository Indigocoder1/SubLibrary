using Nautilus.Utility;
using TMPro;
using UnityEngine;

namespace SubLibrary.Monobehaviors.Utilities;

public class ApplySNFont : MonoBehaviour
{
    private void Start()
    {
        GetComponent<TextMeshProUGUI>().font = FontUtils.Aller_Rg;
    }
}
