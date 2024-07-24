
namespace GCR.Commons
{
    /// <summary>
    /// 加解密方法
    /// </summary>
    public static class EncryptDES
    {
        private static string _key = "MYSFEDES";

        public static string key
        {
            get { return _key; }
            set { _key = value; }
        }

        /// <summary>
        /// 加密算法
        /// </summary>
        /// <param name="encryptString"></param>
        /// <returns></returns>
        public static string strEncryptToDES(string encryptString)
        {
            StringBuilder ret = new StringBuilder();

            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.Default.GetBytes(encryptString);
                des.Key = ASCIIEncoding.ASCII.GetBytes(key);
                des.IV = ASCIIEncoding.ASCII.GetBytes(key);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                foreach (byte b in ms.ToArray())
                {
                    ret.AppendFormat("{0:X2}", b);
                }
                ret.ToString();
            }
            catch
            {
            }
            return ret.ToString();
        }

        /// <summary>
        /// 解密算法
        /// </summary>
        /// <param name="pToDecrypt">加密后</param>
        /// <returns></returns>
        public static string strEncryptByDES(string pToDecrypt)
        {
            MemoryStream ms = new MemoryStream();

            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
                for (int x = 0; x < pToDecrypt.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }
                des.Key = ASCIIEncoding.ASCII.GetBytes(key);
                des.IV = ASCIIEncoding.ASCII.GetBytes(key);

                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                StringBuilder ret = new StringBuilder();
            }
            catch
            {
            }

            return System.Text.Encoding.Default.GetString(ms.ToArray());
        }
    }
}
