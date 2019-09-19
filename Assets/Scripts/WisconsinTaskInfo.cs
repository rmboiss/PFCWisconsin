using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;

public class WisconsinTaskInfo : BaseTaskInfo {
    public static WisconsinTaskInfo m_instance = null;
    public void Awake() 
    {
        if (m_instance == null) 
        {
            m_instance = this;
        }
    }

    public List<GameObject> targetWalls = new List<GameObject>();

    public List<Modifiers> myModifiers = new List<Modifiers>();
    public List<ConditionTypes> myConditionTypes = new List<ConditionTypes>();
    public List<ResponseTypes> myResponseTypes = new List<ResponseTypes>();

    void Start() 
    {
        targetHold = 2.0f;
        // Populate Conditions lists with ones selected in inspector
        foreach (var cond in Conditions)
        {
            if (!myModifiers.Contains(cond.modifier))
            {
                myModifiers.Add(cond.modifier);
            }
            if (!myConditionTypes.Contains(cond.conditionType)) 
            {
                myConditionTypes.Add(cond.conditionType);
            }
            if (!myResponseTypes.Contains(cond.responseType)) 
            {
                myResponseTypes.Add(cond.responseType);
            }
        }
    }
}