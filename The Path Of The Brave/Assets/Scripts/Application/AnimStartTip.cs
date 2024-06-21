using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    public class AnimStartTip : MonoBehaviour
    {
        public GameObject startTip;

        public void HideMe()
        {
            startTip.SetActive(false);

            // 回合开始
            GameFacade.Instance.SendNotification(AppConst.C_ChangeBattleState, BattleStageEnum.RoundBegin);
        }
    }

}

