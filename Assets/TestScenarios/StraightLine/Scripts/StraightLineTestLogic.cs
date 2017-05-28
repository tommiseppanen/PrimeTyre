using UnityEngine;

namespace Assets.TestScenarios.StraightLine.Scripts
{
    public static class StraigtLineTestLogic {
        public static float GetTorque(float currentTorque, float slip1, float slip2, float slipLimit, out bool tractionControl)
        {
            tractionControl = false;

            if (Time.time < 4.0f)
                return 200.0f;

            if (slip1 < slipLimit && slip2 < slipLimit)
                return currentTorque + 2.0f;

            tractionControl = true;
            return currentTorque - 2.0f;
        }
    }
}
