using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class weaponControls : MonoBehaviour
{
    public GameObject target;
    //public GameObject origin;
    public List<GameObject> soldiers;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0)){
            foreach(GameObject soldier in soldiers){
                crewMember crewMember = soldier.GetComponentInChildren<crewMember>();
                if(crewMember != null){
                    crewMember.fireWeapon();
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.T)){
            foreach(GameObject soldier in soldiers){
                crewMember crewMember = soldier.GetComponentInChildren<crewMember>();
                if(crewMember != null){
                    IRangedWeapon weapon = soldier.GetComponentInChildren<IRangedWeapon>();
                    if(weapon != null){
                        weapon.nextAmmoType();
                    }
                }
            }
        }
    }

    public void addSoldier(GameObject soldier){
        soldiers.Add(soldier);
    }

    public void removeSoldier(GameObject soldier){
        soldiers.Remove(soldier);
    }
}
