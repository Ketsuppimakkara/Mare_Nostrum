using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followObject : MonoBehaviour
{
    public Vector3 cameraOffset;
    public Transform target;
    //public Transform lookAt;
    public new Camera camera;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {       
        transform.position = target.position + transform.forward * cameraOffset.z + transform.up * cameraOffset.y + transform.right * cameraOffset.x; 
        if(Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0){
            transform.Rotate(-Vector3.right,Input.GetAxis("Mouse Y") * 500f * Time.deltaTime);
            transform.Rotate(Vector3.up,Input.GetAxis("Mouse X") * 500f * Time.deltaTime, Space.World);
        }
        //Vector3 lookAt = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camera.nearClipPlane));
        //transform.LookAt(lookAt);
    }
}
