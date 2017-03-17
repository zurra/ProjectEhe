using System;
using Scripts;
using UnityEngine;
using UnityEngine.Networking;
 
public class NetworkManagerCustom : NetworkManager
{
    public delegate void PlayerAddedEventHandler(int connId);

    public event PlayerAddedEventHandler PlayerAdded;

    public GameObject PlayerGreen;
    public GameObject PlayerBlue;
    public GameObject PlayerRed;
    public GameObject PlayerYellow;

    public GameObject GameManagerPrefab;
    private bool _gameManagerInstantiated;

    private int playerNumber;

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

    private int _playerCount = 0;

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        //base.OnServerAddPlayer(conn, playerControllerId);

        GameObject playerPrefab;
        switch (playerNumber)
        {
            case (0):
                playerPrefab = PlayerGreen;
                break;
            case (1):
                playerPrefab = PlayerBlue;
                break;
            case (2):
                playerPrefab = PlayerRed;
                break;
            case (3):
                playerPrefab = PlayerYellow;
                break;
            default:
                playerPrefab = PlayerGreen;
                break;
        }

        Debug.Log("PlayerControllerId " + playerControllerId);
        var player = Instantiate(playerPrefab, GetStartPosition().position, transform.rotation);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        playerNumber++;
        if (!_gameManagerInstantiated)
        {
            var gameManagerPfb = Instantiate(GameManagerPrefab);

            // spawn the bullet on the clients
            NetworkServer.Spawn(gameManagerPfb);
            _gameManagerInstantiated = true;
        }

        GameObject.FindObjectOfType<GameManager>().SetPlayerIds(conn.hostId, _playerCount);

        _playerCount++;



        if (PlayerAdded != null)
            PlayerAdded(conn.hostId);
    }
}