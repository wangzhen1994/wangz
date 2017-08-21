
using Model;
namespace SocketHelper
{
    /// <summary>
    /// Agv协议接口
    /// </summary>
    public interface IProtocol
    {
        /// <summary>
        /// 呼叫
        /// </summary>
        /// <returns></returns>
        byte[] Call();

        /// <summary>
        /// 命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        byte[] Command(AgvCommand cmd);

        /// <summary>
        /// 呼叫回复
        /// </summary>
        /// <returns></returns>
        AGV DeCall(byte[] buffer);//呼叫回复

        /// <summary>
        /// 实时汇报
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AGV Report(ushort id, byte[] buffer);//实时汇报
    }
}
