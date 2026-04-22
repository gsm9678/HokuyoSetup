using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum TrackState
{
   Active,
   Occluded,
   Merged,
   Lost
}

[System.Serializable]
public class TrackedSensorDataFormat
{
    public int Id;
    public Vector3 Position;
    public TrackState State;
}

[System.Serializable]
public class SensorDataFormat
{
    public Vector2 RectSize;
    public List<Vector3> Position = new List<Vector3>();
    public List<TrackedSensorDataFormat> TrackedObjects = new List<TrackedSensorDataFormat>();
}

public enum SensorEnum
{
   Front,
   Back,
   Right,
   Left,
   Down
}
