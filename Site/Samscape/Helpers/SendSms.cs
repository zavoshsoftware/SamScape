using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmsIrRestful;

namespace Arako.Helpers
{
    public static class SendSms
    {
        public static void SendOtp(string cellNumber, string code)
        {
            var token = new Token().GetToken("773e6490afdaeccca1206490", "123qwe!@#QWE");


            var ultraFastSend = new UltraFastSend()
            {
                Mobile = Convert.ToInt64(cellNumber),
                TemplateId = 23679,
                ParameterArray = new List<UltraFastParameters>()
                {
                    new UltraFastParameters()
                    {
                        Parameter = "verifyCode" , ParameterValue = code.ToString()
                    }
                }.ToArray()

            };

            UltraFastSendRespone ultraFastSendRespone = new UltraFast().Send(token, ultraFastSend);

            if (ultraFastSendRespone.IsSuccessful)
            {

            }
            else
            {

            }
        }

    }
}