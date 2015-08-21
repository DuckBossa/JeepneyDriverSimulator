using UnityEngine;
using System.Collections;

public class G27_Demo : MonoBehaviour {

    public GameObject test_wheel;
    LogitechGSDK.LogiControllerPropertiesData properties;
    Rigidbody car_rb;
    float x_axis;
    int throttle; //y-axis from input
    int car_break; //z-rotation from input
    int clutch; // extra axis
    static int max_int = 32767;
    float angle_interval;
	// Use this for initialization
	void Start () {
        car_rb = GetComponentInChildren<Rigidbody>();
        angle_interval = 120f / (max_int);
        test_wheel.transform.rotation = Quaternion.EulerAngles(0, 0, 90f);

        LogitechGSDK.LogiSteeringInitialize(false);
    }
	
	// Update is called once per frame
	void Update () {
        if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0)) {
            LogitechGSDK.DIJOYSTATE2ENGINES wheel;
            wheel = LogitechGSDK.LogiGetStateUnity(0);
            x_axis = wheel.lX; // right is positive, left is negative
            throttle = wheel.lY; // negative is pressed.
            car_break = wheel.lZ; // positive is not pressed, negative is floored.
            clutch = wheel.rglSlider[0]; // positive is not pressed, negative is floored.
            //rec = LogitechGSDK.LogiGetStateUnity(0);
            //actualState += "x-axis position :" + rec.lX + "\n";
            //actualState += "y-axis position :" + rec.lY + "\n";
            //actualState += "z-axis position :" + rec.lZ + "\n";
            //actualState += "x-axis rotation :" + rec.lRx + "\n";
            //actualState += "y-axis rotation :" + rec.lRy + "\n";
            //actualState += "z-axis rotation :" + rec.lRz + "\n";
            //actualState += "extra axes positions 1 :" + rec.rglSlider[0] + "\n";
            //actualState += "extra axes positions 2 :" + rec.rglSlider[1] + "\n";


        }
	}

    void FixedUpdate() {
        Debug.Log(angle_interval * x_axis);
        Vector3 currRot = test_wheel.transform.localRotation.eulerAngles;
        Vector3 nextRot = new Vector3(0, 0, angle_interval * x_axis);
        //test_wheel.transform.Rotate(nextRot - currRot);
        test_wheel.transform.rotation = Quaternion.EulerAngles(nextRot);

    }
}
