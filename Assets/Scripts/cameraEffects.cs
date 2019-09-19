using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;
// using UnityStandardAssets.ImageEffects;

public class cameraEffects : MonoBehaviour
{
    public GameObject VRMode;
    public GameObject noVRMode;
    public GameObject expProps;
    public GameObject scoreHolder;
    public GameObject camAngleBox;
    private int camAngle = 0;
    public int CamAngle 
    {
        get { return camAngle; }
        set 
        {
            camAngle = value;
            camAngleBox.GetComponent<InputField>().text = value.ToString();
            OnCameraAngleEdit(value.ToString());
            SetExpObjects();
        }
    }
    public static cameraEffects instance = null;
    public void Awake() // give access to other scripts if other scripts need to use methods in here
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        // VR support in non-windows platforms are not optimized
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            VRMode.SetActive(true);
        }
        else
        {
            XRSettings.enabled = false;
            noVRMode.SetActive(true);
        }

        // TODO: Add CameraAngle GUI to all experiments
        camAngleBox.GetComponent<InputField>().onEndEdit.AddListener(OnCameraAngleEdit);
        camAngleBox.GetComponent<InputField>().text = "SelectInput";
        camAngleBox.GetComponent<InputField>().interactable = false;
    }

    void OnCameraAngleEdit(string value) 
    {
        int camAng = int.Parse(value);
        Camera mainCam = Camera.main;
        mainCam.GetComponent<Transform>().localRotation = Quaternion.Euler(-camAng, 0.0f, 0.0f);
    }

    public void CameraConfig(string device) 
    {
        if (device == "NoVR") 
        {
            GameObject.Find("ExperimenterCamera").tag = "MainCamera";
            GameObject.Find("ExperimenterCamera").transform.SetParent(noVRMode.transform);
            camAngleBox.GetComponent<InputField>().interactable = true;
            camAngleBox.GetComponent<InputField>().text = camAngle.ToString();
        }
        if (device == "VR") 
        {            
            GameObject.Find("ExperimenterCamera").tag = "Untagged";
        }
        // Additional setup config here    
    }

    public void ToggleCameraEffects(bool toggle) 
    {
        /* NatureStarterKit camera effects
        PostEffectsBase[] cameraScripts = gameObject.GetComponents<PostEffectsBase>();
        foreach (PostEffectsBase script in cameraScripts)
        {
            script.enabled = false;
        }
        */    
    }
    // Reset Position of targets to be in front of subject
    public void SetExpObjects() 
    {
        Camera currCam = Camera.main;
        expProps.transform.position = VRMode.activeSelf ? new Vector3(currCam.transform.position.x + 0.55f, currCam.transform.position.y - 0.05f, currCam.transform.position.z) :
                                        new Vector3(currCam.transform.position.x, currCam.transform.position.y - 0.1f, currCam.transform.position.z) + currCam.transform.forward * 0.5f;
        scoreHolder.transform.position = VRMode.activeSelf ? expProps.transform.position + new Vector3(0.0f, -0.15f, 0.0f) :
                                        expProps.transform.position + currCam.transform.forward * 0.3f;
        // TODO: scoreHolder LookAt camera
    }

    void Update()
    {
        
    }
}