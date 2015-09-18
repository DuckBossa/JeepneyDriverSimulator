using UnityEngine;
using System.Collections;

public class Player_Manager : MonoBehaviour {

	public void OnEnable(){
		Input_Manager.Move += Move;
		Input_Manager.Break += Reverse;
		Input_Manager.Steering += SetWheel;
	}


	public void OnDisable(){
		Input_Manager.Move -= Move;
		Input_Manager.Break -= Reverse;
		Input_Manager.Steering -= SetWheel;
	}

	void Move (float intensity){
		gameObject.transform.position += transform.forward.normalized * intensity;
	}

	void Reverse (float intensity){
		gameObject.transform.position += transform.forward.normalized * intensity;
	}

	void SetWheel(float degrees){
		gameObject.transform.rotation = Quaternion.Euler(new Vector3 (0, 0, degrees + 90));

	}
}
