using SubLibrary.Interfaces;
using UnityEngine;

namespace SubLibrary.Monobehaviors.UI;

internal class SpeedUIManager : MonoBehaviour, IUIElement
{
    [SerializeField] private Animator animator;
    [SerializeField] private CyclopsMotorMode motorMode;

    private bool engineOnLastUpdate;

    private void Start()
    {
        animator.SetBool("Active", false);
    }

    public void UpdateUI()
    {
        if(motorMode.engineOn != engineOnLastUpdate)
        {
            animator.SetBool("Active", motorMode.engineOn);
        }

        engineOnLastUpdate = motorMode.engineOn;
    }

    public void OnSubDestroyed()
    {
        animator.SetBool("Active", false);
    }
}
