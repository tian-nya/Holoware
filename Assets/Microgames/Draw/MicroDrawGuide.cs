namespace Micro.Draw
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MicroDrawGuide : MonoBehaviour
    {
        [HideInInspector] public MicroDraw microgame;

        public void NodePrep()
        {
            MicroDrawNode[] nodes = GetComponentsInChildren<MicroDrawNode>();
            microgame.nodeAmount = nodes.Length;
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i].microgame = microgame;
                nodes[i].transform.localScale = Vector3.one * microgame.nodeSize;
            }
        }
    }
}