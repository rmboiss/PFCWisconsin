using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Random = System.Random;
using static WisconsinTrialState;

public class WisconsinExperimentController : BaseExperimentController {
    public static WisconsinExperimentController m_instance;
    public void Awake() 
    {
        if (m_instance == null) 
        {
            m_instance = this;
        }
    }

    public new WisconsinTrialState currentTrial;

    [NonSerialized] public new WisconsinTaskInfo taskInfo;
    private Animator animator;
    private int selectedPositionIndex = -1;

    private float scoreReactionTime;
    private bool isImperative = false;
    private Timer reactionTimer;
    private Timer _gateTimer;
    private float gateTimer;
    private Timer _fixationTimer;
    private List<WisconsinTrialState> proTrials = new List<WisconsinTrialState>();

    // During setup phase, this function will generate the trials 
    // that will be iterated over throughout the experiment
    public override void PrepareAllTrials() 
    {
        Populate(50); // TODO: How do we decide this?
   
        // Commands below can be optimized

        foreach (var trial in proTrials)
        {
            allTrials.Add(trial);
        }
         
        // Clear memory
        proTrials.Clear();
    }

    // Generate conditions (TODO: Look at Guillaume's combinations/permutations generator)
    private void Populate (int multiplier) 
    {
        System.Random randomGenerator = new System.Random();

        // Shape relates to TargetObject indexes (0:Cube, 1: Cross, 3: Spheres, 4: Star)
        int[] shape = { 0, 1, 2, 3 };
        int[] number = { 1, 2, 3, 4 };
        Color[] color = { Color.red, Color.blue, Color.green, Color.yellow };
        Color[] _sColor;
        int[] _sShape;
        int[] _sNumber;

        WisconsinTaskInfo myTaskInfo = WisconsinTaskInfo.m_instance;
        for (int i = 0; i < multiplier; i++) // multiplier loop
        {
            for (int _modifiers = 0; _modifiers < myTaskInfo.myModifiers.Count; _modifiers++)
            {
                for (int _conditionTypes = 0; _conditionTypes < myTaskInfo.myConditionTypes.Count; _conditionTypes++)
                {
                    for (int _responseTypes = 0; _responseTypes < myTaskInfo.myResponseTypes.Count; _responseTypes++)
                    {
                        // rule will be set randomly from 1..3 (1:Color, 2: Shape, 3: Number)
                        int rule = UnityEngine.Random.Range(1, 4);
                        int target_offset = 0;
                        for (int _rule_length = 0; _rule_length < UnityEngine.Random.Range(3, 8); _rule_length++)
                        {
                            _sColor = color.OrderBy(x => randomGenerator.Next(0, 3)).ToArray();
                            _sShape = shape.OrderBy(x => randomGenerator.Next(0, 3)).ToArray();
                            _sNumber = number.OrderBy(x => randomGenerator.Next(0, 3)).ToArray();
                            Array.Resize(ref _sColor, _sColor.Length + 1);
                            Array.Resize(ref _sShape, _sShape.Length + 1);
                            Array.Resize(ref _sNumber, _sNumber.Length + 1);

                            _sColor[_sColor.Length - 1] = color[UnityEngine.Random.Range(0, 4)];
                            // Adding extra 4 to access cue target objects which are following the 4 targets.
                            _sShape[_sShape.Length - 1] = shape[UnityEngine.Random.Range(0, 4)] + 4;
                            _sNumber[_sNumber.Length - 1] = number[UnityEngine.Random.Range(0, 4)];

                            switch (rule)
                            {
                                case 1:
                                    Color target_color = _sColor[_sColor.Length - 1];
                                    target_offset = Array.FindIndex(_sColor, tcolor => tcolor == target_color);
                                    break;
                                case 2:
                                    int target_shape = _sShape[_sShape.Length - 1] - 4;
                                    target_offset = Array.FindIndex(_sShape, tshape => tshape == target_shape);
                                    break;
                                case 3:
                                    int target_index = _sNumber[_sNumber.Length - 1];
                                    target_offset = Array.FindIndex(_sNumber, tnumber => tnumber == target_index);
                                    break;
                                default:
                                    break;
                            }
                            AssignTrialProperties(_modifiers, _conditionTypes, _responseTypes, rule, target_offset, _sColor, _sShape, _sNumber, _rule_length);
                            Array.Clear(_sColor, 0, _sColor.Length);
                            Array.Clear(_sShape, 0, _sShape.Length);
                            Array.Clear(_sNumber, 0, _sNumber.Length);
                        }
                    }
                }
            }
        }
    }

