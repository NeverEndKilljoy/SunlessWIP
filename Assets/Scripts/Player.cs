using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Unity.Profiling;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(RaycastsController))]
public class Player : MonoBehaviour
{
#region Variables funcionando 
    public float jumpHeight = 4;
    public float timeToJumpApex = .4f;
    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = .1f;
    public float moveSpeed = 6;
    public float smallJumpFactor = 0.5f;
    public float hangTime = 0.2f;
    public float jumpBufferLength = 0.1f;

    public bool canDoubleJump = false;
    public bool canWallSlide = false;

    private float _hangCounter;
    private float _jumpBufferCount;

    public float dashSpeed = 30;
    public float dashDirection;

    public float wallSlideSpeedMax = 3;
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    float gravity;
    float jumpVelocity;
    Vector3 velocity;
    Vector3 attackDashVelocity;
    float velocityXSmoothing;
    private Vector3 origLocalScale;

    RaycastsController controller;
    public Animator anim;
    #endregion
    //Attack Variables
    public int attackDamage = 20;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public bool _isAttacking = false;
    public bool canAttack = true;
    public bool canJump = true;
   
    public int attackCounter = 0;
    public bool canDash = true;
    public bool isDashing = false;
    float lastAttackTime = 0f;
    public float maxComboDelay = 0.9f;
    public float attackDashDistance = 1f;
    //float lastDashTime = 0f;
    //public int dashCounter = 0;
    //public float maxDashDelay = 0.4f;

    private void Awake()
    {
        controller = GetComponent<RaycastsController>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        controller.ChangeCollisions(0); 
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        print("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity);

        origLocalScale = transform.localScale; //local scale original del sprite
    }

    void Update()
    {
        //Vector input
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        int wallDirX = (controller.collisions.left) ? -1 : 1;

        bool wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0 && canWallSlide)
        {
            wallSliding = true;
            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }
        }

        #region Movimiento
        //Revisa si hay colisión sobre o debajo del jugador
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        //CoyoteTime manager
        if (controller.collisions.below)
        {
            _hangCounter = hangTime;
            if (_isAttacking)
            {
                canJump = false;
            }
            else
            {
                canJump = true;
            }
        }
        else
        {
            _hangCounter -= Time.deltaTime;
        }

        //JumpBuffer manager
        if (Input.GetButtonDown("Jump") && wallSliding)
        {
            if (wallDirX == input.x)
            {
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
            }
            else if (input.x == 0)
            {
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;
            }
            else
            {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;
            }
        }

        if (Input.GetButtonDown("Jump"))
        {
            _jumpBufferCount = jumpBufferLength;
        } else {
            _jumpBufferCount -= Time.deltaTime;
        }
        

        //Salto
        if (_jumpBufferCount > 0f && _hangCounter > 0f && canJump)
        {
            velocity.y = jumpVelocity;
            _jumpBufferCount = 0;
        }
        //Doble salto
        else if (velocity.y !=0 && Input.GetButtonDown("Jump") && canJump && !wallSliding && canDoubleJump)
        {
            velocity.y = jumpVelocity;
            _jumpBufferCount = 0;
            canJump = false;
        }
        //Salto corto
        if (Input.GetButtonUp("Jump") && velocity.y > 0)
        {
            velocity.y = velocity.y * smallJumpFactor;
        }

        //Movement
        float targetVelocityX = input.x * moveSpeed;
        //No moverse si está atacando:
        if (_isAttacking)
        {
            //canJump = false;
            velocity.x = 0;
            velocity.y += (gravity * 10 / 100) * Time.deltaTime;            
            //velocity.x = 5 * Input.GetAxisRaw("Horizontal");
        } else if (isDashing)
        {
            velocity.x = dashDirection * dashSpeed;
            velocity.y = 0;
        } else
        {
            canDash = true;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
            velocity.y += gravity * Time.deltaTime;
        }
        
        controller.Move(velocity * Time.deltaTime);

