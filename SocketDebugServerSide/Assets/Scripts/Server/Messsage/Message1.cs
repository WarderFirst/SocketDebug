﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using SocketDebug.Server.Messsage;

namespace SocketDebug {


    [System.Serializable]
    public class DebugData
    {
        public string logString;
        public string stackTrace;
        public MessageType type;
    }
    //
    // Summary:
    //     The type of the log message in Debug.unityLogger.Log or delegate registered with
    //     Application.RegisterLogCallback.
    //public enum LogType
    //{
    //    //

    //    // Summary:
    //    //     LogType used for Errors.
    //    Error = 0,
    //    //
    //    // Summary:
    //    //     LogType used for Asserts. (These could also indicate an error inside Unity itself.)
    //    Assert = 1,
    //    //
    //    // Summary:
    //    //     LogType used for Warnings.
    //    Warning = 2,
    //    //
    //    // Summary:
    //    //     LogType used for regular log messages.
    //    Log = 3,
    //    //
    //    // Summary:
    //    //     LogType used for Exceptions.
    //    Exception = 4
    //}


    //[ProtoContract]
    //public class Message1 {
    //    [ProtoMember(1)]
    //    public LogType _messageType;
    //    [ProtoMember(2)]
    //    public string _Id;
    //    //[ProtoMember(4)]
    //    //public object[] _parameters;

    //    public Message1() {

    //    }
    //    public Message1(LogType messageType, string id) {
    //        _messageType = messageType;
    //        _Id = id;
    //    }

    //}
}
