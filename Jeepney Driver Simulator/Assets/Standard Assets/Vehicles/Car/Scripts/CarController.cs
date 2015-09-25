using System;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    internal enum CarDriveType
    {
        FrontWheelDrive,
        RearWheelDrive,
        FourWheelDrive
    }

    internal enum SpeedType
    {
        MPH,
        KPH
    }

	internal enum TransmissionType {
		Manual,
		Automatic
	}

    public class CarController : MonoBehaviour
    {
        [SerializeField] private CarDriveType m_CarDriveType = CarDriveType.FourWheelDrive;
        [SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4];
        [SerializeField] private GameObject[] m_WheelMeshes = new GameObject[4];
        [SerializeField] private WheelEffects[] m_WheelEffects = new WheelEffects[4];
        [SerializeField] private Vector3 m_CentreOfMassOffset;
        [SerializeField] private float m_MaximumSteerAngle;
        [Range(0, 1)] [SerializeField] private float m_SteerHelper; // 0 is raw physics , 1 the car will grip in the direction it is facing
        [Range(0, 1)] [SerializeField] private float m_TractionControl; // 0 is no traction control, 1 is full interference
        [SerializeField] private float m_FullTorqueOverAllWheels;
        [SerializeField] private float m_ReverseTorque;
        [SerializeField] private float m_MaxHandbrakeTorque;
        [SerializeField] private float m_Downforce = 100f;
        [SerializeField] private SpeedType m_SpeedType;
        [SerializeField] private float m_Topspeed = 200;
        [SerializeField] private static int NoOfGears = 5;
        [SerializeField] private float m_RevRangeBoundary = 1f;
        [SerializeField] private float m_SlipLimit;
        [SerializeField] private float m_BrakeTorque;
		[SerializeField] private TransmissionType m_TransmissionType = TransmissionType.Automatic;
		[SerializeField] private float[] m_GearRatios = new float[6];
		[SerializeField] private float m_FinalRatio;
		[SerializeField] private float m_MaxRPM;
		[SerializeField] private float m_HardRPM;
		[SerializeField] private float m_MinRPM;
		[SerializeField] private float m_StartTorque;
		[SerializeField] private float m_MaxTorque;
		[SerializeField] private float m_MinTorque;
		[SerializeField] private float m_FallOffTorque;
		[SerializeField] private float m_TireCircumference;

		private float m_RPM;
		public float m_OutputTorque;
        private Quaternion[] m_WheelMeshLocalRotations;
        private Vector3 m_Prevpos, m_Pos;
        private float m_SteerAngle;
        private int m_GearNum;
        private float m_GearFactor;
        private float m_OldRotation;
        private float m_CurrentTorque;
        private Rigidbody m_Rigidbody;
        private const float k_ReversingThreshold = 0.01f;

        public bool Skidding { get; private set; }
        public float BrakeInput { get; private set; }
        public float CurrentSteerAngle{ get { return m_SteerAngle; }}
		public float CurrentSpeed{ get { return m_Rigidbody.velocity.magnitude*3.6f; }}
        public float MaxSpeed{get { return m_Topspeed; }}
        public float Revs { get; private set; }
        public float AccelInput { get; private set; }

        // Use this for initialization
        private void Start()
        {
//            m_WheelMeshLocalRotations = new Quaternion[4];
//            for (int i = 0; i < 4; i++)
//            {
//                m_WheelMeshLocalRotations[i] = m_WheelMeshes[i].transform.localRotation;
//            }
            m_WheelColliders[0].attachedRigidbody.centerOfMass = m_CentreOfMassOffset;

            m_MaxHandbrakeTorque = float.MaxValue;

			if (m_TransmissionType == TransmissionType.Manual)
				m_GearNum = 1;
            m_Rigidbody = GetComponent<Rigidbody>();
            m_CurrentTorque = m_FullTorqueOverAllWheels - (m_TractionControl*m_FullTorqueOverAllWheels);
			m_RPM = m_MinRPM;
        }

		private float GetTorquefromRPM(float x) {
			float m = Math.Abs (x / m_MaxRPM);
			// start point, peak point, fallout point
			float sp = 1000f/m_MaxRPM;
			float pp = 0.6f;
			float fp = 1.15f;
			// torque = a(RPM - peakPoint)^2 + maxTorque)
			//float a = -1*(m_StartTorque - m_MaxTorque)/(sp*sp - 2 * pp * sp + pp*pp );
			if (m < sp) {
				//linear torque
				return 0; //Mathf.Lerp(0, m_StartTorque, m/sp);
			}else if (m < pp) {
				//parabolic torque
				return Mathf.Lerp(m_StartTorque, m_MaxTorque, CurveFactor((m-sp)/pp-sp));//a*(x-pp)*(x-pp) + m_MaxTorque;
			}else if (m < fp){
				//float f = Mathf.SmoothStep(m_MaxTorque, 1, ((m-pp)/(1-pp)*2)/2);
				return Mathf.Lerp(m_MaxTorque, m_FallOffTorque, (m-pp)/(fp-pp)*(m-pp)/(fp-pp));
			}else return 0;
		}

//		public void SetTorquefromRPM() {
//			if(m_RPM < m_MinRPM || m_GearNum < 0)
//				if(m_GearNum < 0)
//					m_OutputTorque = GetTorquefromRPM(m_MinRPM) * m_GearRatios[1] * m_FinalRatio;
//				else
//					m_OutputTorque = GetTorquefromRPM(m_MinRPM) * m_GearRatios[m_GearNum] * m_FinalRatio;
//			else
//				m_OutputTorque = GetTorquefromRPM(m_RPM) * m_GearRatios[m_GearNum] * m_FinalRatio;
//		}

		public void SetRPM() {
			if(m_RPM > m_HardRPM){m_RPM = m_HardRPM; return; }
			if(m_GearNum < 0){
				if( CurrentSpeed < 5f){
					m_RPM = m_MinRPM;
				} else {
					m_OutputTorque = 0;
					m_RPM = 0;
				}
			}else if (CurrentSpeed < 1000f/m_GearRatios[m_GearNum]/m_FinalRatio/60f*m_TireCircumference){
				m_RPM = m_MinRPM;
			}else{
				m_RPM = Math.Abs(CurrentSpeed) / 3.6f * 60f * m_GearRatios[m_GearNum] * m_FinalRatio / m_TireCircumference ;
			}
			return;
		}

		public void TestTorque() {
			for(int i = 0; i < 4500; i+= 50){
				Debug.Log(GetTorquefromRPM(i));
			}
		}

		private void applyThrottle(float throttle, float brakes) {
			if(m_RPM < m_MinRPM || m_GearNum < 0){
				if(m_GearNum == -1) m_OutputTorque = 0;
				else m_OutputTorque = GetTorquefromRPM(m_MinRPM) * m_GearRatios[m_GearNum] * m_FinalRatio;
			}
			else
				m_OutputTorque = GetTorquefromRPM(m_RPM) * m_GearRatios[m_GearNum] * m_FinalRatio;

			var thrustTorque =  m_OutputTorque * throttle;
			if (m_GearNum != -1) {
				switch (m_CarDriveType) {
				case CarDriveType.FourWheelDrive:
					for (int i = 0; i < 4; i++) {
						m_WheelColliders [i].motorTorque = thrustTorque / 4f;
					}
					break;
				
				case CarDriveType.FrontWheelDrive:
					m_WheelColliders [0].motorTorque = m_WheelColliders [1].motorTorque = thrustTorque / 2f;
					break;
				
				case CarDriveType.RearWheelDrive:
					m_WheelColliders [2].motorTorque = m_WheelColliders [3].motorTorque = thrustTorque / 2f;
					break;
				}
			}
			for (int i = 0; i < 4; i++)
			{
				if (CurrentSpeed > 5 && Vector3.Angle(transform.forward, m_Rigidbody.velocity) < 50f)
				{
					m_WheelColliders[i].brakeTorque = m_BrakeTorque*brakes;
				}
				else if (brakes > 0)
				{
					m_WheelColliders[i].brakeTorque = 0f;
					m_WheelColliders[i].motorTorque = -m_ReverseTorque*brakes;
				}
			}
		}

		public void SetGear(int n) {
			m_GearNum = n;
			if (n < -1)
				m_GearNum = -1;
			else if (n > 5)
				m_GearNum = 5;

		}

        private void GearChanging()
        {
            float f = Mathf.Abs(CurrentSpeed/MaxSpeed);
            float upgearlimit = (1/(float) NoOfGears)*(m_GearNum + 1);
            float downgearlimit = (1/(float) NoOfGears)*m_GearNum;

            if (m_GearNum > 0 && f < downgearlimit)
            {
                m_GearNum--;
            }

            if (f > upgearlimit && (m_GearNum < (NoOfGears - 1)))
            {
                m_GearNum++;
            }
        }


        // simple function to add a curved bias towards 1 for a value in the 0-1 range
        private static float CurveFactor(float factor)
        {
			return 1 - (1 - factor)*(1 - factor)*(1 - factor);
        }


        // unclamped version of Lerp, to allow value to exceed the from-to range
        private static float ULerp(float from, float to, float value)
        {
            return (1.0f - value)*from + value*to;
        }


        private void CalculateGearFactor()
        {
            float f = (1/(float) NoOfGears);
            // gear factor is a normalised representation of the current speed within the current gear's range of speeds.
            // We smooth towards the 'target' gear factor, so that revs don't instantly snap up or down when changing gear.
            var targetGearFactor = Mathf.InverseLerp(f*m_GearNum, f*(m_GearNum + 1), Mathf.Abs(CurrentSpeed/MaxSpeed));
            m_GearFactor = Mathf.Lerp(m_GearFactor, targetGearFactor, Time.deltaTime*5f);
        }


        private void CalculateRevs()
        {
            // calculate engine revs (for display / sound)
            // (this is done in retrospect - revs are not used in force/power calculations)
            CalculateGearFactor();
            var gearNumFactor = m_GearNum/(float) NoOfGears;
            var revsRangeMin = ULerp(0f, m_RevRangeBoundary, CurveFactor(gearNumFactor));
            var revsRangeMax = ULerp(m_RevRangeBoundary, 1f, gearNumFactor);
            Revs = ULerp(revsRangeMin, revsRangeMax, m_GearFactor);
        }


        public void Move(float steering, float accel, float footbrake, float handbrake)
        {
            for (int i = 0; i < 4; i++)
            {
                Quaternion quat;
                Vector3 position;
                m_WheelColliders[i].GetWorldPose(out position, out quat);
                m_WheelMeshes[i].transform.position = position;
                m_WheelMeshes[i].transform.rotation = quat;
            }

            //clamp input values
            steering = Mathf.Clamp(steering, -1, 1);
            AccelInput = accel = Mathf.Clamp(accel, 0, 1);
            BrakeInput = footbrake = -1*Mathf.Clamp(footbrake, -1, 0);
            handbrake = Mathf.Clamp(handbrake, 0, 1);

            //Set the steer on the front wheels.
            //Assuming that wheels 0 and 1 are the front wheels.
            m_SteerAngle = steering*m_MaximumSteerAngle;
            m_WheelColliders[0].steerAngle = m_SteerAngle;
            m_WheelColliders[1].steerAngle = m_SteerAngle;

            SteerHelper();

            if (m_TransmissionType == TransmissionType.Automatic)
				ApplyDrive (accel, footbrake);
			else {
				//Debug.Log ("TORQUE1 "+ m_CurrTorque);

				//Debug.Log ("TORQUE2 "+ m_CurrTorque);
				applyThrottle(accel, footbrake);
				//Debug.Log ("GEAR " + m_GearNum);
				//Debug.Log ("RPM " + m_RPM);
				//Debug.Log ("TORQUE3 "+ m_CurrTorque);
				//Debug.Log ("Speed " + CurrentSpeed);
				//Debug.Log ("Brake " + footbrake);

			}

            CapSpeed();
            //Set the handbrake.
            //Assuming that wheels 2 and 3 are the rear wheels.
            if (handbrake > 0f)
            {
                var hbTorque = handbrake*m_MaxHandbrakeTorque;
                m_WheelColliders[2].brakeTorque = hbTorque;
                m_WheelColliders[3].brakeTorque = hbTorque;
            }

			if(m_TransmissionType == TransmissionType.Automatic) {
				CalculateRevs ();
				GearChanging ();
			}
            AddDownForce();
            CheckForWheelSpin();
			TractionControl();
        }


        private void CapSpeed()
        {
            float speed = m_Rigidbody.velocity.magnitude;
            switch (m_SpeedType)
            {
                case SpeedType.MPH:

                    speed *= 2.23693629f;
                    if (speed > m_Topspeed)
                        m_Rigidbody.velocity = (m_Topspeed/2.23693629f) * m_Rigidbody.velocity.normalized;
                    break;

                case SpeedType.KPH:
                    speed *= 3.6f;
                    if (speed > m_Topspeed)
                        m_Rigidbody.velocity = (m_Topspeed/3.6f) * m_Rigidbody.velocity.normalized;
                    break;
            }
        }


        private void ApplyDrive(float accel, float footbrake)
        {

            float thrustTorque;
            switch (m_CarDriveType)
            {
                case CarDriveType.FourWheelDrive:
                    thrustTorque = accel * (m_CurrentTorque / 4f);
                    for (int i = 0; i < 4; i++)
                    {
                        m_WheelColliders[i].motorTorque = thrustTorque;
                    }
                    break;

                case CarDriveType.FrontWheelDrive:
                    thrustTorque = accel * (m_CurrentTorque / 2f);
                    m_WheelColliders[0].motorTorque = m_WheelColliders[1].motorTorque = thrustTorque;
                    break;

                case CarDriveType.RearWheelDrive:
                    thrustTorque = accel * (m_CurrentTorque / 2f);
                    m_WheelColliders[2].motorTorque = m_WheelColliders[3].motorTorque = thrustTorque;
                    break;

            }

            for (int i = 0; i < 4; i++)
            {
                if (CurrentSpeed > 5 && Vector3.Angle(transform.forward, m_Rigidbody.velocity) < 50f)
                {
                    m_WheelColliders[i].brakeTorque = m_BrakeTorque*footbrake;
                }
                else if (footbrake > 0)
                {
                    m_WheelColliders[i].brakeTorque = 0f;
                    m_WheelColliders[i].motorTorque = -m_ReverseTorque*footbrake;
                }
            }
        }


        private void SteerHelper()
        {
            for (int i = 0; i < 4; i++)
            {
                WheelHit wheelhit;
                m_WheelColliders[i].GetGroundHit(out wheelhit);
                if (wheelhit.normal == Vector3.zero)
                    return; // wheels arent on the ground so dont realign the rigidbody velocity
            }

            // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
            if (Mathf.Abs(m_OldRotation - transform.eulerAngles.y) < 10f)
            {
                var turnadjust = (transform.eulerAngles.y - m_OldRotation) * m_SteerHelper;
                Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
                m_Rigidbody.velocity = velRotation * m_Rigidbody.velocity;
            }
            m_OldRotation = transform.eulerAngles.y;
        }


        // this is used to add more grip in relation to speed
        private void AddDownForce()
        {
            m_WheelColliders[0].attachedRigidbody.AddForce(-transform.up*m_Downforce*
                                                         m_WheelColliders[0].attachedRigidbody.velocity.magnitude);
        }


        // checks if the wheels are spinning and is so does three things
        // 1) emits particles
        // 2) plays tiure skidding sounds
        // 3) leaves skidmarks on the ground
        // these effects are controlled through the WheelEffects class
        private void CheckForWheelSpin()
        {
            // loop through all wheels
            for (int i = 0; i < 4; i++)
            {
                WheelHit wheelHit;
                m_WheelColliders[i].GetGroundHit(out wheelHit);

                // is the tire slipping above the given threshhold
                if (Mathf.Abs(wheelHit.forwardSlip) >= m_SlipLimit || Mathf.Abs(wheelHit.sidewaysSlip) >= m_SlipLimit)
                {
                    m_WheelEffects[i].EmitTyreSmoke();

                    // avoiding all four tires screeching at the same time
                    // if they do it can lead to some strange audio artefacts
                    if (!AnySkidSoundPlaying())
                    {
                        m_WheelEffects[i].PlayAudio();
                    }
                    continue;
                }

                // if it wasnt slipping stop all the audio
                if (m_WheelEffects[i].PlayingAudio)
                {
                    m_WheelEffects[i].StopAudio();
                }
                // end the trail generation
                m_WheelEffects[i].EndSkidTrail();
            }
        }

        // crude traction control that reduces the power to wheel if the car is wheel spinning too much
        private void TractionControl()
        {
            WheelHit wheelHit;
            switch (m_CarDriveType)
            {
                case CarDriveType.FourWheelDrive:
                    // loop through all wheels
                    for (int i = 0; i < 4; i++)
                    {
                        m_WheelColliders[i].GetGroundHit(out wheelHit);

                        AdjustTorque(wheelHit.forwardSlip);
                    }
                    break;

                case CarDriveType.RearWheelDrive:
                    m_WheelColliders[2].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);

                    m_WheelColliders[3].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);
                    break;

                case CarDriveType.FrontWheelDrive:
                    m_WheelColliders[0].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);

                    m_WheelColliders[1].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);
                    break;
            }
        }


        private void AdjustTorque(float forwardSlip)
        {
			if(m_TransmissionType == TransmissionType.Manual){
				if (forwardSlip >= m_SlipLimit && m_OutputTorque >= 0)
				{
					m_OutputTorque -= 200 * m_TractionControl;
				}
				else if (m_OutputTorque >=0)
				{
					m_OutputTorque -= 0.2f* m_OutputTorque;
				}
			}else {
				if (forwardSlip >= m_SlipLimit && m_CurrentTorque >= 0)
				{
					m_CurrentTorque -= 10 * m_TractionControl;
				}
				else
				{
					m_CurrentTorque += 10 * m_TractionControl;
					if (m_CurrentTorque > m_FullTorqueOverAllWheels)
					{
						m_CurrentTorque = m_FullTorqueOverAllWheels;
					}
				}
			}
        }


        private bool AnySkidSoundPlaying()
        {
            for (int i = 0; i < 4; i++)
            {
                if (m_WheelEffects[i].PlayingAudio)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
