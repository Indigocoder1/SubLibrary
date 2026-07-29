namespace SubLibrary.UI;

public interface IUIElement
{
    // Code credit:
    // SealSub's IUIElement https://github.com/32Kallies/SealSub/blob/main/SealSubMod/Interfaces/IUIElement.cs
    // Contributors:
    // - EldritchCarMaker
    
    /// <summary>
    /// Called every frame when the sub is alive and in LOD distance
    /// </summary>
    public void UpdateUI();

    /// <summary>
    /// Called when the sub is destroyed
    /// </summary>
    public void OnSubDestroyed();
}
