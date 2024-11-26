using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class AIControls : MonoBehaviour
{
    public Rigidbody boat;
    public bool facingTarget = false;
    public bool onCourse = false;
    public bool inRange = false;
    public List<Oar> oars;
    public List<Rudder> rudders;
    public List<Yard> yards;
    public List<IRangedWeapon> weapons;
    public GameObject destination;
    public GameObject weaponTarget;
    private Rigidbody targetRb;
    public int attackMode = 0;
    public bool oarsReady = true;
    public Ram ram;
    public int frames = 1;
    public int updateInterval = 5;
    public float optimalWeaponRange = 25;
    public bool inWeaponRange = false;

    private enum rowingMode{
        Port,
        Starboard,
        Both
    }

    // Start is called before the first frame update
    void Start(){       
        weapons = new List<IRangedWeapon>(GetComponentsInChildren<IRangedWeapon>());
        foreach(IRangedWeapon weapon in weapons){
            weapon.setTarget(destination);
        }
    }

    public void FixedUpdate(){
        frames++;
        if (frames % updateInterval == 0) {
            FrameNUpdate();
        }
        switch (attackMode){
            case 0:
                navigateTo(destination,5);
                break;
            case 1:
                ramAttack(destination);
                break;
            case 2:
                rangedAttack(weaponTarget);
                break;
        }
    }

    void FrameNUpdate(){
        weapons = new List<IRangedWeapon>(GetComponentsInChildren<IRangedWeapon>());
            foreach(IRangedWeapon weapon in weapons){
            weapon.setTarget(destination);
            if(weapon.targetIsInRange()){
                fireWeapons();
            }
        frames = 0;
    }
    }


    void rangedAttack(GameObject target){
        Vector3 targetWorldSpace = target.transform.TransformPoint(target.transform.position);
        targetWorldSpace.y = 0;
        navigateTo((targetWorldSpace-transform.position),8);
        //navigateTo((transform.position-targetWorldSpace).normalized*optimalWeaponRange,8);
    }

    void fireWeapons(){
        if(weapons.Count > 0){
            foreach(IRangedWeapon weapon in weapons){
                if(weapon.targetIsInRange()){
                    weapon.fire();
                }
            }
        }
    }

    void ramAttack(GameObject target){
        float distanceToTarget = (new Vector3(ram.gameObject.transform.position.x,0,ram.gameObject.transform.position.z)-new Vector3(target.transform.position.x,0,target.transform.position.z)).magnitude;
        if(distanceToTarget < 300){
            ramTarget(target,1);
        }
        else{
            navigateTo(target,3);
        }
        fireWeapons();
    }

    void ramTarget(GameObject target,float maxDeviation){
        target.TryGetComponent<Rigidbody>(out Rigidbody targetrb);
        float distanceToTarget = (new Vector3(ram.gameObject.transform.position.x,0,ram.gameObject.transform.position.z)-new Vector3(target.transform.position.x,0,target.transform.position.z)).magnitude;
        float secondsUntilCollision = distanceToTarget / (new Vector3(boat.velocity.x,0,boat.velocity.z) - new Vector3(targetrb.velocity.x,0,targetrb.velocity.z)).magnitude;
        Vector3 deviatedPosition = (target.transform.position + (targetrb.velocity * secondsUntilCollision));
        deviatedPosition.y = 0;
        faceTarget(deviatedPosition,maxDeviation);
        //Debug.DrawLine(transform.position,deviatedPosition,Color.red);
        if(facingTarget){
            moveForwards();
        }

    }

    void navigateTo(GameObject destination,float maxDeviation){
        faceTarget(destination,maxDeviation);
        if(facingTarget){
            moveForwards();
        }
    }    

    void navigateTo(Vector3 targetPosition, float maxDeviation){
        faceTarget(targetPosition,maxDeviation);
        if(facingTarget){
            moveForwards();
        }
        //Debug.DrawLine(transform.position,targetPosition,Color.yellow);
    }   

    void faceTarget(Vector3 targetPosition, float maxDeviation){
        
        alignSailsWithDestination(targetPosition);
        Vector3 directionToTarget = targetPosition - transform.position;
        directionToTarget.y = 0;
        directionToTarget.Normalize();

        Vector3 forwardVector = transform.forward;
        forwardVector.y = 0;
        forwardVector.Normalize();

        float deviation = Vector3.SignedAngle(forwardVector, directionToTarget, Vector3.up);

        //Debug.DrawLine(transform.position, targetPosition, Color.red);

        if(deviation > maxDeviation){
            facingTarget = false;
            row(rowingMode.Port,1);
        }
        else if(deviation < -maxDeviation){
            facingTarget = false;
            row(rowingMode.Starboard,1);
        }
        else{
            facingTarget = true;
        }
    }

    void faceTarget(GameObject target,float maxDeviation){
        faceTarget(target.transform.position,maxDeviation);
    }

    void alignSailsWithDestination(GameObject destinationGameobject){
        alignSailsWithDestination(destinationGameobject.transform.position);
    }

    void alignSailsWithDestination(Vector3 destinationVector){
        foreach(Yard yard in yards){
           if(Vector3.SignedAngle(new Vector3(destinationVector.x,0,destinationVector.z)-new Vector3(yard.transform.position.x,0,yard.transform.position.z),new Vector3(yard.transform.forward.x,0,yard.transform.forward.z),Vector3.up) > 0){
                yard.rotateLeft();
            }
            else{
                yard.rotateRight();
            }
        }
    }

    void moveForwards(){
        row(rowingMode.Both,1);
    }

    void row(rowingMode mode, int direction){
            if(mode == rowingMode.Both){
                    foreach(Oar oar in oars){
                        if(oar.isReadyToRow(direction)){  
                            oar.row(direction);
                        }
                        else{
                            oar.stop();
                        }
                    }
            }
            else if(mode == rowingMode.Port){
                foreach(Oar oar in oars){
                    if(oar.portSide){
                        if(oar.isReadyToRow(direction)){
                            oar.row(direction); 
                        }
                        else{
                            oar.stop();
                        }
                    }
                }
            }
            else if(mode == rowingMode.Starboard){
                foreach(Oar oar in oars){
                    if(!oar.portSide){
                        if(oar.isReadyToRow(direction)){
                            oar.row(direction); 
                        }                        
                        else{
                            oar.stop();
                        }
                    }
                }
            } 
    }

}
