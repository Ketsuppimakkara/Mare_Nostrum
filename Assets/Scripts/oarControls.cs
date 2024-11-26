using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Timeline;

public class OarControls : MonoBehaviour
{
    public Rigidbody boat;
    public List<Oar> oars;
    private const float raisedOarSpeedMultiplier = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        UserInput();
    }

    void UserInput()
    {
        foreach(Oar oar in oars){
            if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S)){
                oar.stop();
            }

            //Forward / reverse
            if (Input.GetKey(KeyCode.W))
            {
                oar.row(1);
            }

            if (Input.GetKey(KeyCode.S))
            {
                oar.row(-1);
            }

            //Uncomment the commented lines for neutral steering
            if (Input.GetKey(KeyCode.D))
            {
                if(oar.portSide){
                    oar.row(1);
                }
                else{
                    //boat.AddForceAtPosition(-transform.forward * thrust, oar.transform.position);
                }
            }
            if (Input.GetKey(KeyCode.A))
            {
                if(oar.portSide){
                    //boat.AddForceAtPosition(-transform.forward * thrust, oar.transform.position);
                }
                else{
                    oar.row(1);
                }
            }
        }
    }
}
