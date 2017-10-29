using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    public enum ErrorCodes
    {
        Successed = 0,
        Exception = 1,
        NotExist = 2,
        Duplicated = 3,
        WrongVerifyCode = 4,
        AccountPasswordNotMatch = 5,
        NotSupported = 6,
        NotAllowed = 7,
        NotValidDateTime = 8,
        HasBeenUsed = 9,
        ParentNotExists = 10,
        IDCardError = 11,
        MemberNotExist = 12
    }
}
