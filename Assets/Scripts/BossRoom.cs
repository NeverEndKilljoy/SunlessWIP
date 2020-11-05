using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom : MonoBehaviour
{
    public GameObject virtualCam;
    public GameObject playerCam;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !other.isTrigger)
        {
            playerCam.SetActive(false);
            virtualCam.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            playerCam.SetActive(true);
            virtualCam.SetActive(false);
        }
    }
}
