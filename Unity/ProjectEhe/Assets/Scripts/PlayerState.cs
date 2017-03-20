using UnityEngine.Networking;
using UnityEngine;
using System.ComponentModel;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class PlayerState : NetworkBehaviour, INotifyPropertyChanged
    {
        public const int maxHealth = 5;
        public UIManager UIManager;
        public List<Enumerations.Action> ActionList;
        
        [SyncVar (hook = "OnHealthChanged")]
        public int Health;

        public event PropertyChangedEventHandler PropertyChanged;

        public void Start()
        {
            UIManager = FindObjectOfType<UIManager>();
            Health = maxHealth;
            ActionList = new List<Enumerations.Action>();
        }

        public void TakeDamage(int amount)
        {
            if (!isServer)
                return;

            Health -= amount;
        }

        [ClientRpc]
        void RpcRespawn()
        {
            if (isLocalPlayer)
            {
                transform.position =
                    GameObject.Find("Spawns").transform.GetChild(UnityEngine.Random.Range(0, 3)).position;
            }
        }

        public void KillPlayer(int id)
        {
            if (isServer && gameObject.GetComponent<PlayerMovement>().Id == id)
            {
                Debug.Log("Kill player!");
                Health = 0;
            }
        }

        public void OnHealthChanged(int health)
        {
            if(UIManager == null)
                UIManager = FindObjectOfType<UIManager>();

            if (isLocalPlayer)
            {
                UIManager.ChangeHealthText(health);
            }
            else if (!isLocalPlayer)
            {
                UIManager.ChangeOppText(health);
            }

            if (health <= 0)
            {
                Health = maxHealth;
                //if (isLocalPlayer)
                //{
                //    UIManager.Lose();
                  
                //}
                //else if (!isLocalPlayer)
                //{
                //    UIManager.Win();
                //}
   
            }

            if(isServer)
                RpcRespawn();
        }

        public void EmptyCommands()
        {
            UIManager.ClearCommands();
        }

        public void DisplayActiveCommand(int i)
        {
            UIManager.DisplayActiveCommand(i);
        }

        public void ResetActiveCommands()
        {
            UIManager.ResetActiveCommands();
        }

        public void DisplayCommands(Enumerations.Action action)
        {
            UIManager.ChangeCommandText(action);
        }

        public void DisplayPlayerName(int i)
        {
            if(UIManager == null)
                UIManager = FindObjectOfType<UIManager>();

            UIManager.SetPlayer(i);
        }

        public void HitByRay()
        {
            TakeDamage(1);
        }

    }
}
