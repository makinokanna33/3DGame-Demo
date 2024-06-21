﻿using System.Collections;
using System.Collections.Generic;
using MyApplication;
using UnityEngine;
using UnityEngine.Events;

namespace FrameWork.Base
{
    public class ABManager : SingletonManager<ABManager>
    {
        //主包
        private AssetBundle mainAB = null;

        //主包依赖获取配置文件
        private AssetBundleManifest manifest = null;

        //字典知识 用来存储 AB包对象
        private Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>();

        /// <summary>
        /// 获取AB包加载路径
        /// </summary>
        private string PathUrl
        {
            get
            {
                return Application.streamingAssetsPath + "/";
            }
        }

        /// <summary>
        /// 主包名 根据平台不同 报名不同
        /// </summary>
        private string MainName
        {
            get
            {
#if UNITY_IOS
            return "IOS";
#elif UNITY_ANDROID
            return "Android";
#else
                return "StandaloneWindows";
#endif
            }
        }

        /// <summary>
        /// 加载主包 和 配置文件
        /// </summary>
        private void LoadMainAB()
        {
            if (mainAB == null)
            {
                mainAB = AssetBundle.LoadFromFile(PathUrl + MainName);
                manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }
        }

        /// <summary>
        /// 加载指定包的依赖包
        /// </summary>
        /// <param name="abName"></param>
        private void LoadDependencies(string abName)
        {
            //加载主包
            LoadMainAB();
            //获取依赖包
            string[] strs = manifest.GetAllDependencies(abName);
            for (int i = 0; i < strs.Length; i++)
            {
                if (!abDic.ContainsKey(strs[i]))
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                    abDic.Add(strs[i], ab);
                }
            }
        }

        /// <summary>
        /// 泛型资源同步加载
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName"></param>
        /// <param name="resName"></param>
        /// <returns></returns>
        public T LoadRes<T>(string abName, string resName) where T : Object
        {
            //加载依赖包
            LoadDependencies(abName);
            //加载目标包
            if (!abDic.ContainsKey(abName))
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                abDic.Add(abName, ab);
            }
            //得到加载出来的资源
            T obj = abDic[abName].LoadAsset<T>(resName);
            //如果是GameObject 因为GameObject 100%都是需要实例化的
            //所以我们直接实例化
            if (obj is GameObject)
                return Object.Instantiate(obj);
            else
                return obj;
        }

        /// <summary>
        /// 所有泛型资源同步加载
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName"></param>
        /// <returns></returns>
        public T[] LoadAllRes<T>(string abName) where T : Object
        {
            //加载依赖包
            LoadDependencies(abName);
            //加载目标包
            if (!abDic.ContainsKey(abName))
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                abDic.Add(abName, ab);
            }
            //得到加载出来的资源
            T[] objs = abDic[abName].LoadAllAssets<T>();

            return objs;
        }

        /// <summary>
        /// Type同步加载指定资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="resName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Object LoadRes(string abName, string resName, System.Type type)
        {
            //加载依赖包
            LoadDependencies(abName);
            //加载目标包
            if (!abDic.ContainsKey(abName))
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                abDic.Add(abName, ab);
            }

            //得到加载出来的资源
            Object obj = abDic[abName].LoadAsset(resName, type);

            if (obj is GameObject)
                return Object.Instantiate(obj);
            else
                return obj;
        }

        /// <summary>
        /// 名字 同步加载指定资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="resName"></param>
        /// <returns></returns>
        public Object LoadRes(string abName, string resName)
        {
            //加载依赖包
            LoadDependencies(abName);
            //加载目标包
            if (!abDic.ContainsKey(abName))
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                abDic.Add(abName, ab);
            }

            //得到加载出来的资源
            Object obj = abDic[abName].LoadAsset(resName);

            if (obj is GameObject)
                return Object.Instantiate(obj);
            else
                return obj;
        }

        /// <summary>
        /// 泛型异步加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName"></param>
        /// <param name="resName"></param>
        /// <param name="callBack"></param>
        public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack) where T : Object
        {
            GameManager.Instance.StartCoroutine(ReallyLoadResAsync<T>(abName, resName, callBack));
        }
        //协程函数
        private IEnumerator ReallyLoadResAsync<T>(string abName, string resName, UnityAction<T> callBack) where T : Object
        {
            //加载依赖包
            LoadDependencies(abName);
            //加载目标包
            if (!abDic.ContainsKey(abName))
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                abDic.Add(abName, ab);
            }
            //异步加载包中资源
            AssetBundleRequest abq = abDic[abName].LoadAssetAsync<T>(resName);
            yield return abq;

            if (abq.asset is GameObject)
                callBack(Object.Instantiate(abq.asset) as T);
            else
                callBack(abq.asset as T);

        }

        /// <summary>
        /// Type异步加载资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="resName"></param>
        /// <param name="type"></param>
        /// <param name="callBack"></param>
        public void LoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack)
        {
            GameManager.Instance.StartCoroutine(ReallyLoadResAsync(abName, resName, type, callBack));
        }

        private IEnumerator ReallyLoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack)
        {
            //加载依赖包
            LoadDependencies(abName);
            //加载目标包
            if (!abDic.ContainsKey(abName))
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                abDic.Add(abName, ab);
            }
            //异步加载包中资源
            AssetBundleRequest abq = abDic[abName].LoadAssetAsync(resName, type);
            yield return abq;

            if (abq.asset is GameObject)
                callBack(Object.Instantiate(abq.asset));
            else
                callBack(abq.asset);
        }

        /// <summary>
        /// 名字 异步加载 指定资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="resName"></param>
        /// <param name="callBack"></param>
        public void LoadResAsync(string abName, string resName, UnityAction<Object> callBack)
        {
            GameManager.Instance.StartCoroutine(ReallyLoadResAsync(abName, resName, callBack));
        }

        private IEnumerator ReallyLoadResAsync(string abName, string resName, UnityAction<Object> callBack)
        {
            //加载依赖包
            LoadDependencies(abName);
            //加载目标包
            if (!abDic.ContainsKey(abName))
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                abDic.Add(abName, ab);
            }
            //异步加载包中资源
            AssetBundleRequest abq = abDic[abName].LoadAssetAsync(resName);
            yield return abq;

            if (abq.asset is GameObject)
                callBack(Object.Instantiate(abq.asset));
            else
                callBack(abq.asset);
        }

        //卸载AB包的方法
        public void UnLoadAB(string name)
        {
            if (abDic.ContainsKey(name))
            {
                abDic[name].Unload(false);
                abDic.Remove(name);
            }
        }

        //清空AB包的方法
        public void ClearAB()
        {
            AssetBundle.UnloadAllAssetBundles(false);
            abDic.Clear();
            //卸载主包
            mainAB = null;
        }
    }
}

