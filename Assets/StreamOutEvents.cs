using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;

public class StreamOutEvents : MonoBehaviour {

    public string StreamName = "PFCSaccadeTaskEvents";
    public string StreamType = "Markers";
    public string UniqueID = "uid98765";
    
    private liblsl.StreamOutlet outlet;

    // Use this for initialization
    void Start () {
        if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("You're running " + SystemInfo.operatingSystem + 
                ". Aborting StreamOutEvents.cs");
            Debug.Log("Remember to check MExperimentController.ResetCamera script if attempting VR");
            return; // LSL crashes OSX
        }
        // 1 - Create the stream descriptor.
        int channel_count = 1;
        liblsl.StreamInfo streamInfo = new liblsl.StreamInfo(StreamName, StreamType, channel_count, liblsl.IRREGULAR_RATE, liblsl.channel_format_t.cf_string, UniqueID);

        // TODO 2 - Fill in the stream header using info obtained from MExperimentController.
        liblsl.XMLElement streamInfoXML = streamInfo.desc();
        /* 
        foreach (GameObject target in MExperimentController.instance.taskInfo.animalHolder)
        {
            obj_map.Add(MExperimentController.instance.taskInfo.animalHolder.IndexOf(target), target.name);
        }
        */
        List<IDictionary<int, string>> metadata_dicts = new List<IDictionary<int, string>>
        {
            phase_map, task_type_map, countermand_map, cuedPositionIndex, targetPositionIndex, obj_map
        };
        // Insert names of lists ..
        IDictionary<string, IDictionary<int, string>> metadata_dicts_names = new Dictionary<string, IDictionary<int, string>> 
        {
            { "phase_map", phase_map },
            { "task_type_map", task_type_map }, 
            { "countermand_map", countermand_map }, 
            { "cuedPositionIndex", cuedPositionIndex },
            { "targetPositionIndex", targetPositionIndex },
            { "obj_map", obj_map }
        };

        foreach (var map_name in metadata_dicts_names)
        {
            liblsl.XMLElement map_el = streamInfo.desc().append_child(map_name.Key);
            foreach (var property in map_name.Value)
            {
                map_el = map_el.append_child_value(property.Key.ToString(), property.Value);
                // Debug.Log(property.Key.ToString() + property.Value);
                // System.Console.WriteLine(property.Key.ToString() + property.Value);
            }
        };

        // 3 - Create the stream and push the first event.
        outlet = new liblsl.StreamOutlet(streamInfo);
        // string[] events_array = { "Begin event stream." };
        // outlet.push_sample(events_array);

        // 4 - Register as a listener for MExperimentController publish events.
        GetComponent<WisconsinExperimentController>().OnPublish += OnPublish;
        
        // Test desc XML header: resolve the stream and open an inlet
        liblsl.StreamInfo[] results = liblsl.resolve_stream("name", StreamName);
        liblsl.StreamInlet inlet = new liblsl.StreamInlet(results[0]);
        liblsl.StreamInfo inf = inlet.info();
        Debug.Log("The stream's XML meta-data is: ");
        Debug.Log(inf.as_xml());
    }
    IDictionary<int, string> obj_map = new Dictionary<int, string>();
    IDictionary<int, string> phase_map = new Dictionary<int, string>
    {
        { 1, "Intertrial" },
        { 2, "Fixate" }, 
        { 3, "Cue" }, 
        { 4, "Delay" }, 
        { 5, "Target" }, 
        { 6, "Go" },
        { 7, "Countermand" },
        { 8, "Response" },
        { 9, "Feedback" },
        { -1, "UserInput" }
    };
    IDictionary<int, string> task_type_map = new Dictionary<int, string>
    {
        { 0, "AttendShape" },
        { 1, "AttendColour" },
        { 2, "AttendDirection" }
    };
    IDictionary<int, string> countermand_map = new Dictionary<int, string>
    {
        { 0, "Prosaccade" },
        { 1, "TargetSwitch" },
        { 2, "NoGo" },
        { 3, "Antisaccade" }
    };
    IDictionary<int, string> cuedPositionIndex = new Dictionary<int, string>
    {
        { 0, "Left" },
        { 1, "Right" }
    };
    IDictionary<int, string> targetPositionIndex = new Dictionary<int, string>
    {
        { 0, "Left" },
        { 1, "Right" }
    };
    void OnPublish(string pubstring)
    {
        string[] events_array = { pubstring };
        outlet.push_sample(events_array);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
