using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;


public class PaymentSystem : MonoBehaviour {
	
	public enum PaymentState{
		NoPayment, ReceivePayment, GiveChange
	}

	public float INITIAL_MONEY = 50f;
	private PaymentState currState;
	private float currMoney;
	private float currChange;
	private Queue<float> transactions = new Queue<float>();
	public void Start(){
		currMoney = INITIAL_MONEY;
		currChange = 0;
		currState = PaymentState.NoPayment;
	}


	public void OnEnable(){
		RazerHydraInput.ReceivePay += CheckPayment;
	}

	public void OnDisable(){
		RazerHydraInput.ReceivePay -= CheckPayment;
	}



	private void AddPayment(float amount){
		transactions.Enqueue(amount);
	}
	private void CheckPayment(Ray forward){
		RaycastHit hit;

		if(Physics.Raycast(forward, out hit, 3f)){
//			Debug.Log(hit);
		}

	}
}
