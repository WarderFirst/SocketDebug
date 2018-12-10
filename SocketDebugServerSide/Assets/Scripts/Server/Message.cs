using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ServerUnity;

namespace NetworkClasses {


    [System.Serializable]
    public class DebugData
    {
        public string logString;
        public string stackTrace;
        public LogType type;
    }
    //
    // Summary:
    //     The type of the log message in Debug.unityLogger.Log or delegate registered with
    //     Application.RegisterLogCallback.
    public enum LogType
    {
        //

        // Summary:
        //     LogType used for Errors.
        Error = 0,
        //
        // Summary:
        //     LogType used for Asserts. (These could also indicate an error inside Unity itself.)
        Assert = 1,
        //
        // Summary:
        //     LogType used for Warnings.
        Warning = 2,
        //
        // Summary:
        //     LogType used for regular log messages.
        Log = 3,
        //
        // Summary:
        //     LogType used for Exceptions.
        Exception = 4
    }


    [ProtoContract]
    public class Message {
        [ProtoMember(1)]
        public MessageType _messageType;
        [ProtoMember(2)]
        public string _Id;
        [ProtoMember(3)]
        public string _methodName;
        [ProtoMember(4)]
        public Parameters _methodParameters;
        //[ProtoMember(4)]
        //public object[] _parameters;

        public Message() {

        }
        public Message(MessageType messageType, string id, string methodname, Parameters prms) {
            _messageType = messageType;
            _Id = id;
            _methodName = methodname;
            _methodParameters = prms;
        }

    }

    //Class parameters for RPC Methods
    [System.Serializable, ProtoContract(SkipConstructor =false)]
    public class Parameters {
        [ProtoMember(1)]
        public string [] _strParams = null;
        [ProtoMember(2)]
        public int? [] _intParams = null;
        [ProtoMember(3)]
        public float? [] _floatParams = null;
        //[ProtoMember(4)]
        //public TankData _tankParams;
        [ProtoMember(5)]
        public bool? [] _boolParams = null;
        [ProtoMember(6)] 
        public Type[] _types;

        public Parameters() {

        }
        
        public Parameters(params object[] parameters) {
            //  strParam.GetType();
            //  UnityEngine.Debug.Log(parameters[0].GetType().ToString());
            //  UnityEngine.Debug.Log(parameters.Length);
            int floatCount = 0;
            float? [] floatprms = new float?[parameters.Length];
            int stringCount = 0;
            string[] stringprms = new string[parameters.Length];
            int intCount = 0;
            int?[] intprms = new int?[parameters.Length];
            int boolCount = 0;
            bool?[] boolprms = new bool?[parameters.Length];

            _types = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; ++i) {

                if (parameters[i].GetType() == typeof(string)) {
                    Console.WriteLine("String Type in Constructor-----------");
                    stringprms[stringCount] = parameters[i] as string;
                    stringCount++;
                    _types[i] = stringprms.GetType();
                    //    UnityEngine.Debug.Log(strParam);
                }
                if (parameters[i].GetType() == typeof(bool)) {
                   boolprms[boolCount] = (bool)parameters[i];
                    boolCount++;
                    _types[i] = boolprms.GetType();
                      // UnityEngine.Debug.Log(boolParam);
                }
                if (parameters[i].GetType() == typeof(int)) {
                    intprms[intCount] = (int)parameters[i];
                    intCount++;
                    _types[i] = intprms.GetType();
                    //   UnityEngine.Debug.Log(intParams);
                }
                //if (parameters[i].GetType() == typeof(TankData)) {
                //    _tankParams = parameters[i] as TankData;
                //    _types[i] = _tankParams.GetType();
                //    //   UnityEngine.Debug.Log(_tankParams.x);
                //}
                if (parameters[i].GetType() == typeof(float)) {
                    Console.WriteLine("Float Type --------------");
                    floatprms[floatCount] = (float)parameters[i];
                    floatCount++;
                    _types[i] = floatprms.GetType();
                    //   UnityEngine.Debug.Log(_tankParams.x);
                }


            }

