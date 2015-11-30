using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
using System;

public class World_Manager : MonoBehaviour {
	public static event Action EndGame;

	public float TOTAL_TIME = 0f;
	float currTime;
	// Use this for initialization
	void Start () {
		float currTime = 0f;
	}

	void Update () {
		currTime += Time.deltaTime;
		if(currTime >= TOTAL_TIME){
		}
	}

	void CountStatistics(){

	}

}
