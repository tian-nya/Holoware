namespace Micro.Grab
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MicroGrabGoal : MonoBehaviour
    {
        [HideInInspector] public MicroGrab microgame;

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (!microgame.cleared)
            {
                if (collision.tag == "Pickup")
                {
                    microgame.cleared = true;
                } else if (collision.tag == "Hazard")
                {
                    microgame.fail = true;
                }
            }
        }
    }
}