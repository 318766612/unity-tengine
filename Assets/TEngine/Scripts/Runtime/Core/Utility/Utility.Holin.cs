using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;
using UnityEngine;

namespace TEngine.Runtime
{
    /// <summary>
    /// 实用函数集。
    /// </summary>
    public static partial class Utility
    {

        public static class Holin
        {
            /// <summary>
            /// 字符串>>>Base64字符串
            /// </summary>
            /// <param name="data"></param>
            /// <returns></returns>
            public static string StrToBase64(string data)
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
                string temp = System.Convert.ToBase64String(bytes);//转换后的64位字符串
                return temp;
            }

            /// <summary>
            /// Base64>>>字符串
            /// </summary>
            /// <param name="data"></param>
            /// <returns></returns>
            public static string Base64ToStr(string data)
            {
                byte[] bytes = System.Convert.FromBase64String(data);
                string temp = System.Text.Encoding.UTF8.GetString(bytes);//正常信息
                return temp;
            }
            //JavaBase64字符串转换
            public static string JavaBase64(string str)
            {
                byte[] by = Encoding.UTF8.GetBytes(str);
                sbyte[] sby = new sbyte[by.Length];
                for (int i = 0; i < by.Length; i++)
                {
                    if (by[i] > 127)
                        sby[i] = (sbyte)(by[i] - 256);
                    else
                        sby[i] = (sbyte)by[i];
                }
                byte[] newby = (byte[])(object)sby;
                return Convert.ToBase64String(newby);
            }

            //流数据转换为16进制字符串数据

            public static string HexBytesToHexStr(byte[] info)
            {
                System.Text.StringBuilder sbuild = new System.Text.StringBuilder();
                foreach (var item in info)
                {
                    sbuild.AppendFormat("{0:X2} ", item);
                }
                return sbuild.ToString();
            }


            //十六进制字符串转换为流数据
            public static byte[] HexStrToHexByte(string hexString)
            {
                hexString = hexString.Replace(" ", "");
                if ((hexString.Length % 2) != 0)
                    hexString += " ";
                byte[] returnBytes = new byte[hexString.Length / 2];
                for (int i = 0; i < returnBytes.Length; i++)
                    returnBytes[i] = System.Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                return returnBytes;
            }

            /// <summary>
            /// 串口命令，传入数据添加两位crc校验
            /// </summary>
            /// <param name="datas"></param>
            /// <returns></returns>
            public static byte[] GetCRCDatas(byte[] datas)
            {
                int length = datas.Length;
                byte[] crc16 = GetModbusCrc16(datas);
                byte[] crcDatas = new byte[length + 2];
                Array.Copy(datas, crcDatas, length);
                Array.Copy(crc16, 0, crcDatas, length, 2);
                return crcDatas;
            }
            static byte[] GetModbusCrc16(byte[] bytes)
            {
                byte crcRegister_H = 0xFF, crcRegister_L = 0xFF;// 预置一个值为 0xFFFF 的 16 位寄存器
                byte polynomialCode_H = 0xA0, polynomialCode_L = 0x01;// 多项式码 0xA001
                for (int i = 0; i < bytes.Length; i++)
                {
                    crcRegister_L = (byte)(crcRegister_L ^ bytes[i]);
                    for (int j = 0; j < 8; j++)
                    {
                        byte tempCRC_H = crcRegister_H;
                        byte tempCRC_L = crcRegister_L;
                        crcRegister_H = (byte)(crcRegister_H >> 1);
                        crcRegister_L = (byte)(crcRegister_L >> 1);
                        // 高位右移前最后 1 位应该是低位右移后的第 1 位：如果高位最后一位为 1 则低位右移后前面补 1
                        if ((tempCRC_H & 0x01) == 0x01)
                        {
                            crcRegister_L = (byte)(crcRegister_L | 0x80);
                        }

                        if ((tempCRC_L & 0x01) == 0x01)
                        {
                            crcRegister_H = (byte)(crcRegister_H ^ polynomialCode_H);
                            crcRegister_L = (byte)(crcRegister_L ^ polynomialCode_L);
                        }
                    }
                }
                return new byte[] { crcRegister_H, crcRegister_L };
            }

            //获取字符串的md5
            public static string Get_MD5(string data)
            {
                byte[] byteStr = Encoding.UTF8.GetBytes(data);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] md5Byte = md5.ComputeHash(byteStr);
                System.Text.StringBuilder sbuild = new System.Text.StringBuilder();
                foreach (var item in md5Byte)
                {
                    sbuild.AppendFormat("{0:X2}", item);
                }
                return sbuild.ToString();
            }

