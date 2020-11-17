using FISCTest.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FISCTest
{
    class Program
    {
        static readonly string testUrl = "https://www.focas-test.fisc.com.tw/FOCAS_WS/API20/V1/FISCII/";
        static readonly string auth = "auth2525";
        static readonly string verify = "acctVerify";

        static void Main(string[] args)
        {
            CreditVerify();
            //Auth();
        }

        /// <summary>
        /// 持卡人身分驗證
        /// </summary>
        private static void CreditVerify()
        {
            var url = testUrl + verify;
            var requestXml = "";


            #region API文件測試資料
            //var otherInfo = new OtherInfo
            //{
            //    tag90 = "AB41E66700D097C5C09E22A15AB09243",
            //    tag91 = "03",
            //    tag92 = "BD5A109098FEA61B"
            //};
            //var model = new CreditVerifyReq
            //{
            //    mti = "0100",
            //    cardNumber = "4907060600015101",
            //    processingCode = "003001",
            //    amt = "000000000000",
            //    traceNumber = "191025",
            //    localTime = "191025",
            //    localDate = "20180508",
            //    posEntryMode = "812",
            //    expiredDate = "60101674DE3FAD13",
            //    acqBank = "006",
            //    terminalId = "00010002",
            //    merchantId = "006263015610001",
            //    otherInfo = JsonConvert.SerializeObject(otherInfo),
            //};
            //model.verifyCode = GetVerifyCode(model.acqBank, model.localDate, model.localTime, model.merchantId, model.terminalId, model.mti, model.processingCode);

            // 文件p.51範例
            var testCodeP51 = GetVerifyCode("943", "20170314", "031410", "943000000000001", "00010001", "0800", "000000", "112233445566");
            var testCodeP33 = GetVerifyCode("006", "20180508", "191025", "006263015610001", "00010002", "0100", "003001");
            #endregion

            var otherInfo = new CreditVerifyOtherInfo
            {
                // 加密方式加入Padding = PaddingMode.None則與API文件結果相符
                // 舊格式2021/06/30停止支援，須改用新格式
                tag90 = GetTag90Old("113005", "A123456789"),
                // 新格式2020/12/31才啟用
                //tag90 = GetTag90("113005", "A123456789", "0900111111", "20991111"),
                tag91 = "03",
                // 加密方式加入Padding = PaddingMode.None則與API文件結果相符
                tag92 = GetTag92("113005", "123")
            };
            // test 解密
            var tag92d = GetTag92Decryptor(otherInfo.tag92);
            var model = new CreditVerifyReq
            {
                // 查詢res 0100
                mti = "0100",
                cardNumber = "4907060600015101",
                // 查詢:003001
                processingCode = "003001",
                // 固定000000000000
                amt = "000000000000",
                // 交易識別碼(訂單編號)
                traceNumber = "191025",
                localTime = "191025",
                localDate = "20180508",
                // 設備輸入型態 預設812
                posEntryMode = "812",
                // 加密方式加入Padding = PaddingMode.None則與API文件結果相符
                expiredDate = GetExpiredDate("113005", "2318"),
                acqBank = "006",
                terminalId = "00010002",
                merchantId = "006263015610001",
                otherInfo = JsonConvert.SerializeObject(otherInfo),
            };
            model.verifyCode = GetVerifyCode(model.acqBank, model.localDate, model.localTime, model.merchantId, model.terminalId, model.mti, model.processingCode);


            using (var stringwriter = new Utf8StringWriter())
            {
                var serializer = new XmlSerializer(typeof(CreditVerifyReq));
                var xmlNS = new XmlSerializerNamespaces();

                // 加入下行會刪除NameSpance xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"
                //xmlNS.Add("", "http://www.focas.fisc.com.tw/FiscII/auth2525");
                //serializer.Serialize(stringwriter, model, xmlNS);

                serializer.Serialize(stringwriter, model);
                requestXml = stringwriter.ToString();
            }

            var response = PostXMLData(url, requestXml);
            var result = LoadFromXMLString<CreditVerifyResp>(response);
        }

        private static void Auth()
        {
            var url = testUrl + auth;
            var requestXml = "";
            var otherInfo = new AuthOtherInfo
            {
                //input data：端末交易序號6碼+特店代號15碼+交易日期8碼+端末代號8碼+交易時間6碼+收單行代碼3碼+固定值00。(共48 bytes)
                // 與API文件結果不符:49AF9C6A7F92C2E55A5E02E0AECA35B81DB0D82D2C2950C794F1440E97027F1E02638A6B3141BB37909FE832E89057B2 !=
                // 49AF9C6A7F92C2E55A5E02E0AECA35B81DB0D82D2C2950C794F1440E97027F1E02638A6B3141BB37909FE832E89057B24975AEC2E8D2608B
                tag05 = GetTag05("715304008PC6802620001201709126802620119214400800"),
                tag12 = "629",
                tag80 = "B"
            };

            var model = new FiscIIAuth2525Req
            {
                mti = "0200",
                cardNumber = "0006147854124258",
                processingCode = "002525",
                amt = "000007434000",
                traceNumber = "185830",
                localTime = "185830",
                localDate = "20180508",
                countryCode = "158",
                posEntryMode = "071",
                posConditionCode = "77",
                acqBank = "006",
                terminalId = "00010002",
                merchantId = "006263015610001",
                orderNumber = "P201712310000003000",
                otherInfo = JsonConvert.SerializeObject(otherInfo),
                txnCurrencyCode = "901",
                chipData = "MjUyNTQ2MjAwMDAwMDAwMDAwMDAwMDExMTExMTExMTEyMjIyMjIyMjIyMDAwNjE0Nzg1NDEyNDI1OAgwMDAwMDAwMQAIMTIzNDU2NzgVNTk5OTAwNjAgICAgICAg",
                verifyCode = "A066AFE7B454D7F5FD60908A93958A17D31E8D899B483979D409C122D78CF7F1",
            };

            using (var stringwriter = new Utf8StringWriter())
            {
                var serializer = new XmlSerializer(typeof(FiscIIAuth2525Req));
                var xmlNS = new XmlSerializerNamespaces();

                // 加入下行會刪除NameSpance xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"
                //xmlNS.Add("", "http://www.focas.fisc.com.tw/FiscII/auth2525");
                //serializer.Serialize(stringwriter, model, xmlNS);

                serializer.Serialize(stringwriter, model);
                requestXml = stringwriter.ToString();
            }

            var response = PostXMLData(url, requestXml);
            var result = LoadFromXMLString<FiscIIAuth2525Resp>(response);
        }

        /// <summary>
        /// 卡片效期加密
        /// </summary>
        /// <param name="time">交易時間HHmmss</param>
        /// <param name="date">卡片效期YYMM</param>
        /// <returns></returns>
        private static string GetExpiredDate(string time, string date)
        {
            // key 由雙方共同約定
            var key = ConfigurationManager.AppSettings["FISCAPIkey"].ToString();
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider
            {
                Key = HexToByte(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.None
            };

            ICryptoTransform desEncrypt = des.CreateEncryptor();
            var hexString = $"{ByteToString(Encoding.UTF8.GetBytes(time))}{date}";
            byte[] buffer = HexToByte(hexString);
            var encrypt = desEncrypt.TransformFinalBlock(buffer, 0, buffer.Length);
            var result = ByteToString(encrypt);

            return result;
        }

        private static string GetVerifyCode(string acqBank, string localData, string localTime, string merchantId, string terminalId, string mti, string processingCode, string srrn = "")
        {
            // key 由雙方共同約定
            var key = ConfigurationManager.AppSettings["FISCAPIkey"].ToString();
            var data = acqBank + localData + localTime + merchantId + mti + processingCode;
            if (!string.IsNullOrEmpty(srrn)) data += srrn;
            data += terminalId;

            // SHA-256加密後轉Hex String
            byte[] messageBytes = Encoding.UTF8.GetBytes(data + key);
            using (SHA256 SHA256 = SHA256.Create())
            {
                byte[] hashValue = SHA256.ComputeHash(messageBytes);
                return ByteToString(hashValue);
            }
        }

        private static string GetChipData(string data)
        {
            return "";
        }

        /// <summary>
        /// 取得端末設備查核碼
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string GetTag05(string data)
        {
            // key 由雙方共同約定
            var key = ConfigurationManager.AppSettings["FISCAPIkey"].ToString();
            // iv值為固定
            var iv = "FOCASAPI";
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider
            {
                Key = HexToByte(key),
                Mode = CipherMode.CBC,
                IV = Encoding.UTF8.GetBytes(iv),
                Padding = PaddingMode.Zeros
            };

            ICryptoTransform desEncrypt = des.CreateEncryptor();
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            var hexString = ByteToString(desEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));
            // 取末4 bytes轉成HexString即為端末設備查核碼
            return hexString.Substring(hexString.Length - 8, 8);
        }

        /// <summary>
        /// Tag05 舊格式
        /// </summary>
        /// <param name="time"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private static string GetTag90Old(string time, string id)
        {
            // key 由雙方共同約定 ex:"5E8B6E1998F421204C6576544FE1A26B44FC775982D8CE2E"
            var key = ConfigurationManager.AppSettings["FISCAPIkey"].ToString();

            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider
            {
                Key = HexToByte(key),
                Mode = CipherMode.ECB,
                // 補0後結果與API文件結果相同
                Padding = PaddingMode.None
            };

            ICryptoTransform desEncrypt = des.CreateEncryptor();
            byte[] buffer = HexToByte($"{ByteToString(Encoding.UTF8.GetBytes(id))}{ByteToString(Encoding.UTF8.GetBytes(time))}");
            var result = ByteToString(desEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));

            return result;
        }

        /// <summary>
        /// 取得tag90加密HexString
        /// </summary>
        /// <param name="time">時間{HHmmss}</param>
        /// <param name="id">身分證</param>
        /// <param name="mobile">手機</param>
        /// <param name="birthday">生日yyyyMMdd</param>
        /// <returns></returns>
        private static string GetTag90(string time, string id, string mobile = "", string birthday = "")
        {
            // key 由雙方共同約定 ex:"5E8B6E1998F421204C6576544FE1A26B44FC775982D8CE2E"
            var key = ConfigurationManager.AppSettings["FISCAPIkey"].ToString();
            // iv值為固定
            var iv = "FOCASAPI";
            // 比對資訊 ex: "01-A123456789,02-0900111111,03-20991111";
            var info = $"01-{id}";
            if (!string.IsNullOrEmpty(mobile))
                info += $",02-{mobile}";
            if (!string.IsNullOrEmpty(birthday))
                info += $",03-{birthday}";

            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider
            {
                Key = HexToByte(key),
                Mode = CipherMode.CBC,
                IV = Encoding.UTF8.GetBytes(iv),
                Padding = PaddingMode.Zeros
            };

            ICryptoTransform desEncrypt = des.CreateEncryptor();
            byte[] buffer = HexToByte($"{ByteToString(Encoding.UTF8.GetBytes(time))}{ByteToString(Encoding.UTF8.GetBytes(info))}");
            var result = ByteToString(desEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));

            return result;
        }

        private static string GetTag92(string time, string cvc)
        {
            // key 由雙方共同約定
            var key = ConfigurationManager.AppSettings["FISCAPIkey"].ToString();
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider
            {
                Key = HexToByte(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.None
            };

            ICryptoTransform desEncrypt = des.CreateEncryptor();
            var hexString = $"{ByteToString(Encoding.UTF8.GetBytes(time))}{cvc}0";
            byte[] buffer = HexToByte(hexString);
            var result = ByteToString(desEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));

            return result;
        }

        #region 解密
        private static string GetTag92Decryptor(string hexString)
        {
            // key 由雙方共同約定
            var key = ConfigurationManager.AppSettings["FISCAPIkey"].ToString();
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider
            {
                Key = HexToByte(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.None
            };

            ICryptoTransform desDecrypt = des.CreateDecryptor();
            //var hexString = $"{ByteArrayToString(Encoding.UTF8.GetBytes(time))}{cvc}0";
            byte[] buffer = HexToByte(hexString);
            var result = ByteToString(desDecrypt.TransformFinalBlock(buffer, 0, buffer.Length));

            return result;
        }
        #endregion

        public static string ByteToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        /// <summary>
        /// Hex String To Byte[]
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] HexToByte(string hex)
        {
            int NumberChars = hex.Length;
            // 運算後的位元組長度:16進位數字字串長/2
            byte[] bytes = new byte[NumberChars / 2];
            // 每2位16進位數字轉換為一個10進位整數
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }

        public static string PostXMLData(string destinationUrl, string requestXml)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);
            byte[] bytes;
            bytes = Encoding.UTF8.GetBytes(requestXml);
            request.ContentType = "application/xml";
            request.ContentLength = bytes.Length;
            request.Method = "POST";
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = response.GetResponseStream();
                string responseStr = new StreamReader(responseStream, Encoding.UTF8).ReadToEnd();
                return responseStr;
            }
            return null;
        }

        public static T LoadFromXMLString<T>(string xmlText)
        {
            using (var stringReader = new StringReader(xmlText))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(stringReader);
            }
        }
    }
}
