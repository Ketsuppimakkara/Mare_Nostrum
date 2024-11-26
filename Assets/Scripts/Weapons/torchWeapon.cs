using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class thrownWeapon : MonoBehaviour, IRangedWeapon
{

    public GameObject weaponOperator;
    public weaponControls weaponControls;
    public GameObject ammunition;
    public GameObject target;
    public crewMember crewMemberScript;
    private Vector3 firingVector;
    public float launchForce;
    private float launchVelocity;
    public float reloadTime;
    public float dispersion;
    public int projectilesPerShot;
    public int shotsBeforeReload;
    private float gravity;
    public bool isLoaded;
    public float rotationSpeed;
    public GameObject projectileModel;
    private int ammoIndex = 0;
    public GameObject[] ammotypes;
    public float maxRange;
    // Start is called before the first frame update
    void Start()
    {
        ammunition = ammotypes[ammoIndex];
        weaponOperator = transform.GetComponentInParent<crewMember>().gameObject;
        weaponControls = transform.root.GetComponentInChildren<weaponControls>();
        if(weaponControls != null){
            weaponControls.addSoldier(weaponOperator);
        }
        crewMemberScript = weaponOperator.GetComponent<crewMember>();
        gameObject.layer = weaponOperator.layer;
        ammunition.layer = gameObject.layer;
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
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        launchVelocity = launchForce*(1 / ammunition.GetComponent<Rigidbody>().mass);
        firingVector = findFiringVector();
        aimAtTarget();
        //Debug.DrawLine(transform.position,transform.position+firingVector,Color.green);
    }
    public void aimAtTarget(){
        transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward,firingVector,rotationSpeed*Time.deltaTime,0.0f));
    }
    public void fire()
    {
        if(isLoaded){
            isLoaded = false;
            crewMemberScript.setAnimatorProperty("isLoaded",isLoaded);
            crewMemberScript.setAnimatorProperty("reloadSpeed",1/reloadTime*2);
            projectileModel.GetComponent<MeshRenderer>().enabled = false;
            StartCoroutine("DelayFiring");
        }
    }

    public void reload()
    {
        StartCoroutine(Reload());
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(reloadTime*0.5f);
        projectileModel.GetComponent<MeshRenderer>().enabled = true;
        isLoaded = true;
        crewMemberScript.setAnimatorProperty("isLoaded",isLoaded);
    }
    private IEnumerator DelayFiring()
    {
        yield return new WaitForSeconds(reloadTime*0.5f);
        if(targetIsInRange()){
            GameObject projectile = Instantiate(ammunition);
            projectile.transform.rotation = transform.rotation;
            projectile.transform.position = new Vector3(transform.position.x,transform.position.y,transform.position.z);
            projectile.layer = gameObject.layer;
            projectile.GetComponent<Rigidbody>().AddForce(Quaternion.Euler(UnityEngine.Random.Range(-dispersion, dispersion), UnityEngine.Random.Range(-dispersion, dispersion),0)* transform.forward * firingVector.magnitude ,ForceMode.VelocityChange);
            reload();
        }
        else{
            projectileModel.GetComponent<MeshRenderer>().enabled = true;
            isLoaded = true;
            crewMemberScript.setAnimatorProperty("isLoaded",isLoaded);
        }

    }
    private Vector3 findFiringVector(){
        Vector3 distanceVector = new Vector3(target.transform.position.x,0,target.transform.position.z) - new Vector3(transform.position.x,0,transform.position.z);
        float distance = distanceVector.magnitude;
        float heightDifference = target.transform.position.y-transform.position.y;
        float sqrt = Mathf.Sqrt(Mathf.Pow(launchVelocity,4)-gravity*(gravity*Mathf.Pow(distance,2)+2*heightDifference*Mathf.Pow(launchVelocity,2)));
        float angleRad = Mathf.Atan((Mathf.Pow(launchVelocity,2)-sqrt)/(gravity*distance));
        float angle = angleRad * 180/Mathf.PI;
        Quaternion myRotation = Quaternion.AngleAxis(angle, -transform.right);
        Vector3 result = myRotation * distanceVector;
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
