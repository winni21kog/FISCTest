using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOCASPay
{
    class Program
    {
        // 0僅授權成功:T12341007,T12341012,T12341013,T12341018,T12341019,T12341020
        // 1已經轉入請款檔:T12341006,T12341008,T12341009,T12341010,T12341014,T12341015,T12341016,T12341023
        // 3取消訂單:T12341011
        // 9已請款結帳:T12341021,T12341022
        // X授權失敗(timeout)可再次刷卡:T12341017
        readonly static string orderNo = "T12341023";

        static void Main(string[] args)
        {
            // 訂單編號不可重複
            // 查詢結果 0表示僅授權成功 1表示已經轉入請款檔 2表已轉出請款檔 3表取消訂單 4表已退貨成功 5表已取消退貨 8表已退貨結帳 9表已請款結帳 X表授權失敗訂單 空值表訂單號碼不存在
            var code = CreditQuery();

            if (code == "" || code == "X")
            {
                // 空值,X表授權失敗同訂單編號可再次刷卡
                CreditAPIAuth();
            }
            else if (code == "1")
            {
                // 查詢結果0,1,3,9再次刷卡會回覆訂單號碼不可重複
                CreditAPIAuth();
            }

            // 泰安不會有取消授權
            //CreditCaptureRe();

        }

        /// <summary>
        /// 查詢
        /// </summary>
        private static string CreditQuery()
        {
            // 建立FOCASPayRequest類別
            FOCASPayRequest Focas = new FOCASPayRequest();
            // 設置request參數
            Focas.SetMerConfigName("009033367049001.conf");
            Focas.SetSerConfigName("HiServer.conf");
            Focas.SetTransType("16");
            Focas.SetMerchantID("009033367049001");
            Focas.SetLidm(orderNo);

            //執行交易
            Focas.transaction();

            // 取得回傳之代碼 00,05,'':處理完成 6105:訂單號碼不存在
            string errCode = Focas.GetErrcode();
            // 取得回傳之說明
            string errDesc = Focas.GetErrDesc();
            // 該筆訂單在FOCAS帳務系統的狀態:
            // 0表示僅授權成功 1表示已經轉入請款檔 2表已轉出請款檔 3表取消訂單 4表已退貨成功 5表已取消退貨 8表已退貨結帳 9表已請款結帳 X表授權失敗訂單 空值表訂單號碼不存在
            // 取得帳戶系統的狀態
            string status = Focas.GetTxstate();
            return status;
        }

        /// <summary>
        /// 直接授權一般交易
        /// </summary>
        private static void CreditAPIAuth()
        {
            // 建立FOCASPayRequest類別
            FOCASPayRequest Focas = new FOCASPayRequest();
            // 設置request參數
            Focas.SetMerConfigName("009033367049001.conf");
            Focas.SetSerConfigName("HiServer.conf");
            Focas.SetMerchantID("009033367049001");
            Focas.SetMerID("03336704");
            Focas.SetTerminalID("90011801");
            // 特店名
            Focas.SetMerchantName("TEST");
            // 預設000
            Focas.SetCurrencyNote("000");
            // 預設000
            Focas.SetExtend("000");
            // 自動轉入請款檔 預設1
            Focas.SetAutoCap("1");
            // 1為授權交易
            Focas.SetTransType("1");
            // 訂單編號
            Focas.SetLidm(orderNo);
            // 金額
            Focas.SetPurchAmt("1980");
            // 卡號
            Focas.SetCardNo("4907060600015101");//4907060600015101
            // 到期日YYMM
            Focas.SetExpiry("2012");//2012
            // 末三碼
            Focas.SetCvv("615");//615

            //執行交易
            Focas.transaction();

            // 取得回傳之代碼
            // 0000:處理完成
            // 05:交易失敗，請洽詢發卡銀行->卡號錯誤使用查詢會回覆X授權失敗
            // 6101:特店端末不存在或尚未啟動 6128:timeout 6406:查無商店公鑰
            // 6104:訂單號碼重複
            // 6106:需帶有cvv2
            // 6903:cardNo為必填參數或長度太長,expiry為必填參數且長度需為4且需為數字,cvv需為數字且長度需為3,autoCap為自動轉入請款，purchAmt必須大於0
            string errCode = Focas.GetErrcode();
            // 取得回傳之說明
            string errDesc = Focas.GetErrDesc();
        }

        /// <summary>
        /// 取消授權
        /// </summary>
        private static void CreditCaptureRe()
        {
            // 建立FOCASPayRequest類別
            FOCASPayRequest Focas = new FOCASPayRequest();
            // 設置request參數
            Focas.SetMerConfigName("009033367049001.conf");
            Focas.SetSerConfigName("HiServer.conf");
            Focas.SetTransType("5");
            Focas.SetMerchantID("009033367049001");
            Focas.SetLidm(orderNo);

            //執行交易
            Focas.transaction();

            // 取得回傳之代碼
            string errCode = Focas.GetErrcode();
            // 取得回傳之說明
            string errDesc = Focas.GetErrDesc();
            // 狀態
            string status = Focas.GetStatus();
        }


    }
}
