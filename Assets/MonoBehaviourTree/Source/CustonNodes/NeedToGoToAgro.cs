using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

namespace MBTExample
{
    [AddComponentMenu("")]
    [MBTNode("Custom/NeedToGoToAgro", 512)]
    public class NeedToGoToAgro : Leaf
    {
        public override NodeResult Execute()
        {
            if (!gameObject.GetComponent<Gegner>().ReturnedToAgroPunkt) return NodeResult.success;
            return NodeResult.failure;
        }
    }
}
