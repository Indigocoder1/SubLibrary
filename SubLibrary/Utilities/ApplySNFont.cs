using Nautilus.Utility;
using TMPro;
using UnityEngine;

namespace SubLibrary.Utilities;

public class ApplySNFont : MonoBehaviour
{
    [SerializeField] private ApplicationMode applicationMode;

    private void Start()
    {
        switch (applicationMode)
        {
            case ApplicationMode.SingleText:
                GetComponent<TextMeshProUGUI>().font = FontUtils.Aller_Rg;
                break;
            case ApplicationMode.AllChildTexts:
                GetComponentsInChildren<TextMeshProUGUI>().ForEach(t => t.font = FontUtils.Aller_Rg);
                break;
            case ApplicationMode.AllChildTextsIncludeInactive:
                GetComponentsInChildren<TextMeshProUGUI>(true).ForEach(t => t.font = FontUtils.Aller_Rg);
                break;
        }

    }

    private enum ApplicationMode
    {
        SingleText,
        AllChildTexts,
        AllChildTextsIncludeInactive
    }
}
