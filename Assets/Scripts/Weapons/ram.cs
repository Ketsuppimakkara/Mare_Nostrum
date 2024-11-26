using System.Collections;
using UnityEngine;

public class Ram : MonoBehaviour
{
    private Rigidbody rb;
    private Rigidbody boat;
    private Rigidbody cloneRigidBody;
    private CapsuleCollider trigger;
    private BoxCollider boxCollider;
    public int triggerCount;
    private Vector3 contactPoint;

    void Start()
    {
        triggerCount = 0;
        rb = GetComponent<Rigidbody>();
        boat = transform.root.GetComponent<Rigidbody>();
        trigger = GetComponent<CapsuleCollider>();
        boxCollider = GetComponent<BoxCollider>();
    }

    void OnCollisionEnter(Collision collision){
        contactPoint = collision.GetContact(0).point;
        DisableGameObject();
    }

    void OnTriggerStay(){
        boat.AddForceAtPosition(-boat.transform.forward * 500f,transform.position);
    }

    void OnTriggerEnter(Collider other)
    {
        triggerCount++;
        if (triggerCount == 1)
        {
            DisableGameObject();
        }
    }

    void OnTriggerExit(Collider other)
    {
        triggerCount--;
        if (triggerCount == 0)
        {
            StartCoroutine(EnableGameObjectWithDelay(2f));
        }
    }

    void DisableGameObject()
    {
        boxCollider.enabled = false;
    }

    IEnumerator EnableGameObjectWithDelay(float seconds){
        yield return new WaitForSeconds(seconds);
        boxCollider.enabled = true;
    }
}