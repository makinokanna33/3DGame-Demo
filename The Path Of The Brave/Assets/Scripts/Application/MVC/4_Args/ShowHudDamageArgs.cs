using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    public class ShowHudDamageArgs
    {
        public int damage;
        public Vector3 worldPos;

        public ShowHudDamageArgs(int damage, Vector3 worldPos)
        {
            this.damage = damage;
            this.worldPos = worldPos;
        }
    }

}
