using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    [CreateAssetMenu(fileName = "LevelConfiguration", menuName = "Level/Level")]
    public class LevelSOData : ScriptableObject
    {
        public int maxLevel;
        public List<int> exp;
    }
}