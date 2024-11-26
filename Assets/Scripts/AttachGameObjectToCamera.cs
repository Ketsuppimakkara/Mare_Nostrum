using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachGameObjectToCamera : MonoBehaviour
{
    public GameObject target;
    private new Camera camera;
    public float yOffSet;
    public int layerToCollideWith = 9;
    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit hit;
        if(Physics.Raycast(camera.transform.position,camera.transform.forward + new Vector3(0,yOffSet,0) ,out hit,Mathf.Infinity,1 << layerToCollideWith)){
            target.transform.position = hit.point;
        }
    }
}
