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

        private float _angularSpeed;
        private float _angularPosition;

        private float _differentialSlipRatio;
        private Vector3 _position;
        private float _previousSuspensionDistance;
        private float _normalForce;

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
            var tyreRotation = Quaternion.Euler((_angularPosition * 180.0f) / Mathf.PI, 0, 0);
            position = GetTyrePosition();
            rotation = Rigid.rotation * tyreRotation;
        }

        private Vector3 GetTyrePosition()
        {
            RaycastHit hit;
            IsGrounded = Physics.Raycast(new Ray(transform.position, -Rigid.transform.up), out hit,
                WheelRadius + _suspensionTravel);
            if (IsGrounded)
            {
                return hit.point + Rigid.transform.up * WheelRadius;
            }
            return transform.position - Rigid.transform.up * _suspensionTravel;
        }

        void Start()
        {
            _previousSuspensionDistance = _suspensionTravel;
        }

        void FixedUpdate()
        {
            _position = GetTyrePosition();
            UpdateSuspension();

            var longitudinalCarSpeed = Vector3.Dot(Rigid.velocity, Rigid.transform.forward);
            var longitudinalTyreSpeed = _angularSpeed * WheelRadius;
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
            _angularSpeed += angularAcceleration * Time.fixedDeltaTime;
            _angularPosition = (_angularPosition + _angularSpeed * Time.fixedDeltaTime) % (2*Mathf.PI);
        }

        void UpdateSuspension()
        {
            var distance = Vector3.Distance(transform.position-Rigid.transform.up*_suspensionTravel, _position);
            var springForce = _spring * distance;
            var damperForce = _damper * ((distance - _previousSuspensionDistance) / Time.fixedDeltaTime);
            _normalForce = springForce + damperForce;
            if (IsGrounded)
                Rigid.AddForceAtPosition(_normalForce * Rigid.transform.up, _position);
            _previousSuspensionDistance = distance;
        }
    }
}