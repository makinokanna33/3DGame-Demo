using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Base
{
    // 非继承MonoBehaviour的单例设计模式模板
    public class SingletonManager<T> where T : new()
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                    instance = new T();
                return instance;
            }

        }
    }
}


