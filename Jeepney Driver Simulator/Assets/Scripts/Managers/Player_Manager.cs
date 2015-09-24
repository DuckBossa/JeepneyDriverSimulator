﻿using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
namespace UnityStandardAssets.Vehicles.Car{
	[RequireComponent(typeof (CarController))]
	public class Player_Manager : MonoBehaviour {
		
		
		public GameObject steeringWheel;

		private CarController cc;


		public void Start(){
			cc = GetComponent<CarController>();
		}

		public void OnEnable(){
			Input_Manager.Move += Move;
			Input_Manager.Steering += SetWheel;
		}
		
		
		public void OnDisable(){
			Input_Manager.Move -= Move;
			Input_Manager.Steering -= SetWheel;
		}
		
		void Move (float steer,float gas){
			cc.Move(steer,gas,gas,0);
		}
		
		void SetWheel(float degrees){
			Debug.Log(degrees);
			steeringWheel.transform.localRotation = Quaternion.Euler(new Vector3 (0, 0, degrees));
			
		}
	}

}
