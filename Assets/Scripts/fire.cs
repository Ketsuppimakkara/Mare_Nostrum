using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fire : MonoBehaviour
{
    public float intensity;
    public float decayRate;
    private float effectiveDecayRate;
    public float maxRandomnessOnInitialization = 0.4f;
    public float maxFireIntensity = 5f;
    public int frame = 0;
    public int updateInterval = 5;
    public float fireComineTimeSeconds = 1f;
    public AudioSource fireAudio;
    // Start is called before the first frame update
    void Start()
    {
        effectiveDecayRate = decayRate;
        float intensityModifier = Random.Range(1-maxRandomnessOnInitialization*0.5f , 1+maxRandomnessOnInitialization*0.5f);
        intensity = intensity*intensityModifier;
        fireAudio.loop = true;
        fireAudio.Play();
    }

    // Update is called once per frame
    void FixedUpdate(){
        if(frame % updateInterval == 0){
            FrameNUpdate();
        }
    }

    void FrameNUpdate()
    {
        //if(gameObject.GetComponent<Rigidbody>().isKinematic){
        //    effectiveDecayRate = decayRate*(0.6f);
        //}
        intensity = Mathf.Clamp(intensity - (effectiveDecayRate * updateInterval * Random.Range(0.9f,1f)),0f,maxFireIntensity);        
        fireAudio.volume = Mathf.Clamp01(intensity/maxFireIntensity);
        if(intensity <= 0){
            Destroy(gameObject);
        }
        else{
            float sqrtIntensity = Mathf.Sqrt(intensity);
            transform.localScale = new Vector3(sqrtIntensity,sqrtIntensity,sqrtIntensity);
        }
        frame = 0;
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable)){
            gameObject.transform.parent = other.gameObject.transform;
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }
    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.TryGetComponent<fire>(out fire otherFire)){
            float intensityTransfer = (intensity * Time.deltaTime)*fireComineTimeSeconds;
            if(otherFire.transform.parent.TryGetComponent<IDamageable>(out IDamageable damageable)){
                if(otherFire.intensity > intensity && otherFire.intensity < maxFireIntensity - intensityTransfer){
                    otherFire.increaseIntensity(intensityTransfer);
                    intensity = intensity - intensityTransfer;
                    Debug.DrawLine(transform.position,otherFire.transform.position,Color.red);
                }
            }
        }
    }

    void increaseIntensity(float intensity){
        this.intensity = this.intensity+intensity;
    }
}
