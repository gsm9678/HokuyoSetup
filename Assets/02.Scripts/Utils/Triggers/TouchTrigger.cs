using UnityEngine;

namespace SensorTrigger.Base
{
    public abstract class TouchTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player" || other.tag == "TouchPoint")
            {
                TriggerEnter();
            }
        }
        abstract public void TriggerEnter();
    }
}
