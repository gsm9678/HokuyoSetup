using UnityEngine;

abstract class TrackedPoint : MonoBehaviour
{
    public int id;
    public TrackState State;

    public void SetTrackedPoint(TrackedSensorDataFormat data)
    {
        id = data.Id;
        State = data.State;
    }
}
