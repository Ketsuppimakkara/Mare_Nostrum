using System;
using System.Linq;
using UnityEngine;

public class crewMember : MonoBehaviour, IDamageable
{
    public float armorvalue;
    public float health;
    public GameObject bodyArmor;
    public GameObject headArmor;
    public Animator animator;
    public weaponControls weaponControls;
    public GameObject weaponGameobject;
    private IRangedWeapon weapon;
    public float fireDamageModifier;
    public float reloadTime;
    public bool isAlive = true;
    public bool destroyOnDeath = false;
    public Score score;
    public int scoreGainOnKill = 50;
    public UI ui;
    public AudioSource[] deathSounds;
    // Start is called before the first frame update
    void Start()
    {
        isAlive = true;
        if(weaponGameobject != null){
            weapon = weaponGameobject.GetComponent<IRangedWeapon>();
            weaponControls = transform.root.GetComponentInChildren<weaponControls>();
        }
        armorvalue = 0f;
        animator = GetComponent<Animator>();
        Rigidbody[] rigidBodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidBodies){
            rb.gameObject.layer = gameObject.layer;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(health <= 0f){
            die();
        }
    }

    public void fireWeapon(){
        if(weapon != null){
            if(weapon.targetIsInRange()){
                weapon.fire();
            }
        }
    }

	public void handleDamage(Collision collision, GameObject projectileGameObject){
        float damage = collision.relativeVelocity.magnitude * projectileGameObject.GetComponent<hitDetection>().projectileRadius * projectileGameObject.GetComponent<Rigidbody>().mass;
        //Debug.Log("damage is "+damage);
        health -=damage;
	}

    public void die(){
        if(isAlive == true){
            if(weaponControls != null){
                weaponControls.removeSoldier(gameObject);
            }
            if(weapon != null){
                Destroy(weapon.gameObject);
            }
            if(animator != null){
                Destroy(animator);
            }
            Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
            
            foreach (Rigidbody rigidbody in rigidbodies){
                    rigidbody.isKinematic = false;
                    rigidbody.gameObject.layer = 8;
                }
                
            if (transform.root.TryGetComponent(out GridFormation gridFormation)){
                    gridFormation.RemoveSoldier(gameObject);
            }
            if(gameObject.transform.parent != null){
                Oar oar = gameObject.transform.parent.GetComponentInChildren<Oar>();
                if (oar != null){
                    oar.RemoveCrewmember(gameObject);
                }
            }
            
            if (TryGetComponent(out crewMember crewMember)){
                    crewMember.enabled = false;
            }
            
            if (TryGetComponent(out LookAt lookat)){
                    lookat.enabled = false;
            }
            
            if(destroyOnDeath == true){
                Destroy(gameObject);
            }
            deathSounds[UnityEngine.Random.Range(0,deathSounds.Length)].Play();
            isAlive = false;
            if(ui != null){
                ui.UpdateCrewMemberStatistics();
            }

            if(gameObject.layer != LayerMask.NameToLayer("Player")){
                Score.score = Score.score + scoreGainOnKill;
            }
        }
    }
    
    public void lodgeProjectile(Collision collision, GameObject projectile)
    {
        // It would be monstrous to lodge projectiles into crewmembers
	}

    public bool isEnabled(){
        return this.enabled;
    }

    public void setAnimatorProperty(string propertyName, bool value){
        if(animator != null){
            animator.SetBool(propertyName,value);
        }
    }
    public void setAnimatorProperty(string propertyName, float value){
        if(animator != null){
            animator.SetFloat(propertyName,value);
        }
    }

    public void lodgeProjectile(Collision collision, GameObject projectile, RaycastHit hit)
    {
        lodgeProjectile(collision,projectile);
    }

    public void handleDamageOverTime(Collider collider, float damagePerSecond)
    {
        //Debug.Log("{} took fire damage! ");
        if(damagePerSecond > 0.1f){
		    health = health - damagePerSecond * Time.deltaTime * fireDamageModifier;
        }
    }
}
