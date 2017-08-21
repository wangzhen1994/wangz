
namespace Model
{
    /// <summary>
    /// Agv命令
    /// </summary>
    public enum AgvCommand
    {
        /// <summary>
        /// 保持
        /// </summary>
        None=0,

        /// <summary>
        /// 开始
        /// </summary>
        Start,

        /// <summary>
        /// 左转
        /// </summary>
        TurnLeft,

        /// <summary>
        /// 右转
        /// </summary>
        TurnRight,

        /// <summary>
        /// 卸货
        /// </summary>
        Unload,

        /// <summary>
        /// 一步
        /// </summary>
        OneStep,

        /// <summary>
        /// 停止
        /// </summary>
        Stop,

        /// <summary>
        /// 充电
        /// </summary>
        Charge
    }
}
