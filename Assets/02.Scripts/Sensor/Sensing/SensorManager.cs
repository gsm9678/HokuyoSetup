using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorManager : MonoBehaviour
{
    public SensorEnum sensorEnum;

    private OSCManager m_senserData;
    private SensorDataFormat SensorData;
    List<Vector3> vector3 = new List<Vector3>();
    List<Vector3> worldVector3 = new List<Vector3>();
    List<TrackedSensorDataFormat> trackedObjects = new List<TrackedSensorDataFormat>();
    List<TrackedSensorDataFormat> worldTrackedObjects = new List<TrackedSensorDataFormat>();

    [SerializeField]
    RectTransform SensorPosi;//맵핑할 대상

    [Header("Sprite Renderer Map")]
    [SerializeField] private Transform spriteMapRoot;
    [SerializeField] private SpriteRenderer spriteMapRenderer;
    [SerializeField] private bool syncSpriteMapWithSensorPosi = true;

    private readonly Vector3[] sensorPosiWorldCorners = new Vector3[4];
    private Vector2 spriteMapSize;

    void Start()
    {
        m_senserData = OSCManager.Instance;
        UpdateSpriteMap();
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
            worldVector3.Clear();
            trackedObjects.Clear();
            worldTrackedObjects.Clear();
            UpdateSpriteMap();

            for (int i = 0; i < SensorData.Position.Count; i++)//호쿠요 센서 로우 데이터를 컨텐츠에서 사용할 수 있게 맵핑
            {
                vector3.Add(MapSensorPosition(SensorData.Position[i]));
                worldVector3.Add(MapSensorWorldPosition(SensorData.Position[i]));
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
                worldTrackedObjects.Add(new TrackedSensorDataFormat
                {
                    Id = rawObject.Id,
                    Position = MapSensorWorldPosition(rawObject.Position),
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
                worldVector3.Clear();
                trackedObjects.Clear();
                worldTrackedObjects.Clear();
                MousePosition = Input.mousePosition;

                Vector3 localPosition = new Vector3(scale(-camera.pixelWidth / 2, camera.pixelWidth / 2, SensorPosi.position.x - SensorPosi.rect.width / 2, SensorPosi.position.x + SensorPosi.rect.width / 2, MousePosition.x - camera.pixelWidth / 2),
                                                    scale(-camera.pixelHeight / 2, camera.pixelHeight / 2, SensorPosi.position.y - SensorPosi.rect.height / 2, SensorPosi.position.y + SensorPosi.rect.height / 2, MousePosition.y - camera.pixelHeight / 2),
                                                    0);

                vector3.Add(localPosition);

                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(SensorPosi, MousePosition, camera, out Vector3 worldPosition))
                    worldVector3.Add(worldPosition);
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

    public List<Vector3> getSensorWorldVector()
    {
        return worldVector3;
    }

    public List<TrackedSensorDataFormat> getTrackedSensorObjects()
    {
        return trackedObjects;
    }

    public List<TrackedSensorDataFormat> getTrackedSensorWorldObjects()
    {
        return worldTrackedObjects;
    }

    public Vector2 GetSpriteMapSize()
    {
        return spriteMapSize;
    }

    public Vector3 GetSpriteMapCenter()
    {
        if (SensorPosi == null)
            return Vector3.zero;

        SensorPosi.GetWorldCorners(sensorPosiWorldCorners);
        return (sensorPosiWorldCorners[0] + sensorPosiWorldCorners[2]) * 0.5f;
    }

    private Vector3 MapSensorPosition(Vector3 sensorPosition)
    {
        return new Vector3(scale(-SensorData.RectSize.x / 2, SensorData.RectSize.x / 2, SensorPosi.position.x - SensorPosi.rect.width / 2, SensorPosi.position.x + SensorPosi.rect.width / 2, sensorPosition.x),
                           scale(-SensorData.RectSize.y / 2, SensorData.RectSize.y / 2, SensorPosi.position.y - SensorPosi.rect.height / 2, SensorPosi.position.y + SensorPosi.rect.height / 2, sensorPosition.y),
                           0);
    }

    private Vector3 MapSensorWorldPosition(Vector3 sensorPosition)
    {
        if (SensorPosi == null || SensorData == null)
            return Vector3.zero;

        SensorPosi.GetWorldCorners(sensorPosiWorldCorners);

        Vector3 bottomLeft = sensorPosiWorldCorners[0];
        Vector3 topLeft = sensorPosiWorldCorners[1];
        Vector3 topRight = sensorPosiWorldCorners[2];
        Vector3 bottomRight = sensorPosiWorldCorners[3];

        float normalizedX = Mathf.InverseLerp(-SensorData.RectSize.x / 2, SensorData.RectSize.x / 2, sensorPosition.x);
        float normalizedY = Mathf.InverseLerp(-SensorData.RectSize.y / 2, SensorData.RectSize.y / 2, sensorPosition.y);

        Vector3 bottom = Vector3.Lerp(bottomLeft, bottomRight, normalizedX);
        Vector3 top = Vector3.Lerp(topLeft, topRight, normalizedX);

        return Vector3.Lerp(bottom, top, normalizedY);
    }

    private void UpdateSpriteMap()
    {
        if (SensorPosi == null)
            return;

        SensorPosi.GetWorldCorners(sensorPosiWorldCorners);

        float width = Vector3.Distance(sensorPosiWorldCorners[0], sensorPosiWorldCorners[3]);
        float height = Vector3.Distance(sensorPosiWorldCorners[0], sensorPosiWorldCorners[1]);
        spriteMapSize = new Vector2(width, height);

        if (!syncSpriteMapWithSensorPosi)
            return;

        Transform mapTransform = spriteMapRoot != null ? spriteMapRoot : spriteMapRenderer != null ? spriteMapRenderer.transform : null;
        if (mapTransform == null)
            return;

        mapTransform.position = GetSpriteMapCenter();
        mapTransform.rotation = SensorPosi.rotation;
        mapTransform.localScale = Vector3.one;

        if (mapTransform is RectTransform mapRectTransform)
        {
            mapRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, spriteMapSize.x);
            mapRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, spriteMapSize.y);
        }

        if (spriteMapRenderer != null)
            ResizeSpriteRenderer(spriteMapRenderer, mapTransform, spriteMapSize);
    }

    private void ResizeSpriteRenderer(SpriteRenderer targetRenderer, Transform mapTransform, Vector2 size)
    {
        if (targetRenderer.sprite == null)
            return;

        targetRenderer.drawMode = SpriteDrawMode.Simple;

        if (targetRenderer.transform == mapTransform)
            return;

        Vector2 spriteSize = targetRenderer.sprite.bounds.size;
        if (spriteSize.x == 0 || spriteSize.y == 0)
            return;

        targetRenderer.transform.localPosition = Vector3.zero;
        targetRenderer.transform.localRotation = Quaternion.identity;
        targetRenderer.transform.localScale = new Vector3(size.x / spriteSize.x, size.y / spriteSize.y, targetRenderer.transform.localScale.z);
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
