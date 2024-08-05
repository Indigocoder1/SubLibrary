namespace SubLibrary.UI;

public interface IUIElement
{
    /// <summary>
    /// Called every frame when the sub is alive and in LOD distance
    /// </summary>
    public void UpdateUI();

    /// <summary>
    /// Called when the sub is destroyed
    /// </summary>
    public void OnSubDestroyed();
}
