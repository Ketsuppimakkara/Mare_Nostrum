using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sail : MonoBehaviour, IDamageable
{
    public int furled = 0;
    public Rigidbody boat;
    public float hoistLevel;
    public float usableArea;
    public Wind wind;
    public Cloth cloth;
    public float fireDamageModifier = 1;
    public float area = 1000;
    public float effectiveArea = 1000;
    public Animator animator;
    public float hoistingTime = 10;
    public float sailThrustFactor = 2;
    // Start is called before the first frame update
    void Start()
    {
        effectiveArea = area;
        animator.speed = 0.5f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(hoistLevel != furled){
            if(hoistLevel < furled){
                hoistLevel = hoistLevel + 1/ hoistingTime * Time.deltaTime;
            }
            else{
                hoistLevel = hoistLevel - 1 / hoistingTime * Time.deltaTime;
            }
        }
        usableArea = Mathf.Clamp01(hoistLevel) * effectiveArea / area;
        cloth.externalAcceleration = wind.windDirection*10;
        cloth.randomAcceleration = wind.windDirection*4f;
        float sailToWindAngle = Mathf.Clamp(Mathf.Abs(Vector3.SignedAngle(new Vector3(transform.forward.x,0,transform.forward.z), wind.windDirection,new Vector3(0,boat.transform.up.y,0))),-30,90);
        Vector3 boatSpeedDelta = -new Vector3(wind.windDirection.x,0,wind.windDirection.z).normalized*wind.windSpeed*usableArea*sailThrustFactor*(-90+sailToWindAngle);
        boat.AddForceAtPosition(boatSpeedDelta,boat.transform.position);
    }

    public void unfurlSail(){
        furled = 0;
        animator.SetBool("furled",true);
    }
    public void furlSail(){
        furled = 1;
        animator.SetBool("furled",false);
    }

    public void handleDamage(Collision collision, GameObject projectile)
    {
        return;
    }

    public void handleDamageOverTime(Collider collider, float damagePerSecond)
    {
		effectiveArea = Mathf.Clamp(effectiveArea - damagePerSecond * Time.deltaTime * fireDamageModifier,0,area);
    }

    public void lodgeProjectile(Collision collision, GameObject projectile)
    {
        return;
    }

    public void lodgeProjectile(Collision collision, GameObject projectile, RaycastHit hit)
    {
        return;
    }

    public bool isEnabled()
    {
        return true;
    }
}
