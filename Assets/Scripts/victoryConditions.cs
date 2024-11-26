using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class victoryConditions : MonoBehaviour
{
    public boatDestruction[] enemyBoats;
    
    public boatDestruction[] alliedBoats;
    public boatDestruction playerBoat;
    private int enemiesRemaining; 
    public Score score;
    // Start is called before the first frame update
    void Start()
    {
        Score.score = 0;
        enemiesRemaining = enemyBoats.Length;
    }

    // Update is called once per frame
    void Update(){
        if(playerBoat.isDestroyed){
            SceneManager.LoadScene("playerDefeated");
        }
        if(!enemyBoats.Any(enemy=> enemy.isDestroyed == false)){
            SceneManager.LoadScene("playerVictorious");
        }
    }
}
