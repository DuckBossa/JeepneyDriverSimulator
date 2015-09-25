using UnityEngine;
using System.Collections;

public class BackCollider : MonoBehaviour {

	public void OnTriggerEnter(){
		Debug.Log ("Your hand is in range");
	}

}
