using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimAudio : StateMachineBehaviour
{
    private ObjAudioManager audioManager = null;
    private PlayerControl control = null;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (audioManager == null)
            audioManager = animator.gameObject.GetComponent<ObjAudioManager>();
        if (stateInfo.IsName("Player_Die"))
            audioManager.PlayByName("Dead");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (control == null)
            control = animator.gameObject.GetComponent<PlayerControl>();
        if(stateInfo.IsName("Player_fall"))
            control.reportCollision("Jump");
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
