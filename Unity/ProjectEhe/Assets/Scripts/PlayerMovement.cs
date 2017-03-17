using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Rewired;
using System.Collections;
using Scripts;

namespace Assets.Scripts
{
    public class PlayerMovement : NetworkBehaviour
    {
        public GameObject LaserPrefab;
        public GameObject LaserBase;
        //public GameObject PlayerGreen;
        //public GameObject PlayerYellow;
        //public GameObject PlayerBlue;
        //public GameObject PlayerRed;
        public Player player;
        public PlayerState PlayerState;
        public List<Enumerations.Action> ActionList;
        public int Id;
        public int rewiredPlayerId = 0;

        //[SyncVar(hook = "OnInputAllowedChanged")]
        //public bool InputAllowed = true;

        //public delegate void PlayerActionEventHandler(int hostId, Enumerations.Action action);
        //public PlayerActionEventHandler PlayerActionGiven;

        private GameManager _gameManager;
        private Animator _animator;

        void Awake()
        {
            // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
            ActionList = new List<Enumerations.Action>();
            player = ReInput.players.GetPlayer(rewiredPlayerId);
            _gameManager = FindObjectOfType<GameManager>();
            PlayerState = GetComponent<PlayerState>();
            //PlayerGreen.SetActive(true); 
            _animator = GetComponent<Animator>();
        }

        public override void OnStartLocalPlayer()
        {
            Debug.Log("On Start Local Player");
        }

        void Update()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            GetInput();
        }

        void GetInput()
        {
            if (player.GetButtonDown("Fire"))
            {
                //CmdFire();
                AddActionToList(Enumerations.Action.Shoot);
            }
            else if (player.GetButtonDown("Move Counterclockwise"))
            {
                //DoAction(Enumerations.Action.TurnLeft);
                AddActionToList(Enumerations.Action.TurnLeft);
            }
            else if (player.GetButtonDown("Move Clockwise"))
            {
                //DoAction(Enumerations.Action.TurnRight);
                AddActionToList(Enumerations.Action.TurnRight);
            }
            else if (player.GetButtonDown("Short Move"))
            {
                //DoAction(Enumerations.Action.ShortMove);
                AddActionToList(Enumerations.Action.ShortMove);
            }
            else if (player.GetButtonDown("Long Move"))
            {
                //DoAction(Enumerations.Action.LongMove);
                AddActionToList(Enumerations.Action.LongMove);
            }
            else if (player.GetButtonDown("Reverse"))
            {
                //DoAction(Enumerations.Action.Reverse);
                AddActionToList(Enumerations.Action.Reverse);
            }
        }

        public void DoAction(Enumerations.Action action)
        {
            Debug.Log("Player " + Id + " Do action " + action);

            switch (action)
            {
                case Enumerations.Action.TurnRight:
                    //_animator.SetTrigger("Turn");
                    //transform.RotateAround(transform.position, transform.up, 90);
                    _animator.SetInteger("TurnIndex", 3);
                    _animator.SetTrigger("Turn");
                    break;
                case Enumerations.Action.TurnLeft:
                    //transform.RotateAround(transform.position, transform.up, -90);
                    _animator.SetInteger("TurnIndex", 0);
                    _animator.SetTrigger("Turn");
                    break;
                case Enumerations.Action.Reverse:
                    transform.Translate(-Vector3.forward);
                    break;
                case Enumerations.Action.ShortMove:
                    //transform.Translate(Vector3.forward);
                    _animator.SetTrigger("WalkForward");
                    break;
                case Enumerations.Action.LongMove:
                    //transform.Translate(Vector3.forward * 3);
                    _animator.SetTrigger("SprintForward");
                    break;
                case Enumerations.Action.Shoot:
                    CmdFire();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("action", action, null);
            }
        }

        
        public void AddActionToList(Enumerations.Action action)
        {
            if (ActionList.Count < 4)
            {
                if (isLocalPlayer)
                {
                    PlayerState.DisplayCommands(action);
                }

                ActionList.Add(action);
            }
            else if (ActionList.Count < 5)
            {
                ActionList.Add(action);

                if (isLocalPlayer)
                {
                    PlayerState.DisplayCommands(action);
                }

                CmdSetPlayerReady();
            }
        }

        [Command]
        public void CmdSetPlayerReady()
        {
            if (_gameManager == null)
            {
                _gameManager = FindObjectOfType<GameManager>();
            }

            _gameManager.ServerPlayerReady(Id);

            Debug.Log(Id + " ready!");
        }

        [Command]
        void CmdFire()
        {
            _animator.SetTrigger("EnableAim");
            _animator.SetTrigger("Fire");

            // This [Command] code is run on the server!
            var laser = Instantiate(
                LaserPrefab,
                LaserBase.transform.position,
                transform.rotation);
            NetworkServer.Spawn(laser);

            Destroy(laser, 0.45f);
        }

        
        public void ResolveCommands(int action)
        {
            RpcResolveCommands(action);
        }

        [ClientRpc]
        public void RpcResolveCommands(int action)
        {
            if(!isLocalPlayer)
                return;

            //for (var i = 0; i < ActionList.Count; i++)
            //{
            if (ActionList[action] == Enumerations.Action.Shoot)
            {
                CmdFire();
            }
            else if (ActionList[action] != Enumerations.Action.Shoot) DoAction(ActionList[action]);
            //}
            //ActionList.Clear();
        }

        [ClientRpc]
        public void RpcClearActionList()
        {
            ActionList.Clear();
        }

        //[Command]
        //public void CmdDoSomething()
        //{
        //    _gameManager.
        //}
    }
}

