using System.Collections;
using System.Collections.Generic;
using MyApplication;
using UnityEngine;

namespace MyApplication
{
    public class AnimEnemyTip : MonoBehaviour
    {
        public GameObject midTip;
        public GameObject enemyTip;

        public void HideMe()
        {
            midTip.SetActive(false);
            enemyTip.SetActive(false);

            // 敌人准备阶段
            GameFacade.Instance.SendNotification(AppConst.C_ChangeBattleState, BattleStageEnum.EnemyRound);
        }
    }
}

