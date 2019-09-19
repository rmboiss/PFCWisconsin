using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;

public class WisconsinGUIController : BaseGUIController {
    public static WisconsinGUIController m_instance = null;
    public void Awake() 
    {
        if (m_instance == null) 
        {
            m_instance = this;
        }
    }

    [Header("Task References")]
    public Animator stateMachine;
    // public Dropdown sceneSelector;
    public enum TaskTypes { SelectTask, CuedSaccadeNoGo, PrefrontalMapping, WisconsinSorting }
    public UserInputController inputController;

    [Header("GUI References")]
    public Button moreOptionsRight;
    public GameObject panelTaskParams;
    public Button moreOptionsLeft;
    public GameObject panelMoreOptions;
    public Toggle fixationVisibility;
    public Button recordEyes;
    public Button calibrationButton;
    public Button resetCamera;
    public Button beginTrial;
    private static readonly int s_ResetTrigger = Animator.StringToHash("ResetTrigger");
    private static readonly int s_StageIndex = Animator.StringToHash("StageIndex");
    private static readonly int s_ManualAdvance = Animator.StringToHash("ManualAdvance");


    void Start() 
    {
        PopulateExperimentSelection();
        experimentSelector.onValueChanged.AddListener((x) => { stateMachine.SetInteger(s_StageIndex, x); }); // Find a better way: Loading screen?

        base.PopulateInputMethods();
        inputSelector.onValueChanged.AddListener(InputSelectorHandler);
        
        moreOptionsLeft.GetComponent<Button>().onClick.AddListener(OnClickInputConfig);
        moreOptionsRight.GetComponent<Button>().onClick.AddListener(OnClickTaskParams);
        beginTrial.GetComponent<Button>().onClick.AddListener(OnClickBegin);
        resetCamera.GetComponent<Button>().onClick.AddListener(WisconsinExperimentController.m_instance.ResetCamera);
        fixationVisibility.GetComponent<Toggle>().onValueChanged.
            AddListener(delegate(bool x)
            {
                GameObject.Find("Sphere(Clone)").layer = x ? LayerMask.NameToLayer("Default") : LayerMask.NameToLayer("NoHMD"); });
    }

    public override void PopulateExperimentSelection() {
        experimentSelector.AddOptions(GetEnumSpacedList<TaskTypes>());
    }

    private void OnClickInputConfig()
    {
        Debug.Log("Start OnClickInputConfig startedg.");
        Button inputConfigButton = moreOptionsLeft.GetComponent<Button>();
        Text buttonText = inputConfigButton.GetComponentInChildren<Text>();
        if (buttonText.text == "<")
        {
            inputController.ConfigMenu();
            panelMoreOptions.SetActive(true);
            buttonText.text = ">";
        }
        else
        {
            inputController.DestroyMenu();
            panelMoreOptions.SetActive(false);
            buttonText.text = "<";
        }
    }
    private void OnClickTaskParams() 
    {
        Button inputConfigButton = moreOptionsRight.GetComponent<Button>();
        Text buttonText = inputConfigButton.GetComponentInChildren<Text>();
        if (buttonText.text == ">")
        {
            panelTaskParams.SetActive(true);
            buttonText.text = "<";
        }
        else
        {
            panelTaskParams.SetActive(false);
            buttonText.text = ">";
        }
    }

    void OnSetVisualAngle(string visualAngle) // On-demand/Real-time update target positions
    {
        float newVisualAngle = float.Parse(visualAngle);
        // TODO: Convert newVisualAngle to actual visualAngle in degrees, based on distance from camera to targets.
        //taskInfo.targetOffsets[0].Set(taskInfo.targetOffsets[0].x, taskInfo.targetOffsets[0].y, newVisualAngle); // Left
        //taskInfo.targetOffsets[1].Set(taskInfo.targetOffsets[1].x, taskInfo.targetOffsets[1].y, -newVisualAngle); // Right
        //PrepareTargets();
    }
    public override void OnClickBegin()
    {
        GetComponent<Animator>().SetTrigger(s_ManualAdvance);  // Start the state machine.
    }

    //public override void OnHitBoxUpdated(string newHitBoxValue)
    //{
    //    MExperimentController.m_instance.taskInfo.HitBox = string.IsNullOrEmpty(newHitBoxValue) ? 0f : float.Parse(newHitBoxValue);
    //}

    public void InputSelectorHandler(int selection) 
    {
        // Shut down and inactivate current controller.
        inputController.gameObject.SetActive(false); // OnDisable will be called.
        
        if (inputController is PupilRayController)
        {
            // PupilRayController prc = (PupilRayController)inputController;
            PupilCalibration prc = (PupilCalibration)inputController;
            prc.OnCalibrationStarted -= WisconsinExperimentController.m_instance.HideTargets;
            prc.OnCalibrationSucceeded -= WisconsinExperimentController.m_instance.ShowTargets;
        }
        

        UserInputController.EDevice inputDevice = (UserInputController.EDevice)selection;
        // CameraCheck(inputDevice); // Information about VR or NoVR
        switch (inputDevice)
        {
            case UserInputController.EDevice.Pupil:
                
                inputController = Resources.FindObjectsOfTypeAll<PupilCalibration>()[0];
                PupilCalibration prc = (PupilCalibration)inputController;
                prc.OnCalibrationStarted -= WisconsinExperimentController.m_instance.HideTargets;
                prc.OnCalibrationSucceeded -= WisconsinExperimentController.m_instance.ShowTargets;
                
                break;

            case UserInputController.EDevice.HMD:
                inputController = Resources.FindObjectsOfTypeAll<HMDRayController>()[0];
                break;

            case UserInputController.EDevice.MotionController:
                inputController = Resources.FindObjectsOfTypeAll<ViveInputController>()[0];
                break;

            case UserInputController.EDevice.Keyboard:
                inputController = Resources.FindObjectsOfTypeAll<KeyboardController>()[0];
                break;

            case UserInputController.EDevice.Mouse:
                inputController = Resources.FindObjectsOfTypeAll<MouseController>()[0];
                break;

            case UserInputController.EDevice.Null:
                inputController = Resources.FindObjectsOfTypeAll<NullInputController>()[0];
                break;

            default:
                Debug.Log("Something went wrong.");
                break;
        }
        inputController.gameObject.SetActive(true);  // OnEnable will be called.
    }
    
}