    void AssignTrialProperties(int _modifiers, int _conditionTypes, int _responseTypes, int rule, int target_offset, Color[] _color, int[] _shape, int[] _number, int ruleLength)
    {
        WisconsinTaskInfo myTaskInfo = WisconsinTaskInfo.m_instance;
        WisconsinTrialState.TargetObject[] TargetObjectList = new WisconsinTrialState.TargetObject[5];
        TargetObjectList[0] = new TargetObject(_shape[0], _color[0], _number[0]);
        TargetObjectList[1] = new TargetObject(_shape[1], _color[1], _number[1]);
        TargetObjectList[2] = new TargetObject(_shape[2], _color[2], _number[2]);
        TargetObjectList[3] = new TargetObject(_shape[3], _color[3], _number[3]);
        TargetObjectList[4] = new TargetObject(_shape[4], _color[4], _number[4]);

        var taskParams = new WisconsinTrialState()
        {
            TaskName = taskInfo.taskName,
            Modifier = myTaskInfo.myModifiers[_modifiers],
            Condition = myTaskInfo.myConditionTypes[_conditionTypes],
            Response = myTaskInfo.myResponseTypes[_responseTypes],
            TargetPositionIndex = target_offset,
            TargetObjectL = TargetObjectList,
            SelectedPositionIndex = target_offset,
            TrialRule = rule,
            TrialRuleLength = ruleLength
        };

        switch (_modifiers)
        {
            // Prosacccade Trials
            case 0 when _responseTypes == 0:
                proTrials.Add(taskParams);
                break;
            // Undesirable combinations (Debug)
            default:
                //trashedTrials.Add(taskParams);
                break;
        }
    }

    // This script prepares the trial during the intertrial phase
    public override void PrepareTrial()
    {
        currentTrial.quiet = true;
        currentTrial = allTrials[0];
        allTrials.RemoveAt(0);
        if (allTrials.Count == 0)
            PrepareAllTrials();
        currentTrial.TrialIndex = currentTrialIndex;
        currentTrialIndex++;
          
        // Update Trial Details
        TrialDetailsManager.instance.UpdateTrialDetails(currentTrial);
        
        // Set default trial outcome
        currentTrial.IsCorrect = false;
        currentTrial.Outcome = "Early response";
        currentTrial.quiet = false;
    }
    
    private new void Start()
    {
        base.Start();
        // Custom taskInfo initialization
        taskInfo = gameObject.GetComponentInChildren<WisconsinTaskInfo>();
        taskInfo.OnPublish += Publish;
        // CurrentTrial placeholder
        currentTrial = new WisconsinTrialState();
        animator = WisconsinGUIController.m_instance.stateMachine;
        // ReactionTimer
        reactionTimer = new Timer(Time.deltaTime * 100);
        reactionTimer.Elapsed += (object sender, ElapsedEventArgs e) => currentTrial.ReactionTime++;
        reactionTimer.AutoReset = true;
        reactionTimer.Enabled = false;
        // GateTimer
        _gateTimer = new Timer(10);
        _gateTimer.Elapsed += (object sender, ElapsedEventArgs e) => gateTimer += 0.01f;
        _gateTimer.AutoReset = true;
        _gateTimer.Enabled = false;
        // FixationTimer
        _fixationTimer = new Timer(10);
        _fixationTimer.Elapsed += (object sender, ElapsedEventArgs e) => fixationTimer += 0.01f;
        _fixationTimer.AutoReset = true;
        _fixationTimer.Enabled = false;
        // Reset HUD
        HUDManager.instance.GenerateScore();
    }

    private void Update()
    {
        animator.SetFloat("GateTime", gateTimer);
    }
    
    public void DisableObjects()
    {

    }

    public override void ResetObjects()
    {
        taskInfo.fixationPoint.transform.parent.gameObject.SetActive(true);
        // Reset fixation point.
        SetFixationVisibility(true);

        HideTargets();

        // Reset cues
        // Reset walls
        //foreach (GameObject wall in taskInfo.antisaccadeObjects)
        //{
        //    wall.SetActive(false);
        //}

        // Reset variables
        isImperative = false;
    } 
    
