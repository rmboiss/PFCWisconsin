using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class WisconsinTrialState : BaseTrialState {

    private string outcome;

    public struct TargetObject
    {
        public int tindex;
        public Color tcolor;
        public int tnumber;

        public TargetObject(int index, Color color, int number)
        {
            tindex = index;
            tcolor = color;
            tnumber = number;
        }
    }

    // Rule Information
    [SerializeField]
    private int trialRule = 0;
    public int TrialRule
    {

        get { return trialRule; }
        set
        {
            trialRule = value;
            Publish();
        }
    }

    [SerializeField]
    private int trialRuleLength = 0;
    public int TrialRuleLength
    {

        get { return trialRuleLength; }
        set
        {
            trialRuleLength = value;
            Publish();
        }
    }

    // Target information
    [SerializeField]
    private TargetObject[] targetObjects = new TargetObject[5];
    public WisconsinTrialState.TargetObject[] TargetObjectL
    {
        get { return targetObjects; }
        set
        {
            targetObjects = value;
            Publish();
        }
    }

    public new string Outcome
    {
        get => outcome;
        set
        {
            outcome = value;
            TrialDetailsManager.instance.UpdateTrialDetails(this);
            Publish();
        }
    }
}