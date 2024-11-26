using System.Collections.Generic;
using UnityEngine;

public class sailControls : MonoBehaviour
{
    public Rigidbody boat;
    public List<Yard> yards;
    public GameObject[] masts;
    public Wind windScript;
    private Vector3 windDirection;
    private float windSpeed;
    private Vector3 boatSpeedDelta;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        windDirection = windScript.windDirection;
        windSpeed = windScript.windSpeed;
        foreach(Yard yard in yards){
                if(Input.GetKey(KeyCode.Z)){
                    yard.rotateLeft();
                }
                if(Input.GetKey(KeyCode.C)){
                    yard.rotateRight();
                }
                if(Input.GetKey(KeyCode.LeftShift)){
                    Sail sail = yard.GetComponentInChildren<Sail>();
                    sail.unfurlSail();
                }
                if(Input.GetKey(KeyCode.LeftControl)){
                    Sail sail = yard.GetComponentInChildren<Sail>();
                    sail.furlSail();
                }
            }
        }
    }
