using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Serialization;


[Serializable]
public abstract class ExperimentObject : MonoBehaviour
{
    protected Material red_mat;
    protected Material blue_mat;
    protected Material green_mat;
    protected Material default_mat;
    public abstract void ResetColor();
    public abstract void ApplyColor();
    [NonSerialized]
    public bool quiet = false;
    private bool visibilityMarkerTrigger = false;
    // Properties
    [SerializeField]
    protected bool _isVisible = true;
    public bool IsVisible
    {
        get
        {
            return _isVisible;
        }
        set
        {
            gameObject.SetActive(value);
            _isVisible = value;
            visibilityMarkerTrigger = true;
            Publish();
        }
    }

    // This property hides the target and leaves the hit box
    [SerializeField]
    protected bool _isSkinOn = true;
    public bool IsSkinOn
    {
        get
        { 
            return _isSkinOn;
        }
        set
        {
            int lm = (value) ? LayerMask.NameToLayer("Default") : LayerMask.NameToLayer("NoHMD");
            SetLayerRecursively(gameObject, lm);
            _isSkinOn = value;
        }
    }

    [SerializeField] 
    protected string _identity = "name";
    public string Identity
    {
        get { return _identity; }
        set
        {
            _identity = value;
            Publish();
        }
    }

    [SerializeField]
    protected Color _color = Color.white;
    public Color Color
    {
        get { return _color; }
        set
        {
            _color = value;
            if (value == Color.black) {
                ResetColor();
            }
            else {
                ApplyColor();
            }
            Publish(); // Although animals currently don't change color, the fixation point does
        }
    }

    [SerializeField]
    protected Vector3 _position = new Vector3();
    public Vector3 Position
    {
        get { return GetComponent<Transform>().localPosition; }
        set
        {
            GetComponent<Transform>().localPosition = value;
            _position = value;
            Publish();
        }
    }

    [SerializeField]
    protected Vector3 _pointingTo = new Vector3();
    public Vector3 PointingTo
    {
        get { return _pointingTo; }
        set
        {
            Transform tf = GetComponent<Transform>();
            tf.rotation = Quaternion.identity;
            tf.LookAt(value);
            _pointingTo = value;
            Publish();
        }
    }
    
    // Methods
    private void Awake()
    {
        if (!gameObject.GetComponent<SphereCollider>())
        {
            gameObject.AddComponent<SphereCollider>().radius = 0.1f;
        }
        // Retrieve material references
        List<Material> cueColors = GameObject.Find("TaskInfo").GetComponent<BaseTaskInfo>().cueColors;
        foreach (Material mats in cueColors)
        {
            if (mats.name == "red_mat") 
            {
                red_mat = mats;
            }
            if (mats.name == "blue_mat") 
            {
                blue_mat = mats;
            }
            if (mats.name == "green_mat")
            {
                green_mat = mats;
            }
            if (mats.name == "default_mat") 
            {
                default_mat = mats;    
            }

            //GameObject.Find("FixationPoint").GetComponentInChildren<MeshRenderer>().material = blue_mat;
        }
        
    }
    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }
    
        obj.layer = newLayer;
    
        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
    public delegate void OnPublishDeleg(string pubstring);
    public OnPublishDeleg OnPublish;
    public void Publish()
    {
        // Prevent extraneous marker publishing when object is invisible,
        // but ensure a marker is published when visibility changes.
        if (!quiet && _isVisible || visibilityMarkerTrigger)
        {
            string pubstring = "{\"ObjectInfo\": " + JsonUtility.ToJson(this) + "}";
            // Generate a JSON string representing current state and emit via OnPublish
            if (OnPublish != null)
            {
                OnPublish(pubstring);
            }
            visibilityMarkerTrigger = false;
        }
    }
}