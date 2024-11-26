using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class soldierFormation : MonoBehaviour
{
    public List<GameObject> soldiers = new List<GameObject>();
    public Vector3 area;
    public float xPadding;
    public float zPadding;
    public Vector3 padding;
    private int ranks;
    private int files;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 soldierSize = new Vector3();
        foreach(GameObject soldier in soldiers){
            if(soldier.GetComponent<Renderer>().bounds.size.magnitude > soldierSize.magnitude){
                soldierSize = soldier.GetComponent<Renderer>().bounds.size;
            }
        }
        int soldiersPerRank = (int) Mathf.Floor(area.x / (soldierSize.x + xPadding));
        ranks = Mathf.CeilToInt((float)soldiers.Count / (float)soldiersPerRank);
        files = 3;

        ArrangeGrid();
        //for (int i = 0; i < ranks; i++){
        //    List<GameObject> rankOfSoldiers = new List<GameObject>();
        //    for (int j = i; j < soldiersPerRank; j++){
        //        if(j < soldiers.Count){
        //            rankOfSoldiers.Add(soldiers[j]);
        //            Vector3 soldierPosition = transform.position - transform.rotation * new Vector3(soldierSize.x + xPadding * j ,0,soldierSize.z + zPadding * i);
        //            soldiers[j].transform.position = soldierPosition;
        //        }
        //        }
        //    }
    }

    void ArrangeGrid()
    {
        int childCount = transform.childCount;
        if (childCount == 0)
        {
            Debug.LogWarning("No child objects to arrange.");
            return;
        }

        float parentWidth = GetComponent<Renderer>().bounds.size.x;
        float parentHeight = GetComponent<Renderer>().bounds.size.y;

        float cellWidth = (parentWidth - (files - 1) * padding.x) / files;
        float cellHeight = (parentHeight - (ranks - 1) * padding.y) / ranks;

        float startX = -parentWidth / 2 + cellWidth / 2;
        float startY = parentHeight / 2 - cellHeight / 2;

        int row = 0;
        int col = 0;

        foreach (GameObject child in soldiers)
        {
            if (col >= files)
            {
                col = 0;
                row++;
            }

            if (row >= ranks)
            {
                Debug.LogWarning("Not enough rows and columns to fit all children.");
                break;
            }

            float posX = startX + col * (cellWidth + padding.x);
            float posY = startY - row * (cellHeight + padding.y);

            child.transform.localPosition = new Vector3(posX, posY, 0);

            col++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
