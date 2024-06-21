using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    [CreateAssetMenu(fileName = "CharactersConfiguration", menuName = "Configuration/CharactersConfiguration")]
    public class CharactersSOData : ScriptableObject
    {
        public List<CharacterSOData> configDataList;
    }
}
