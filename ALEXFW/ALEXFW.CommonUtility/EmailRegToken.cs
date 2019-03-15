using System;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace ALEXFW.CommonUtility
{
    public class EmailRegToken : ALEXFWToken
    {
        public string UserName { get; set; }
        public string Email { get; set; }

        public DateTime ExpiredDate { get; set; }

        public override byte[] GetTokenData()
        {
            return Encoding.UTF8.GetBytes(Email).Concat(BitConverter.GetBytes(ExpiredDate.ToBinary())).ToArray();
        }
    }
}