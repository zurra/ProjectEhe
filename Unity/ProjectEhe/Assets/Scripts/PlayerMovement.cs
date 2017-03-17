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
        public GameObject PlayerGreen;
        public GameObject PlayerYellow;
        public GameObject PlayerBlue;
        public GameObject PlayerRed;
        public Player player;
        public PlayerState PlayerState;
        [SyncVar]
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
            //ActionList = new SyncListInt();
            player = ReInput.players.GetPlayer(rewiredPlayerId);
            _gameManager = FindObjectOfType<GameManager>();
            PlayerState = GetComponent<PlayerState>();
           // PlayerGreen.SetActive(true); 
            _animator = gameObject.GetComponent<Animator>();
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
                CmdAddActionToList(Enumerations.Action.Shoot);
            }
            else if (player.GetButtonDown("Move Counterclockwise"))
            {
                //DoAction(Enumerations.Action.TurnLeft);
                CmdAddActionToList(Enumerations.Action.TurnLeft);
            }
            else if (player.GetButtonDown("Move Clockwise"))
            {
                //DoAction(Enumerations.Action.TurnRight);
                CmdAddActionToList(Enumerations.Action.TurnRight);
            }
            else if (player.GetButtonDown("Short Move"))
            {
                //DoAction(Enumerations.Action.ShortMove);
                CmdAddActionToList(Enumerations.Action.ShortMove);
            }
            else if (player.GetButtonDown("Long Move"))
            {
                //DoAction(Enumerations.Action.LongMove);
                CmdAddActionToList(Enumerations.Action.LongMove);
            }
            else if (player.GetButtonDown("Reverse"))
            {
                //DoAction(Enumerations.Action.Reverse);
                CmdAddActionToList(Enumerations.Action.Reverse);
            }
        }

        public void DoAction(Enumerations.Action action)
        {
            Debug.Log("Player " + Id + " Do action " + action);

            //RpcAnimate(action);

            switch (action)
            {
                case Enumerations.Action.TurnRight:
                    //_animator.SetTrigger("Turn");
                    //transform.RotateAround(transform.position, transform.up, 90);
                   
                    break;
                case Enumerations.Action.TurnLeft:
                    //transform.RotateAround(transform.position, transform.up, -90);
                    
                    break;
                case Enumerations.Action.Reverse:
                   
                    break;
                case Enumerations.Action.ShortMove:
                   
                    break;
                case Enumerations.Action.LongMove:
                   
                    break;
                case Enumerations.Action.Shoot:
                    CmdFire();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("action", action, null);
            }
        }

        public void RpcAnimate()
        {
            
        }

        [ClientRpc]
        public void RpcAnimate(Enumerations.Action action)
        {
            if(!isLocalPlayer)
                return;

            switch (action)
            {
                case Enumerations.Action.TurnRight:
                    _animator.SetInteger("TurnIndex", 3);
                    _animator.SetTrigger("Turn");

                    GetComponent<NetworkAnimator>().SetTrigger("Turn");
                    break;
                case Enumerations.Action.TurnLeft:
                    _animator.SetInteger("TurnIndex", 0);
                    _animator.SetTrigger("Turn");

                    GetComponent<NetworkAnimator>().SetTrigger("Turn");
                    break;
                case Enumerations.Action.Reverse:
                    transform.Translate(-Vector3.forward);
                    break;
                case Enumerations.Action.ShortMove:
                    //transform.Translate(Vector3.forward);
                    _animator.SetTrigger("WalkForward");
                    GetComponent<NetworkAnimator>().SetTrigger("WalkForward");
                    break;
                case Enumerations.Action.LongMove:
                    //transform.Translate(Vector3.forward * 3);
                    _animator.SetTrigger("SprintForward");
                    GetComponent<NetworkAnimator>().SetTrigger("SprintForward");
                    break;
                case Enumerations.Action.Shoot:
                    _animator.SetTrigger("Fire");
                    GetComponent<NetworkAnimator>().SetTrigger("Fire");
                    break;
                default:
                    throw new ArgumentOutOfRangeException("action", action, null);
            }
        }

        [Command]
        public void CmdAddActionToList(Enumerations.Action action)
        {
            _gameManager.ServerAddActionToList(this, action);
        }

        [ClientRpc]
        public void RpcDisplayCommand(Enumerations.Action action)
        {
            if (isLocalPlayer)
            {
                PlayerState.DisplayCommands(action);
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
            // This [Command] code is run on the server!
            var laser = Instantiate(
                LaserPrefab,
                transform.position + transform.forward,
                transform.rotation);
            NetworkServer.Spawn(laser);

            Destroy(laser, 0.45f);
        }

        [ClientRpc]
        public void RpcResolveCommands(Enumerations.Action action)
        {
            if(!isLocalPlayer)
                return;

            if (action == (Enumerations.Action.Shoot))
            {
                CmdFire();
            }
            else if (action != Enumerations.Action.Shoot) DoAction(action);
        }

        [ClientRpc]
        public void RcpEmptyDisplayCommands()
        {
            PlayerState.EmptyCommands();
        }
    }
}

