using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace MyApplication
{
    [CreateAssetMenu(fileName = "SkillConfiguration", menuName = "Configuration/SkillConfiguration")]
    public class SkillSOData : ScriptableObject
    {
        public SkillConfigData configData;
    }
}

