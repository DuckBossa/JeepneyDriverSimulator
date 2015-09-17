using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class Input_Manager : MonoBehaviour {
	public static event Action<float> Move; // pedal
	public static event Action<float> Reverse; //stop twice
	public static event Action<float> Steering; // steering wheel, parameter is current degrees
	public static event Action<int> SetGear; // shift
	public static event Action Honk; //one of the buttons
	public static event Action SignalLeft; //left paddle
	public static event Action SignalRight; // right paddle
	public static event Action<int> ReceivePay; //hydra

	LogitechGSDK.LogiControllerPropertiesData properties;

	//G27 Values
	public float max_int = 32767f;

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
		DevInput();
		DrivingInput();
	}

//	public void GameInput(){}

	public void DevInput(){

		if(Input.GetKeyDown(mf)){
			if(Move != null){
				Move(5f);
			}
		}		

		if(Input.GetKeyDown(mb)){
			if(Reverse != null){
				Reverse(-5f);
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
			steeringWheelDegrees = Mathf.Lerp(wheelDegrees/2,-wheelDegrees/2,(rec.lX /32767f + 1f)/2f);
			Debug.Log( "wheel is steering at " + steeringWheelDegrees);
			Debug.Log(rec.lX);
			if(Steering != null){
				Steering(steeringWheelDegrees);
			}
		}
	}


}
