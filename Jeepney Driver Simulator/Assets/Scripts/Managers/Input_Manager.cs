using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class Input_Manager : MonoBehaviour {
	public static event Action<float,float,float> Move; // pedal
	public static event Action<float> Break; //stop twice
	public static event Action<float> Steering; // steering wheel, parameter is current degrees
	public static event Action<int> SetGear; // shift
	public static event Action Honk; //one of the buttons
	public static event Action SignalLeft; //left paddle
	public static event Action SignalRight; // right paddle
	public static event Action<int> ReceivePay; //hydra

	LogitechGSDK.LogiControllerPropertiesData properties;
	
	//Dev
	public KeyCode mf = KeyCode.W;
	public KeyCode mb = KeyCode.S;
	public KeyCode ml = KeyCode.D;
	public KeyCode mr = KeyCode.A;
	public KeyCode hk = KeyCode.H;
	public KeyCode sl = KeyCode.Less;
	public KeyCode sr = KeyCode.Greater;
	public int wheelDegrees = 900;

	private float steeringWheelDegrees;
	private float steeringWheelIntensity;
	private float gasPedal;
	private float breakPedal;
	private float clutchPedal;

	public void Start(){
		LogitechGSDK.LogiSteeringInitialize(false);
		properties.wheelRange = wheelDegrees;
		LogitechGSDK.LogiSetPreferredControllerProperties(properties);
		steeringWheelDegrees = 0f;
	}


	public void FixedUpdate(){
		DevInput();
		DrivingInput();
	}

//	public void GameInput(){}

	public void DevInput(){

		if(Input.GetKeyDown(mf)){
			if(Move != null){
//				Move(5f);
			}
		}		

		if(Input.GetKeyDown(mb)){
			if(Break != null){
				Break(-5f);
			}
		}

		if(Input.GetKeyDown(hk)){
			if(Honk != null){
				Honk();
			}
		}
		if(Input.GetKeyDown(sl)){
			if(SignalLeft != null){
				SignalLeft();
			}
		}
		if(Input.GetKeyDown(sr)){
			if(SignalRight != null){
				SignalRight();
			}
		}
	}


	public void DrivingInput(){
		if(LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0)){
			LogitechGSDK.LogiControllerPropertiesData actualProperties = new LogitechGSDK.LogiControllerPropertiesData();
			LogitechGSDK.DIJOYSTATE2ENGINES rec;
			rec = LogitechGSDK.LogiGetStateUnity(0);

			steeringWheelIntensity = Mathf.Lerp(/*wheelDegrees/2*/1,/*-wheelDegrees/2*/-1,(-rec.lX /32767f + 1f)/2f);
			steeringWheelDegrees = Mathf.Lerp(wheelDegrees/2,-wheelDegrees/2,(-rec.lX /32767f + 1f)/2f);
			gasPedal = Mathf.Lerp(0,1,( -rec.lY/32767f + 1)/2 );
			breakPedal = Mathf.Lerp(0,1,( -rec.lRz/32767f + 1)/2);
			clutchPedal = Mathf.Lerp(0,1,( -rec.rglSlider[1]/32767f + 1)/2);
			if(Steering != null){
				Steering(steeringWheelDegrees);
			}
			if(Move != null && (gasPedal > 0 ||  breakPedal > 0)){
				Move(steeringWheelIntensity,gasPedal,breakPedal);
			}
//			if(Break != null && breakPedal > 0){
//				Break(breakPedal);
//			}


		}
	}


}
