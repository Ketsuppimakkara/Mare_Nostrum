using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[Serializable]
public class newBuyouancy : MonoBehaviour
{
    
    public List<zCompartments> xcompartments; // 3d list of compartments, x,z,y
    public Rigidbody boat;
	private const float DAMPFER = 0.05f;
    private const float yDAMPFER = 200f;
	private const float WATER_DENSITY = 1000;
    WaterSearchParameters Search;
    WaterSearchResult SearchResult;
    public WaterSurface water;
    private int compartmentCount;
    public int frames = 0;
    public int updateInterval = 5;
    public float lateralDampingFactor = 0.05f;
    public float sideslipDampeningFactor = 0.995f;
    // Start is called before the first frame update
    void Start()
    {
        compartmentCount = xcompartments.Count * xcompartments[0].zcompartments.Count * xcompartments[0].zcompartments[0].ycompartments.Count;
    }
    public void FixedUpdate(){
        frames++;
        if (frames % updateInterval == 0) {
            FrameNUpdate();
        }
    }
    // Update is called once per frame
    void FrameNUpdate()
    {
        foreach(zCompartments xcompartments in this.xcompartments){
            foreach(yCompartments zcompartments in xcompartments.zcompartments){
                foreach(Compartment y in zcompartments.ycompartments){
                    y.spreadToNeighbors();
                    float waterLevel = getWaterLevel(y.transform.position.x,y.transform.position.z);
                    if(waterLevel > y.transform.position.y - y.transform.GetComponent<BoxCollider>().transform.localScale.y * y.transform.GetComponent<BoxCollider>().bounds.size.y){
                        float distanceSubmerged = waterLevel - (y.transform.position.y - y.transform.GetComponent<BoxCollider>().size.y/2);
                        float percentageSubmerged = distanceSubmerged/y.transform.GetComponent<BoxCollider>().size.y;
                        float archimedesForceMagnitude = WATER_DENSITY * Mathf.Abs(Physics.gravity.y) * y.volume * Mathf.Clamp(percentageSubmerged,0,1);
                        Vector3 localArchimedesForce = new Vector3(0, archimedesForceMagnitude, 0);
                        

                        var velocity = boat.GetPointVelocity(y.transform.position);
                        var localDampingForce = -velocity * DAMPFER * boat.mass * Mathf.Clamp(1/y.LiftFactor(),0,20);
                        float k = Mathf.Clamp01((waterLevel - y.transform.position.y) / y.transform.GetComponent<BoxCollider>().size.y * 0.5f);

                        // Dampen the vertical movement when sinking
                        if(velocity.y < 0){
                            boat.AddForceAtPosition(new Vector3(0, -velocity.y * yDAMPFER,0),y.transform.position,ForceMode.Impulse);
                            //Debug.DrawLine(y.transform.position,y.transform.position-velocity * yDAMPFER,Color.red);
                        }

                        y.UpdateBuoyancy(waterLevel);
                        var force = localDampingForce + Mathf.Sqrt(k) * (localArchimedesForce * 0.5f) *y.LiftFactor();
                        boat.AddForceAtPosition(force / compartmentCount * updateInterval,y.transform.position);
                        //Debug.Log(y.name+" localDampingForce ("+localDampingForce+") + Mathf.Sqrt(("+k+")) * localArchimedesForce()("+localArchimedesForce+")*1.8f*y.LiftFactor()("+y.LiftFactor()+") = "+force);
                        //Debug.DrawLine(y.position,y.position + transform.right * 10, Color.yellow);
                    }
                }
            }
        }

        Vector3 lateralVelocity = Vector3.Project(boat.velocity, transform.right);
        Vector3 dampingForce = -lateralVelocity * lateralDampingFactor;

        // Apply the damping force to the Rigidbody
        boat.AddForce(dampingForce, ForceMode.Acceleration);
        frames=0;
    }

    public float getWaterLevel(float x, float z){
        Search = new WaterSearchParameters(); 
        Search.startPositionWS = new Vector3(x,0f,z);
        water.ProjectPointOnWaterSurface(Search, out SearchResult);
        //Debug.DrawLine(SearchResult.candidateLocationWS, (Vector3)SearchResult.candidateLocationWS + Vector3.up * SearchResult.projectedPositionWS.y,Color.yellow);
		return SearchResult.projectedPositionWS.y;
    } 

    public float totalFloodingLevel(){
        float totalWaterVolume = 0f;
        float totalVolume = 0f; 
        foreach(zCompartments xcompartments in this.xcompartments){
            foreach(yCompartments zcompartments in xcompartments.zcompartments){
                foreach(Compartment y in zcompartments.ycompartments){
                    totalWaterVolume = totalWaterVolume + y.waterVolume;
                    totalVolume = totalVolume + y.volume;
                }
            }
        }
        return 1-(totalVolume-totalWaterVolume) /totalVolume;
    }

    [Serializable]
    public class zCompartments{
        public List<yCompartments> zcompartments;
    }

    [Serializable]
    public class yCompartments{
        public List<Compartment> ycompartments;
    }
}
