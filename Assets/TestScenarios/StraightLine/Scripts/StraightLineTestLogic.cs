using UnityEngine;

namespace Assets.TestScenarios.StraightLine.Scripts
{
    public static class StraigtLineTestLogic {
        public static float GetTorque(float currentTorque, float slip, float slipLimit, out bool tractionControl)
        {
            tractionControl = false;

            if (Time.time < 4.0f)
                return 200.0f;

            if (slip < slipLimit)
                return currentTorque + 2.0f;

            if (currentTorque > 1.0f && slip > slipLimit)
            {
                tractionControl = true;
                return currentTorque - 2.0f;
            }
            
            return currentTorque;
        }
    }
}
