using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class Input_Manager : MonoBehaviour {
	public static event Action<double> MoveForward; // pedal 
	public static event Action<double> Steering; // steering wheel
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

	public void Start(){
		LogitechGSDK.LogiSteeringInitialize(false);
		properties.wheelRange = wheelDegrees;
		LogitechGSDK.LogiSetPreferredControllerProperties(properties);
		steeringWheelDegrees = 0f;
	}


	public void Update(){
		//DevInput();
		DrivingInput();
	}

//	public void GameInput(){}

	public void DevInput(){
		if(Input.GetKeyDown(mf)){
			if(MoveForward != null){
				MoveForward(1);
			}
		}		

		if(Input.GetKeyDown(mb)){
			if(MoveForward != null){
				MoveForward(-1);
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
			steeringWheelDegrees = Mathf.Lerp(-wheelDegrees/2,wheelDegrees/2,(rec.lX /32763f + 1f)/2f);
			Debug.Log( "wheel is steering at " + steeringWheelDegrees);
			if(Steering != null){
				Steering(steeringWheelDegrees);
			}
		}
	}


}
