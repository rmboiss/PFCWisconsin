using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
// using Battlehub.RTCommon;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {

    // This script will display the HUD depending on user input display (screen vs VR vs tablet)
    // The HUD includes:
    // - Scoring System
    // - Possibly: Trial count (remaining/current), current task (animals/color/direction)
    public Camera MainCamera
    {
        get { return Camera.main;}
    }
    public enum DisplayTypes
    {
        NoVR,
        VR
    }

    public bool result;
    
    public TextMeshProUGUI HUDValues;
    public List<string> scoreElements = new List<string>();
    
    public enum scoreTypes {Score, Multiplier, Grade}
    public enum scoreGrades {Excellent, Null}

    private int streakMultiplier = 0;
    public int StreakMultiplier {
        get { return streakMultiplier; }
        set 
        {
            streakMultiplier = (value < streakMultiplier) ? 0 : value;
        }
    }
    private int scoreCounter = 0;
    public int ScoreCounter
    {
        get { return scoreCounter; }
        set { scoreCounter = scoreCounter + value; }
    }
    public void ProcessScore(scoreGrades grade) 
    {
        string[] scores = { "Perfect", "Wrong" }; // new List<int>(new int[] { 10, 10, 10, 0 });

        gameObject.SetActive(true);
        if (grade == scoreGrades.Null) 
        {
            StreakMultiplier--;
        }
        else 
        {
            StreakMultiplier++;
        }

        ScoreCounter = scores[(int)grade] == "Perfect" ? 1: 0; // scoreValues[(int)grade];
        ElementUpdate(scoreTypes.Grade, scores[(int)grade].ToString());// Values[(int)grade].ToString());
        ElementUpdate(scoreTypes.Score, ScoreCounter.ToString());
        ElementUpdate(scoreTypes.Multiplier, StreakMultiplier.ToString());
        GenerateScore();
    }
    
    private void ElementUpdate(scoreTypes element, string text) 
    {
        scoreElements[(int)element] = text;
    }

    public void GenerateScore()
    {
        HUDValues.text = scoreElements[0] + "\n" + scoreElements[1] + "\n"
                         + scoreElements[2];
        StartCoroutine(ResetEffects());
    }

    IEnumerator ResetEffects()
    {
        yield return new WaitForSeconds(2);
        // Debug.Log("Hiding score");
        this.gameObject.SetActive(false);
    }
    
    // FeedbackScore will have references to this (Singleton)
    public static HUDManager instance = null; 
    public void Awake() 
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void ScoreEffects()
    {
        // TODO: Animate _streakMultiplier into 3D text here that persists from the end of the trial to intertrial
    }
    
}
