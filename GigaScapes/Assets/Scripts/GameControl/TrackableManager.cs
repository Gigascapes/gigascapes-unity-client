﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackableManager : MonoBehaviour {

    public List<TrackableObject> TrackedObjects = new List<TrackableObject>();

    Dictionary<int[], Detector> killList = new Dictionary<int[], Detector>();

    public static TrackableManager Tracker;

    private void Start()
    {
        Tracker = this; 
    }

    private void Update()
    {
        killList.Clear();
        for(int i = 0; i < TrackedObjects.Count; i++)
        {
            TrackedObjects[i].Managed = true;

            killList = TrackedObjects[i].ManagedUpdate(killList);
        }
    }
}
