using System;
using UnityEngine;
using UnityEngine.Networking;
using Rewired;
using Scripts;

namespace Assets.Scripts
{
    public class PlayerMovement : NetworkBehaviour
    {
        public GameObject bulletPrefab;
        public Player player;

        public int playerId = 0;

        void Awake()
        {
            // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
            player = ReInput.players.GetPlayer(playerId);
        }

        public override void OnStartLocalPlayer()
        {
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

            GetInput();
        }

        void GetInput()
        {
            if (player.GetButtonDown("Fire"))
            {
                CmdFire();
            }
            else if (player.GetButtonDown("Move Counterclockwise"))
            {
                DoAction(Enumerations.Action.TurnLeft);
                Debug.Log("Move counterclockwise");
            }
            else if (player.GetButtonDown("Move Clockwise"))
            {
                DoAction(Enumerations.Action.TurnRight);
                Debug.Log("Move clockwise");
            }
            else if (player.GetButtonDown("Short Move"))
            {
                DoAction(Enumerations.Action.ShortMove);
                Debug.Log("Short Move");
            }
            else if (player.GetButtonDown("Long Move"))
            {
                DoAction(Enumerations.Action.LongMove);
                Debug.Log("Long Move");
            }
            else if (player.GetButtonDown("Reverse"))
            {
                DoAction(Enumerations.Action.Reverse);
                Debug.Log("Reverse");
            }
        }

        void DoAction(Enumerations.Action action)
        {
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
    }
}

