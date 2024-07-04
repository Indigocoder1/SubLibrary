using TMPro;
using UnityEngine;

namespace SubLibrary.UI.UIManagers;

internal class DepthUIManager : MonoBehaviour, IUIElement
{
    [SerializeField] private CrushDamage crushDamage;
    [SerializeField] private TextMeshProUGUI depthText;

    public void UpdateUI()
    {
        int currentDepth = (int)crushDamage.GetDepth();
        int maxDepth = (int)crushDamage.crushDepth;
        Color textColor = currentDepth > maxDepth ? Color.red : Color.white;

        depthText.text = $"{currentDepth}m / {maxDepth}m";
        depthText.color = textColor;
    }

    public void OnSubDestroyed()
    {
        //Nothing extra needed here
    }
}
