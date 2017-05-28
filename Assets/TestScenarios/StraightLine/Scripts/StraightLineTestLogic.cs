using UnityEngine;

namespace Assets.TestScenarios.StraightLine.Scripts
{
    public static class StraigtLineTestLogic {
        public static float GetTorque(float currentTorque, float slip, float slipLimit, out bool tractionControl)
        {
            tractionControl = false;

            if (Time.time < 4.0f)
                return 200.0f;

            /*if (Time.time < 14.0f)
                return 3.0f*200.0f;
            else
            {
                return 0;
            }*/

            if (slip < slipLimit)
                return currentTorque + 1.0f;

            if (currentTorque > 1.0f && slip > slipLimit)
            {
                tractionControl = true;
                return currentTorque - 1.0f;
            }
            
            return currentTorque;
        }
    }
}
