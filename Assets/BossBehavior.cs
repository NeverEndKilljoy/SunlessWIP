using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{
    public float dashSpeed = 20;
    public Transform playerPos;
    public Animator anim;

    Vector2 currentTarget;

    public Transform topLeftCorner;
    public Transform topRightCorner;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            IdleState();
        } else if (Input.GetKey(KeyCode.Alpha2))
        {
            DashState();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            print("3 was pressed");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            print("4 was pressed");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            print("5 was pressed");
        }
    }

    public void AquireTarget()
    {
        currentTarget = new Vector2(playerPos.position.x, transform.position.y);
    }

    public void IdleState()
    {
        anim.Play("Idle");
        AquireTarget();
    }

    public void DashState()
    {
        anim.Play("Dash");
        transform.position = Vector2.MoveTowards(transform.position, currentTarget, dashSpeed * Time.deltaTime);
    }

    public void MeleeState()
    {

    }

    public void UpAttackState()
    {

    }

    public void HorizontalTwister()
    {

    }
}
