using igs_backend.Data;
using igs_backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;

using static igs_backend.Code.Utility;

namespace igs_backend.Code
{

    public static class Utility
    {

        /// <summary>
        /// 登入Enum
        /// </summary>
        public enum LoginCase
        {
            /// <summary>
            /// 未登入
            /// </summary>
            /// <remarks></remarks>              
            NotIn,
            /// <summary> 
            /// 登入成功
            /// </summary>
            OK,
            /// <summary> 
            ///登入失敗
            ///</summary>
            Error,
            /// <summary> 
            /// 停用
            /// </summary>
            Stop,
            /// <summary>
            /// 暫停
            /// </summary>
            Pause,
            /// <summary>
            /// 刪除
            /// </summary>
            Delete,
            /// <summary> 
            /// 未啟用
            /// </summary>
            NotActive,
            /// <summary>
            /// 重複登入
            /// </summary>
            /// <remarks></remarks>
            Double
        }

        /// <summary>
        /// 確認輸入格式是否吻合信箱
        /// </summary>
        /// <param name="InputStr"></param>
        /// <returns></returns>
        public static bool CheckEmailFormat(string InputStr)
        {
            return RegexIsMatch(InputStr, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        /// <summary>
        /// 正規表示輸入格式是否吻合
        /// </summary>
        /// <param name="str">輸入字串</param>
        /// <param name="regexStr">判斷格式</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool RegexIsMatch(string str, string regexStr)
        {
            var RegexContents = new Regex(regexStr, RegexOptions.Singleline);
            return RegexContents.IsMatch(str);
        }

        /// <summary>
        /// 加密字串
        /// </summary>
        /// <param name="Type">加密形式 MD5 SHA1 SHA256 SHA512</param>
        /// <param name="Str">字串</param>
        /// <returns></returns>
        public static string Encryption(string Str, string Type = "")
        {
            byte[] source = Encoding.UTF8.GetBytes(Str); // 將字串轉為Byte[]
            byte[] crypto;
            switch (Type)
            {
                case "MD5":
                    {
                        var md5 = new MD5CryptoServiceProvider();
                        crypto = md5.ComputeHash(source);
                        break;
                    }

                case "SHA1":
                    {
                        var sha1 = new SHA1CryptoServiceProvider();
                        crypto = sha1.ComputeHash(source);
                        break;
                    }

                case "SHA256":
                    {
                        var sha256 = new SHA256CryptoServiceProvider();
                        crypto = sha256.ComputeHash(source);
                        break;
                    }
                default:
                    {
                        var sha512 = new SHA512CryptoServiceProvider();
                        crypto = sha512.ComputeHash(source);
                        break;
                    }
            }

            return Bytes2Hex(crypto);
        }

        /// <summary>
        /// byte[] 轉 Hex (大寫)
        /// </summary>
        /// <param name="bytes">byte[]</param>
        /// <returns>Hex String</returns>
        public static string Bytes2Hex(byte[] bytes)
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                var str = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    str.Append(bytes[i].ToString("x2"));
                }
                hexString = str.ToString();
            }
            return hexString.ToUpper();
        }

        /// <summary>
        /// Hex 轉 byte[]
        /// </summary>
        /// <param name="newString">Hex String</param>
        /// <returns>byte[]</returns>
        public static byte[] Hex2Bytes(string HexString)
        {
            int byteLength = HexString.Length / 2;
            var bytes = new byte[byteLength];
            int j = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = byte.Parse(new string(new char[] { HexString[j], HexString[j + 1] }), NumberStyles.HexNumber);
                j = j + 2;
            }
            return bytes;
        }

        public static string EmptyIfNull(this object value)
        {
            if (value == null) return "";
            return value.ToString();
        }


        ///// <summary>
        ///// 帳號登入
        ///// </summary>
        ///// <param name="user"></param>
        ///// <param name="account">帳號</param>
        ///// <param name="password">密碼</param>
        ///// <returns></returns>
        //public static LoginCase Login(this IPrincipal principal, string Account, string Password)
        //{
        //    var account = EmptyIfNull(Account).Trim();
        //    var password = EmptyIfNull(Password).Trim();

        //    if (account == "" || password == "") return LoginCase.Error;


        //    var user = db.Users.FirstOrDefault(x => x.Account == account);

        //    //驗證帳號
        //    if (user == null)
        //    {
        //        //IPLock.Add("PassCode");//加入鎖定IP;
        //        return LoginCase.Error;
        //    }


        //    //驗證密碼
        //    if (user.Password.EmptyIfNull() != Utility.Encryption(password, "SHA256"))
        //    {
        //        //IPLock.Add("PassCode");//加入鎖定IP;
        //        return LoginCase.Error;
        //    }
        //    return LoginCase.OK;
        //}

        public static byte[] GenerateSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public static string ComputePasswordHash(string password, byte[] salt)
        {
            using (var hmac = new HMACSHA512(salt))
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = hmac.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hash);
            }
        }

    }
}