using System.Collections;
using System.Collections.Generic;
// using Battlehub.RTCommon;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class FeedbackScore : FeedbackModality
{
    public override void GiveFeedback(bool result, float time)
    {
        HUDManager HUD = HUDManager.instance;
        if (!result) 
        {
            HUD.ProcessScore(HUDManager.scoreGrades.Null);
        }
        
        if (time < scoreExcellent && result)
        {    
            // Excellent! (Green)
            HUD.ProcessScore(HUDManager.scoreGrades.Excellent);
        }
    }

	public override void RetrieveReferences() 
	{
		
	}

}
