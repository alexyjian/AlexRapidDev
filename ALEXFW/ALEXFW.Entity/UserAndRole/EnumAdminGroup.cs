using System;

namespace ALEXFW.Entity.UserAndRole
{
    /// <summary>
    /// 角色定义，Flag特性，取值为2的幂，如：1，2，4，8，16，32 
    /// </summary>
    [Flags]
    public enum AdminGroup
    {
        业务员 = 1,
        店长 = 2,
        管理员 = 4
    }
}