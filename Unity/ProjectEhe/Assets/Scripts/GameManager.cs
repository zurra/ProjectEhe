using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.Networking;

namespace Scripts
{
    public class GameManager : NetworkBehaviour
    {
        public List<PlayerMovement> PlayerMovements = new List<PlayerMovement>();

        private List<int> players;
        public int AmountOfPlayers;

        //public Dictionary<int, List<Enumerations.Action>> PlayerActions = new Dictionary<int, List<Enumerations.Action>>();

        public NetworkManagerCustom NetworkManager;

        public int MaxActionAmount = 5;
        public float TurnInterval = 3.0f;

        private float turnTimeLeft;

        public bool TurnRunning;

        void Awake()
        {
            NetworkManager = FindObjectOfType<NetworkManagerCustom>();
            NetworkManager.PlayerAdded += NetworkManagerOnPlayerAdded;
            players = new List<int>();
        }

        void Update()
        {
            if (TurnRunning)
            {
                turnTimeLeft -= Time.deltaTime;
                if (turnTimeLeft < 0)
                {
                    //DoNextTurnAction();
                    ResetTimer();
                    turnTimeLeft = 0;
                }
            }
        }

        void ResetTimer()
        {
            turnTimeLeft = TurnInterval;
        }

        private void NetworkManagerOnPlayerAdded(int hostId)
        {
            //Debug.Log("HostId: " + hostId);

            //if (PlayerMovements.All(item => item.connectionToClient.hostId != hostId))
            //{
            //    var playerMovements = GameObject.FindObjectsOfType<PlayerMovement>();

            //    foreach (var playerMovement in playerMovements)
            //    {
            //        Debug.Log("PlayerMovement hostId: " + playerMovement.connectionToClient.hostId);
            //    }

            //    if (playerMovements.Any(item => item.connectionToClient.hostId == hostId))
            //    {
            //        var plrMovement = playerMovements.First(item => item.connectionToClient.hostId == hostId);
            //        PlayerMovements.Add(plrMovement);
            //        playerMovements.First(item => item.connectionToClient.hostId == hostId).Id =
            //            plrMovement.connectionToClient.hostId;
            //        //plrMovement.PlayerActionGiven += PlayerActionGiven;
            //        //PlayerActions.Add(plrMovement.Id, new List<Enumerations.Action>());

            //    }
            //}

            //if (PlayerMovements.Count == 2)
            //    StartCoroutine(WaitAndStartGame());
        }

        public void SetPlayerIds(int hostId)
        {
            if (PlayerMovements.All(item => item.connectionToClient.hostId != hostId))
            {
                var playerMovements = GameObject.FindObjectsOfType<PlayerMovement>();

                foreach (var playerMovement in playerMovements)
                {
                    Debug.Log("PlayerMovement hostId: " + playerMovement.connectionToClient.hostId);
                }

                if (playerMovements.Any(item => item.connectionToClient.hostId == hostId))
                {
                    var plrMovement = playerMovements.First(item => item.connectionToClient.hostId == hostId);
                    PlayerMovements.Add(plrMovement);
                    playerMovements.First(item => item.connectionToClient.hostId == hostId).Id =
                        plrMovement.connectionToClient.hostId;
                    //plrMovement.PlayerActionGiven += PlayerActionGiven;
                    //PlayerActions.Add(plrMovement.Id, new List<Enumerations.Action>());

                }
            }

            if (PlayerMovements.Count == 2)
                StartCoroutine(WaitAndStartGame());
        }

        IEnumerator WaitAndStartGame()
        {
            Debug.Log("Waiting and starting the game");
            yield return new  WaitForSeconds(1F);

            Debug.Log("Starting the game...");

            StartGame();
        }

        void StartGame()
        {
            PlayerMovements = FindObjectsOfType<PlayerMovement>().ToList();

            Debug.Log("Game started!");
        }

        public void StartTurnExecuting()
        {
            TurnRunning = true;
            turnTimeLeft = TurnInterval;
        }

        [Server]
        public void ServerPlayerReady(int id)
        {
            PlayerMovements = FindObjectsOfType<PlayerMovement>().ToList();

            players.Add(id);
            if (players.Count >= PlayerMovements.Count)
            {
                StartCoroutine("ResolveActions");
            }
        }

        public IEnumerator ResolveActions()
        {
            for (int j = 0; j < 5; j++)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    PlayerMovements[i].ResolveCommands(j);
                }
                yield return new WaitForSeconds(0.5f);
            }
            players.Clear();
            foreach(var player in PlayerMovements)
            {
                player.ActionList.Clear();
            }
        }
     
    }
}