using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class MenuEvents : MonoBehaviour
{

    public MainMenuCarousels carousels;
    private void OnEnable(){
        
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        VisualElement tutorialWindow = root.Q<VisualElement>("tutorialWindow");
        Button menuButton = root.Q<Button>("mainMenu");
        Button howToPlayButton = root.Q<Button>("howToPlay");
        Button startButton = root.Q<Button>("startGame");
        Button previousBoatButton = root.Q<Button>("previousBoat");
        Button nextBoatButton = root.Q<Button>("nextBoat");
        Button quitGameButton = root.Q<Button>("quit");
        Label scoreLabel = root.Q<Label>("score");




        if(menuButton != null){            
            menuButton.clicked += () => SceneManager.LoadScene("mainMenu");
        }      
        if(startButton != null){
            startButton.clicked += () => SceneManager.LoadScene(carousels.getBoatName()+"vs"+carousels.getLevelName());
        }
        if(scoreLabel != null){
            scoreLabel.text = Score.score.ToString();
        }
        if(quitGameButton != null){
            quitGameButton.clicked += () => Application.Quit();
        }
        if(tutorialWindow != null){
            tutorialWindow.style.display = DisplayStyle.None;
        }
        if(howToPlayButton != null){
                howToPlayButton.clicked += () => {
                    if(tutorialWindow.style.display == DisplayStyle.None)
                        {
                        tutorialWindow.style.display = DisplayStyle.Flex;
                        }
                    else{
                        tutorialWindow.style.display = DisplayStyle.None;
                        }
                };    
        }
    }
}
