using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;

public class SceneController : MonoBehaviour {
    public List<SceneField> expEnvironments;
    public List<int> assignCheckpoints;
    public enum EnvironmentTypes
    {
        Paintings,
        Campfire,
        Mountains,
        Cactus,
        Forest,
        NEnvironments
    }

    void Start() {

    }
}