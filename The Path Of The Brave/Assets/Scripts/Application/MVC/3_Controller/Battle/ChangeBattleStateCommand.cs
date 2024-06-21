using System.Collections;
using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using UnityEngine;

namespace MyApplication
{
    public class ChangeBattleStateCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);

            GameManager.Instance.battleManager.BattleStage = (BattleStageEnum)notification.Body;
        }
    }

}
