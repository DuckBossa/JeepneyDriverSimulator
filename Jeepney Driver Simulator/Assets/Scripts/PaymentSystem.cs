using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;


public class PaymentSystem : MonoBehaviour {


	public static event Action EmbarkPassenger;

	public enum PaymentState{
		NoPayment, ReceivePayment, GiveChange
	}

	public float INITIAL_MONEY = 50f;
	public float JEEPNEY_FARE = 8f;
	private PaymentState currState;
	private float currMoney;
	private float currChange;
	private float currPayment;
	private Queue<float> transactions = new Queue<float>();
	public void Start(){
		currMoney = INITIAL_MONEY;
		currChange = currPayment = 0;
		currState = PaymentState.NoPayment;
	}


	public void OnEnable(){
		RazerHydraInput.GetChange += GetChange;
		EmbarkPassenger += AddPayment;
	}

	public void OnDisable(){
		RazerHydraInput.GetChange -= GetChange;
		EmbarkPassenger -= AddPayment;
	}



	private void AddPayment(){
		float amount = JEEPNEY_FARE + UnityEngine.Random.Range(1,10);
		if(amount > JEEPNEY_FARE){
			transactions.Enqueue(amount);
		}
		else{

		}	
		if(currState != PaymentState.GiveChange || currState == PaymentState.ReceivePayment){
			currState = PaymentState.ReceivePayment;
		}
	}

	private void GetChange(float amount){
		if(currState == PaymentState.GiveChange){
			if(currMoney - amount < 0)
				Debug.Log ("Poor");
			else{
				currChange += amount;
				currMoney -= amount;
			}
				
		}
	}
	
	private void UndoChange (float amount){
		if(currState == PaymentState.GiveChange){
			if(currChange < amount ){
				currMoney += amount - currChange;
				currChange = 0;
			}
			else{
				currMoney += amount;
				currChange -= amount;
			}
		}
	}


	private void ReturnChange(){
		if(currState == PaymentState.GiveChange){
			if (currChange >= currPayment){

			}
		}
	}

}
