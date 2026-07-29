using TMPro;
using UnityEngine;

namespace SubLibrary.UI.UIManagers;

internal class DepthUIManager : MonoBehaviour, IUIElement
{
    // Code credit:
    // SealSub's DepthUIManager https://github.com/32Kallies/SealSub/blob/main/SealSubMod/MonoBehaviours/UI/DepthUIManager.cs
    // Contributors:
    // - EldritchCarMaker
    
    [SerializeField] private CrushDamage crushDamage;
    [SerializeField] private TextMeshProUGUI depthText;
    [SerializeField] private Color normalTextColor = Color.white;
    [SerializeField] private Color underCrushTextColor = Color.red;

    public void UpdateUI()
    {
        int currentDepth = (int)crushDamage.GetDepth();
        int maxDepth = (int)crushDamage.crushDepth;
        Color textColor = currentDepth > maxDepth ? underCrushTextColor : normalTextColor;

        depthText.text = $"{currentDepth}m / {maxDepth}m";
        depthText.color = textColor;
    }

    public void OnSubDestroyed()
    {
        // Nothing extra needed here
    }
}
