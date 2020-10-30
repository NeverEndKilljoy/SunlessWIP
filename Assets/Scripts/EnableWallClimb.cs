using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableWallClimb : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Pickup(other);
        }
    }

    void Pickup(Collider2D player)
    {
        Player playerScript = player.GetComponent<Player>();
        playerScript.canWallSlide = true;
        Destroy(gameObject);
    }
}
