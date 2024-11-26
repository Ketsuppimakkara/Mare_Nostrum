using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;


public class Bow : MonoBehaviour, IRangedWeapon
{
    public GameObject weaponOperator;
    public crewMember crewMemberScript;
    public weaponControls weaponControls;
    public Rigidbody boat;
    private int ammoIndex = 0;
    public GameObject ammunition;
    public GameObject target;
    private Vector3 firingVector;
    public float launchForce;
    private float launchVelocity;
    public float reloadTime;
    public float dispersion;
    public int projectilesPerShot;
    public int shotsBeforeReload;
    private float gravity;
    private bool isLoaded;
    public float rotationSpeed;
    public GameObject projectileModel;
    public GameObject[] ammotypes;
    public int frames = 0;
    public float maxRange;
    private Vector3 leadOffset = new Vector3();
    public int updateInterval = 5;
    public AudioSource firingSound;
    public float firingForceVariation = 0f;


    public void Start(){
        ammunition = ammotypes[ammoIndex];
        launchVelocity = launchForce*(1 / ammunition.GetComponent<Rigidbody>().mass);
        weaponOperator = transform.GetComponentInParent<crewMember>().gameObject;
        weaponControls = transform.root.GetComponentInChildren<weaponControls>();
        if(weaponControls != null){
            weaponControls.addSoldier(weaponOperator);
        }

        crewMemberScript = weaponOperator.GetComponent<crewMember>();
        gameObject.layer = weaponOperator.layer;
        
        ammunition.layer = gameObject.layer;
        foreach(Transform child in ammunition.transform)
        {
            child.gameObject.layer = gameObject.layer;
        }

        crewMemberScript.reloadTime = reloadTime;
        gravity = Physics.gravity.magnitude;
        isLoaded = true;

        if(target == null){
            if(weaponControls == null){
                target = transform.root.GetComponentInChildren<AIControls>().weaponTarget;
            }
            else{
                target = transform.root.GetComponentInChildren<weaponControls>().target;
            }
        }
        maxRange = calculateMaxWeaponRange();
        firingVector = Vector3.forward;
    }
    public void FixedUpdate(){
        frames++;
        if (frames % updateInterval == 0) {
            FrameNUpdate();
        }
    }

    public void FrameNUpdate(){
        firingVector = findFiringVector();
        aimAtTarget();
        frames = 0;
    }
    public void aimAtTarget(){
        transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward,firingVector,rotationSpeed*Time.deltaTime,0.0f));
    }

    public void fire()
    {
        if(isLoaded){
            
            firingVector = findFiringVector();
            aimAtTarget();
            isLoaded = false;
            crewMemberScript.setAnimatorProperty("isLoaded",isLoaded);
            crewMemberScript.setAnimatorProperty("reloadSpeed",1/reloadTime*2);
            projectileModel.SetActive(false);
            GameObject projectile = Instantiate(ammunition);
            firingSound.pitch = Random.Range(0.85f,1.15f);
            firingSound.PlayDelayed(Random.Range(0.0f,0.4f));
            projectile.transform.rotation = transform.rotation;
            projectile.transform.position = new Vector3(transform.position.x,transform.position.y,transform.position.z);
            projectile.layer = gameObject.layer;
            foreach(Transform child in projectile.transform)
            {
                child.gameObject.layer = gameObject.layer;
            }
            projectile.GetComponent<Rigidbody>().AddForce(Quaternion.Euler(UnityEngine.Random.Range(-dispersion, dispersion), UnityEngine.Random.Range(-dispersion, dispersion),0)* transform.forward * firingVector.magnitude * Random.Range(1-firingForceVariation,1+firingForceVariation) ,ForceMode.VelocityChange);
            reload();
        }
    }

    public void reload()
    {
        StartCoroutine(Reload());
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(reloadTime);
        projectileModel.SetActive(true);
        isLoaded = true;
        crewMemberScript.setAnimatorProperty("isLoaded",isLoaded);
        weaponOperator.GetComponent<crewMember>();
    }

    private Vector3 findFiringVector(){

        Vector3 distanceVector = new Vector3(target.transform.position.x,0,target.transform.position.z) - new Vector3(transform.position.x,0,transform.position.z);
        float distance = distanceVector.magnitude;
        float heightDifference = target.transform.position.y-transform.position.y;
        float sqrt = Mathf.Sqrt(Mathf.Pow(launchVelocity,4)-gravity*(gravity*Mathf.Pow(distance,2)+2*heightDifference*Mathf.Pow(launchVelocity,2)));
        float yAngleRad = Mathf.Atan((Mathf.Pow(launchVelocity,2)-sqrt)/(gravity*distance));
        float yAngle = yAngleRad * 180/Mathf.PI;
        Quaternion elevation = Quaternion.AngleAxis(yAngle, -transform.right);

        target.TryGetComponent<Rigidbody>(out Rigidbody lead);
        if(lead != null){
            float flighttime = (launchVelocity * Mathf.Sin(yAngleRad) + Mathf.Sqrt(Mathf.Abs(Mathf.Pow(launchVelocity*Mathf.Sin(yAngleRad),2)+2*gravity*heightDifference)))/gravity;
            leadOffset = lead.velocity*flighttime;
            leadOffset.y = 0;
    
        }

        Vector3 result = elevation * distanceVector + leadOffset;
        return result.normalized*launchVelocity;
    }


    
    // If findFiringVector() returns a zero, target is outside maximum range.
    public bool targetIsInRange(){
        return findFiringVector().magnitude > 0;
    }

    public void nextAmmoType()
    {
        if(ammoIndex < ammotypes.Count()-1){
            ammoIndex++;
        }
        else{
            ammoIndex = 0;
        }
        ammunition = ammotypes[ammoIndex];
        launchVelocity = launchVelocity*(1 / ammunition.GetComponent<Rigidbody>().mass);
    }
    public void setTarget(GameObject target){
        this.target = target;
    }

    public float calculateMaxWeaponRange(){
        return (launchVelocity*Mathf.Sin(45)+Mathf.Sqrt(Mathf.Pow(launchVelocity*Mathf.Sin(45),2)+(2*gravity*0)))/gravity;
    }

    public float maxWeaponRange(){
        return calculateMaxWeaponRange();
    }
}