        //Animations
        //Flip
        if (velocity.x > 0.1f)
        {
            if (transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(origLocalScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
        else if (velocity.x < -0.1f)
        {
            if (transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-origLocalScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
        //Animaciones movimiento
        if (isDashing == false)
        {
            if (_isAttacking == false)
            {
                if (controller.collisions.below)
                {
                    if (Mathf.Abs(velocity.x) > 0.1f)
                    {
                        anim.Play("MoRun");
                    }
                    else
                    {
                        anim.Play("MoIdle");
                    }
                }
                else
                {
                    if (velocity.y > 0.1f)
                    {
                        anim.Play("MoUp");
                    }
                    else
                    {
                        anim.Play("MoDown");
                    }
                }
            }
        }
        
        //Debug.Log("Horizontal: " + velocity.x + " || Vertical: " + velocity.y + " || IsGrounded: " + controller.collisions.below);
        //Debug.Log("Horizontal input: " + input.x + " || Vertical input: " + input.y);
        //Debug.Log("JumpBuffer: " + _jumpBufferCount + "CoyoteCounter: " + _hangCounter);
        #endregion


        //Revisa la ventana del delay entre presionar ataques
        if (Time.time - lastAttackTime > maxComboDelay)
        {
            attackCounter = 0;
        }
        //Attack
        if (Input.GetButtonDown("Melee") && canAttack)
        {
            lastAttackTime = Time.time;
            attackCounter++;
            _isAttacking = true;
            canJump = false;
            canDash = false;
            velocity.y = velocity.y * smallJumpFactor;

            if(attackCounter == 1)
            {
                //Play animation
                anim.Play("MoMelee1");
                hurtEnemy();
            }

            attackCounter = Mathf.Clamp(attackCounter, 0, 4);
        }

        //Dash
        if (Input.GetButtonDown("Dash") && canDash)
        {
            if(input.x == 0)
            {
                dashDirection = transform.localScale.x;
                canAttack = false;
                canJump = false;
                anim.Play("MoDash");
            } else if (input.x < 0.1f)
            {
                dashDirection = -1;
                canAttack = false;
                canJump = false;
                anim.Play("MoDash");
            } else
            {
                dashDirection = 1;
                canAttack = false;
                canJump = false;
                anim.Play("MoDash");
            }
        }
        /*if (Input.GetButtonDown("LeftDash") && canDash)
        {
            dashDirection = -1;
            canAttack = false;
            canJump = false;
            anim.Play("MoDash");
            
        } else if (Input.GetButtonDown("RightDash") && canDash) {
            dashDirection = 1;
            canAttack = false;
            canJump = false;
            anim.Play("MoDash");
        }*/


    }

    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "EnableDoubleJump")
        {
            Debug.Log("Double jump!");
            //Destroy(collision.gameObject);
            //canDoubleJump = true;
        }
    }*/

    void LateUpdate()
    {


    }

    public void lastFrame1()
    {
        //Vector input del jugador
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        //Flip
        if (input.x > 0.1f)
        {
            if (transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(origLocalScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
        else if (input.x < -0.1f)
        {
            if (transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-origLocalScale.x, transform.localScale.y, transform.localScale.z);
            }
        }


        if (attackCounter >= 2)
        {
            //Play animation
            anim.Play("MoMelee2");
            hurtEnemy();
        } else
        {
            _isAttacking = false;
            canDash = true;
            //canJump = true;
            attackCounter = 0;
        }
    }

    public void lastFrame2()
    {
        //Vector input del jugador
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        //Flip
        if (input.x > 0.1f)
        {
            if (transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(origLocalScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
        else if (input.x < -0.1f)
        {
            if (transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-origLocalScale.x, transform.localScale.y, transform.localScale.z);
            }
        }

        if (attackCounter >= 3)
        {
            //Play animation
            anim.Play("MoMelee3");
            hurtEnemy();
        } else
        {
            _isAttacking = false;
            canDash = true;
            //canJump = true;
            attackCounter = 0;
        }
    }

    public void lastFrame3()
    {
        //Vector input del jugador
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        //Flip
        if (input.x > 0.1f)
        {
            if (transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(origLocalScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
        else if (input.x < -0.1f)
        {
            if (transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-origLocalScale.x, transform.localScale.y, transform.localScale.z);
            }
        }

        _isAttacking = false;
        canDash = true;
        //canJump = true;
        attackCounter = 0;
        /*if (!isGrounded)
        {
            canAttack = false;
        }*/
    }

    public void attackDash()
    {
        float direction = transform.localScale.x;

        
        switch (direction)
        {
            case 1:
                attackDashVelocity.x = 50;
                attackDashVelocity.y = velocity.y * -1;
                controller.Move(attackDashVelocity * Time.deltaTime);
                break;
            case -1:
                attackDashVelocity.x = -50;
                attackDashVelocity.y = velocity.y * -1;
                controller.Move(attackDashVelocity * Time.deltaTime);
                break;
        }
    }

    public void startDash()
    {
        controller.ChangeCollisions(1);
        isDashing = true;
        canDash = false;
        canAttack = false;
    }
    public void endDash()
    {
        controller.ChangeCollisions(0);
        dashDirection = 0;
        isDashing = false;
        canAttack = true;
        velocity.y = 0;
        velocity.x = 0;
    }

    public void hurtEnemy()
    {
        //Detect enemy collision
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        //Damage enemies
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

}
