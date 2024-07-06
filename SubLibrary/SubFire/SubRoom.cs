using UnityEngine;

namespace SubLibrary.SubFire;

[RequireComponent(typeof(Collider))]
internal class SubRoom : MonoBehaviour
{
    [HideInInspector] public int fireValue;
    [HideInInspector] public float smokeValue;

    [SerializeField] private ModdedSubFire subFire;
    [SerializeField] private SubRoom[] linkedRooms;

    private void OnTriggerEnter(Collider col)
    {
        GameObject entity = UWE.Utils.GetEntityRoot(col.gameObject);
        if(!entity) entity = col.gameObject;

        Player player = UWE.Utils.GetComponentInHierarchy<Player>(entity);
        if (!player) return;
        if (player.currentSub == null) return;

        subFire.SetPlayerRoom(this);
    }

    public Transform[] GetSpawnNodes()
    {
        Transform[] children = new Transform[transform.childCount];
        for (int i = 0; i < children.Length; i++)
        {
            children[i] = transform.GetChild(i);
        }

        return children;
    }

    public SubRoom[] GetLinkedRooms()
    {
        return linkedRooms;
    }
}
