using System.Collections.Generic;
using UnityEngine;

public class TouchPointGenerate : MonoBehaviour
{
    public SensorEnum sensorEnum;
    private List<GameObject> TouchPoints = new List<GameObject>();
    private List<TrackedPoint> TrackedPoints = new List<TrackedPoint>();

    [SerializeField] private SensorManager sensorManager;
    [SerializeField] private GameObject TouchPoint;//생성할 프리팹
    [SerializeField] private TrackedPoint TrackedPoint;//생성할 프리팹

    [SerializeField] private Transform TouchPointBasket;//생성한 프리팹 트렌스폼

    // Update is called once per frame
    void Update()
    {
        if (SensorActiveState.Instance.SensorState[((int)sensorEnum)])//호쿠요 메니저가 연결되었으면
        {
            if (TouchPoint != null)
            {
                if (sensorManager.getTrackedSensorObjects().Count != 0)
                {
                    if (sensorManager.getTrackedSensorObjects().Count > TrackedPoints.Count)//오브젝트 풀링
                    {
                        for (int i = TouchPoints.Count; i < sensorManager.getTrackedSensorObjects().Count; i++)
                        {
                            TrackedPoints.Add(Instantiate(TrackedPoint, TouchPointBasket));
                        }
                    }
                    else if (sensorManager.getTrackedSensorObjects().Count < TrackedPoints.Count)//오브젝트 풀링
                    {
                        for (int i = sensorManager.getTrackedSensorObjects().Count; i < TrackedPoints.Count; i++)
                        {
                            TrackedPoints[i].gameObject.SetActive(false);
                        }
                    }

                    for (int i = 0; i < sensorManager.getTrackedSensorObjects().Count; i++)//오브젝트 풀링, 센서 위치에 이동
                    {
                        TrackedPoints[i].gameObject.SetActive(true);
                        TrackedPoints[i].id = sensorManager.getTrackedSensorObjects()[i].Id;
                        TrackedPoints[i].State = sensorManager.getTrackedSensorObjects()[i].State;
                        TrackedPoints[i].transform.localPosition = sensorManager.getTrackedSensorObjects()[i].Position;
                    }

                }
                else
                {
                    if (sensorManager.getSensorVector().Count > TouchPoints.Count)//오브젝트 풀링
                    {
                        for (int i = TouchPoints.Count; i < sensorManager.getSensorVector().Count; i++)
                        {
                            TouchPoints.Add(Instantiate(TouchPoint, TouchPointBasket));
                        }
                    }
                    else if (sensorManager.getSensorVector().Count < TouchPoints.Count)//오브젝트 풀링
                    {
                        for (int i = sensorManager.getSensorVector().Count; i < TouchPoints.Count; i++)
                        {
                            TouchPoints[i].SetActive(false);
                        }
                    }

                    for (int i = 0; i < sensorManager.getSensorVector().Count; i++)//오브젝트 풀링, 센서 위치에 이동
                    {
                        TouchPoints[i].SetActive(true);
                        TouchPoints[i].transform.localPosition = sensorManager.getSensorVector()[i];
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
}