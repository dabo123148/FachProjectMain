using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

namespace MBTExample
{
    [AddComponentMenu("")]
    [MBTNode("Custom/CanMove", 514)]
    public class CanMove : Leaf
    {
        public override NodeResult Execute()
        {
            if (gameObject.GetComponent<MeleeGegner>().CanMove()) return NodeResult.success;
            return NodeResult.failure;
        }
    }
}
