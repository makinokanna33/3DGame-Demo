using UnityEngine.SceneManagement;

namespace MyApplication
{
    public class SceneArgs
    {
        // 场景索引号
        public int sceneBuildIndex;
        // 场景进入方式
        public LoadSceneMode loadSceneMode;

        public SceneArgs(int sceneBuildIndex, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            this.sceneBuildIndex = sceneBuildIndex;
            this.loadSceneMode = loadSceneMode;
        }
    }
}