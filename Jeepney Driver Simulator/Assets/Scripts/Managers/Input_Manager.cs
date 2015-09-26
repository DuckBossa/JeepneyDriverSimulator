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
	public KeyCode sl = KeyCode.Less;
	public KeyCode sr = KeyCode.Greater;
	public KeyCode em = KeyCode.E;
	public int wheelDegrees = 900;

	private float steeringWheelDegrees;
	private float steeringWheelIntensity;
	private float gasPedal;
	private float breakPedal;
	private float clutchPedal;
	private int gear;

	private float gasbreak;
	private float direction;
	private int prev_gear;

	public bool isHardware;


	public void Start(){
		SetupSteeringWheel();
		prev_gear = -2;
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
		//ADD FIXED UPDATE RESOLUTION LISTEN HERE
		DrivingInput();
		if(Input.GetKeyDown(em))
			if(EmbarkPassenger != null)
				EmbarkPassenger();		
//		if (isHardware) DrivingInput();
//		else DevInput();
	}

	public void FixedUpdate(){
		if(LogitechGSDK.LogiIsConnected(0)){
			if((SetGear!= null && clutchPedal > 0.5f && prev_gear == -1  && gasPedal < 0.3f)|| gear == -1){
				SetGear(gear);
			}
			prev_gear = gear;
			
			if(Steering != null){
				Steering(steeringWheelDegrees);
			}
			if(Move != null && (gasPedal > 0 ||  breakPedal < 0 || direction != 0)){
				Move(direction,gasbreak);
				
			}
		}
	}
	
	//	public void GameInput(){}
	public void resetKeys(){
		gasbreak = 0;
		direction = 0;
	}
	private void DevInput(){

		if(Input.GetKey(mf)){
			gasbreak += 1;
		}		

		if(Input.GetKey(mb)){
			gasbreak -= 1;
		}

		if (Input.GetKey(ml)) {
			direction -= 0.5f;
		}

		if (Input.GetKey(mr)) {
			direction += 0.5f;
		}

		if (Move != null) {
			Move(direction,gasbreak);
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
