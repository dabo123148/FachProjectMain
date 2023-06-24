using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

namespace MBTExample
{
    [AddComponentMenu("")]
    [MBTNode("Custom/VisitedAgro", 511)]
    public class VisitedAgroPunkt : Leaf
    {
        public override NodeResult Execute()
        {
            if (gameObject.GetComponent<Gegner>().ReturnedToAgroPunkt) return NodeResult.success;
            return NodeResult.failure;
        }
    }
}
