using System;
using System.Collections.Generic;
using RemoteXDataLibary;
/// <summary>
/// 调试后端
/// 因为Unity项目在Editorm模式中没办法使用win32中程序通信间的办法
/// 所以搞出这个调试后端，将控制器的输入都储存到后端上
/// Unity中使用Editor模式时，用WebRequest获取这些东西
/// </summary>
namespace RemoteXDebugBackend
{
    public class DebugBackend
    {
        private static DebugBackend _Instance;
        public static DebugBackend Instance
        {
            get
            {
                if(_Instance == null)
                {
                    _Instance = new DebugBackend();
                }
                return _Instance;
            }
        }

        public bool Running { get; private set; }

        private Dictionary<int, Data> latestData;
        
        public void Start()
        {
            latestData = new Dictionary<int, Data>();
        }
        public void Set(Data data)
        {
            if (latestData.ContainsKey(data.dataType))
            {
                latestData[data.dataType] = data;
            }
            else
            {
                latestData.Add(data.dataType, data);
            }
        }

        
    }
}
