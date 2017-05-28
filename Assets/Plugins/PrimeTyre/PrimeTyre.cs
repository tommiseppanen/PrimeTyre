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
        private float _suspensionTravel = 0.5f;

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

        void FixedUpdate()
        {
            //TODO: suspension
            var normalForce = 250.0f *  9.81f;

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

            var longitudinalCarSpeed = Vector3.Dot(Rigid.velocity, Rigid.transform.forward);
            var longitudinalTyreSpeed = AngularSpeed * WheelRadius;
            var slipdelta = (longitudinalTyreSpeed - longitudinalCarSpeed) -
                            Mathf.Abs(longitudinalCarSpeed) * _differentialSlipRatio;
            slipdelta /= RelaxationLenght;
            _differentialSlipRatio += slipdelta * Time.fixedDeltaTime;

            var tyreForce = Mathf.Sign(_differentialSlipRatio) * normalForce * ForwardFriction.CalculateCoefficient(_differentialSlipRatio);
            Rigid.AddForce(transform.forward * tyreForce);

            var rollingResistanceForce = longitudinalTyreSpeed * _rollingResistance * normalForce;
            var angularAcceleration = (MotorTorque - (rollingResistanceForce + tyreForce) * WheelRadius) / Inertia;
            AngularSpeed += angularAcceleration * Time.fixedDeltaTime;
        }
    }
}