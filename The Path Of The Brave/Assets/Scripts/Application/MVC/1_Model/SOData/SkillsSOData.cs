using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MyApplication
{
    [CreateAssetMenu(fileName = "SkillsConfiguration", menuName = "Configuration/SkillsConfiguration")]
    public class SkillsSOData : ScriptableObject
    {
        public List<SkillSOData> skillSoDataList;
    }
}
