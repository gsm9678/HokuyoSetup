using UnityEngine;

//OSC 통신을 통해 HokuyoManager로부터 데이터 받아오는 스크립트
public class OSCManager : Singleton<OSCManager>
{
    public OSC _isOSC;

    public SensorDataFormat[] SensorData; //호쿠요로부터 받은 데이터를 저장

    //초기화
    private void Start()
    {
        SetOSC_EventHandler();
        SensorData = new SensorDataFormat[System.Enum.GetValues(typeof(SensorEnum)).Length];
        for (int i = 0; i < SensorData.Length; i++)
            SensorData[i] = new SensorDataFormat();
    }

    #region FrontSensor
    //센서 연결상태 최신화, 호쿠요 매니저에서 설정한 방 크기 값 받기
    public void getFrontStartMessage(OscMessage message)
    {
        StartSensor(SensorEnum.Front, message);

        if (!SensorActiveState.Instance.SensorState[((int)SensorEnum.Front)])
        {
            SensorActiveState.Instance.SensorState[((int)SensorEnum.Front)] = true;
            Debug.Log("정면 센서 연결");
        }
    }

    //인식한 물체의 위치값 받기
    public void getFrontSensorMessage(OscMessage message)
    {
        AddSensorData(SensorEnum.Front, message);
    }

    public void getFrontStopMessage(OscMessage message)
    {
    }

    //센서 종료 신호 받기
    public void FrontSensorQuit(OscMessage message)
    {
        SensorActiveState.Instance.SensorState[((int)SensorEnum.Front)] = false;
        Debug.Log("정면 센서 종료");
    }
    #endregion

    #region BackSensor
    public void getBackStartMessage(OscMessage message)
    {
        StartSensor(SensorEnum.Back, message);

        if (!SensorActiveState.Instance.SensorState[((int)SensorEnum.Back)])
        {
            SensorActiveState.Instance.SensorState[((int)SensorEnum.Back)] = true;
            Debug.Log("후면 센서 연결");
        }
    }

    public void getBackSensorMessage(OscMessage message)
    {
        AddSensorData(SensorEnum.Back, message);
    }

    public void getBackStopMessage(OscMessage message)
    {
    }

    public void BackSensorQuit(OscMessage message)
    {
        SensorActiveState.Instance.SensorState[((int)SensorEnum.Back)] = false;
        Debug.Log("후면 센서 종료");
    }
    #endregion

    #region RightSensor
    public void getRightStartMessage(OscMessage message)
    {
        StartSensor(SensorEnum.Right, message);

        if (!SensorActiveState.Instance.SensorState[((int)SensorEnum.Right)])
        {
            SensorActiveState.Instance.SensorState[((int)SensorEnum.Right)] = true;
            Debug.Log("우면 센서 연결");
        }
    }

    public void getRightSensorMessage(OscMessage message)
    {
        AddSensorData(SensorEnum.Right, message);
    }

    public void getRightStopMessage(OscMessage message)
    {
    }

    public void RightSensorQuit(OscMessage message)
    {
        SensorActiveState.Instance.SensorState[((int)SensorEnum.Right)] = false;
        Debug.Log("우면 센서 종료");
    }
    #endregion

    #region LeftSensor
    public void getLeftStartMessage(OscMessage message)
    {
        StartSensor(SensorEnum.Left, message);

        if (!SensorActiveState.Instance.SensorState[((int)SensorEnum.Left)])
        {
            SensorActiveState.Instance.SensorState[((int)SensorEnum.Left)] = true;
            Debug.Log("좌면 센서 연결");
        }
    }

    public void getLeftSensorMessage(OscMessage message)
    {
        AddSensorData(SensorEnum.Left, message);
    }

    public void getLeftStopMessage(OscMessage message)
    {
    }

    public void LeftSensorQuit(OscMessage message)
    {
        SensorActiveState.Instance.SensorState[((int)SensorEnum.Left)] = false;
        Debug.Log("좌면 센서 종료");
    }
    #endregion

