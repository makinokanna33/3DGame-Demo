using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PureMVC.Patterns.Proxy;
using UnityEngine;

namespace MyApplication
{
    public class BattleDataProxy : Proxy
    {
        public new const string NAME = "BattleDataProxy";

        public BattleData MyData { get { return Data as BattleData; } }

        public BattleDataProxy() : base(NAME)
        {
            // 在构造函数中初始化一个数据进行关联
            BattleData battleData = new BattleData();
            Data = battleData;
        }

        public List<MyCharacterController> GetEnemyControllers(CharacterCamp camp)
        {
            List<MyCharacterController> tmp = new List<MyCharacterController>();

            foreach (var item in MyData.CharacterControllers)
            {
                if (item.myCharacterData.camp != camp)
                {
                    tmp.Add(item);
                }
            }

            return tmp;
        }
    }
}

