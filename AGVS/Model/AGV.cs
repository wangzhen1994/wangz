
namespace Model
{
    public class AGV
    {
        private ushort _ID;
        /// <summary>
        /// 小车编号
        /// </summary>
        public ushort ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        private Direction _AgvDirection;
        /// <summary>
        /// 方向
        /// </summary>
        public Direction AgvDirection
        {
            get { return _AgvDirection; }
            set { _AgvDirection = value; }
        }

        private Speed _AgvSpeed;
        /// <summary>
        /// 速度
        /// </summary>
        public Speed AgvSpeed
        {
            get { return _AgvSpeed; }
            set { _AgvSpeed = value; }
        }

        private ushort _X;
        /// <summary>
        /// X坐标
        /// </summary>
        public ushort X
        {
            get { return _X; }
            set { _X = value; }
        }

        private ushort _Y;
        /// <summary>
        /// Y坐标
        /// </summary>
        public ushort Y
        {
            get { return _Y; }
            set { _Y = value; }
        }

        private bool _AgvPower;
        /// <summary>
        /// 是否需要充电
        /// </summary>
        public bool AgvPower
        {
            get { return _AgvPower; }
            set { _AgvPower = value; }
        }

        private State _AgvState;
        /// <summary>
        /// Agv状态
        /// </summary>
        public State AgvState
        {
            get { return _AgvState; }
            set { _AgvState = value; }
        }

        private bool _IsError;
        /// <summary>
        /// 是否异常
        /// </summary>
        public bool IsError
        {
            get { return _IsError; }
            set { _IsError = value; }
        }
    }
}
