using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bluetooth_Mouse_Controller_Receiver
{
    class BTTaskManager
    {
        private Dictionary<Guid, BTTask> btTasks;

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<Guid, int> taskIds;
        private BTTaskManager()
        {
            btTasks = new Dictionary<Guid, BTTask>();
            taskIds = new Dictionary<Guid, int>();
        }
        
        private static BTTaskManager _instance;
        public static BTTaskManager instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new BTTaskManager();
                }
                return _instance;
            }
        }
        private int getFreeIndex()
        {
            return taskIds.Count;
        }
        public int getIndex(BTTask bTTask)
        {
            return taskIds[bTTask.taskId];
        }

        /// <summary>
        /// 新建Task的唯一入口
        /// </summary>
        /// <returns></returns>
        public BTTask newTask()
        {
            if (btTasks.Count >= 9)
            {
                return null;
            }
            Guid taskId = Guid.NewGuid();
            BTTask btTask = new BTTask(taskId);
            btTasks.Add(taskId, btTask);
            int index = getFreeIndex();
            taskIds.Add(taskId, index);
            System.Diagnostics.Debug.WriteLine("UUID:" + btTask.uuid);
            return btTask;
        }

        
        

    }
}
