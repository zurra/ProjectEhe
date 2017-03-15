using System;
using Scripts;
using UnityEngine;
using UnityEngine.Networking;
 
public class NetworkManagerCustom : NetworkManager
{
    public delegate void PlayerAddedEventHandler(int connId);

    public event PlayerAddedEventHandler PlayerAdded;

    public GameObject GameManagerPrefab;
    private bool _gameManagerInstantiated;

    //When a new client connect to the Host server.
    public override void OnClientConnect(NetworkConnection Conn)
    {
        base.OnClientConnect(Conn);

        Debug.Log("Client has connected " + Conn.hostId);
    }

    //When a new client connect to the Host server.
    public override void OnServerConnect(NetworkConnection Conn){

        base.OnServerConnect(Conn);

        if (!_gameManagerInstantiated)
        {
            var gameManagerPfb = Instantiate(GameManagerPrefab);

            // spawn the bullet on the clients
            NetworkServer.Spawn(gameManagerPfb);
            _gameManagerInstantiated = true;
        }

        Debug.Log("New Player has joined" + Conn.hostId);

        //if(PlayerAdded != null)
        //    PlayerAdded(Conn.hostId);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);

        Debug.Log("PlayerControllerId " + playerControllerId);

        if (!_gameManagerInstantiated)
        {
            var gameManagerPfb = Instantiate(GameManagerPrefab);

            // spawn the bullet on the clients
            NetworkServer.Spawn(gameManagerPfb);
            _gameManagerInstantiated = true;
        }

        GameObject.FindObjectOfType<GameManager>().SetPlayerIds(conn.hostId);

        if (PlayerAdded != null)
            PlayerAdded(conn.hostId);
    }
}