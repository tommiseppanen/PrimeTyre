using UnityEngine;

namespace Assets.Plugins.PrimeTyre
{
    public class PrimeTyre : MonoBehaviour
    {

        [SerializeField]
        private float WheelRadius = 0.3f;
        [SerializeField]
        private float Inertia = 1.3f;
        [SerializeField]
        private Rigidbody Rigid;

        [SerializeField]
        private float _suspensionTravel = 0.3f;

        [SerializeField]
        private float _spring = 35000.0f;

        [SerializeField]
        private float _damper = 4500.0f;

        [SerializeField]
        private float _rollingResistance = 0.001f;

        [SerializeField]
        private FrictionCurve ForwardFriction = new FrictionCurve
        {
            ExtremumSlip = 0.4f,
            ExtremumValue = 1.0f,
            AsymptoteSlip = 0.8f,
            AsymptoteValue = 0.5f,
            Stiffness = 1.0f
        };

        [SerializeField]
        private FrictionCurve SidewaysFriction = new FrictionCurve
        {
            ExtremumSlip = 0.4f,
            ExtremumValue = 1.0f,
            AsymptoteSlip = 0.8f,
            AsymptoteValue = 0.5f,
            Stiffness = 1.0f
        };

        public float MotorTorque { get; set; }
        public float BrakeTorque { get; set; }

        public bool IsGrounded { get; private set; }

        public float AngularSpeed { get; private set; }

        private float _differentialSlipRatio;
        private Vector3 _position;
        private float _previousSuspensionDistance;
        private float _normalForce { get; set; }

        private const float RelaxationLenght = 0.0914f;

        public bool GetGroundHit(out TyreHit hit)
        {
            hit = new TyreHit();
            if (IsGrounded)
                hit.ForwardSlip = _differentialSlipRatio;
            return IsGrounded;
        }

        public void GetWorldPose(out Vector3 position, out Quaternion rotation)
        {
            position = _position;
            rotation = Quaternion.identity;
        }

        void Start()
        {
            _previousSuspensionDistance = _suspensionTravel;
        }

        void FixedUpdate()
        {
            RaycastHit hit;
            IsGrounded = Physics.Raycast(new Ray(transform.position, -transform.up), out hit,
                WheelRadius + _suspensionTravel);
            if (IsGrounded)
            {
                _position = hit.point+transform.up* WheelRadius;
            }
            else
            {
                _position = transform.position;
            }

            UpdateSuspension();

            var longitudinalCarSpeed = Vector3.Dot(Rigid.velocity, Rigid.transform.forward);
            var longitudinalTyreSpeed = AngularSpeed * WheelRadius;
            var slipdelta = (longitudinalTyreSpeed - longitudinalCarSpeed) -
                            Mathf.Abs(longitudinalCarSpeed) * _differentialSlipRatio;
            slipdelta /= RelaxationLenght;
            _differentialSlipRatio += slipdelta * Time.fixedDeltaTime;

            var tyreForce = Mathf.Sign(_differentialSlipRatio) * _normalForce * ForwardFriction.CalculateCoefficient(_differentialSlipRatio);
            //TODO: substract rolling resistance
            if (IsGrounded)
                Rigid.AddForceAtPosition(transform.forward * tyreForce, _position);

            var rollingResistanceForce = longitudinalTyreSpeed * _rollingResistance * _normalForce;
            var angularAcceleration = (MotorTorque - (rollingResistanceForce + tyreForce) * WheelRadius) / Inertia;
            AngularSpeed += angularAcceleration * Time.fixedDeltaTime;
        }

        void UpdateSuspension()
        {
            var distance = Vector3.Distance(transform.position-transform.up*_suspensionTravel, _position);
            var springForce = _spring * distance;
            var damperForce = _damper * ((distance - _previousSuspensionDistance) / Time.fixedDeltaTime);
            _normalForce = springForce + damperForce;
            if (IsGrounded)
                Rigid.AddForceAtPosition(_normalForce * transform.up, transform.position);
            Debug.Log(_spring * distance);
            _previousSuspensionDistance = distance;
        }
    }
}