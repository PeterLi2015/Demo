using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Runtime.InteropServices;

namespace XDropsWater.CrossCutting.WinApi
{
    /// <summary>
    /// 域用户密码验证器
    /// </summary>
    public class DomainUserPwdValidator : IDisposable
    {

        public const int LOGON32_LOGON_INTERACTIVE = 2;
        public const int LOGON32_PROVIDER_DEFAULT = 0;

        WindowsImpersonationContext impersonationContext;

        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        public static extern int LogonUser(string lpszUserName,
                                          string lpszDomain,
                                          string lpszPassword,
                                          int dwLogonType,
                                          int dwLogonProvider,
                                          ref IntPtr phToken);
        [DllImport("advapi32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        public extern static int DuplicateToken(IntPtr hToken,
                                          int impersonationLevel,
                                          ref IntPtr hNewToken);
        /**/
        /// <summary>
        /// 输入用户名、密码、登录域判断是否成功
        /// </summary>
        /// <example>
        /// if (impersonateValidUser(UserName, Domain, Password)){}
        /// </example>
        /// <param name="userName">账户名称，如：string UserName = UserNameTextBox.Text;</param>
        /// <param name="domain">要登录的域，如：string Domain   = DomainTextBox.Text;</param>
        /// <param name="password">账户密码, 如：string Password = PasswordTextBox.Text;</param>
        /// <returns>成功返回true,否则返回false</returns>
        public bool Validate(string userName, string domain, string password)
        {
            WindowsIdentity tempWindowsIdentity;
            IntPtr token = IntPtr.Zero;
            IntPtr tokenDuplicate = IntPtr.Zero;

            if (LogonUser(userName, domain, password, LOGON32_LOGON_INTERACTIVE,
            LOGON32_PROVIDER_DEFAULT, ref token) != 0)
            {
                if (DuplicateToken(token, 2, ref tokenDuplicate) != 0)
                {
                    tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
                    impersonationContext = tempWindowsIdentity.Impersonate();
                    if (impersonationContext != null)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (impersonationContext != null)
            {
                try
                {
                    impersonationContext.Undo();
                }
                catch { }
                impersonationContext = null;
            }
        }
    }
}
