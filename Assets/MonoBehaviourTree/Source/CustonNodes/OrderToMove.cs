using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

namespace MBTExample
{
    [AddComponentMenu("")]
    [MBTNode("Custom/OrderToMove", 517)]
    public class OrderToMove : Leaf
    {
        public override NodeResult Execute()
        {
            if (gameObject.GetComponent<Gegner>().IsOrderedToGoToLocation) return NodeResult.success;
            return NodeResult.failure;
        }
    }
}
