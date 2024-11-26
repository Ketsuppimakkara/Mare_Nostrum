using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selfDestroying : MonoBehaviour
{
    public float delaySeconds;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,delaySeconds);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
