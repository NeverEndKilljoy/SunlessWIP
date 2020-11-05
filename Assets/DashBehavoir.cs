using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashBehavoir : StateMachineBehaviour
{
    private Transform playerPos;
    public float speed;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector2 target = new Vector2(playerPos.position.x, animator.transform.position.y);
        animator.transform.position = Vector2.MoveTowards(animator.transform.position, target, speed * Time.deltaTime);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

}
