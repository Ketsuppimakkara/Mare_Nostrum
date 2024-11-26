using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class Wind : MonoBehaviour


{
    public float maxWindSpeed = 1500;
    public float minWindSpeed = 500;
    public float windSpeed;
    public float windDispersion = 5;
    public Vector3 windDirection;
    public Vector3 newDirection;
    public int probability;
    public WaterSurface ocean;
    public int frame = 0;
    public int updateInterval = 5;

    // Start is called before the first frame update
    void Start()
    {
        probability = 0;
        windSpeed = 1200;
        newDirection = new Vector3(UnityEngine.Random.Range(-20,20),0,UnityEngine.Random.Range(-20,20));
        windDirection = new Vector3(UnityEngine.Random.Range(-20,20),0,UnityEngine.Random.Range(-20,20)); // Randomize starting wind
        ocean.largeOrientationValue = -Vector3.SignedAngle(ocean.transform.right,windDirection,Vector3.up);
        ocean.largeWindSpeed = UnityEngine.Random.Range(minWindSpeed,maxWindSpeed)*0.025f;
    }

    void FixedUpdate(){
        if(frame % updateInterval == 0){
            FrameNUpdate();
        }
        frame++;
    }

    // Update is called once per frame
    void FrameNUpdate()
    {
        windDirection = Vector3.RotateTowards(windDirection.normalized,newDirection.normalized,0.015f,0.81f*Time.deltaTime);
        ocean.largeOrientationValue = -Vector3.SignedAngle(ocean.transform.right,windDirection,Vector3.up);
        float currentWindSpeed = ocean.largeWindSpeed;
        int modifiedWindSpeed = Mathf.RoundToInt(windSpeed * 0.025f);
        if(currentWindSpeed < modifiedWindSpeed){
            ocean.largeWindSpeed = ocean.largeWindSpeed+(5*Time.deltaTime);
        }
        else if(currentWindSpeed > modifiedWindSpeed){
            ocean.largeWindSpeed = ocean.largeWindSpeed-(5*Time.deltaTime);
        }

        if(probability > UnityEngine.Random.Range(40,100)){
            newDirection = new Vector3(UnityEngine.Random.Range(-windDispersion,windDispersion),0,UnityEngine.Random.Range(-windDispersion,windDispersion));
            probability = 0;
        }
        else if(probability > UnityEngine.Random.Range(40,100)){
            windSpeed = UnityEngine.Random.Range(minWindSpeed,maxWindSpeed);
            probability = 0;
        }
        else if(UnityEngine.Random.Range(0,100) > 50){
            probability++;
        }
        frame = 0;
    }
}
