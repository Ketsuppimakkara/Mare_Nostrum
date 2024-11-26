using System.Collections.Generic;
using UnityEngine;


public class Compartment : MonoBehaviour, IDamageable
{
    public int compartmentName;
	public Vector3 position;
	public float volume;
	public float waterVolume;
	public List<Breach> breaches = new List<Breach>();
	public List<Compartment> neighbors;
	public crewMember[] crewMembers;
	public int currentRepairmen= -1;
	private float gravity;
	public float armorvalue;
	public float hullIntegrity;
	public float fireDamageModifier;

	public void Start(){

		volume = GetComponent<BoxCollider>().bounds.size.magnitude * 100;
		
		crewMembers = gameObject.GetComponentsInChildren<crewMember>();

		Compartment[] compartments = transform.parent.GetComponentsInChildren<Compartment>();
		int forwardNeigborIndex = compartmentName-3;
		int backwardNeigborIndex = compartmentName+3;
		int leftNeigborIndex = compartmentName-1;
		int rightNeigborIndex = compartmentName+1;
		int bottomNeigborIndex = compartmentName-18;
		int topNeigborIndex = compartmentName+18;

		gravity = Physics.gravity.magnitude;

		if (Mathf.Sign(transform.position.x) < 0)
		{
			if (IsValidIndex(compartments, forwardNeigborIndex)){
				neighbors.Add(compartments[forwardNeigborIndex]);
			}
			if (IsValidIndex(compartments, backwardNeigborIndex)){
				neighbors.Add(compartments[backwardNeigborIndex]);
			}
			if (IsValidIndex(compartments, rightNeigborIndex)){
				neighbors.Add(compartments[rightNeigborIndex]);
			}
			if (IsValidIndex(compartments, topNeigborIndex)){
				neighbors.Add(compartments[topNeigborIndex]);
			}
			if (IsValidIndex(compartments, bottomNeigborIndex)){
				neighbors.Add(compartments[bottomNeigborIndex]);
			}
		}

		if (Mathf.Sign(transform.position.x) > 0)
		{
			if (IsValidIndex(compartments, forwardNeigborIndex)){
				neighbors.Add(compartments[forwardNeigborIndex]);
			}
			if (IsValidIndex(compartments, backwardNeigborIndex)){
				neighbors.Add(compartments[backwardNeigborIndex]);
			}
			if (IsValidIndex(compartments, leftNeigborIndex)){
				neighbors.Add(compartments[leftNeigborIndex]);
			}
			if (IsValidIndex(compartments, topNeigborIndex)){
				neighbors.Add(compartments[topNeigborIndex]);
			}
			if (IsValidIndex(compartments, bottomNeigborIndex)){
				neighbors.Add(compartments[bottomNeigborIndex]);
			}
		}

		if (Mathf.Sign(transform.position.x) > 0)
		{
			if (IsValidIndex(compartments, forwardNeigborIndex)){
				neighbors.Add(compartments[forwardNeigborIndex]);
			}
			if (IsValidIndex(compartments, backwardNeigborIndex)){
				neighbors.Add(compartments[backwardNeigborIndex]);
			}
			if (IsValidIndex(compartments, rightNeigborIndex)){
				neighbors.Add(compartments[rightNeigborIndex]);
			}
			if (IsValidIndex(compartments, leftNeigborIndex)){
				neighbors.Add(compartments[leftNeigborIndex]);
			}
			if (IsValidIndex(compartments, topNeigborIndex)){
				neighbors.Add(compartments[topNeigborIndex]);
			}
			if (IsValidIndex(compartments, bottomNeigborIndex)){
				neighbors.Add(compartments[bottomNeigborIndex]);
			}
		}

		bool IsValidIndex(Compartment[] compartments, int index)
		{
			return index >= 0 && index < compartments.Length;
		}
	}

	public void UpdateBuoyancy(float waterLevel){
		if(breaches.Count > 0){
			foreach (var breach in breaches){
				float depthOfBreach = new Vector3(transform.position.x,waterLevel).y-transform.position.y+breach.position.y;
				float waterInflow = breach.size * Mathf.Pow(2*gravity*Mathf.Clamp(depthOfBreach,0,1000),0.5f)*Time.deltaTime;
				waterVolume += waterInflow;
			}
		}
		waterVolume = Mathf.Clamp(waterVolume,0,volume);
	}

