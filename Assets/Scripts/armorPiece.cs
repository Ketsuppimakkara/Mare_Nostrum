using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class armorPiece : MonoBehaviour, IDamageable
{
    public float armorvalue;
    public float health;
    public float maxHealth;
    public float fireDamageModifier;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.layer = GetComponentInParent<crewMember>().gameObject.layer;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void handleDamage(Collision collision, GameObject projectileGameObject){
        hitDetection hitDetection = projectileGameObject.GetComponent<hitDetection>();
        float damage = collision.relativeVelocity.magnitude * hitDetection.projectileRadius * projectileGameObject.GetComponent<Rigidbody>().mass;
        float mitigatedDamage;
        mitigatedDamage = health/maxHealth/2 * damage;
        Debug.Log("effective pen: "+ hitDetection.effectivePenetration);
        Debug.Log("armor: "+ armorvalue);
        if(hitDetection.effectivePenetration >= armorvalue){
            mitigatedDamage = mitigatedDamage/6;
        };
        damage = damage - mitigatedDamage;
        Debug.Log("dealth damage: "+damage);
        health -=damage;
        if(health <= 0f){
            GameObject.Destroy(gameObject);
        }
	}
    public void lodgeProjectile(Collision collision, GameObject projectile)
    {
        Vector3 jointPoint = collision.collider.transform.root.GetComponent<Rigidbody>().transform.InverseTransformPoint(collision.GetContact(0).point);
		SpringJoint joint = gameObject.AddComponent<SpringJoint>();
		joint.anchor = jointPoint;
		joint.connectedBody = projectile.GetComponent<Rigidbody>();
		joint.minDistance = 0;
		joint.maxDistance = 5;
		joint.spring = 100;
    }

    public bool isEnabled(){
        return this.enabled;
    }

    public void lodgeProjectile(Collision collision, GameObject projectile, RaycastHit hit)
    {
        lodgeProjectile(collision,projectile);
    }
    public void handleDamageOverTime(Collider collider, float damagePerSecond)
    {
			health =- damagePerSecond * Time.deltaTime * fireDamageModifier;
    }
}

