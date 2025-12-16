using UnityEngine;

namespace NipaGameKit
{
    public class CompDataSystemUpdater : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            CompDataSystemCollection.Instance.UpdateSystems(Time.time, Time.deltaTime);
        }
    }
}
