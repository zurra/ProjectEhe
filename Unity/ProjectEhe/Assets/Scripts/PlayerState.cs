using UnityEngine.Networking;
using UnityEngine;
using System.ComponentModel;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class PlayerState : NetworkBehaviour, INotifyPropertyChanged
    {
        public const int maxHealth = 10;
        public UIManager UIManager;
        public List<Enumerations.Action> ActionList;
        
        [SyncVar (hook = "OnHealthChanged")]
        public int Health;

        //public int Health {
        //    get { return health; }
        //    set { if(health != value) {
        //            health = value;
        //            if (isLocalPlayer)
        //            {
        //                OnPropertyChanged("Health");
        //            }
        //            else if (!isLocalPlayer)
        //            {
        //                OnPropertyChanged("OppHealth");
        //            }
        //        }
        //    } }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Start()
        {
            //UIManager = FindObjectOfType<UIManager>();
            //if (isLocalPlayer)
            //{
            //    UIManager.SetPlayerState(GetComponent<PlayerState>());
            UIManager = FindObjectOfType<UIManager>();
            Health = maxHealth;
            ActionList = new List<Enumerations.Action>();
            //if (isLocalPlayer)
            //{
            //UIManager.SetPlayerState(GetComponent<PlayerState>());
            //}
            //else if (!isLocalPlayer)
            //{
            //UIManager.SetEnemyPlayerState(GetComponent<PlayerState>());
            //}
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
                transform.position = Vector3.zero;
            }
        }

        public void OnHealthChanged(int health)
        {
            if (isLocalPlayer)
            {
                UIManager.ChangeHealthText(health);
                //OnPropertyChanged("Health");
            }
            else if (!isLocalPlayer)
            {
                UIManager.ChangeOppText(health);
                //OnPropertyChanged("OppHealth");
            }

            if (health <= 0)
            {
                Health = maxHealth;
                if (isLocalPlayer)
                {
                    UIManager.Lose();
                    //OnPropertyChanged("YouDead");
                }
                else if (!isLocalPlayer)
                {
                    UIManager.Win();
                    //OnPropertyChanged("YouWin");
                }
                RpcRespawn();
            }
        }

        public void DisplayCommands(Enumerations.Action action)
        {
            UIManager.ChangeCommandText(action);
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
