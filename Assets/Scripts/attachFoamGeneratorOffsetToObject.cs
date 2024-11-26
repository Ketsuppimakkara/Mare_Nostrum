using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class attachFoamGeneratorOffsetToObject : MonoBehaviour
{
    public GameObject attachTo;
    public WaterSurface waterSurface;
    // Start is called before the first frame update
    void Start()
    {
        waterSurface = GetComponent<WaterSurface>();
    }

    // Update is called once per frame
    void Update()
    {
        waterSurface.foamAreaOffset.x = attachTo.transform.position.x;
        waterSurface.foamAreaOffset.y = attachTo.transform.position.z;
    }
}