    #region DownSensor
    public void getDownStartMessage(OscMessage message)
    {
        StartSensor(SensorEnum.Down, message);

        if (!SensorActiveState.Instance.SensorState[((int)SensorEnum.Down)])
        {
            SensorActiveState.Instance.SensorState[((int)SensorEnum.Down)] = true;
            Debug.Log("바닥 센서 연결");
        }
    }

    public void getDownSensorMessage(OscMessage message)
    {
        AddSensorData(SensorEnum.Down, message);
    }

    public void getDownStopMessage(OscMessage message)
    {
    }

    public void DownSensorQuit(OscMessage message)
    {
        SensorActiveState.Instance.SensorState[((int)SensorEnum.Down)] = false;
        Debug.Log("바닥 센서 종료");
    }
    #endregion

    #region TrackedSensor
    private void StartSensor(SensorEnum sensorEnum, OscMessage message)
    {
        SensorDataFormat sensorData = SensorData[(int)sensorEnum];
        sensorData.RectSize = new Vector2(message.GetFloat(0), message.GetFloat(1));
        sensorData.Position.Clear();
        sensorData.TrackedObjects.Clear();
    }

    private void AddSensorData(SensorEnum sensorEnum, OscMessage message)
    {
        SensorDataFormat sensorData = SensorData[(int)sensorEnum];

        if (IsTrackedDataMessage(message))
        {
            TrackedSensorDataFormat trackedObject = new TrackedSensorDataFormat
            {
                Id = message.GetInt(0),
                Position = new Vector3(message.GetFloat(1), message.GetFloat(2), 0),
                State = ParseTrackState(message.GetString(3))
            };

            sensorData.TrackedObjects.Add(trackedObject);
            sensorData.Position.Add(trackedObject.Position);
            return;
        }

        sensorData.Position.Add(new Vector3(message.GetFloat(0), message.GetFloat(1), 0));
    }

    private bool IsTrackedDataMessage(OscMessage message)
    {
        return message.values.Count >= 4 && message.values[3] is string;
    }

    private TrackState ParseTrackState(string value)
    {
        switch (value.ToLowerInvariant())
        {
            case "occluded":
                return TrackState.Occluded;
            case "merged":
                return TrackState.Merged;
            case "lost":
                return TrackState.Lost;
            default:
                return TrackState.Active;
        }
    }
    #endregion

    private void SetOSC_EventHandler()
    {
        _isOSC.SetAddressHandler("/Front/Start", getFrontStartMessage);
        _isOSC.SetAddressHandler("/Front/Data", getFrontSensorMessage);
        _isOSC.SetAddressHandler("/Front/End", getFrontStopMessage);
        _isOSC.SetAddressHandler("/Front/Quit", FrontSensorQuit);
        _isOSC.SetAddressHandler("/Back/Start", getBackStartMessage);
        _isOSC.SetAddressHandler("/Back/Data", getBackSensorMessage);
        _isOSC.SetAddressHandler("/Back/End", getBackStopMessage);
        _isOSC.SetAddressHandler("/Back/Quit", BackSensorQuit);
        _isOSC.SetAddressHandler("/Left/Start", getLeftStartMessage);
        _isOSC.SetAddressHandler("/Left/Data", getLeftSensorMessage);
        _isOSC.SetAddressHandler("/Left/End", getLeftStopMessage);
        _isOSC.SetAddressHandler("/Left/Quit", LeftSensorQuit);
        _isOSC.SetAddressHandler("/Right/Start", getRightStartMessage);
        _isOSC.SetAddressHandler("/Right/Data", getRightSensorMessage);
        _isOSC.SetAddressHandler("/Right/End", getRightStopMessage);
        _isOSC.SetAddressHandler("/Right/Quit", RightSensorQuit);
        _isOSC.SetAddressHandler("/Down/Start", getDownStartMessage);
        _isOSC.SetAddressHandler("/Down/Data", getDownSensorMessage);
        _isOSC.SetAddressHandler("/Down/End", getDownStopMessage);
        _isOSC.SetAddressHandler("/Down/Quit", DownSensorQuit);
    }
}
