
using Model;
using System.Collections.Generic;
using System.Linq;
namespace BufferPool
{
    /// <summary>
    /// AGV信息缓存类
    /// </summary>
    public class AgvBuffer
    {
        public readonly static AgvBuffer Instance = new AgvBuffer();
        private Dictionary<ushort, AGV> agvs = new Dictionary<ushort, AGV>();

        private AgvBuffer()
        { }

        /// <summary>
        /// 获取AGV数量
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            try
            {
                return agvs.Count;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 更新AGV缓存
        /// </summary>
        /// <param name="agv"></param>
        public void Set(AGV agv)
        {
            try
            {
                if (agvs.ContainsKey(agv.ID))
                {
                    agvs[agv.ID] = agv;
                }
                else
                {
                    agvs.Add(agv.ID, agv);
                }
            }
            catch
            { }
        }

        /// <summary>
        /// 获取AGV信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AGV Get(ushort id)
        {
            try
            {
                if (agvs.ContainsKey(id))
                {
                    return agvs[id];
                }
                return null;
            }
            catch
            { return null; }
        }

        /// <summary>
        /// 移除AGV信息
        /// </summary>
        /// <param name="id"></param>
        public void Remove(ushort id)
        {
            try
            {
                if (agvs.ContainsKey(id))
                {
                    agvs.Remove(id);
                }
            }
            catch
            { }
        }

        /// <summary>
        /// 获取所有AGV信息
        /// </summary>
        /// <returns></returns>
        public AGV[] GetArray()
        {
            try
            {
                return agvs.Values.ToArray();
            }
            catch
            { return null; }
        }
    }
}
