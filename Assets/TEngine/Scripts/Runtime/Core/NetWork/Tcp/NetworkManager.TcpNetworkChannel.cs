﻿using System;
using System.Net;
using System.Net.Sockets;

namespace TEngine.Runtime
{
    public sealed partial class NetworkManager
    {
        /// <summary>
        /// TCP 网络频道。
        /// </summary>
        private sealed class TcpNetworkChannel : NetworkChannelBase
        {
            private readonly AsyncCallback m_ConnectCallback;
            private readonly AsyncCallback m_SendCallback;
            private readonly AsyncCallback m_ReceiveCallback;

            /// <summary>
            /// 初始化网络频道的新实例。
            /// </summary>
            /// <param name="name">网络频道名称。</param>
            /// <param name="networkChannelHelper">网络频道辅助器。</param>
            public TcpNetworkChannel(string name, INetworkChannelHelper networkChannelHelper)
                : base(name, networkChannelHelper)
            {
                m_ConnectCallback = ConnectCallback;
                m_SendCallback = SendCallback;
                m_ReceiveCallback = ReceiveCallback;
            }

            /// <summary>
            /// 获取网络服务类型。
            /// </summary>
            public override ServiceType ServiceType
            {
                get { return ServiceType.Tcp; }
            }

            /// <summary>
            /// 连接到远程主机。
            /// </summary>
            /// <param name="ipAddress">远程主机的 IP 地址。</param>
            /// <param name="port">远程主机的端口号。</param>
            /// <param name="userData">用户自定义数据。</param>
            public override void Connect(IPAddress ipAddress, int port, object userData)
            {
                base.Connect(ipAddress, port, userData);
                m_Socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                if (m_Socket == null)
                {
                    string errorMessage = "Initialize network channel failure.";
                    if (NetworkChannelError != null)
                    {
                        NetworkChannelError(this, NetworkErrorCode.SocketError, SocketError.Success, errorMessage);
                        return;
                    }

                    throw new Exception(errorMessage);
                }

                m_NetworkChannelHelper.PrepareForConnecting();
                ConnectAsync(ipAddress, port, userData);
            }

            protected override bool ProcessSend()
            {
                if (base.ProcessSend())
                {
                    SendAsync();
                    return true;
                }

                return false;
            }

            private void ConnectAsync(IPAddress ipAddress, int port, object userData)
            {
                try
                {
                    m_Socket.BeginConnect(ipAddress, port, m_ConnectCallback, new ConnectState(m_Socket, userData));
                }
                catch (Exception exception)
                {
                    if (NetworkChannelError != null)
                    {
                        SocketException socketException = exception as SocketException;
                        NetworkChannelError(this, NetworkErrorCode.ConnectError,
                            socketException != null ? socketException.SocketErrorCode : SocketError.Success,
                            exception.ToString());
                        return;
                    }

                    throw;
                }
            }

            private void ConnectCallback(IAsyncResult ar)
            {
                ConnectState socketUserData = (ConnectState)ar.AsyncState;
                try
                {
                    socketUserData.Socket.EndConnect(ar);
                }
                catch (ObjectDisposedException)
                {
                    return;
                }
                catch (Exception exception)
                {
                    m_Active = false;
                    if (NetworkChannelError != null)
                    {
                        SocketException socketException = exception as SocketException;
                        NetworkChannelError(this, NetworkErrorCode.ConnectError,
                            socketException != null ? socketException.SocketErrorCode : SocketError.Success,
                            exception.ToString());
                        return;
                    }

                    throw;
                }

                m_SentPacketCount = 0;
                m_ReceivedPacketCount = 0;

                lock (m_SendPacketPool)
                {
                    m_SendPacketPool.Clear();
                }

                lock (m_HeartBeatState)
                {
                    m_HeartBeatState.Reset(true);
                }

                if (NetworkChannelConnected != null)
                {
                    NetworkChannelConnected(this, socketUserData.UserData);
                }

                m_Active = true;
                ReceiveAsync();
            }

