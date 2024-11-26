using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class windCompass : MonoBehaviour
{
    public Wind wind;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.LookAt(wind.windDirection+transform.position);
    }
}
