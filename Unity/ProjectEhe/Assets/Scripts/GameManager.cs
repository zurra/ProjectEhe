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
        public Dictionary<int, List<Enumerations.Action>> Actions = new Dictionary<int, List<Enumerations.Action>>();

        private List<int> players;
        public int AmountOfPlayers;

        public NetworkManagerCustom NetworkManager;

        public int MaxActionAmount = 5;
        public float TurnInterval = 3.0f;

        private float turnTimeLeft;

        public bool TurnRunning;

        void Awake()
        {
            NetworkManager = FindObjectOfType<NetworkManagerCustom>();
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

            foreach (var playerMovement in PlayerMovements)
            {
                if (playerMovement.gameObject.transform.position.y < -20)
                {
                    ServerKillPlayer(playerMovement.Id);
                }
            }
        }

        void ResetTimer()
        {
            turnTimeLeft = TurnInterval;
        }

        public void SetPlayerIds(int Id)
        {
            PlayerMovements = FindObjectsOfType<PlayerMovement>().ToList();

            if (PlayerMovements.Any(item => item.Id == -1))
            {
                var plrMovement = PlayerMovements.First(item => item.Id == -1);

                PlayerMovements.Add(plrMovement);
                PlayerMovements.Last().Id = Id;
                PlayerMovements.Last().RpcDisplayPlayer(Id);

                //plrMovement.PlayerActionGiven += PlayerActionGiven;
                //PlayerActions.Add(plrMovement.Id, new List<Enumerations.Action>());

                Actions.Add(plrMovement.Id, new List<Enumerations.Action>());
            }
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
                    PlayerMovements[i].RpcResolveCommands(Actions[PlayerMovements[i].Id][j], j);
                    PlayerMovements[i].RpcAnimate(Actions[PlayerMovements[i].Id][j]);
                }
                yield return new WaitForSeconds(1.5f);
            }
            players.Clear();
            foreach(var player in PlayerMovements)
            {
                Actions[player.Id].Clear();
                player.RpcEmptyDisplayCommands();
                player.RpcResetActiveCommands();

            }
        }

        [Server]
        public void ServerAddActionToList(PlayerMovement playerMovement, Enumerations.Action action)
        {
            if (Actions[playerMovement.Id].Count < 4)
            {
                Actions[playerMovement.Id].Add(action);
                playerMovement.RpcDisplayCommand(action);
            }
            else if (Actions[playerMovement.Id].Count < 5)
            {
                Actions[playerMovement.Id].Add(action);

                ServerPlayerReady(playerMovement.Id);
                playerMovement.RpcDisplayCommand(action);
            }
        }

        [Server]
        public void ServerKillPlayer(int id)
        {
            PlayerMovements.First(item => item.Id == id).GetComponent<PlayerState>().KillPlayer(id);
        }


    }
}