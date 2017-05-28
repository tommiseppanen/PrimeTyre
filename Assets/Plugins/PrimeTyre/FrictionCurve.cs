using UnityEngine;

namespace Assets.Plugins.PrimeTyre
{
    [System.Serializable]
    public class FrictionCurve
    {
        [SerializeField]
        private float _extremumSlip;
        [SerializeField]
        private float _extremumValue;
        [SerializeField]
        private float _asymptoteSlip;
        [SerializeField]
        private float _asymptoteValue;
        [SerializeField]
        private float _stiffness;

        public float ExtremumSlip
        {
            get { return _extremumSlip; }
            set { _extremumSlip = value; }
        }

        public float ExtremumValue
        {
            get { return _extremumValue; }
            set { _extremumValue = value; }
        }

        public float AsymptoteSlip
        {
            get { return _asymptoteSlip; }
            set { _asymptoteSlip = value; }
        }

        public float AsymptoteValue
        {
            get { return _asymptoteValue; }
            set { _asymptoteValue = value; }
        }

        public float Stiffness
        {
            get { return _stiffness; }
            set { _stiffness = value; }
        }

        public float CalculateCoefficient(float slip)
        {
            var tractionCoefficient = _asymptoteValue;
            var absoluteSlip = Mathf.Abs(slip);
            if (absoluteSlip <= _extremumSlip)
            {
                tractionCoefficient = (_extremumValue / _extremumSlip) * absoluteSlip;
            }         
            else if (absoluteSlip > _extremumSlip && absoluteSlip < _asymptoteSlip)
            {
                tractionCoefficient = ((_asymptoteValue - _extremumValue) / (_asymptoteSlip - _extremumSlip))
                                      * (absoluteSlip - _extremumSlip) + _extremumValue;
            }
            return tractionCoefficient*_stiffness;
        }
    }
}
