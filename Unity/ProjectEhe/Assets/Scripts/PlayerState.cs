using UnityEngine.Networking;
using UnityEngine;
using System.ComponentModel;

namespace Assets.Scripts
{
    public class PlayerState : NetworkBehaviour, INotifyPropertyChanged
    {
        public const int maxHealth = 10;
        public UIManager UIManager;
        
        [SyncVar]
        private int health;

        public int Health {
            get { return health; }
            set { if(health != value) {
                    health = value;
                    if (isLocalPlayer)
                    {
                        Debug.Log("Health lost!");
                        OnPropertyChanged("Health");
                    }
                }
            } }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Start()
        {
            Health = maxHealth;
            UIManager = FindObjectOfType<UIManager>();
            if (isLocalPlayer)
            {
                UIManager.SetPlayerState(GetComponent<PlayerState>());
            }
        }

        public void TakeDamage(int amount)
        {
            if (!isServer)
                return;

            Health -= amount;
            if (Health <= 0)
            {
                Health = maxHealth;
                Debug.Log("Dead!");
                RpcRespawn();
            }
        }

        [ClientRpc]
        void RpcRespawn()
        {
            if (isLocalPlayer)
            {
                transform.position = Vector3.zero;
            }
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
