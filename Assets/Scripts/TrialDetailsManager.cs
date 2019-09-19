using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrialDetailsManager : MonoBehaviour
{
    public static TrialDetailsManager instance = null;
    public Text TrialIndicesLeft;

    public Text TrialIndicesRight;

    private WisconsinTrialState currentTrialDetails;
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void UpdateTrialDetails(WisconsinTrialState current)
    {
        string trule = (current.TrialRule == 1 ? "Color" :
                        current.TrialRule == 2 ? "Shape" :
                        current.TrialRule == 3 ? "Number" : "Unknown");
        string tposition = (current.TargetPositionIndex == 0 ? "Bottom" :
                            current.TargetPositionIndex == 1 ? "Upper" :
                            current.TargetPositionIndex == 2 ? "Right" :
                            current.TargetPositionIndex == 3 ? "Left" : "Unknown");
        string sposition = (current.SelectedPositionIndex == 0 ? "Bottom" :
                    current.SelectedPositionIndex == 1 ? "Upper" :
                    current.SelectedPositionIndex == 2 ? "Right" :
                    current.SelectedPositionIndex == 3 ? "Left" : "Unknown");

        TrialIndicesLeft.text = current.TrialIndex.ToString() + "\n" +
                                trule + "\n" +
                                current.TrialRuleLength.ToString() + "\n" +
                                current.Response.ToString() + "\n";
        TrialIndicesRight.text = (current.TrialRuleLength == 0 ? "New Rule" : "Same Rule") + "\n" +
                                 tposition + "\n" +
                                 sposition + "\n" +
                                 current.Outcome;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
