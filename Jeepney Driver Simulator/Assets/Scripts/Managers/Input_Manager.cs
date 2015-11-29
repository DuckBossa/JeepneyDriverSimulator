using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class Input_Manager : MonoBehaviour {
	public static event Action<float,float> Move; // pedal
	public static event Action<float> Break; //stop twice
	public static event Action<float> Steering; // steering wheel, parameter is current degrees
	public static event Action<int> SetGear; // shift
	public static event Action Honk; //one of the buttons
	public static event Action SignalLeft; //left paddle
	public static event Action SignalRight; // right paddle
	public static event Action EmbarkPassenger;//should be in the passenger manager

	//Logitech G27
	LogitechGSDK.LogiControllerPropertiesData properties;
	
	//Dev
	public KeyCode mf = KeyCode.W;
	public KeyCode mb = KeyCode.S;
	public KeyCode ml = KeyCode.A;
	public KeyCode mr = KeyCode.D;
	public KeyCode hk = KeyCode.H;
	public KeyCode g1 = KeyCode.Alpha1;
	public KeyCode g2 = KeyCode.Alpha2;
	public KeyCode g3 = KeyCode.Alpha3;
	public KeyCode g4 = KeyCode.Alpha4;
	public KeyCode g5 = KeyCode.Alpha5;
	public KeyCode gR = KeyCode.BackQuote;
	public KeyCode cl = KeyCode.Tab;
	public KeyCode sl = KeyCode.Less;
	public KeyCode sr = KeyCode.Greater;
	public KeyCode em = KeyCode.E;
	public int wheelDegrees = 900;

	private float steeringWheelDegrees;
	private float steeringWheelIntensity;
	private float gasPedal;
	private float breakPedal;
	private float clutchPedal;

	private float gasbreak;
	private float direction;
	private int gear;
	private int prev_gear;

	public bool isHardware;

	public void Start(){
		SetupSteeringWheel();
		prev_gear = -2;
		gear = -1;
	}

	private void SetupSteeringWheel(){
		LogitechGSDK.LogiSteeringInitialize(false);
//		properties.allowGameSettings = true;
		properties.forceEnable = true;
//		properties.overallGain = 80;
//		properties.springGain = 80;
//		properties.damperGain = 540;
		properties.defaultSpringEnabled = true;
//		properties.defaultSpringGain = 80;
		properties.wheelRange = wheelDegrees;
		LogitechGSDK.LogiSetPreferredControllerProperties(properties);
		steeringWheelDegrees = 0f;
	}

	public void Update(){
		resetKeys ();
		if(Input.GetKeyDown(em))
			if(EmbarkPassenger != null)
				EmbarkPassenger();
		if (isHardware) DrivingInput();
		else DevInput();
	}

	public void FixedUpdate(){
		if (isHardware && LogitechGSDK.LogiIsConnected (0)) {
			if ((SetGear != null && clutchPedal > 0.5f && prev_gear == -1 && gasPedal < 0.3f) || gear == -1) {
				SetGear (gear);
			}
		} else {
			if ((SetGear != null && clutchPedal > 0.5f) || gear == -1) {
				SetGear (gear);
			}
		}
		prev_gear = gear;
		
		if(Steering != null){
			Steering(steeringWheelDegrees);
		}
		if(Move != null && (gasPedal > 0 ||  breakPedal < 0 || direction != 0)){
			Move(direction,gasbreak);
			
		}
	}
	
	//	public void GameInput(){}
	public void resetKeys(){
		gasbreak = 0;
		direction = 0;
		gasPedal = 0;
		breakPedal = 0;
		clutchPedal = 0;
	}
	private void DevInput(){
		if(Input.GetKey(mf)){
			gasbreak += 1;
			gasPedal = 1f;
		}		

		if(Input.GetKey(mb)){
			gasbreak -= 1;
			breakPedal = 1f;
		}

		if (Input.GetKey(ml)) {
			direction -= 0.5f;
			steeringWheelDegrees = -90f;
		}

		if (Input.GetKey(mr)) {
			direction += 0.5f;
			steeringWheelDegrees = 90f;
		}

		if (Input.GetKey(cl)) {
			clutchPedal = 1f;
		}

		if (Input.GetKey(g1)) {
			gear = 1;
		} else if (Input.GetKey(g2)) {
			gear = 2;
		} else if (Input.GetKey(g3)) {
			gear = 3;
		} else if (Input.GetKey(g4)) {
			gear = 4;
		} else if (Input.GetKey(g5)) {
			gear = 5;
		} else if (Input.GetKey(gR)) {
			gear = 0;
		} else if (Input.GetKey(cl)) {
			gear = -1;
		}
		
		if (Move != null) {
			Move(direction, gasbreak);
		} 
	}
	

	private void DrivingInput(){
		if(LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0)){
			LogitechGSDK.LogiControllerPropertiesData actualProperties = new LogitechGSDK.LogiControllerPropertiesData();
			LogitechGSDK.DIJOYSTATE2ENGINES rec;
			rec = LogitechGSDK.LogiGetStateUnity(0);

			direction = Mathf.Lerp(/*wheelDegrees/2*/1,/*-wheelDegrees/2*/-1,(-rec.lX /32767f + 1f)/2f);
			steeringWheelDegrees = Mathf.Lerp(wheelDegrees/2,-wheelDegrees/2,(-rec.lX /32767f + 1f)/2f);
			gasPedal = Mathf.Lerp(0,1,( -rec.lY/32767f + 1)/2 );
			breakPedal = Mathf.Lerp(0,-1,( -rec.lRz/32767f + 1)/2);
			clutchPedal = Mathf.Lerp(0,1,( -rec.rglSlider[1]/32767f + 1)/2);
			gasbreak = gasPedal + breakPedal;
			gear = -1;
			for(int i = 8; i < 14; i++){
				if(rec.rgbButtons[i] == 128){
					gear = i - 7;
					if(gear == 6){
						gear = 0;
					}
					break;
				}
				gear = -1;
			}
			


		}
	}


}
