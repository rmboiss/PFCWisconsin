using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;

public class FeedbackParticle : FeedbackModality
{

	public ParticleSystem particleExcellent;
	public ParticleSystem particleWrong;
	
	public override void GiveFeedback(bool result, float time)
	{
		try 
		{
			//Vector3 objPosition = this.gameObject.transform.position;
            Vector3 center = new Vector3(0.0f, 1.0f, 1.1f);
			if (!result) 
			{
                particleWrong.transform.position = center;
				particleWrong.Play();
			}
			// TODO: Animate 300/100/50 grades
			if (time < scoreExcellent && result)
			{
                // Excellent! (Green)
                particleExcellent.transform.position = center;// objPosition;
				particleExcellent.Play();
			}
		}
		catch 
		{
			Debug.Log("The gameObject '" + gameObject.name + "' triggered null reference");
		}
		
	}

	public override void RetrieveReferences() 
	{
		List<ParticleSystem> particleReference = new List<ParticleSystem>();
		try
		{
			particleReference.AddRange(gameObject.GetComponentInParent<feedbackReferenceHolder>().particleEffects);
		}
		catch
		{
			particleReference.AddRange(GameObject.Find("Targets").GetComponent<feedbackReferenceHolder>().particleEffects);
		}	
		finally 
		{
			particleExcellent = particleReference[0];
			particleWrong = particleReference[3];
			particleReference.Clear();
		}
			
		
	}
	void Awake() 
	{
		try 
		{
			RetrieveReferences();
		}
		catch (Exception ex) 
		{
			Debug.Log("This gameObject, " + this.gameObject.name + " has no parent with the component reference holder" + ex.GetType() +
						"Attempting to correct");
		}

	}
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
