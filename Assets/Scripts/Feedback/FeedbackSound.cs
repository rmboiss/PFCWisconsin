using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;

public class FeedbackSound : FeedbackModality {

	public AudioClip soundExcellent;
	public AudioClip soundWrong;

	void Awake() 
	{
		try 
		{
			RetrieveReferences();
		}
		catch (Exception ex) 
		{
			Debug.Log("This object has no parent with the component reference holder" + ex.GetType() +
						"Attempting to correct");
		}
	}

	public override void GiveFeedback(bool result, float time)
	{
		if (!result) 
		{
			gameObject.GetComponent<AudioSource>().PlayOneShot(soundWrong);
		}
			// TODO: Replace with different sounds
		if (time < scoreExcellent && result)
		{    
				// Excellent! (Green)
				gameObject.GetComponent<AudioSource>().PlayOneShot(soundExcellent);
		}
	}

	public override void RetrieveReferences() 
	{
		
		// TODO: Play directly from the reference holder?
		List<AudioClip> audioReference = new List<AudioClip>();
		try 
		{
			foreach (AudioClip item in gameObject.GetComponentInParent<feedbackReferenceHolder>().audioEffects)
			{
				audioReference.Add(item);
			}
		}
		catch
		{
			foreach (AudioClip item in GameObject.Find("TargetsAndCues").GetComponent<feedbackReferenceHolder>().audioEffects)
			{
				audioReference.Add(item);
			}
		}
		soundExcellent = audioReference[0];
		soundWrong = audioReference[3];
	}

	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
