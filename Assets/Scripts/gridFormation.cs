using UnityEngine;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System;
using TMPro;
using UnityEngine.UIElements;
using UnityEngine.Animations;

public class GridFormation : MonoBehaviour
{
    public List<List<Vector3>> positions;
    public List<List<GameObject>> gameObjects;
    public Vector3 area;
    public Vector3 spacing;
    public GameObject archerPrefab;
    public UI ui;
    private int numGameobjects;
    public int soldiersToSpawn;

    public void Start(){
        positions = new List<List<Vector3>>();
        gameObjects = new List<List<GameObject>>();
        numGameobjects = 0;
        
        int rows = Mathf.CeilToInt(area.x/spacing.x);
        int columns = Mathf.CeilToInt(area.z/spacing.z);    
        Vector3 startPosition = new Vector3(-area.x/2+(spacing.x/2),0,area.z/2-(spacing.z/2));
        for (int row = 0; row < rows; row++)
        {
            positions.Add(new List<Vector3>());
            gameObjects.Add(new List<GameObject>());
            int column = 0;
            for (; column < columns; column++){
                Vector3 localPosition = startPosition + new Vector3(row * spacing.x, 0, -column * spacing.z);
                positions[row].Add(localPosition);
                gameObjects[row].Add(null);
            }
        }
        for(int i = 0; i<soldiersToSpawn;i++){
            GameObject soldier = Instantiate<GameObject>(archerPrefab,transform);
            if(ui != null){
                soldier.GetComponent<crewMember>().ui = ui;
            }
           soldier.layer = gameObject.layer;
           foreach (Transform child in soldier.transform)
           {
                child.gameObject.layer = gameObject.layer;
           }
           AddSoldier(soldier);
        }
    }

    public void Update(){
       if(Input.GetKeyDown(KeyCode.UpArrow) && numGameobjects < positions.Count * positions[0].Count){
           GameObject soldier = Instantiate<GameObject>(archerPrefab,transform);
           soldier.layer = gameObject.layer;
           foreach (Transform child in soldier.transform)
           {
                child.gameObject.layer = gameObject.layer;
           }
           AddSoldier(soldier);
       }        
       if(Input.GetKeyDown(KeyCode.DownArrow)){
           System.Random rand = new System.Random();
           int randint = rand.Next(0, gameObjects.Count);
           List<GameObject> randomRow = gameObjects[randint];
           int randint2 = rand.Next(0, randomRow.Count);
           GameObject soldier = randomRow[randint2];
           RemoveSoldier(soldier);
       }
    }

    public void RemoveSoldier(GameObject gameObjectToRemove){
        for(int row = 0; row < gameObjects.Count; row++){
            for(int column = 0; column < gameObjects[row].Count; column++){
                if(gameObjects[row][column] != null){
                    if(gameObjects[row][column].Equals(gameObjectToRemove)){
                        gameObjects[row][column] = null;
                        gameObjectToRemove.transform.SetParent(null);
                        numGameobjects--;
                        return;
                    }
                }
            }
        }
    }

    public void AddSoldier(GameObject gameObjectToAdd){
        for(int row = 0; row < gameObjects.Count; row++){
            for(int column = 0; column < gameObjects[row].Count; column++){
                if(gameObjects[row][column] == null){
                    gameObjects[row][column] = gameObjectToAdd;
                    gameObjectToAdd.transform.SetParent(gameObject.transform);
                    gameObjectToAdd.transform.position = transform.TransformPoint(positions[row][column]);
                    numGameobjects++;
                    return;
                }
            }
        }
    }
}