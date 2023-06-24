using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

namespace MBTExample
{
    [AddComponentMenu("")]
    [MBTNode("Custom/CanAttack", 509)]
    public class CanAttack : Leaf
    {
        public override NodeResult Execute()
        {
            if (gameObject.GetComponent<Gegner>().AttackPossible()) return NodeResult.success;
            return NodeResult.failure;
        }
    }
}
