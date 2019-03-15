using System;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace ALEXFW.CommonUtility
{
    [Serializable]
    public class EmailVerifyToken : ALEXFWToken
    {
        public string Email { get; set; }

        public DateTime ExpiredDate { get; set; }

        public Guid UserId { get; set; }

        public override byte[] GetTokenData()
        {
            return
                UserId.ToByteArray()
                    .Concat(Encoding.UTF8.GetBytes(Email))
                    .Concat(BitConverter.GetBytes(ExpiredDate.ToBinary()))
                    .ToArray();
        }
    }
}