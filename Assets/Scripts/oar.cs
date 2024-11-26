using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Oar : MonoBehaviour{
    public Rigidbody boat;
    public float thrustMultiplier;
    public float zAngle;
    public float yAngle;
    public bool portSide;
    // Range between 0-1 indicating how far the oar has been pulled. 1 indicates ready for a full-strength row 
    public float rangeOfMotion;
    public float yRangeOfMotion;
    public int lastRowingDirection;
    public bool recovering = false;
    public float maxRowingSpeed;
    private const float raisedOarSpeedMultiplier = 2.0f;
    public bool oarIsLowered = false;
    private float maxRowerCount;
    private float efficiency = 1;
    public List<GameObject> rowers;
    public AudioSource rowingSound;

    public void Start(){
        maxRowerCount = rowers.Count();

        thrustMultiplier = thrustMultiplier*maxRowerCount;

        rangeOfMotion = 1f;
        yRangeOfMotion = 1f;
        lastRowingDirection = 1;
    }

    public void FixedUpdate(){

        if(!oarIsLowered){
            rangeOfMotion = Mathf.Clamp(rangeOfMotion + maxRowingSpeed * raisedOarSpeedMultiplier * lastRowingDirection * efficiency * Time.deltaTime,-1,1);
            yRangeOfMotion = Mathf.Clamp(yRangeOfMotion + 6f * Time.deltaTime,-1,1);
        }
        else{
            yRangeOfMotion = Mathf.Clamp(yRangeOfMotion - 6f * Time.deltaTime,-1,1);
            if(!rowingSound.isPlaying){
                rowingSound.pitch = Random.Range(0.9f,1.1f);
                rowingSound.Play();
            }
        }
        UpdateOarModelPosition();
        LowerOarModel();
    }
    
    public void row(int direction){
            lastRowingDirection = direction;
            if(!recovering){
                oarIsLowered = true;
                rangeOfMotion = Mathf.Clamp(rangeOfMotion - maxRowingSpeed * direction * Time.deltaTime,-1,1);
                if(rangeOfMotion*direction > -1){
                    Vector3 rowingForce = new Vector3(boat.transform.forward.x,0,boat.transform.forward.z)*thrustMultiplier*maxRowingSpeed*efficiency*direction;
                    boat.AddForceAtPosition(rowingForce,new Vector3(transform.position.x,boat.centerOfMass.y,transform.position.z));
                }
            }

    }
    
    public void stop(){
        oarIsLowered = false;
    }

    public void RemoveCrewmember(GameObject rower){
        rowers.Remove(rower);
        efficiency = rowers.Count / maxRowerCount;
    }

    public void UpdateOarModelPosition(){
        if(efficiency != 0){
            if(portSide){
                transform.rotation = boat.transform.rotation * Quaternion.AngleAxis(rangeOfMotion * zAngle, Vector3.up);
            }
            else{
                transform.rotation = boat.transform.rotation * Quaternion.AngleAxis(-rangeOfMotion * zAngle, Vector3.up);
            }
        }
    }

    public void LowerOarModel(){
        if(efficiency != 0){
            if(portSide){
                transform.Rotate(0,0,-yAngle*yRangeOfMotion);
            }
            else{
                transform.Rotate(0,0,yAngle*yRangeOfMotion);
            }
        }
    }

    public bool isReadyToRow(int direction){
        if(lastRowingDirection == direction && rangeOfMotion == direction){ // If range of motion is full
            recovering = false;
        }
        if(lastRowingDirection == direction && rangeOfMotion == -direction){ // If range of motion is exhausted
            recovering = true;
        }
        return !recovering;
    }
}