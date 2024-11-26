using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomizeTimeOfDay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.eulerAngles = new Vector3(Random.Range(13,60),Random.Range(0,359),0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
