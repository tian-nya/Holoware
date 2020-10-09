namespace Micro.Draw
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MicroDrawNode : MonoBehaviour
    {
        [HideInInspector] public MicroDraw microgame;
        bool covered;

        public void CoverNode()
        {
            if (covered) return;
            microgame.nodesCovered++;
            covered = true;
        }
    }
}