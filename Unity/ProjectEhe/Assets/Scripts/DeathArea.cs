using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Scripts;
using UnityEngine;
using UnityEngine.Networking;

public class DeathArea : NetworkBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("ON TRIGGER ENTER!");

        GameObject.FindObjectOfType<GameManager>().ServerKillPlayer(other.gameObject.GetComponent<PlayerMovement>().Id);
    }
}
