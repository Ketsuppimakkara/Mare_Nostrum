using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class trailFoamAmplitude : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody boat;
    public WaterFoamGenerator foam;
    public int frames = 0;
    public int updateInterval = 20;
    public double maxVelocity = 15f;
    public float foamIntensity = 0.25f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(frames % updateInterval == 0){
            FrameNUpdate();
        }
    }

    void FrameNUpdate(){
        foam.regionSize.y = boat.velocity.magnitude;
        foam.surfaceFoamDimmer = (float)(boat.velocity.magnitude / maxVelocity * foamIntensity);
    }
}
