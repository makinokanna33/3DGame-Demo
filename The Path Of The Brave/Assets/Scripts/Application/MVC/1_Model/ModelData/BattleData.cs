using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    public class BattleData
    {
        #region 字段
        private List<MyCharacterController> characterControllers = new List<MyCharacterController>();
        #endregion

        #region 属性
        public List<MyCharacterController> CharacterControllers
        {
            get => characterControllers;
            set => characterControllers = value;
        }
        #endregion

        public BattleData()
        {

        }

       
    }

}
