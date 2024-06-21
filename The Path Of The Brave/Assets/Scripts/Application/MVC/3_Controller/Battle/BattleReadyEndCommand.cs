using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    public class BattleReadyEndCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            CharacterDataProxy characterDataProxy = Facade.RetrieveProxy(CharacterDataProxy.NAME) as CharacterDataProxy;
            BattleReadyViewMediator battleReadyViewMediator = Facade.RetrieveMediator(BattleReadyViewMediator.NAME) as BattleReadyViewMediator;
            foreach (var item in battleReadyViewMediator.pictureFrameViews)
            {
                if(item.transformAnchor != null)
                {
                    // 根据 id 找到角色数据
                    var data = characterDataProxy.FindCharacterData(item.id, CharacterCamp.Player);
                    data.InitBattleData();

                    // 生成预制体，绑定角色数据
                    data.startMovePos = item.transformAnchor.position;
                    var myObj = Object.Instantiate(data.configData.pfCharacter, item.transformAnchor.position, item.transformAnchor.rotation);
                    myObj.GetComponent<MyCharacterController>().myCharacterData = data;

                    item.transformAnchor.gameObject.SetActive(false);
                    item.gameObject.SetActive(false);
                }
            }

            GameManager.Instance.battleManager.BattleReadyEnd();
        }
    }
}
