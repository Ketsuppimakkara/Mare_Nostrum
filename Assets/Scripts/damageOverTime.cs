using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class damageOverTime : MonoBehaviour
{
    public float damagePerSecond;
    public float damageModifier = 1;
    public fire fire;
    public int updateInterval = 15;
    public int frames = 0;
    // Start is called before the first frame update
    void Start()
    {
        fire = GetComponent<fire>();
        if(fire != null){
            damagePerSecond = fire.intensity * damageModifier;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(frames % updateInterval == 0){  
            FrameNUpdate();
            frames = 0;
        }
    }

    void FrameNUpdate(){
            damagePerSecond = fire.intensity * damageModifier;
    }

    void OnTriggerStay(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if(damageable == null){
            damageable = other.GetComponentInChildren<IDamageable>();
        }
        if(damageable == null){
            damageable = other.GetComponentInParent<IDamageable>();
        }
        if(damageable != null){
            //Debug.Log(damageable + " got hit by FIRE");
            //Debug.Log(other.gameObject.name +" - " + other.name+" got hit by FIRE");
            damageable.handleDamageOverTime(other,damagePerSecond);
        }
        //Debug.Log(gameObject.name + " colliding with "+ other.gameObject.name + " specifically "+ other.name + " damageable is "+damageable);
    }
}
