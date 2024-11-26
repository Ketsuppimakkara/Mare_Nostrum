using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    public GameObject target;
    int frames = 0;
    public int updateInterval = 5;
    // Start is called before the first frame update
    void Start()
    {
        weaponControls weaponControls = transform.root.GetComponentInChildren<weaponControls>();
        if(weaponControls == null){
            target = transform.root.GetComponentInChildren<AIControls>().weaponTarget;
        }
        else{
            target = transform.root.GetComponentInChildren<weaponControls>().target;
        }
    }
    public void FixedUpdate(){
        frames++;
        if (frames % updateInterval == 0) {
            FrameNUpdate();
        }
    }

    // Update is called once per frame
    void FrameNUpdate()
    {
        gameObject.transform.LookAt(new Vector3(target.transform.position.x,gameObject.transform.position.y,target.transform.position.z));
    }
}
