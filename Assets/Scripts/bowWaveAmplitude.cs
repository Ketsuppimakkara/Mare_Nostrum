using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class bowWaveAmplitude : MonoBehaviour
{
    public Rigidbody boat;
    public WaterDeformer bowWave;
    public int frames = 0;
    public int updateInterval = 20;
    public float maxVelocity = 15f;
    public float amplitudeIntensity = 2f;
    public float maxWaveLength = 2f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(frames % updateInterval == 0){
            FrameNUpdate();
            frames = 0;
        }
        frames++;
    }

    void FrameNUpdate(){
        bowWave.regionSize.y = boat.velocity.magnitude * maxWaveLength; 
        bowWave.amplitude = boat.velocity.magnitude/maxVelocity * -amplitudeIntensity;
    }
}
