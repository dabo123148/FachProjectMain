using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

namespace MBTExample
{
    [AddComponentMenu("")]
    [MBTNode("Custom/SpezialAttackOnCooldown", 516)]
    public class SpezialAttackOnCooldown : Leaf
    {
        public override NodeResult Execute()
        {
            if (gameObject.GetComponent<RangedBossGegner>().SpezialAttackAvalible) return NodeResult.success;
            return NodeResult.failure;
        }
    }
}
