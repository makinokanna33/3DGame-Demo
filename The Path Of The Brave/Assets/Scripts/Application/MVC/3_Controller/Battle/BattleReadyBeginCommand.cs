using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    public class BattleReadyBeginCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            CharacterDataProxy characterDataProxy = Facade.RetrieveProxy(CharacterDataProxy.NAME) as CharacterDataProxy;
            BattleReadyViewMediator battleReadyViewMediator = Facade.RetrieveMediator(BattleReadyViewMediator.NAME) as BattleReadyViewMediator;
            foreach (var item in characterDataProxy.playerDataList)
            {
                battleReadyViewMediator.GeneratePictureFrameView(item.configData.id, item.configData.sprite);
            }
           
        }
    }
}

