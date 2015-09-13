using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class Input_Manager : MonoBehaviour {
	public static event Action MoveForward;
	public static event Action MoveBackward;
	public static event Action MoveLeft;
	public static event Action MoveRight;


	//Dev
	public KeyCode mf = KeyCode.W;
	public KeyCode mb = KeyCode.S;
	public KeyCode ml = KeyCode.D;
	public KeyCode mr = KeyCode.A;



	public void Start(){
		MoveForward += DoSomething;
	}


	public void Update(){

		if(Input.GetKeyDown(mf)){
			if(MoveForward != null){
				MoveForward();
			}
		}

		if(Input.GetKeyDown(ml)){
			if(MoveLeft != null){
				MoveLeft();
			}
		}

		if(Input.GetKeyDown(mr)){
			if(MoveRight != null){
				MoveRight();
			}
		}

		if(Input.GetKeyDown(mb)){
			if(MoveBackward != null){
				MoveBackward();
			}
		}



	}

	public void DoSomething(){
		Debug.Log("LOL");
	}


}
