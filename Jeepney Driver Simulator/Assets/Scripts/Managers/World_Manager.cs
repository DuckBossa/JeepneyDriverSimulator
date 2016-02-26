using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
using System;

public class World_Manager : MonoBehaviour {
	public static event Action EndGame;
	public PaymentSystem ps;
	public float TOTAL_TIME = 0f;
	public string ExitGame = "End";
	float currTime;
	// Use this for initialization
	bool activated = false;
	void Start () {
		float currTime = 0f;
		Screen.fullScreen = true;
	}

	void Update () {
//		currTime += Time.deltaTime;
//		if(currTime >= TOTAL_TIME && !activated){
//			activated = true;
//			Debug.Log("Start");
//			if (EndGame!= null) EndGame();
//			Debug.Log("End");
//			Application.Quit();
////			Application.LoadLevel(ExitGame);
//		}
	}
	

}
