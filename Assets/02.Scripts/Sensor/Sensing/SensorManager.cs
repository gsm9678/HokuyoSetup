using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorManager : MonoBehaviour
{
    public SensorEnum sensorEnum;

    private OSCManager m_senserData;
    private SensorDataFormat SensorData;
    List<Vector3> vector3 = new List<Vector3>();
    List<TrackedSensorDataFormat> trackedObjects = new List<TrackedSensorDataFormat>();

    [SerializeField]
    RectTransform SensorPosi;//맵핑할 대상

    void Start()
    {
        m_senserData = OSCManager.Instance;
        StartCoroutine(GetSensorData());
    }

    IEnumerator GetSensorData()
    {
        while (true)
        {
            yield return new WaitUntil(() => SensorActiveState.Instance.SensorState[(int)sensorEnum]); //센서가 연결될때까지 대기
            yield return new WaitForFixedUpdate();
            SensorData = m_senserData.SensorData[((int)sensorEnum)];//호쿠요 센서 로우 데이터 받기
            vector3.Clear();
            trackedObjects.Clear();

            for (int i = 0; i < SensorData.Position.Count; i++)//호쿠요 센서 로우 데이터를 컨텐츠에서 사용할 수 있게 맵핑
            {
                vector3.Add(MapSensorPosition(SensorData.Position[i]));
            }

            for (int i = 0; i < SensorData.TrackedObjects.Count; i++)
            {
                TrackedSensorDataFormat rawObject = SensorData.TrackedObjects[i];
                trackedObjects.Add(new TrackedSensorDataFormat
                {
                    Id = rawObject.Id,
                    Position = MapSensorPosition(rawObject.Position),
                    State = rawObject.State
                });
            }
        }
    }

    public new Camera camera;

#if UNITY_EDITOR
    Vector3 MousePosition;

    private void Update()
    {
        if(camera != null)
        {
            SensorActiveState.Instance.SensorState[((int)sensorEnum)] = true;
            if (Input.GetMouseButton(0))
            {
                vector3.Clear();
                trackedObjects.Clear();
                MousePosition = Input.mousePosition;

                vector3.Add(new Vector3(scale(-camera.pixelWidth / 2, camera.pixelWidth / 2, SensorPosi.position.x - SensorPosi.rect.width / 2, SensorPosi.position.x + SensorPosi.rect.width / 2, MousePosition.x - camera.pixelWidth / 2),
                                            scale(-camera.pixelHeight / 2, camera.pixelHeight / 2, SensorPosi.position.y - SensorPosi.rect.height / 2, SensorPosi.position.y + SensorPosi.rect.height / 2, MousePosition.y - camera.pixelHeight / 2),
                                            0));
            }
        }
    }
#endif

#if !UNITY_EDITOR

    private void OnEnable()
    {
        if (camera != null)
            Destroy(camera.gameObject);
    }

#endif


    //외부에서 vector3을 받기
    public List<Vector3> getSensorVector()
    {
        return vector3;
    }

    public List<TrackedSensorDataFormat> getTrackedSensorObjects()
    {
        return trackedObjects;
    }

    private Vector3 MapSensorPosition(Vector3 sensorPosition)
    {
        return new Vector3(scale(-SensorData.RectSize.x / 2, SensorData.RectSize.x / 2, SensorPosi.position.x - SensorPosi.rect.width / 2, SensorPosi.position.x + SensorPosi.rect.width / 2, sensorPosition.x),
                           scale(-SensorData.RectSize.y / 2, SensorData.RectSize.y / 2, SensorPosi.position.y - SensorPosi.rect.height / 2, SensorPosi.position.y + SensorPosi.rect.height / 2, sensorPosition.y),
                           0);
    }

    //호쿠요 매니저에서 받은 위치 데이터를 컨텐츠 위치에 맵핑
    private float scale(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
    {
        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

        return (NewValue);
    }
}
