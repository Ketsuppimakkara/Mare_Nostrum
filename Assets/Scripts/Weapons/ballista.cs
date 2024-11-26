using System.Collections;
using System.Linq;
using UnityEngine;

public class Ballista : MonoBehaviour, IRangedWeapon
{
    public GameObject ammunition;
    public GameObject target;
    public GameObject rightArm;
    public GameObject leftArm;
    public GameObject rightString;
    public GameObject leftString;
    private Vector3 firingVector;
    public GameObject weaponOperator;
    public crewMember crewMemberScript;
    public weaponControls weaponControls;
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
    private int ammoIndex = 0;
    public GameObject[] ammotypes;
    public float maxRange;
    public AudioSource firingSound;
    public void Start(){
        ammunition = ammotypes[ammoIndex];
        launchVelocity = launchForce*(1 / ammunition.GetComponent<Rigidbody>().mass);

        if(weaponOperator == null){
            weaponOperator = transform.GetComponent<crewMember>().gameObject;
        }
        if(weaponOperator == null){
         weaponOperator = transform.GetComponentInParent<crewMember>().gameObject;
        }
        if(weaponOperator == null){
            weaponOperator = transform.GetComponentInParent<crewMember>().gameObject;
        }

        crewMemberScript = weaponOperator.GetComponent<crewMember>();
        weaponControls = transform.root.GetComponentInChildren<weaponControls>();

        if(target == null){
            target = weaponControls.target;
        }
        
        weaponControls.addSoldier(weaponOperator);
        gravity = Physics.gravity.magnitude;
        isLoaded = true;
        if(target == null){
            target = transform.parent.GetComponent<weaponControls>().target;
        }
        maxRange = calculateMaxWeaponRange();
    }
    public void FixedUpdate(){
        firingVector = findFiringVector();
        aimAtTarget();
        //Debug.DrawLine(transform.position,transform.position+firingVector,Color.green);
    }
    public void aimAtTarget(){
        transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward,firingVector,rotationSpeed*Time.deltaTime,0.0f));
    }
    public void setAmmunition(GameObject ammunition){
        this.ammunition = ammunition;
    }

    public void fire()
    {
        if(isLoaded){
            isLoaded = false;
            crewMemberScript.setAnimatorProperty("isLoaded",isLoaded);
            leftArm.transform.Rotate(0,42f,0);
            rightArm.transform.Rotate(0,-42f,0);
            leftString.transform.Rotate(0,-90f,0);
            rightString.transform.Rotate(0,90f,0);
            projectileModel.SetActive(false);
            GameObject projectile = Instantiate(ammunition);
            firingSound.Play();
            projectile.transform.rotation = transform.rotation;
            projectile.transform.position = new Vector3(transform.position.x,transform.position.y,transform.position.z);
            projectile.layer = gameObject.layer;
            projectile.GetComponent<Rigidbody>().AddForce(Quaternion.Euler(UnityEngine.Random.Range(-dispersion, dispersion), UnityEngine.Random.Range(-dispersion, dispersion),0)* transform.forward * firingVector.magnitude ,ForceMode.VelocityChange);
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
        leftArm.transform.rotation = transform.rotation;
        rightArm.transform.rotation = transform.rotation;
        leftString.transform.rotation = transform.rotation;
        rightString.transform.rotation = transform.rotation;
        projectileModel.SetActive(true);
        isLoaded = true;
        crewMemberScript.setAnimatorProperty("isLoaded",isLoaded);

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
        //Debug.DrawLine(new Vector3(origin.transform.position.x,0,origin.transform.position.z),new Vector3(target.transform.position.x,0,target.transform.position.z));
        //Debug.DrawLine(new Vector3(0,0,0),distanceVector,Color.red);
        //Debug.DrawLine(new Vector3(0,0,0),result,Color.green);
        
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
