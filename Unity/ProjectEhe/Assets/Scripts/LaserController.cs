using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class LaserController : MonoBehaviour
    {
        public LineRenderer Line;

        public void Start()
        {
            Line = gameObject.GetComponent<LineRenderer>();
            Line.enabled = false;
            StartLaser();
        }

        public void StartLaser()
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            Line.SetPosition(0, ray.origin);

            if (Physics.Raycast(ray, out hit, 20))
            {
                Line.SetPosition(1, hit.point);
                if (hit.rigidbody)
                    hit.transform.SendMessage("HitByRay");
            }
            else
                Line.SetPosition(1, ray.GetPoint(20));
        }
    }
}