            //获取文件md5，读取文件全部数据，适合小文件
            public static string Get_File_MD5(string path)
            {
                //string md5 = "";
                FileStream file = new FileStream(path, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();

                //return md5;
            }
            //获取文件md5，读取文件全部数据，适合小文件
            public static string Get_File_MD5Form_Block(string path)
            {
                if (!File.Exists(path)) return "";
                int bufferSize = 1024 * 16;//自定义缓冲区大小16K            
                byte[] buffer = new byte[bufferSize];
                Stream inputStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                HashAlgorithm hashAlgorithm = new MD5CryptoServiceProvider();
                int readLength = 0;//每次读取长度            
                var output = new byte[bufferSize];
                while ((readLength = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    //计算MD5                
                    hashAlgorithm.TransformBlock(buffer, 0, readLength, output, 0);
                }
                //完成最后计算，必须调用(由于上一部循环已经完成所有运算，所以调用此方法时后面的两个参数都为0)            		  
                hashAlgorithm.TransformFinalBlock(buffer, 0, 0);

                string md5 = BitConverter.ToString(hashAlgorithm.Hash);
                md5 = md5.Replace("-", "").ToLower();
                hashAlgorithm.Clear();
                inputStream.Close();
                return md5;
            }

            //异或加密
            public static byte[] XOREncry(byte[] data)
            {
                byte[] dataTemp = new byte[data.Length];
                byte register_key = 0xFF;
                for (int i = 0; i < data.Length; i++)
                {
                    dataTemp[i] = (byte)(data[i] ^ register_key);
                }
                return dataTemp;
            }

            //异或解密
            public static byte[] XORDecry(byte[] data)
            {
                byte[] dataTemp = new byte[data.Length];
                byte register_key = 0xFF;
                for (int i = 0; i < data.Length; i++)
                {
                    dataTemp[i] = (byte)(data[i] ^ register_key);
                }
                return dataTemp;
            }

            static byte[] keyByte;
            public static byte[] KeyByte
            {
                get
                {
                    if (keyByte == null)
                    {
                        //keyByte = File.ReadAllBytes(Application.dataPath + "/power.dat");
                    }
                    return keyByte;
                }
            }

            //AES加密
            public static byte[] AESEncry(byte[] data)
            {
                RijndaelManaged aesDe = new RijndaelManaged();
                aesDe.Key = KeyByte;
                aesDe.IV = KeyByte;
                aesDe.Mode = CipherMode.CFB;
                aesDe.Padding = PaddingMode.ISO10126;

                ICryptoTransform cryDe = aesDe.CreateEncryptor();
                byte[] resultByteDe = cryDe.TransformFinalBlock(data, 0, data.Length);
                return resultByteDe;
            }

            //AES加密
            public static byte[] AESDecry(byte[] data)
            {
                RijndaelManaged aesDe = new RijndaelManaged();
                aesDe.Key = KeyByte;
                aesDe.IV = KeyByte;
                aesDe.Mode = CipherMode.CFB;
                aesDe.Padding = PaddingMode.ISO10126;

                ICryptoTransform cryDe = aesDe.CreateDecryptor();
                byte[] resultByteDe = cryDe.TransformFinalBlock(data, 0, data.Length);
                return resultByteDe;
            }


            //get computer internet wake up magic byte 
            public static byte[] GetMagicByte(string mac)
            {
                string head = "FFFFFFFFFFFF";
                for (int i = 0; i < 16; i++)
                {
                    head = head + mac;
                }
                byte[] buffer = new byte[102];
                for (int i = 0; i < 102; i++)
                {
                    buffer[i] = Convert.ToByte(head.Substring(i * 2, 2), 16);
                }
                return buffer;
            }

            public static int EnergyData(string code)
            {
                string[] codes = code.Split(' ');
                string codeData = codes[3] + codes[4];
                //Debug.Log(codeData);
                return Convert.ToInt32(codeData, 16);
            }


            public static bool CheckPhoneNum(string phoneNumber)
            {
                if (string.IsNullOrEmpty(phoneNumber)) return false;

                if (phoneNumber.Length != 11) return false;

                return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^[1]+[3,4,5,7,8,9]+\d{9}");
            }
        }
    }
}