// Logitech Gaming SDK
//
// Copyright (C) 2011-2014 Logitech. All rights reserved.
// Author: Tiziano Pigliucci
// Email: devtechsupport@logitech.com

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;



public class LogitechGKey : MonoBehaviour {
	
	bool usingCallback;
	// Use this for initialization
	void Start () {
		//Value used to show the two different ways to implement g-keys support in your game
		//change it to false to try the non-callback version
		usingCallback = true;
		if (usingCallback){
			LogitechGSDK.logiGkeyCB cbInstance = new LogitechGSDK.logiGkeyCB(this.GkeySDKCallback);
			LogitechGSDK.LogiGkeyInitWithoutContext(cbInstance);
		}
		else
			LogitechGSDK.LogiGkeyInitWithoutCallback();
	}
	
	// Update is called once per frame
	void Update(){
		if(!usingCallback){
			for (int index = 6; index <= LogitechGSDK.LOGITECH_MAX_MOUSE_BUTTONS; index++) {
				if (LogitechGSDK.LogiGkeyIsMouseButtonPressed(index) == 1) {
					//These functions are not going to work if you didn't initialize the LCD SDK and calling LogiLcdUpdate every frame
					LogitechGSDK.LogiLcdColorSetText(0, "MOUSE",255,0,0);
					LogitechGSDK.LogiLcdColorSetText(1, "Button : "+index,255,0,0);
				} 
			} 
			
			for (int index = 1; index <= LogitechGSDK.LOGITECH_MAX_GKEYS; index++) { 
				for (int mKeyIndex = 1; mKeyIndex <= LogitechGSDK.LOGITECH_MAX_M_STATES; mKeyIndex++) {
					if (LogitechGSDK.LogiGkeyIsKeyboardGkeyPressed(index, mKeyIndex) == 1) {
						//These functions are not going to work if you didn't initialize the LCD SDK and calling LogiLcdUpdate every frame
						LogitechGSDK.LogiLcdColorSetText(0, "KEYBOARD/HEADSET",255,0,0);
						LogitechGSDK.LogiLcdColorSetText(1, "Button : "+index,255,0,0);
					} 
				}
			}
		}
	}
	
	void GkeySDKCallback(LogitechGSDK.GkeyCode gKeyCode, String gKeyOrButtonString, IntPtr context){ 
			if(gKeyCode.keyDown == 0){
				if(gKeyCode.mouse == 1){
					//These functions are not going to work if you didn't initialize the LCD SDK and calling LogiLcdUpdate every frame
					LogitechGSDK.LogiLcdColorSetText(0, "MOUSE "+gKeyOrButtonString,255,0,0);
					LogitechGSDK.LogiLcdColorSetText(2, LogitechGSDK.LogiGkeyGetMouseButtonStr(gKeyCode.keyIdx),255,0,0);
					LogitechGSDK.LogiLcdMonoSetText(2, "MOUSE "+gKeyOrButtonString);
					LogitechGSDK.LogiLcdMonoSetText(3, LogitechGSDK.LogiGkeyGetMouseButtonStr(gKeyCode.keyIdx));
				}
				else{
					//These functions are not going to work if you didn't initialize the LCD SDK and calling LogiLcdUpdate every frame
					LogitechGSDK.LogiLcdColorSetText(0, "KEYBOARD/HEADSET "+gKeyOrButtonString,255,0,0);
					LogitechGSDK.LogiLcdColorSetText(2, LogitechGSDK.LogiGkeyGetKeyboardGkeyStr(gKeyCode.keyIdx, gKeyCode.mState),255,0,0);
					LogitechGSDK.LogiLcdMonoSetText(2, "KEYBOARD/HEADSET "+gKeyOrButtonString);
					LogitechGSDK.LogiLcdMonoSetText(3, LogitechGSDK.LogiGkeyGetKeyboardGkeyStr(gKeyCode.keyIdx, gKeyCode.mState));
				}
				//These functions are not going to work if you didn't initialize the LCD SDK and calling LogiLcdUpdate every frame
				LogitechGSDK.LogiLcdColorSetText(1, " Released button :"+gKeyCode.keyIdx+" mode :"+gKeyCode.mState,255,0,0);
				LogitechGSDK.LogiLcdMonoSetText(3, " Released button :"+gKeyCode.keyIdx+" mode :"+gKeyCode.mState);
			}
			else{
				if(gKeyCode.mouse == 1){
					//These functions are not going to work if you didn't initialize the LCD SDK and calling LogiLcdUpdate every frame
					LogitechGSDK.LogiLcdColorSetText(0, "MOUSE "+gKeyOrButtonString,255,0,0);
					LogitechGSDK.LogiLcdColorSetText(2, LogitechGSDK.LogiGkeyGetMouseButtonStr(gKeyCode.keyIdx),255,0,0);
					LogitechGSDK.LogiLcdMonoSetText(2, "MOUSE "+gKeyOrButtonString);
					LogitechGSDK.LogiLcdMonoSetText(3, LogitechGSDK.LogiGkeyGetMouseButtonStr(gKeyCode.keyIdx));
				}
				else{ 
					//These functions are not going to work if you didn't initialize the LCD SDK and calling LogiLcdUpdate every frame
					LogitechGSDK.LogiLcdColorSetText(0, "KEYBOARD/HEADSET "+gKeyOrButtonString,255,0,0);
					LogitechGSDK.LogiLcdColorSetText(2, LogitechGSDK.LogiGkeyGetKeyboardGkeyStr(gKeyCode.keyIdx, gKeyCode.mState),255,0,0);
					LogitechGSDK.LogiLcdMonoSetText(2, "KEYBOARD/HEADSET "+gKeyOrButtonString);
					LogitechGSDK.LogiLcdMonoSetText(3, LogitechGSDK.LogiGkeyGetKeyboardGkeyStr(gKeyCode.keyIdx, gKeyCode.mState));
				}
				//These functions are not going to work if you didn't initialize the LCD SDK and calling LogiLcdUpdate every frame
				LogitechGSDK.LogiLcdColorSetText(1, " Pressed button :"+gKeyCode.keyIdx+" mode :"+gKeyCode.mState,255,0,0);
				LogitechGSDK.LogiLcdMonoSetText(3, " Pressed button :"+gKeyCode.keyIdx+" mode :"+gKeyCode.mState);
			}
				
	}
	
	void OnDestroy () {
		//Free G-Keys SDKs before quitting the game
     	LogitechGSDK.LogiGkeyShutdown();
	}
}
