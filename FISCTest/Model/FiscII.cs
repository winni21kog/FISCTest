using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FISCTest.Model
{
    #region 一般授權交易(固定費率)
    /// <summary>
    /// 一般授權交易請求
    /// </summary>
    [XmlRoot(Namespace = "http://www.focas.fisc.com.tw/FiscII/auth2525", IsNullable = false)]
    public class FiscIIAuth2525Req
    {
        public string mti { get; set; }
        public string cardNumber { get; set; }
        public string processingCode { get; set; }
        public string amt { get; set; }
        public string traceNumber { get; set; }
        public string localTime { get; set; }
        public string localDate { get; set; }
        public string countryCode { get; set; }
        public string posEntryMode { get; set; }
        public string posConditionCode { get; set; }
        public string acqBank { get; set; }
        public string terminalId { get; set; }
        public string merchantId { get; set; }
        public string orderNumber { get; set; }
        public string otherInfo { get; set; }
        public string txnCurrencyCode { get; set; }
        public string chipData { get; set; }
        public string verifyCode { get; set; }
    }

    /// <summary>
    /// 一般授權交易回應
    /// </summary>
    [XmlRoot(Namespace = "http://www.focas.fisc.com.tw/FiscII/auth2525", IsNullable = false)]
    public class FiscIIAuth2525Resp
    {
        public string mti { get; set; }
        public string cardNumber { get; set; }
        public string processingCode { get; set; }
        public string amt { get; set; }
        public string systemDateTime { get; set; }
        public string traceNumber { get; set; }
        public string acqBank { get; set; }
        public string srrn { get; set; }
        public string responseCode { get; set; }
        public string terminalId { get; set; }
        public string merchantId { get; set; }
        public string orderNumber { get; set; }
        public string verifyCode { get; set; }
    }
    #endregion

    #region 信用卡持卡人驗證交易
    /// <summary>
    /// 信用卡持卡人驗證請求
    /// </summary>
    [XmlRoot(Namespace = "http://www.focas.fisc.com.tw/FiscII/acctVerify", IsNullable = false)]
    public class CreditVerifyReq
    {
        public string mti { get; set; }

        public string cardNumber { get; set; }

        public string processingCode { get; set; }

        public string amt { get; set; }

        public string traceNumber { get; set; }

        public string localTime { get; set; }

        public string localDate { get; set; }

        public string posEntryMode { get; set; }

        public string expiredDate { get; set; }

        public string acqBank { get; set; }

        public string terminalId { get; set; }

        public string merchantId { get; set; }

        public string otherInfo { get; set; }

        public string verifyCode { get; set; }
    }

    /// <summary>
    /// 信用卡持卡人驗證回應
    /// </summary>
    [XmlRoot(Namespace = "http://www.focas.fisc.com.tw/FiscII/acctVerify", IsNullable = false)]
    public class CreditVerifyResp
    {
        public string mti { get; set; }

        public string cardNumber { get; set; }

        public string processingCode { get; set; }

        public string amt { get; set; }

        public string traceNumber { get; set; }

        public string systemDateTime { get; set; }

        public string acqBank { get; set; }

        public string authCode { get; set; }

        public string srrn { get; set; }

        public string responseCode { get; set; }

        public string terminalId { get; set; }

        public string merchantId { get; set; }

        public string verifyCode { get; set; }
    }
    #endregion

    /// <summary>
    /// 持卡人驗證交易其他資訊 CreditVerify
    /// </summary>
    public class CreditVerifyOtherInfo
    {
        public string tag90 { get; set; }
        public string tag91 { get; set; }
        public string tag92 { get; set; }
    }

    /// <summary>
    /// 授權其他資訊 Auth
    /// </summary>
    public class AuthOtherInfo
    {
        public string tag05 { get; set; }
        public string tag12 { get; set; }
        public string tag80 { get; set; }
    }
}