    public void SetFixationVisibility(bool isVisible)
    {
        ExperimentObject fixPoint = taskInfo.fixationPoint.GetComponent<ExperimentObject>();
        fixPoint.transform.localPosition = taskInfo.fixationOffset;
        fixPoint.IsVisible = isVisible;
        if (Camera.main != null) fixPoint.PointingTo = Camera.main.transform.position;
    }

    public override void PrepareTargets() 
    {

        for (int target_index = 0; target_index < 5; target_index++)
        {
            if (currentTrial.TargetObjectL[target_index].tindex != -1)
            {
                ExperimentObject target = taskInfo.targetObjects[currentTrial.TargetObjectL[target_index].tindex].GetComponent<ExperimentObject>();
                target.IsVisible = true;
                for (int child_index = 0; child_index < target.transform.childCount; child_index++)
                {

                    if (child_index <= currentTrial.TargetObjectL[target_index].tnumber -1)
                    {
                        target.transform.GetChild(child_index).GetComponent<Renderer>().enabled = true;
                        target.transform.GetChild(child_index).GetComponent<Renderer>().material.color = currentTrial.TargetObjectL[target_index].tcolor;
                    }
                    else
                    {
                        target.transform.GetChild(child_index).GetComponent<Renderer>().enabled = false;
                    }
                }

                target.gameObject.transform.localPosition = taskInfo.targetOffsets[target_index];

                target.IsVisible = false;
                target.PointingTo = Camera.main.transform.position;

                target.IsSkinOn = true;
            }
        }
    }

    public override void ShowCues() 
    {
        // Change fixation color to red/blue
        taskInfo.fixationPoint.GetComponent<ExperimentObject>().Color = (currentTrial.Response == BaseTaskInfo.ResponseTypes.Pro) ? Color.blue : Color.red;
        // Show target if this trial is a cued trial
        ShowTargets();    

    }
    public override void HideCues() 
    {
        // Change fixation color to default
        taskInfo.fixationPoint.GetComponent<ExperimentObject>().Color = Color.white;
        taskInfo.fixationPoint.GetComponent<ExperimentObject>().IsVisible = false;
    }

    public override void ShowTargets()
    {
        bool isCued = currentTrial.Modifier == BaseTaskInfo.Modifiers.Cued || isImperative;

        // Targets should have already been placed in their relative positions during an earlier state but made invisible.
        // Here we only need to make them visible.
        for (int target_index = 0; target_index < 5; target_index++)
        {
            if (currentTrial.TargetObjectL[target_index].tindex != -1)
            {
                ExperimentObject target = taskInfo.targetObjects[currentTrial.TargetObjectL[target_index].tindex].GetComponent<ExperimentObject>();
                target.IsVisible = true;
            }
        }

    }

    public override void HideTargets() 
    {
        for (int target_index = 0; target_index < 8; target_index++)
        {
            ExperimentObject target = taskInfo.targetObjects[target_index].GetComponent<ExperimentObject>();
            target.ResetColor();
            target.IsVisible = false;
        }
    }

    public override void ShowImperative() 
    {
        // Fixation point disappears cues the subject to initiate a saccade
        taskInfo.fixationPoint.GetComponent<ExperimentObject>().IsVisible = false;
        isImperative = true;
        ShowTargets();
        reactionTimer.Enabled = true;
    }

    public void SingleObjectSkinOff(GameObject invisiMe)
    {
        // Can either go into each child and disable the mesh or set them inactive
        // Or set the layerMask for all children to hide from HMD
        invisiMe.GetComponent<ExperimentObject>().IsSkinOn = false;
        
    }

    public override void EndFeedback() 
    {

    }

    public override int CheckResponse()
    {
        switch (currentTrial.Response)
        {
            // Prosaccade
            case BaseTaskInfo.ResponseTypes.Pro:
                return ((selectedObject == SelectedObjectClass.Wall) ? selectedTargetIndex : -1);

            default:
                Debug.Log(currentTrial.Response);
                Debug.Log("This shouldn't happen. CheckResponse()");
                return -1;
        }
    }

    public void EndResponse ()
    {
        if (currentTrial.Response != BaseTaskInfo.ResponseTypes.Hold) return;
        taskInfo.fixationPoint.GetComponent<ExperimentObject>().ResetColor();
        taskInfo.fixationPoint.GetComponent<ExperimentObject>().IsVisible = false;
    }

