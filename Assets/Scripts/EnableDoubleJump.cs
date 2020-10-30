using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDoubleJump : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Collision with " + other);
        if (other.tag == "Player")
        {
            Pickup(other);
        }
    }

    void Pickup(Collider2D player)
    {
        Player playerScript = player.GetComponent<Player>();
        playerScript.canDoubleJump = true;
        Destroy(gameObject);
    }
}
