namespace Micro.Wear
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MicroWearColliders : MonoBehaviour
    {
        public Collider2D goal;
        public bool touchingGoal;

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision == goal)
            {
                touchingGoal = true;
            }
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (collision == goal)
            {
                touchingGoal = false;
            }
        }
    }
}