	public void Repair(){
		for (int i = 0; i<currentRepairmen; i++){
			waterVolume -= 0.5f;
		}
		waterVolume = Mathf.Clamp(waterVolume,0,volume);
	}

	public void spreadToNeighbors(){
		if(waterVolume >= volume){
			foreach(Compartment neighbor in neighbors){
				waterVolume -=5;
				neighbor.GetComponent<Compartment>().waterVolume += 5;
			}
		}
	}

	public float LiftFactor(){
		return Mathf.Clamp01(1-waterVolume/volume);
	}

	public void createBreach(float breachSize,Vector3 position){
		breaches.Add(new Breach(breachSize,transform.InverseTransformPoint(position)));
		Debug.Log(gameObject.name +" breached! Breach size: "+ breachSize);
	}

	public void handleDamage(Collision collision, GameObject projectileGameObject){
		Vector3 breachLocation = collision.GetContact(0).point;
		hitDetection projectile = projectileGameObject.GetComponent<hitDetection>();
		if(projectile.effectivePenetration > armorvalue){
			if(projectile.GetComponent<Rigidbody>().isKinematic){
				float damage = (collision.gameObject.transform.root.GetComponent<Rigidbody>().velocity - projectile.transform.root.GetComponent<Rigidbody>().velocity).magnitude * projectile.projectileRadius * projectile.GetComponent<Rigidbody>().mass;
				createBreach(damage,breachLocation);
				hullIntegrity = hullIntegrity - damage;
				}
			else{
				float damage = collision.relativeVelocity.magnitude * projectile.projectileRadius * projectile.GetComponent<Rigidbody>().mass;
				createBreach(damage,breachLocation);
				hullIntegrity = hullIntegrity - damage;
			}
			if(hullIntegrity <= 0f){
				Debug.Log("BIG BREACH IN "+name);
				createBreach(100f, transform.position);
			}
		}
		else{	
			//Debug.Log("Insufficient penetration! "+projectile.effectivePenetration + " < " + armorvalue);
		}
	}



    public void lodgeProjectile(Collision collision, GameObject projectile)
    {
		foreach(Transform child in projectile.transform){
			child.parent = null;
		}
        Destroy(projectile.GetComponent<Rigidbody>());
		float projectileLength = projectile.gameObject.GetComponent<Renderer>().bounds.extents.z*1.6f;
		projectile.gameObject.transform.position = collision.collider.ClosestPointOnBounds(collision.GetContact(0).point) + (projectile.gameObject.transform.forward * -projectileLength);
		projectile.gameObject.transform.SetParent(gameObject.transform);
		Destroy(projectile.GetComponent<Collider>());
		Destroy(projectile.GetComponent<TrailRenderer>());
		Destroy(projectile.GetComponent<hitDetection>());
		Destroy(projectile.GetComponent<stabilizedProjectile>());
	}
	public bool isEnabled(){
        return this.enabled;
    }

    public void lodgeProjectile(Collision collision, GameObject projectile, RaycastHit hit)
    {
        Destroy(projectile.GetComponent<Rigidbody>());
		Destroy(projectile.GetComponent<Collider>());
		Destroy(projectile.GetComponent<TrailRenderer>());
		Destroy(projectile.GetComponent<hitDetection>());
		Destroy(projectile.GetComponent<stabilizedProjectile>());
		//float projectileLength = projectile.gameObject.GetComponent<Renderer>().bounds.extents.z*1.6f;
		projectile.gameObject.transform.position = hit.point - (projectile.gameObject.transform.forward);
		projectile.gameObject.transform.SetParent(gameObject.transform);
		foreach(Transform child in projectile.transform){
			child.parent = null;
		}
    }

    public void handleDamageOverTime(Collider collider, float damagePerSecond)
    {
		if(hullIntegrity > 0f){
			hullIntegrity = hullIntegrity - damagePerSecond * Time.deltaTime * fireDamageModifier;
			if(hullIntegrity <= 0f){
				Debug.Log("BIG BREACH IN "+name);
				createBreach(100f, transform.position);
			}
		}
		foreach (crewMember crew in crewMembers){
			crew.handleDamageOverTime(collider,damagePerSecond);
		}

    }
}