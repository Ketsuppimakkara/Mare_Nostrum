using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drawPhysicsLines : MonoBehaviour
{
    public Rigidbody rb;
    public GameObject rower;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position,rb.velocity,Color.yellow);
    }
}
