using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPhase : StateMachineBehaviour {

    // This phase presents the (already prepared) targets.
    // Note that, unlike most other phases, here we do not clean up our stimuli during phase exit because we want them to persist.
	private float startTime = 0.0f;
	private float variableDelayTimer = 0.0f;

	private WisconsinTrialState currentTrial;
	
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        WisconsinExperimentController.m_instance.ShowTargets();
		startTime = Time.time;
		currentTrial = WisconsinExperimentController.m_instance.currentTrial;
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	/*override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		// Start timer for VariableDelayTimer
		if (variableDelayTimer > 
		    currentTrial.VariableGoDelay && 
		    variableDelayTimer != Mathf.Infinity) 
		{
			animator.SetTrigger("ManualAdvance");
			MExperimentController.m_instance.ShowImperative();
		}
		else 
		{
			variableDelayTimer = Time.time;
		}
	}*/

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		variableDelayTimer = Mathf.Infinity;
	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