            if (floatprms[0]!=null) {
                _floatParams = new float?[floatCount];
                Array.Copy(floatprms, _floatParams, floatCount);
                Console.WriteLine(_floatParams.Length);
             //   Console.WriteLine("Float values {0} {1} {2}",_floatParams[0],_floatParams[1],_floatParams[2]);
               // Console.WriteLine(_floatParams.Length);
            }
            if (stringprms[0]!=null) {
                _strParams = new string[stringCount];
                Array.Copy(stringprms, _strParams, stringCount);
                Console.WriteLine(_strParams.Length);
              //  Console.WriteLine("String values {0} {1} {2}", _strParams[0], _strParams[1], _strParams[2]);

            }
            if (intprms[0]!=null) {
                _intParams = new int?[intCount];
                Array.Copy(intprms,_intParams,intCount);
            }
            if (boolprms[0] != null) {
                _boolParams = new bool?[boolCount];
                Array.Copy(boolprms, _boolParams, boolCount);
            }



        }
        object[] ResortParams() {//sort parametrs by Type for new object array
            //int intCount=0;
            //int floatCount=0;
            //int stringCount=0;
            //int boolCount=0;
            int totalElements = 0;
            if (_types != null) {
                object[] prms = new object[_types.Length];
                object[] tempprms = new object[_types.Length];
                //object[] prms1 = new object[] { new int[5],new float[2]};

                for (int i = 0; i < _types.Length; ++i) {
                    //  UnityEngine.Debug.Log(i);
                    for (int j = 0; j < this.GetType().GetFields().Length; ++j) {

                        if (this.GetType().GetFields()[j].FieldType == _types[i]) {
                            prms[i] = this.GetType().GetFields()[j].GetValue(this);
                           // Console.WriteLine(this.GetType().GetFields()[j].FieldType);
                            //  Array tmpArray = this.GetType().GetFields()[j].GetValue(this) as Array;
                            //  tempprms = this.GetType().GetFields()[j].GetValue(this) as ICollection<float>;
                            // Array.Copy(tmpArray,tempprms,tmpArray.Length);
                            // UnityEngine.Debug.Log(prms[i].GetType());
                        }
                        if (this.GetType().GetFields()[j].FieldType == _types[i]) {
                            if (this.GetType().GetFields()[j].GetValue(this) != null) {
                              //  Console.WriteLine("FloatCheck111111111111111111111111111111");
                                //  prms[i] = this.GetType().GetFields()[j].GetValue(this);
                                // Console.WriteLine(this.GetType().GetFields()[j].GetValue(this));
                                Array tmpArray = this.GetType().GetFields()[j].GetValue(this) as Array;
                               // Console.WriteLine("Длинна масива типов " + _types.Length);
                               // Console.WriteLine("Длинна временного массива " + tmpArray.Length);
                              //  Console.WriteLine("Длинна конечного массива " + tempprms.Length);
                                //Console.WriteLine(totalElements);
                                // Console.WriteLine(tmpArray.GetType());
                                //  tempprms = this.GetType().GetFields()[j].GetValue(this) as ICollection<float>;
                                Array.Copy(tmpArray, 0, tempprms, totalElements, tmpArray.Length);
                                // Console.WriteLine("{0} {1} {2}", tempprms[0], tempprms[1], tempprms[2]);
                                totalElements += tmpArray.Length;
                                i = totalElements - 1;
                                // Console.WriteLine(tempprms[1].GetType());
                              //  Console.WriteLine("FloatCheck222222222222222222222222222");
                                // UnityEngine.Debug.Log(prms[i].GetType());
                            }
                        }

                    }
                }

                return tempprms as Object[];
            }
            else {
                return null;
            }
        }

        public object[] GetParams() {
            return ResortParams();
        }



    }
    [System.Serializable]
    [ProtoContract]
    public class TankData {
        [ProtoMember(1)]
        public float x;
        [ProtoMember(2)]
        public float y;
        [ProtoMember(3)]
        public float z;

        public int _level;

        public string _id;
        public string _name;
        //[ProtoMember(4)]
        //public string str;
        //[ProtoMember(5)]
        //public int? a = null;
        public TankData() {

        }
        //public  void ConvertPosition(Vector3 pos) {
        //    x = pos.x;
        //    y = pos.y;
        //    z = pos.z;
        //}

    }
}
