using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rudderControls : MonoBehaviour
{
    public Rigidbody boat;
    public Rudder[] rudders;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        steeringInput();
    }
    private void steeringInput(){
        if (Input.GetKey(KeyCode.E))
        {
            foreach(Rudder rudder in rudders){
                rudder.turnRight();
            }
        }
        if (Input.GetKey(KeyCode.Q))
        {
            foreach(Rudder rudder in rudders){
                rudder.turnLeft();
            }
        }
    }
}
