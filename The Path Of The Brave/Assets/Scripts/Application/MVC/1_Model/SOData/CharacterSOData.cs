using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    [CreateAssetMenu(fileName = "CharacterConfiguration", menuName = "Configuration/CharacterConfiguration")]
    public class CharacterSOData : ScriptableObject
    {
        public CharacterConfigData configData;
    }
}
