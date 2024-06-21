using System.Collections;
using System.Collections.Generic;
using MyApplication;
using UnityEngine;

namespace MyApplication
{
    public class AnimPlayerTip : MonoBehaviour
    {
        public GameObject midTip;
        public GameObject playerTip;

        public void HideMe()
        {
            midTip.SetActive(false);
            playerTip.SetActive(false);

            // 玩家准备阶段
            GameFacade.Instance.SendNotification(AppConst.C_ChangeBattleState, BattleStageEnum.PlayerRound);
        }
    }

}