    public override void GiveFeedback()
    {
        currentTrial.quiet = true;
        reactionTimer.Enabled = false; // Stop Reaction Timer
        bool phaseOK = taskInfo.responseOKPhases.Contains(currentTrial.TrialPhaseIndex);
        // Find the position that was selected. This is a bit of a hack but it's necessary
        // as long as we randomize non-preferred target locations.
        if (selectedObject == SelectedObjectClass.Wall  && phaseOK)
        {
            selectedPositionIndex = selectedTargetIndex;
        }

        currentTrial.IsCorrect =
             // Prosaccade
             ((phaseOK && currentTrial.Modifier == BaseTaskInfo.Modifiers.Cued || currentTrial.Response == BaseTaskInfo.ResponseTypes.Pro)
              && selectedObject == SelectedObjectClass.Wall && selectedPositionIndex == currentTrial.TargetPositionIndex);

        var feedbackTypes = goCache.GetComponents(typeof(FeedbackModality));
        // Give all relevant feedbacks
        foreach (var component in feedbackTypes)
        {
            var feedback = (FeedbackModality) component;
            try
            {
                scoreReactionTime = currentTrial.ReactionTime;
                feedback.GiveFeedback(currentTrial.IsCorrect, scoreReactionTime);
            }
            catch (Exception ex)
            {
                Debug.Log("Progressing without particles" + ex.GetType());
            }
        }
        // Publish a string containing information about the response.
        currentTrial.Outcome = "Good trial";
        if (!phaseOK)
        {
            currentTrial.Outcome = "Too soon";
        }
        else if (((currentTrial.Response == BaseTaskInfo.ResponseTypes.Pro) &&
                 (selectedObject == SelectedObjectClass.Wall) &&
                  selectedPositionIndex != currentTrial.TargetPositionIndex))
        {
            currentTrial.Outcome = "Incorrect wall prosaccade";
        }

        // Update Response markers
        currentTrial.SelectedPositionIndex = selectedPositionIndex;
        currentTrial.SelectedObjectIndex = selectedTargetIndex;
        currentTrial.quiet = false;
    }

    private GameObject goCache;
    public override void CursorSelect(GameObject go)
    {
        // 'cursors' with colliderScript on them will call this function when they collide with a game object.
        if (go != taskInfo.fixationPoint)
        {
            _gateTimer.Enabled = false;
            _fixationTimer.Enabled = false;
            gateTimer = -Mathf.Infinity;
            fixationTimer = -Mathf.Infinity;
        }
        if (go == taskInfo.fixationPoint)
        {
            selectedObject = SelectedObjectClass.Fixation;
            if (float.IsNegativeInfinity(gateTimer) && !_gateTimer.Enabled)
            {
                gateTimer = 0.0f;
                _gateTimer.Enabled = true; // This is the start time of entry into fixationPoint.
            }

            if (float.IsNegativeInfinity(fixationTimer) && !_fixationTimer.Enabled)
            {
                fixationTimer = 0.0f;
                _fixationTimer.Enabled = true;
            }        
        }

        else if (taskInfo.targetWalls.Contains(go))
        {
            selectedObject = SelectedObjectClass.Wall;
            selectedTargetIndex = taskInfo.targetWalls.IndexOf(go);
            selectedPositionIndex = taskInfo.targetWalls.IndexOf(go);
        }

        else if (go == taskInfo.backgroundObject)
        {
            selectedObject = SelectedObjectClass.Background;
            if (!taskInfo.responseOKPhases.Contains(currentTrial.TrialPhaseIndex))
            {
                _gateTimer.Enabled = false;
                gateTimer = -Mathf.Infinity;
            }
        }
        if (go == goCache) 
        {
            return;
        }
        goCache = go;
        var cursorMarkerInfo = new CursorMarker
        {
            // trialIndex = trialState.TrialIndex,
            selectedObjectClass = selectedObject.ToString(),
            info = "Selected: " + go.name,
        };
        if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("{\"Input\": " + JsonUtility.ToJson(cursorMarkerInfo) + "}");
            return;
        }
        base.Publish("{\"Input\": " + JsonUtility.ToJson(cursorMarkerInfo) + "}");
    }

}