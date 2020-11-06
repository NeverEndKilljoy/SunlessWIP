using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;

public class FirstBoss : MonoBehaviour
{
    public int maxHealth = 1200;
    int currentHealth;
    public float moveSpeed;

    public Animator bossAnim;
    public Slider healthBar;

    public Transform centerPoint;
    public float detectRange = 0.5f;
    public LayerMask playerLayer;
    public Transform centerAirPoint;
    public float detectAirRange = 0.5f;

    public bool playerInRange;
    public Transform playerPos;

    private Vector3 origLocalScale;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;

        playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        origLocalScale = transform.localScale; //local scale original del sprite
    }

    // Update is called once per frame
    void Update()
    {
        
        healthBar.value = currentHealth;

        //Detect player collision on ground
        Collider2D[] playerDetected = Physics2D.OverlapCircleAll(centerPoint.position, detectRange, playerLayer);
                
        /*if (playerDetected.Length > 0)
        {
            playerInRange = true;
            bossAnim.SetBool("playerInRange", true);
            bossAnim.SetTrigger("attack");
        } else
        {
            playerInRange = false;
            bossAnim.SetBool("playerInRange", false);
        }*/

        //Detect player collision on ground
        Collider2D[] playerAirDetected = Physics2D.OverlapCircleAll(centerPoint.position, detectRange, playerLayer);
        if (playerAirDetected.Length > 0 || playerDetected.Length > 0)
        {

            playerInRange = true;
            bossAnim.SetBool("playerInRange", true);
            bossAnim.SetTrigger("attack");
        }
        else
        {
            playerInRange = false;
            bossAnim.SetBool("playerInRange", false);
        }



        //Flip
        if (playerPos.position.x > transform.position.x)
        {
            if (transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(-origLocalScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
        else if (playerPos.position.x < transform.position.x)
        {
            if (transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(origLocalScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
    }

    public void Intro()
    {
        bossAnim.Play("IntroAnim");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        //Play hurt animation

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died!");

        //Die animation

        //Disable the enemy
    }

    /*public void MoveToPlayer()
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        Vector2 target = new Vector2(playerPos.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }*/

    private void OnDrawGizmosSelected()
    {
        if (centerPoint == null) return;

        Gizmos.DrawWireSphere(centerPoint.position, detectRange);
        Gizmos.DrawWireSphere(centerAirPoint.position, detectAirRange);
    } 
}
