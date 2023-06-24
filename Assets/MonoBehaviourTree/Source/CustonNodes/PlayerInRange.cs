using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

namespace MBTExample
{
    [AddComponentMenu("")]
    [MBTNode("Custom/PlayerInAgroRange", 508)]
    public class PlayerInRange : Leaf
    {
        public override NodeResult Execute()
        {
            if (gameObject.GetComponent<Gegner>().PlayerInAgroRange()) return NodeResult.success;
            return NodeResult.failure;
        }
    }
}