            private void SendAsync()
            {
                try
                {
                    var buffer = m_SendState.Stream.GetBuffer();
                    var buffBodyCount = buffer[0];
                    var buffTotalCount = m_NetworkChannelHelper.PacketHeaderLength + buffBodyCount;
                    m_SendState.Stream.SetLength(buffTotalCount);
                    m_Socket.BeginSend(buffer, 0, buffTotalCount, SocketFlags.None, m_SendCallback, m_Socket);
                }
                catch (Exception exception)
                {
                    m_Active = false;
                    if (NetworkChannelError != null)
                    {
                        SocketException socketException = exception as SocketException;
                        NetworkChannelError(this, NetworkErrorCode.SendError,
                            socketException != null ? socketException.SocketErrorCode : SocketError.Success,
                            exception.ToString());
                        return;
                    }

                    throw;
                }
            }

            private void SendCallback(IAsyncResult ar)
            {
                Socket socket = (Socket)ar.AsyncState;
                if (!socket.Connected)
                {
                    return;
                }

                int bytesSent = 0;
                try
                {
                    bytesSent = socket.EndSend(ar);
                }
                catch (Exception exception)
                {
                    m_Active = false;
                    if (NetworkChannelError != null)
                    {
                        SocketException socketException = exception as SocketException;
                        NetworkChannelError(this, NetworkErrorCode.SendError,
                            socketException != null ? socketException.SocketErrorCode : SocketError.Success,
                            exception.ToString());
                        return;
                    }

                    throw;
                }

                m_SentPacketCount++;
                m_SendState.Reset();
            }

            private void ReceiveAsync()
            {
                m_ReceiveState.Stream.SetLength(ReceiveState.DefaultBufferLength);
                try
                {
                    m_Socket.BeginReceive(m_ReceiveState.Stream.GetBuffer(), (int)m_ReceiveState.Stream.Position,
                        (int)(m_ReceiveState.Stream.Length - m_ReceiveState.Stream.Position), SocketFlags.None,
                        m_ReceiveCallback, m_Socket);
                }
                catch (Exception exception)
                {
                    m_Active = false;
                    if (NetworkChannelError != null)
                    {
                        SocketException socketException = exception as SocketException;
                        NetworkChannelError(this, NetworkErrorCode.ReceiveError,
                            socketException != null ? socketException.SocketErrorCode : SocketError.Success,
                            exception.ToString());
                        return;
                    }

                    throw;
                }
            }

            private void ReceiveCallback(IAsyncResult ar)
            {
                Socket socket = (Socket)ar.AsyncState;
                if (!socket.Connected)
                {
                    return;
                }

                int bytesReceived = 0;
                try
                {
                    bytesReceived = socket.EndReceive(ar);
                }
                catch (Exception exception)
                {
                    m_Active = false;
                    if (NetworkChannelError != null)
                    {
                        SocketException socketException = exception as SocketException;
                        NetworkChannelError(this, NetworkErrorCode.ReceiveError,
                            socketException != null ? socketException.SocketErrorCode : SocketError.Success,
                            exception.ToString());
                        return;
                    }

                    throw;
                }

                if (bytesReceived <= 0)
                {
                    Close();
                    return;
                }

                m_ReceiveState.Stream.Position = 0L;

                m_ReceiveState.Stream.SetLength(bytesReceived);

                ProtoUtils.PrintBuffer(m_ReceiveState.Stream.GetBuffer(),m_NetworkChannelHelper.PacketHeaderLength + m_ReceiveState.Stream.GetBuffer()[0]);
                
                bool processSuccess = false;
                processSuccess = ProcessPacket();
                m_ReceivedPacketCount++;

                if (processSuccess)
                {
                    ReceiveAsync();
                }
                else
                {
                    ReceiveAsync();
                }
            }
        }
    }
}