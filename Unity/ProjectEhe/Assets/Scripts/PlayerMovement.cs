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
        public GameObject bulletPrefab;
        public Player player;
        public PlayerState PlayerState;
        public List<Enumerations.Action> ActionList;
        public int Id;

        public int rewiredPlayerId = 0;

        public bool InputAllowed;

        //public delegate void PlayerActionEventHandler(int hostId, Enumerations.Action action);
        //public PlayerActionEventHandler PlayerActionGiven;

        private List<Enumerations.Action> _actions = new List<Enumerations.Action>();
        private GameManager _gameManager;

        void Awake()
        {
            // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
            ActionList = new List<Enumerations.Action>();
            player = ReInput.players.GetPlayer(rewiredPlayerId);
            _gameManager = FindObjectOfType<GameManager>();
            PlayerState = GetComponent<PlayerState>();
        }

        public override void OnStartLocalPlayer()
        {
            Debug.Log("On Start Local Player");

            GetComponent<MeshRenderer>().material.color = Color.black;
        }

        void Update()
        {
            if (!isLocalPlayer) { 
                GetComponent<MeshRenderer>().material.color = Color.green;
                return;
            }

            //var x = Input.GetAxis("Horizontal") * 0.1f;
            //var z = Input.GetAxis("Vertical") * 0.1f;

            //transform.Translate(x, 0, z);

            if (!InputAllowed)
                return;

            RpcGetInput();
        }

        public void AskForInput()
        {
            _actions.Clear();
            InputAllowed = true;
        }

        [ClientRpc]
        void RpcGetInput()
        {
            if (player.GetButtonDown("Fire"))
            {
                //CmdFire();
                DisplayCommands(Enumerations.Action.Shoot);
            }
            else if (player.GetButtonDown("Move Counterclockwise"))
            {
                //DoAction(Enumerations.Action.TurnLeft);
                DisplayCommands(Enumerations.Action.TurnLeft);
            }
            else if (player.GetButtonDown("Move Clockwise"))
            {
                //DoAction(Enumerations.Action.TurnRight);
                DisplayCommands(Enumerations.Action.TurnRight);
            }
            else if (player.GetButtonDown("Short Move"))
            {
                //DoAction(Enumerations.Action.ShortMove);
                DisplayCommands(Enumerations.Action.ShortMove);
            }
            else if (player.GetButtonDown("Long Move"))
            {
                //DoAction(Enumerations.Action.LongMove);
                DisplayCommands(Enumerations.Action.LongMove);
            }
            else if (player.GetButtonDown("Reverse"))
            {
                //DoAction(Enumerations.Action.Reverse);
                DisplayCommands(Enumerations.Action.Reverse);
                _gameManager.PlayerActionGiven(Id, Enumerations.Action.Shoot);

                //if(PlayerActionGiven != null)
                //    PlayerActionGiven(Id, Enumerations.Action.Shoot);
                //CmdFire();
            }
            //else if (player.GetButtonDown("Move Counterclockwise"))
            //{
            //    _gameManager.PlayerActionGiven(Id, Enumerations.Action.TurnLeft);

            //    //DoAction(Enumerations.Action.TurnLeft);
            //    //if (PlayerActionGiven != null)
            //    //    PlayerActionGiven(Id, Enumerations.Action.TurnLeft);
            //    Debug.Log("Move counterclockwise");
            //}
            //else if (player.GetButtonDown("Move Clockwise"))
            //{
            //    _gameManager.PlayerActionGiven(Id, Enumerations.Action.TurnRight);

            //    //DoAction(Enumerations.Action.TurnRight);
            //    //if (PlayerActionGiven != null)
            //    //    PlayerActionGiven(Id, Enumerations.Action.TurnRight);
            //    Debug.Log("Move clockwise");
            //}
            //else if (player.GetButtonDown("Short Move"))
            //{
            //    _gameManager.PlayerActionGiven(Id, Enumerations.Action.ShortMove);

            //    //DoAction(Enumerations.Action.ShortMove);
            //    //if (PlayerActionGiven != null)
            //    //    PlayerActionGiven(Id, Enumerations.Action.ShortMove);
            //    Debug.Log("Short Move");
            //}
            //else if (player.GetButtonDown("Long Move"))
            //{
            //    _gameManager.PlayerActionGiven(Id, Enumerations.Action.LongMove);

            //    //DoAction(Enumerations.Action.LongMove);
            //    //if (PlayerActionGiven != null)
            //    //    PlayerActionGiven(Id, Enumerations.Action.LongMove);
            //    Debug.Log("Long Move");
            //}
            //else if (player.GetButtonDown("Reverse"))
            //{
            //    _gameManager.PlayerActionGiven(Id, Enumerations.Action.Reverse);

            //    //DoAction(Enumerations.Action.Reverse);
            //    //if (PlayerActionGiven != null)
            //    //    PlayerActionGiven(Id, Enumerations.Action.Reverse);

            //    Debug.Log("Reverse");
            //}
        }

        public void DoAction(Enumerations.Action action)
        {
            Debug.Log("Player " + Id + " Do action " + action);

            switch (action)
            {
                case Enumerations.Action.TurnRight:
                    transform.RotateAround(transform.position, transform.up, 90);
                    break;
                case Enumerations.Action.TurnLeft:
                    transform.RotateAround(transform.position, transform.up, -90);
                    break;
                case Enumerations.Action.Reverse:
                    transform.Translate(-Vector3.forward);
                    break;
                case Enumerations.Action.ShortMove:
                    transform.Translate(Vector3.forward);
                    break;
                case Enumerations.Action.LongMove:
                    transform.Translate(Vector3.forward * 2);
                    break;
                case Enumerations.Action.Shoot:
                    CmdFire();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("action", action, null);
            }
        }

        public void DisplayCommands(Enumerations.Action action)
        {
            if (ActionList.Count < 4)
            {
                PlayerState.DisplayCommands(action);
                ActionList.Add(action);
            }
            else if (ActionList.Count >= 4)
            {
                ActionList.Add(action);
                PlayerState.DisplayCommands(action);
                _gameManager.CmdPlayerReady(Id);
            }
        }

        [Command]
        void CmdFire()
        {
            // This [Command] code is run on the server!

            // create the bullet object locally
            var bullet = Instantiate(
                 bulletPrefab,
                 transform.position + transform.forward,
                 Quaternion.identity);

            bullet.GetComponent<Rigidbody>().velocity = transform.forward * 40;

            // spawn the bullet on the clients
            NetworkServer.Spawn(bullet);

            // when the bullet is destroyed on the server it will automaticaly be destroyed on clients
            Destroy(bullet, 2.0f);
        }

        public void ResolveCommands()
        {
            for (var i = 0; i < ActionList.Count; i++)
            {
                if (ActionList[i] == Enumerations.Action.Shoot) CmdFire();
                else if (ActionList[i] != Enumerations.Action.Shoot) DoAction(ActionList[i]);
            }
            ActionList.Clear();
        }
    }
}

