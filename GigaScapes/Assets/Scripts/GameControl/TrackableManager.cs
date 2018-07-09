using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackableManager : MonoBehaviour {

    public List<TrackableObject> TrackedObjects = new List<TrackableObject>();

    Dictionary<int[], Detector> killList = new Dictionary<int[], Detector>();

    private void Update()
    {
        for(int i = 0; i < TrackedObjects.Count; i++)
        {
            TrackedObjects[i].Managed = true;

            killList = TrackedObjects[i].ManagedUpdate(killList);
        }
    }
}
