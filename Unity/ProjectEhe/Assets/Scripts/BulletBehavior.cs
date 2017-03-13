using UnityEngine;

namespace Scripts
{
    public class BulletBehavior : MonoBehaviour
    {
        void OnCollisionEnter(Collision collision)
        {
            var hit = collision.gameObject;
            var hitPlayer = hit.GetComponent<PlayerMovement>();
            if (hitPlayer != null)
            {
                Debug.Log("BOOM");
                Destroy(gameObject);
            }
        }
    }
}
