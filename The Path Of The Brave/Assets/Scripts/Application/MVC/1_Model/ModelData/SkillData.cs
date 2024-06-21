using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyApplication
{
    public enum SkillType
    {
        Active,
        Passive
    }

    [System.Serializable]
    public class SkillConfigData
    {
        public int id;
        public string name;
        public Sprite icon;
        public SkillType type;
        public int cd;
        [TextArea] public string description;
    }

    public class SkillData
    {
        public SkillConfigData configData;
        public int currentCD; // 当前 CD

        public SkillData(SkillConfigData configData)
        {
            this.configData = configData;
            currentCD = 0;
        }
    }
}