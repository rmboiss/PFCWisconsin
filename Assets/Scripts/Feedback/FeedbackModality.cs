using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class FeedbackModality : MonoBehaviour 
{
	public abstract void GiveFeedback(bool result, float time);
    public abstract void RetrieveReferences();
    protected float scoreExcellent = 475.0f;
    // public float scoreBad;
    // TODO: Implement scalable scoring system
}
