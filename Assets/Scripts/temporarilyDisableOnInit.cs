using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class temporarilyDisableOnInit : MonoBehaviour
{
    public float disabledForSeconds = 0;
    public Collider colliderToDisable;
    // Start is called before the first frame update
    void Start()
    {
        colliderToDisable.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        disabledForSeconds = disabledForSeconds-Time.deltaTime;
        if(disabledForSeconds<=0){
            colliderToDisable.enabled = true;
        }
    }
}
