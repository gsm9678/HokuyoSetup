using System.Collections.Generic;
using UnityEngine;

public class TouchPointGenerate : MonoBehaviour
{
    private enum PointPositionSpace
    {
        Auto,
        Local,
        World
    }

    public SensorEnum sensorEnum;
    private List<GameObject> TouchPoints = new List<GameObject>();
    private List<TrackedPoint> TrackedPoints = new List<TrackedPoint>();

    [SerializeField] private SensorManager sensorManager;
    [SerializeField] private GameObject TouchPoint;//생성할 프리팹
    [SerializeField] private TrackedPoint TrackedPoint;//생성할 프리팹

    [SerializeField] private Transform TouchPointBasket;//생성한 프리팹 트렌스폼
    [SerializeField] private PointPositionSpace pointPositionSpace = PointPositionSpace.Auto;

    // Update is called once per frame
    void Update()
    {
        if (SensorActiveState.Instance.SensorState[((int)sensorEnum)])//호쿠요 메니저가 연결되었으면
        {
            if (TouchPoint != null)
            {
                bool useWorldPosition = ShouldUseWorldPosition();
                List<TrackedSensorDataFormat> trackedSensorObjects = useWorldPosition ? sensorManager.getTrackedSensorWorldObjects() : sensorManager.getTrackedSensorObjects();
                List<Vector3> sensorVector = useWorldPosition ? sensorManager.getSensorWorldVector() : sensorManager.getSensorVector();

                if (trackedSensorObjects.Count != 0)
                {
                    if (trackedSensorObjects.Count > TrackedPoints.Count)//오브젝트 풀링
                    {
                        for (int i = TrackedPoints.Count; i < trackedSensorObjects.Count; i++)
                        {
                            TrackedPoints.Add(Instantiate(TrackedPoint, TouchPointBasket));
                        }
                    }
                    else if (trackedSensorObjects.Count < TrackedPoints.Count)//오브젝트 풀링
                    {
                        for (int i = trackedSensorObjects.Count; i < TrackedPoints.Count; i++)
                        {
                            TrackedPoints[i].gameObject.SetActive(false);
                        }
                    }

                    for (int i = 0; i < trackedSensorObjects.Count; i++)//오브젝트 풀링, 센서 위치에 이동
                    {
                        TrackedPoints[i].gameObject.SetActive(true);
                        TrackedPoints[i].SetTrackedPoint(trackedSensorObjects[i]);
                        SetPointPosition(TrackedPoints[i].transform, trackedSensorObjects[i].Position, useWorldPosition);
                    }
                }
                else
                {
                    if (sensorVector.Count > TouchPoints.Count)//오브젝트 풀링
                    {
                        for (int i = TouchPoints.Count; i < sensorVector.Count; i++)
                        {
                            TouchPoints.Add(Instantiate(TouchPoint, TouchPointBasket));
                        }
                    }
                    else if (sensorVector.Count < TouchPoints.Count)//오브젝트 풀링
                    {
                        for (int i = sensorVector.Count; i < TouchPoints.Count; i++)
                        {
                            TouchPoints[i].SetActive(false);
                        }
                    }

                    for (int i = 0; i < sensorVector.Count; i++)//오브젝트 풀링, 센서 위치에 이동
                    {
                        TouchPoints[i].SetActive(true);
                        SetPointPosition(TouchPoints[i].transform, sensorVector[i], useWorldPosition);
                    }
                }
            }
        }
        else//호쿠요 메니저가 연결이 안되어있으면 모든 오브젝트 False
        {
            for(int i = 0; i < TouchPoints.Count; i++)
            {
                TouchPoints[i].SetActive(false);
            }
            for (int i = 0; i < TrackedPoints.Count; i++)
            {
                TrackedPoints[i].gameObject.SetActive(false);
            }
        }
    }

    private bool ShouldUseWorldPosition()
    {
        if (pointPositionSpace == PointPositionSpace.World)
            return true;

        if (pointPositionSpace == PointPositionSpace.Local)
            return false;

        bool touchPointUsesRectTransform = TouchPoint != null && TouchPoint.GetComponent<RectTransform>() != null;
        bool basketUsesRectTransform = TouchPointBasket != null && TouchPointBasket.GetComponent<RectTransform>() != null;

        return !(touchPointUsesRectTransform && basketUsesRectTransform);
    }

    private void SetPointPosition(Transform target, Vector3 position, bool useWorldPosition)
    {
        if (useWorldPosition)
            target.position = position;
        else
            target.localPosition = position;
    }
}
