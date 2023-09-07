
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using XLua;

public static class XLuaSettings
{
    public static List<Type> BlackGenericTypeList = new List<Type>()
    {
        typeof(Span<>),
        typeof(ReadOnlySpan<>),
        typeof(UnityEngine.Light),
    };

    private static bool IsBlacklistedGenericType(Type type)
    {
        if (!type.IsGenericType) return false;
        return BlackGenericTypeList.Contains(type.GetGenericTypeDefinition());
    }

    [BlackList] public static Func<MemberInfo, bool> GenericTypeFilter = (memberInfo) =>
    {
        switch (memberInfo)
        {
            case PropertyInfo propertyInfo:
                return IsBlacklistedGenericType(propertyInfo.PropertyType);

            case ConstructorInfo constructorInfo:
                return constructorInfo.GetParameters().Any(p => IsBlacklistedGenericType(p.ParameterType));

            case MethodInfo methodInfo:
                if(memberInfo.DeclaringType == typeof(UnityEngine.Light))
                {
                    return true;
                }
                return methodInfo.GetParameters().Any(p => IsBlacklistedGenericType(p.ParameterType));
            
            default:
                return false;
        }
    };
    [LuaCallCSharp]
    public static List<Type> LuaCallCSharp = new List<Type>()
    {
        typeof(GameObject),
        typeof(Vector3),
        typeof(DG.Tweening.ShortcutExtensions), // DOTween 的扩展方法
        typeof(DG.Tweening.TweenSettingsExtensions), // DOTween 设置
        // 可能还需要其他相关的类型
    };
}