using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class feedbackReferenceHolder : MonoBehaviour
{
    // Feedback children will get gameObject references from this component via RetrieveReferences()
    // This component is placed in TargetsAndCues
    public List<ParticleSystem> particleEffects;
    public List<AudioClip> audioEffects;

    // If GetComponentInParent is sufficient, then no need to Singleton
    public static feedbackReferenceHolder instance = null; 
    public void Awake() 
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    

}
