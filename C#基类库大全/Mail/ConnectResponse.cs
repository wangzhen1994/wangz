/// <summary>
/// 类说明：Assistant
/// 源码来自：95网络源码下载
/// 联系方式：78630559  
/// 更多源码请防问：code.995w.com
/// </summary>
using System;
using System.IO;

namespace DotNet.Utilities
{
    internal sealed class ConnectResponse : Pop3Response
    {
        private Stream _networkStream;
        public Stream NetworkStream
        {
            get
            {
                return _networkStream;
            }
        }

        public ConnectResponse(Pop3Response response, Stream networkStream)
            : base(response.ResponseContents, response.HostMessage, response.StatusIndicator)
        {
            if (networkStream == null)
            {
                throw new ArgumentNullException("networkStream");
            }
            _networkStream = networkStream;
        }
    }
}
