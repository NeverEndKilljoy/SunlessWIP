using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBossBattle : MonoBehaviour
{
    public GameObject bossObject;
    public GameObject doorLock;
    public GameObject hpBar;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Pickup(other);
        }
    }

    void Pickup(Collider2D player)
    {
        doorLock.SetActive(true);
        hpBar.SetActive(true);
        FirstBoss bossAction = bossObject.GetComponent<FirstBoss>();
        bossAction.Intro();
        Destroy(gameObject);
    }
}
