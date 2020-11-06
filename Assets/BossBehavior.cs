using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{
    public float dashSpeed = 20;
    public Player playerReference;
    public Transform topLeftCorner;
    public Transform topRightCorner;

    // Start is called before the first frame update
    void Start()
    {
        Transform playerPos = playerReference.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            print("space key was pressed");
        } else if (Input.GetKeyDown(KeyCode.Alpha2))
        {

        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {

        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {

        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {

        }
    }

    public void IdleState()
    {

    }

    public void DashState()
    {

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
