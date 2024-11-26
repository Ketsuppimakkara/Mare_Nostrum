using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stabilizedProjectile : MonoBehaviour
{
    private new Rigidbody rigidbody;
    public int frames = 0;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void FixedUpdate(){
        frames++;
        if (frames % 10 == 0) {
            FrameNUpdate();
            frames = 0;
        }
    }

    // Update is called once per frame
public void FrameNUpdate()
	{
        if(rigidbody!=null){
		    transform.forward = rigidbody.velocity.normalized;
        }
		//rigidbody.rotation = Quaternion.LookRotation(rigidbody.velocity);  
	}
}
