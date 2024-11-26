using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuCarousels : MonoBehaviour
{
    public Texture2D[] boatImages;
    public Texture2D[] levelImages;
    VisualElement currentBoatImage;
    VisualElement currentLevelImage;
    public int boatSelection = 0;
    public int levelSelection = 0;


    private void OnEnable()
    {
        
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        currentBoatImage = root.Q<VisualElement>("boatImage");
        currentBoatImage.style.backgroundImage = boatImages[0];
        currentLevelImage = root.Q<VisualElement>("levelImage");
        currentLevelImage.style.backgroundImage = levelImages[0];

        Button nextBoatButton = root.Q<Button>("nextBoat");
        Button previousBoatButton = root.Q<Button>("previousBoat");
        Button nextLevelButton = root.Q<Button>("nextLevel");
        Button previousLevelButton = root.Q<Button>("previousLevel");

        nextBoatButton.clicked += () => {
            boatSelection++;
            if(boatSelection >= boatImages.Length){
                boatSelection = 0;
            }
            currentBoatImage.style.backgroundImage = boatImages[boatSelection];
        };

        previousBoatButton.clicked += () => {
            boatSelection--;
            if(boatSelection < 0){
                boatSelection = boatImages.Length-1;
            }
            currentBoatImage.style.backgroundImage = boatImages[boatSelection];
        };

        nextLevelButton.clicked += () => {
            levelSelection++;
            if(levelSelection >= levelImages.Length){
                levelSelection = 0;
            }
            currentLevelImage.style.backgroundImage = levelImages[levelSelection];
        };

        previousLevelButton.clicked += () => {
            levelSelection--;
            if(levelSelection < 0){
                levelSelection = levelImages.Length-1;
            }
            currentLevelImage.style.backgroundImage = levelImages[levelSelection];
        };
    }

    public string getLevelName(){
        return levelImages[levelSelection].name;
    }
    public string getBoatName(){
        return boatImages[boatSelection].name;
    }

}
