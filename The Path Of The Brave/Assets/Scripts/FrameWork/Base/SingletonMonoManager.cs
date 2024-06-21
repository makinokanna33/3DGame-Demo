using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Base
{
    // 继承MonoBehaviour的单例设计模式模板
    public class SingletonMonoManager<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; } = null;

        // 避免子类重写 Awake 方法时忘记调用父类的 Awake 方法进行实例化，定义为虚函数
        protected virtual void Awake()
        {
            // 如果挂多个脚本，单例模式会破坏掉，加一个 Instance 是否为空的判断
            if (Instance != null)
            {
                Destroy(this);
            }
            else
            {
                Instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
        }

    }
}


