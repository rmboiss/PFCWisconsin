using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class postTargetDelay : StateMachineBehaviour {
    // Script placed in Delay2, and the stop signal delay (SSD) is currently controlled 
    // by the transition's exit time.
    // TODO: Make step-wise system to adjust SSD difficulty

    private float startTime = 0.0f;
    private float SSDTimer;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        startTime = Time.time;
        animator.SetTrigger("ProsaccadeOK");

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    /*override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Start timer for SSDTimer
        if (SSDTimer > ((MTrialState)MExperimentController.m_instance.currentTrial).StopSignalDelay && !float.IsPositiveInfinity(SSDTimer)) 
        {
            animator.SetTrigger("CountermandingOK");
        }
        else 
        {
            SSDTimer = Time.time - startTime;
        }

    }*/

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    /*override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat("SSDTimer", Mathf.Infinity);
    }*/

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
