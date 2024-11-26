using UnityEngine;

public class hitDetection : MonoBehaviour
{
    private new Rigidbody rigidbody;
    public float projectileRadius;
    public float effectivePenetration;
    public float penetration;
    public bool destroyOnContact;
    public bool getsLodged;
    public bool hasOnHitEffect;
    public Collider onHitEffectCollider;
    public AudioSource hitSound;
    private bool audioPlayed;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = transform.root.transform.GetComponent<Rigidbody>();
        transform.TryGetComponent<CapsuleCollider>(out CapsuleCollider capsuleCollider);
        if(capsuleCollider != null){
            projectileRadius = capsuleCollider.radius*transform.lossyScale.x;
        }
        else{
            transform.TryGetComponent<BoxCollider>(out BoxCollider boxCollider);
            projectileRadius = boxCollider.size.x*transform.lossyScale.x;
        }
        audioPlayed = false;
    }
        //If your GameObject starts to collide with another GameObject with a Collider
    void OnCollisionEnter(Collision collision)
    {
        if(rigidbody != null){
            effectivePenetration = Mathf.Clamp(penetration * (rigidbody.velocity.magnitude/7f),0,penetration);
            IDamageable idamageable = collision.collider.gameObject.GetComponent<IDamageable>();
            if(idamageable == null){
                idamageable = collision.collider.gameObject.GetComponentInParent<IDamageable>();
            }
            if(idamageable == null){
                idamageable = collision.collider.gameObject.GetComponentInChildren<IDamageable>();
            }
            if(idamageable != null && idamageable.isEnabled()){
                
                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit,3f, 1<<collision.gameObject.layer)){
                    if(getsLodged){
                        idamageable.lodgeProjectile(collision,gameObject,hit);
                    }
                }
    
                idamageable.handleDamage(collision,gameObject);
    
            }
        }

        if (hitSound != null && audioPlayed == false){
            hitSound.pitch =  Random.Range(0.5f, 1f);
            hitSound.Play();
            audioPlayed = true;
        }
        if(hasOnHitEffect){
            if(onHitEffectCollider != null){
                onHitEffectCollider.enabled = true;
            }
        }
        if(destroyOnContact){
            Destroy(gameObject);
        }

    }

    //If your GameObject keeps colliding with another GameObject with a Collider, do something

    // Update is called once per frame
    void Update()
    {
        
    }
}
