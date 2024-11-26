using System.Collections;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public class boatDestruction : MonoBehaviour
{
    public GameObject boat;
    public newBuyouancy buyouancy;
    public crewMember[] crewMembers;
    public Score score;
    public bool isDestroyed = false;
    public int scoreGainOnKill = 500;
    public bool coroutineRunning = false;
    // Start is called before the first frame update
    void Start()
    {
        if(buyouancy == null){
            buyouancy = boat.GetComponentInChildren<newBuyouancy>();
        }
    }

    private void getCrew(){
        crewMembers = boat.GetComponentsInChildren<crewMember>();
    }

    
    // Update is called once per frame
    void Update()
    {
        if(!isDestroyed && coroutineRunning == false){
            StartCoroutine(checkDeath());
        }
    }

    IEnumerator checkDeath()
        {
            coroutineRunning = true;
            yield return new WaitForSeconds(1);
            getCrew();
            if(buyouancy.totalFloodingLevel() >= 0.6f && transform.position.y <= -10f){
                    isDestroyed = true;
                    if(gameObject.layer != LayerMask.NameToLayer("Player")){
                        Score.score = Score.score + scoreGainOnKill;
                    }
            }
            int crewMembersLeft = 0;
            foreach(crewMember crewMember in crewMembers){
                if(crewMember == null){
                    continue;
                }
                if(crewMember.isAlive){
                    crewMembersLeft++;
                }
            }
            if((float)crewMembersLeft < 0.2f* (float)crewMembers.Length){
                isDestroyed = true;
                if(gameObject.layer != LayerMask.NameToLayer("Player")){
                    Score.score = Score.score + scoreGainOnKill;
                }
            }
            coroutineRunning = false;
        }
}
