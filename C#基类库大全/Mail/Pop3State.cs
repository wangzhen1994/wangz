/// <summary>
/// 类说明：Assistant
/// 源码来自：95网络源码下载
/// 联系方式：78630559  
/// 更多源码请防问：code.995w.com
/// </summary>
using System;


namespace DotNet.Utilities
{
    [Flags()]
    public enum Pop3State
    {
        /// <summary>
        /// Undefined.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Connected to Pop3 server, but not authorized.
        /// May issue any of the following commands QUIT, USER, PASS
        /// </summary>
        Authorization = 1,
        /// <summary>
        /// Authorized to Pop3 server, can issue any of the following commands;
        /// STAT, LIST, RETR, DELE, RSET
        /// </summary>
        Transaction = 2,
        /// <summary>
        /// Quit command was sent to server indicating deleted
        /// messages should be removed.
        /// </summary>
        Update = 4
    }
}
