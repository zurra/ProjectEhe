using System;
using UnityEngine;
using UnityEngine.Networking;
 
public class NetworkManagerCustom : NetworkManager
{
    public delegate void PlayerAddedEventHandler(int connId);

    public event PlayerAddedEventHandler PlayerAdded;

    //When a new client connect to the Host server.
    public override void OnClientConnect(NetworkConnection Conn)
    {
        base.OnClientConnect(Conn);

        Debug.Log("Client has connected " + Conn.hostId);
    }

    //When a new client connect to the Host server.
    public override void OnServerConnect(NetworkConnection Conn){

        base.OnServerConnect(Conn);

        Debug.Log("New Player has joined" + Conn.hostId);

        //if(PlayerAdded != null)
        //    PlayerAdded(Conn.hostId);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);

        Debug.Log("PlayerControllerId " + playerControllerId);

        if (PlayerAdded != null)
            PlayerAdded(conn.hostId);
    }
}