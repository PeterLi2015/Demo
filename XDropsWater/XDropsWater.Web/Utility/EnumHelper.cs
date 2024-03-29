﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace XDropsWater.Web.Utility
{
    public static class EnumHelper<T> 
    {
        public static IList<T> GetValues(T value)
        {
            var enumValues = new List<T>();

            foreach (FieldInfo fi in value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                enumValues.Add((T)Enum.Parse(value.GetType(), fi.Name, false));
            }
            return enumValues;
        }

        public static T Parse(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static IList<string> GetNames()
        {
            return typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name).ToList();
        }

        public static IList<string> GetDisplayValues()
        {
            return GetNames().Select(obj => GetDisplayValue(Parse(obj))).ToList();
        }

        public static string GetDisplayValue(T value)
        {
            var fieldInfo = typeof(T).GetField(value.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(
                typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (descriptionAttributes == null) return string.Empty;
            return (descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : value.ToString();
        }

        public static SelectList GetSelectList()
        {
            Dictionary<object,string> lst = new Dictionary<object,string>();
            foreach (var val in Enum.GetValues(typeof(T)))
            {
                lst.Add(val, GetDisplayValue((T)Enum.Parse(typeof(T), val.ToString())));
            }

            return new SelectList(lst, "Key", "Value");
        }
    }
}