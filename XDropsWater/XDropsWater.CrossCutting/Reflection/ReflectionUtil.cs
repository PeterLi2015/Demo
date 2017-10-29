using XDropsWater.CrossCutting.IO;
using XDropsWater.CrossCutting.String;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XDropsWater.CrossCutting.Reflection
{
    /// <summary>
    /// 反射相关函数
    /// </summary>
    public class ReflectionUtil
    {
        /// <summary>
        /// 是否实现指定接口
        /// </summary>
        /// <param name="checkType">查找类型</param>
        /// <param name="interfaceType">接口类型</param>
        /// <returns></returns>
        public static bool HasInterface(Type checkType, Type interfaceType)
        {
            var findInterfaceType = checkType.GetInterface(interfaceType.Name);
            return findInterfaceType != null && findInterfaceType == interfaceType;
        }

        /// <summary>
        /// 查找指定类型是否继承指定基类 containBaseType是否包含接口本身 默认true
        /// </summary>
        /// <param name="checkType">查找类型</param>
        /// <param name="baseType">基类类型</param>
        /// <returns></returns>
        public static bool IsNestFromBaseType(Type checkType, Type baseType)
        {
            if (checkType == null) return false;
            if (checkType == typeof(Object)) return false;
            if (checkType == baseType)
            {
                return true;
            }
            return IsNestFromBaseType(checkType.BaseType, baseType);
        }

        /// <summary>
        /// 判断类型是否可空类型
        /// </summary>
        /// <param name="theType"></param>
        /// <returns></returns>
        public static bool IsNullableType(Type theType)
        {
            return Nullable.GetUnderlyingType(theType) != null;
            //return (theType.IsGenericType && theType.
            //  GetGenericTypeDefinition().Equals
            //  (typeof(Nullable<>)));
        }

        /// <summary>
        /// 根据可空值类型获得对应的值类型
        /// </summary>
        /// <param name="nullableType"></param>
        /// <returns></returns>
        public static Type GetUnNullableType(Type nullableType)
        {
            if (!IsNullableType(nullableType)) return nullableType;
            //return nullableType.GetGenericArguments()[0];
            return Nullable.GetUnderlyingType(nullableType);
        }

        #region 枚举相关功能
        /// <summary>
        /// 获取枚举相关信息 表：EnumName枚举名，EnumValue枚举值，EnumDes枚举描述
        /// </summary>
        /// <typeparam name="TEnumValue">枚举值类型</typeparam>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static DataTable GetEnumInfo(Type enumType)
        {
            var dtInfo = new DataTable();
            dtInfo.Columns.Add("EnumName", typeof(string));
            dtInfo.Columns.Add("EnumValue", typeof(object));
            dtInfo.Columns.Add("EnumDes", typeof(string));

            FieldInfo[] fields = enumType.GetFields();
            for (int i = 1, count = fields.Length; i < count; i++)
            {
                FieldInfo field = fields[i];

                //文本列赋值
                string name = field.Name;
                //值列
                object value = System.Enum.Parse(enumType, field.Name);
                string des = string.Empty;

                object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (objs == null || objs.Length == 0)
                {
                    des = field.Name;
                }
                else
                {
                    DescriptionAttribute da = (DescriptionAttribute)objs[0];
                    des = da.Description;
                }

                dtInfo.Rows.Add(name, value, des);
            }
            dtInfo.AcceptChanges();
            return dtInfo;
        }

        /// <summary>
        /// 根据枚举显示值获取枚举描述
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <param name="e">枚举值</param>
        /// <returns></returns>
        public static string GetEnumDes<TEnum>(TEnum e)
        {
            string name = e.ToString();
            FieldInfo fieldinfo = e.GetType().GetField(name);
            if (fieldinfo != null)
            {
                Object[] objs = fieldinfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (objs == null || objs.Length == 0)
                {
                    return name;
                }
                else
                {
                    DescriptionAttribute da = (DescriptionAttribute)objs[0];
                    return da.Description;
                }
            }
            else
            {
                return "未定义枚举" + e;
            }
        }

        /// <summary>
        /// 根据枚举值获取枚举描述
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetEnumDesByValue<TEnum>(object e)
        {
            return GetEnumDes((TEnum)e);
        }
        #endregion

        /// <summary>
        /// 根据所有程序集获取类型 
        /// dllFullNameFilter 如果被过滤返回 false
        /// </summary>
        public static Type GetTypeByDll(
            string typeFullName, Assembly dll = null, Func<AssemblyName, bool> dllFullNameFilter = null, HashSet<string> notCheckDllFullNameHS = null, bool throwOnError = true)
        {
            if (dll == null)
            {
                dll = Assembly.GetCallingAssembly();
            }
            if (notCheckDllFullNameHS == null)
            {
                notCheckDllFullNameHS = new HashSet<string>();
            }
            if (dllFullNameFilter != null)
            {
                if (!dllFullNameFilter(dll.GetName())) return null;
            }
            var retType = dll.GetType(typeFullName);
            if (retType != null)
            {
                return retType;
            }
            //已扫描过 就过滤，防止重复扫描
            if (!notCheckDllFullNameHS.Contains(dll.FullName))
            {
                notCheckDllFullNameHS.Add(dll.FullName);
            }
            foreach (var dllItem in dll.GetReferencedAssemblies())
            {
                try
                {
                    if (notCheckDllFullNameHS.Contains(dllItem.FullName)) continue;
                    if (dllFullNameFilter != null)
                    {
                        if (!dllFullNameFilter(dllItem)) continue;
                    }
                    retType = GetTypeByDll(typeFullName, Assembly.Load(dllItem), dllFullNameFilter, notCheckDllFullNameHS, throwOnError);
                    if (retType != null)
                    {
                        return retType;
                    }
                    if (!notCheckDllFullNameHS.Contains(dllItem.FullName))
                    {
                        notCheckDllFullNameHS.Add(dllItem.FullName);
                    }
                }
                catch (Exception ex)
                {
                    if (throwOnError) throw ex;
                }
            }
            return null;
        }

        /// <summary>
        /// 扫描类型
        /// dllFullNameFilter 如果被过滤返回 false
        /// </summary>
        public static void ScanTypeByDll(Action<Type> scanFunc, Assembly dll = null, Func<AssemblyName, bool> dllFullNameFilter = null, HashSet<string> notCheckDllFullNameHS = null)
        {
            if (dll == null)
            {
                dll = Assembly.GetCallingAssembly();
            }
            if (notCheckDllFullNameHS == null)
            {
                notCheckDllFullNameHS = new HashSet<string>();
            }
            if (dllFullNameFilter != null)
            {
                if (!dllFullNameFilter(dll.GetName())) return;
            }
            foreach (var t in dll.GetTypes())
            {
                scanFunc(t);
            }
            //已扫描过 就过滤，防止重复扫描
            if (!notCheckDllFullNameHS.Contains(dll.FullName))
            {
                notCheckDllFullNameHS.Add(dll.FullName);
            }
            foreach (var dllItem in dll.GetReferencedAssemblies())
            {
                if (notCheckDllFullNameHS.Contains(dllItem.FullName)) continue;
                if (dllFullNameFilter != null)
                {
                    if (!dllFullNameFilter(dllItem)) continue;
                }
                var childDll = Assembly.Load(dllItem);
                ScanTypeByDll(scanFunc, childDll, dllFullNameFilter, notCheckDllFullNameHS);
            }
        }

        /// <summary>
        /// 判断dll是否属于微软
        /// </summary>
        /// <param name="dllPath"></param>
        /// <returns></returns>
        public static bool IsMicroSoftDll(Assembly dll)
        {
            if (dll == null) return false;
            var dllPath = FileUtil.GetAssemblyPath(dll);
            if (FilterUtil.IsEmptyWithTrim(ref dllPath)) return false;
            return FileUtil.IsMicroSoftDll(dllPath);
        }
    }
}
