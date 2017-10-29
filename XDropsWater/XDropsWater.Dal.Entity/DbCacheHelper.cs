using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace XDropsWater.Dal.Entity
{
    public class DbCacheHelper
    {  //your entities might be in another assembly than the db context. The assembly containing your entities
        //should be used for checking against obsoletion
        private static readonly Assembly domainAssembly = typeof(XDropsWater.Dal.Entity.UserEntity).Assembly;
        private static string edmxCacheFile;
        private static string viewCacheFile;

        public static string EdmxCacheFile = edmxCacheFile ?? (edmxCacheFile = GetPath(typeof(SimpleWebUnitOfWork).FullName + ".edmx"));
        public static string ViewCacheFile = viewCacheFile ?? (viewCacheFile = GetPath(typeof(SimpleWebUnitOfWork).Name + ".xml"));

        private static string GetPath(string filename)
        {
            //try to find file that was deployed in the same folder as the assembly. Shouldn't be outdated though.
            string deployedFile = Path.Combine(Path.GetDirectoryName(domainAssembly.Location), filename);
            if (File.Exists(deployedFile) && !IsObsolete(deployedFile))
                return deployedFile;

            //no up do date file deployed -> use default path (AppData).
            string version = domainAssembly.GetName().Version.ToString();
            //string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            //                           @"ShuiJi\DevDiary\EFCache\{version}",
            //                           filename);
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                       @"EFCache",
                                       filename);

            //Delete file it if it is obsolete. It will be recreated then.
            if (File.Exists(path) && IsObsolete(path))
                File.Delete(path);

            return path;
        }
        private static bool IsObsolete(string path)
        {
            DateTime cacheWriteTime = File.GetLastWriteTimeUtc(path);
            DateTime assemblyWriteTime = File.GetLastWriteTimeUtc(domainAssembly.Location);
            return assemblyWriteTime > cacheWriteTime;
        }
    }
}
