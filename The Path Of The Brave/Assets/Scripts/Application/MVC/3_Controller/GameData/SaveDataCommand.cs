using System.Collections;
using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using UnityEngine;

namespace MyApplication
{
    public class SaveDataCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            SaveDataProxy saveDataProxy = Facade.RetrieveProxy(SaveDataProxy.NAME) as SaveDataProxy;

            saveDataProxy.SaveDate();
        }
    }
}
