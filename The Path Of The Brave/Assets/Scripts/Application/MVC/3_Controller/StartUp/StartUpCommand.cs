using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using UnityEngine.SceneManagement;

namespace MyApplication
{
    public class StartUpCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);

            // 注册代理
            if (!Facade.HasProxy(GameDataProxy.NAME))
                Facade.RegisterProxy(new GameDataProxy());

            // 注册代理
            if (!Facade.HasProxy(ItemDataProxy.NAME))
                Facade.RegisterProxy(new ItemDataProxy());

            // 注册代理
            if (!Facade.HasProxy(CharacterDataProxy.NAME))
                Facade.RegisterProxy(new CharacterDataProxy());

            // 注册代理
            if (!Facade.HasProxy(BattleDataProxy.NAME))
                Facade.RegisterProxy(new BattleDataProxy()); 

            // 注册代理
            if (!Facade.HasProxy(ShopBuyDataProxy.NAME))
                Facade.RegisterProxy(new ShopBuyDataProxy());

            // 注册代理
            if (!Facade.HasProxy(SkillDataProxy.NAME))
                Facade.RegisterProxy(new SkillDataProxy());

            // 注册代理
            if (!Facade.HasProxy(SelectLevelDataProxy.NAME))
                Facade.RegisterProxy(new SelectLevelDataProxy());

            // 注册代理
            if (!Facade.HasProxy(StarAwardDataProxy.NAME))
                Facade.RegisterProxy(new StarAwardDataProxy());

            // 注册代理
            if (!Facade.HasProxy(SaveDataProxy.NAME))
                Facade.RegisterProxy(new SaveDataProxy());

            // 初始化完毕，从场景 0 进入场景 1
            SceneManager.LoadScene(1);
        }
    }
}

