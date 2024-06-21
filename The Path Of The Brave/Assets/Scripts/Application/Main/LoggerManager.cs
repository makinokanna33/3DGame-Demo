using FrameWork.Base;
using UnityEngine;

namespace MyApplication
{
    public class LoggerManager : SingletonManager<LoggerManager>
    {
        public void Log(string message)
        {
            Debug.Log(message);
        }

        public void LogError(string message)
        {
            Debug.LogError(message);
        }

        public void LogCommandArgsError(string className, string type)
        {
            Debug.LogError($"{className} Execute error, notification.Body is not {type}");
        }
    }
}
