using UnityEngine.Networking;
using UnityEngine;

namespace Scripts
{
    public class PlayerState : NetworkBehaviour
    {
        public const int maxHealth = 10;

        [SyncVar]
        public int health = maxHealth;

        public void TakeDamage(int amount)
        {
            if (!isServer)
                return;

            health -= amount;
            if (health <= 0)
            {
                health = maxHealth;
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
    }
}
