
namespace Model
{
    /// <summary>
    /// 表示状态
    /// </summary>
    public enum State
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,

        /// <summary>
        /// 急停
        /// </summary>
        EmergencyStop,

        /// <summary>
        /// 充电完成
        /// </summary>
        ChargeComplete,

        /// <summary>
        /// 卸货完成
        /// </summary>
        UnloadComplete
    }
}
