using UnityEngine;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    
    public GameObject boat;
    public GameObject compartments;
    public GridFormation gridFormation;
    public GameObject deck;
    ProgressBar floodingStatusBar;
    Label currentSoldierLabel;
    Label currentRowerLabel;
    Label maxSoldierLabel;
    Label maxRowerLabel;
    Label scoreLabel;
    newBuyouancy newBuyouancy;
    crewMember[] currentRowers;    
    crewMember[] currentSoldiers;
    public Score score;

    private void OnEnable(){
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        floodingStatusBar = root.Q<ProgressBar>("FloodingStatusBar");
        currentSoldierLabel = root.Q<Label>("CurrentSoldierCount");
        currentRowerLabel = root.Q<Label>("CurrentRowerCount");
        maxSoldierLabel = root.Q<Label>("MaxSoldierCount");
        maxRowerLabel = root.Q<Label>("MaxRowerCount");
        scoreLabel = root.Q<Label>("score");

        newBuyouancy = boat.GetComponent<newBuyouancy>();
        currentRowers = compartments.GetComponentsInChildren<crewMember>();
        currentSoldiers = deck.GetComponentsInChildren<crewMember>();

        int maxRowers = currentRowers.Length;
        int maxSoldiers = gridFormation.soldiersToSpawn;
        maxRowerLabel.text = maxRowers.ToString();
        maxSoldierLabel.text = maxSoldiers.ToString();
        currentSoldierLabel.text = maxSoldiers.ToString();
        currentRowerLabel.text = maxRowers.ToString();
    }

    private void Update(){
        UpdateFloodingStatistics();
        scoreLabel.text = Score.score.ToString();
    }

    public void UpdateCrewMemberStatistics(){
        currentRowers = compartments.GetComponentsInChildren<crewMember>();
        currentSoldiers = deck.GetComponentsInChildren<crewMember>();

        int currentRowerCount = 0;
        int currentSoldierCount = 0;
        foreach(crewMember crewMember in currentRowers){
            if(crewMember.isAlive == true){
                    currentRowerCount++;
            }
        }        
        foreach(crewMember crewMember in currentSoldiers){
            if(crewMember.isAlive == true){
                    currentSoldierCount++;
            }
        }
        currentRowerLabel.text = currentRowerCount.ToString();
        currentSoldierLabel.text = currentSoldierCount.ToString();
    }

    public void UpdateFloodingStatistics(){
        floodingStatusBar.value = newBuyouancy.totalFloodingLevel();
    }
}
