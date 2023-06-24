using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

namespace MBTExample
{
    [AddComponentMenu("")]
    [MBTNode("Custom/PlayerInFollowRange", 513)]
    public class PlayerInFollowRange : Leaf
    {
        public override NodeResult Execute()
        {
            if (gameObject.GetComponent<Gegner>().PlayerInFollowRange()) return NodeResult.success;
            return NodeResult.failure;
        }
    }
}
