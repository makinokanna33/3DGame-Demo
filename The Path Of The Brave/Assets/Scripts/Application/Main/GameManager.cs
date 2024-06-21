using System.Collections.Generic;
using FrameWork.Base;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyApplication
{
    public class GameManager : SingletonMonoManager<GameManager>
    {
        #region 常量
        #endregion

        #region 事件
        #endregion

        #region 字段
        [HideInInspector] public ABManager abManager = null;    // AB资源管理器
        [HideInInspector] public BattleManager battleManager = null;
        [HideInInspector] public SkillManager skillManager = null;
        [HideInInspector] public EffectManager effectManager = null;
        [HideInInspector] public CameraManager cameraManager = null;

        #endregion

        #region 属性
        #endregion

        #region 方法
        #endregion

        #region Unity回调
        protected override void Awake()
        {
            base.Awake();
        }

        void Start()
        {
            // 实例化管理类
            abManager = ABManager.Instance;
            skillManager = SkillManager.Instance;
            effectManager = EffectManager.Instance;

            // 添加进入场景的监听，剩下的交给 MVC 框架处理
            SceneManager.sceneLoaded += (arg0, arg1) =>
            {
                GameFacade.Instance.SendNotification(AppConst.C_EnterScene, new SceneArgs(arg0.buildIndex, arg1));
            };

            // 添加退出场景的监听，剩下的交给 MVC 框架处理
            SceneManager.sceneUnloaded += (arg0) =>
            {
                GameFacade.Instance.SendNotification(AppConst.C_ExitScene, new SceneArgs(arg0.buildIndex));
            };
            
            // 启动MVC框架
            GameFacade.Instance.StartUp();
        }


        void Update()
        {

        }
        #endregion

        #region 事件回调
        #endregion

        #region 帮助方法
        #endregion
    }
}