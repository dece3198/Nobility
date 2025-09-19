using UnityEngine;

public class AnimationState : StateMachineBehaviour
{
    [SerializeField] private int comboStep;
    [SerializeField] private float comboTime = 1f;
    private PlayerController controller;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<PlayerController>().isAtk = false;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<PlayerController>().isAtk = true;
        if (controller == null)
            controller = animator.GetComponent<PlayerController>();
        if(comboStep <= 4)
        {
            controller.StartCombo(comboStep, comboTime);
        }
        else
        {
            controller.Invoke("ResetCombo", comboTime);
        }
    }
}
