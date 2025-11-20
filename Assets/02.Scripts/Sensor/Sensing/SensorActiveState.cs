public class SensorActiveState : Singleton<SensorActiveState>
{
    public bool[] SensorState; //센서의 연결상태 기록

    //초기화
    private void OnEnable()
    {
        SensorState = new bool[System.Enum.GetValues(typeof(SensorEnum)).Length];
        for (int i = 0; i < SensorState.Length; i++)
            SensorState[i] = false;
    }
}
