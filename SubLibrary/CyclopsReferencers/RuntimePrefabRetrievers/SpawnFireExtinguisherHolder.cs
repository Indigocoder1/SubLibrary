using UnityEngine;

namespace SubLibrary.CyclopsReferencers.RuntimePrefabRetrievers;

internal class SpawnFireExtinguisherHolder : MonoBehaviour, ICyclopsReferencer
{
    [SerializeField] private Vector3 localPosition;
    [SerializeField] private Vector3 localRotation;
    [SerializeField] private Vector3 localScale;

    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        GameObject holder = cyclops.transform.Find("FireExtinguisherHolder_Fore").gameObject;
        GameObject newHolder = Instantiate(holder, transform);

        newHolder.transform.localPosition = localPosition;
        newHolder.transform.localRotation = Quaternion.Euler(localRotation);
        newHolder.transform.localScale = localScale;
    }
}
