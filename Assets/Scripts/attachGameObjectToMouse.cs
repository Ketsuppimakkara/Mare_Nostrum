using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attachObjectToMouse : MonoBehaviour {
 
public GameObject holdingThing;
 
 
void Update() {
if (holdingThing) {
    RaycastHit rchit = new RaycastHit();
    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rchit, 1000f) ) {
    holdingThing.transform.position = rchit.point;
    }
}}}
