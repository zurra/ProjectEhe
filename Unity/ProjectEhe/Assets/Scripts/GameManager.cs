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

        public struct PlayerAction
        {
            public int Id;
            public Enumerations.Action Action;
        }

        public class PlayerActions : SyncListStruct<PlayerAction> {}
        PlayerActions m_actions = new PlayerActions();

        //public Dictionary<int, List<Enumerations.Action>> PlayerActions = new Dictionary<int, List<Enumerations.Action>>();

        public NetworkManagerCustom NetworkManager;

        public int MaxActionAmount = 5;
        public float TurnInterval = 3.0f;

        private float turnTimeLeft;

        public bool TurnRunning;

        void Awake()
        {
           NetworkManager.PlayerAdded += NetworkManagerOnPlayerAdded;
            m_actions.Callback += OnPlayerActionChanged;
            players = new List<int>();
        }

        private void OnPlayerActionChanged(SyncList<PlayerAction>.Operation op, int itemIndex)
        {
            Debug.Log("Player action changed: " + op);
        }

        void Update()
        {
            if (TurnRunning)
            {
                turnTimeLeft -= Time.deltaTime;
                if (turnTimeLeft < 0)
                {
                    DoNextTurnAction();
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
            Debug.Log("HostId: " + hostId);

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
            yield return new  WaitForSeconds(3F);

            Debug.Log("Starting the game...");

            StartGame();
        }

        public void PlayerActionGiven(int hostId, Enumerations.Action action)
        {
            Debug.Log("Player action given : " + hostId + " " + action);

            //foreach (var thing in PlayerActions)
            //{
            //    Debug.Log(thing.Key);
            //}

            //if (PlayerActions[hostId].Count < MaxActionAmount)
            //{
            //    PlayerActions[hostId].Add(action);
            //}

            //if (PlayerActions.All(item => item.Value.Count == 5))
            //{
            //    StartTurnExecuting();
            //}

            foreach (var thing in m_actions)
            {
                Debug.Log(thing.Id + " " + thing.Action);
            }

            if (m_actions.Count(item => item.Id == hostId) < MaxActionAmount)
            {
                m_actions.Add(new PlayerAction() { Action = action, Id = hostId});
            }

            //if (PlayerActions.All(item => item.Value.Count == 5))
            //{
            //    StartTurnExecuting();
            //}
        }

        void StartGame()
        {
            foreach (var playerMovement in PlayerMovements)
            {
                playerMovement.AskForInput();
            }
        }

        public void StartTurnExecuting()
        {
            TurnRunning = true;
            turnTimeLeft = TurnInterval;
        }

        private void DoNextTurnAction()
        {
            //foreach (var hostId in PlayerActions.Keys)
            //{
            //    if (PlayerActions[hostId].Any())
            //    {
            //        PlayerMovements.First(item => item.Id == hostId).DoAction(PlayerActions[hostId].First());
            //        PlayerActions[hostId].RemoveAt(0);
            //    }
            //}
        }

        [Command]
        public void CmdPlayerReady(int id)
        {
            players.Add(id);
            if (players.Count >= PlayerMovements.Count)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    PlayerMovements[i].ResolveCommands();
                }
            }
        }
    }
}