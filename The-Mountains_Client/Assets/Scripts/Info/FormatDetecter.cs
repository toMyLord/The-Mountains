using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormatDetecter
{
    public static bool HasSpecialSymbol(string str)
    {
        bool hasSpecialSymbol = false;
        for (int i = 0; i < str.Length; i++)
        {
            char ch = str[i];
            if (!((ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z')))
            {
                hasSpecialSymbol = true;
                break;
            }
        }
        return hasSpecialSymbol;
    }
    public static bool AccountDetect(string str, out string detectInfo)     //账号格式检测
    {
        //本地检测账号格式1.不含空格 2.开头字母 3.长度8~16位 4.不为空 5.只含数字和字母 6.账号是否已存在
        int len = str.Length;
        bool isNotNull = (len > 0);
        //bool hasNotSpace = !(str.Contains(" "));
        bool isLengthCorrect = (len >= 8 && len <= 16);
        //bool isHeadLetter = (len > 0 && char.IsLetter(str[0]));
        bool res = false;
        bool hasSpecialSymbol = HasSpecialSymbol(str);
        string tipStr = "";
        if (!isNotNull)
        {
            tipStr += "账号不为空;";
        }
        /*if (!hasNotSpace)
        {
            tipStr += "账号不含空格;";
        }*/
        if (!isLengthCorrect)
        {
            tipStr += "长度为8-16位;";
        }
        /*if (!isHeadLetter)
        {
            tipStr += "以字母开头;";
        }*/
        if(hasSpecialSymbol)
        {
            tipStr += "不能含特殊符号";
        }
        if (isNotNull /*&& hasNotSpace */&& isLengthCorrect/* && isHeadLetter*/ && !hasSpecialSymbol)
        {
            tipStr = "√";
            res = true;
        }
        else
        {
            res = false;
        }
        detectInfo = tipStr;
        return res;
    }   

    public static bool PasswordDetect(string str, out string detectInfo)    //密码格式检测
    {
        //检测密码格式 1.不含空格2.长度8~16位 3.不为空
        int len = str.Length;
        bool isNotNull = (len > 0);
        bool hasNotSpace = !(str.Contains(" "));
        bool isLengthCorrect = (len >= 8 && len <= 16);
        bool res = false;
        string tipStr = "";
        if (!isNotNull)
        {
            tipStr += "密码不为空;";
        }
        if (!hasNotSpace)
        {
            tipStr += "密码不含空格;";
        }
        if (!isLengthCorrect)
        {
            tipStr += "长度为8-16位;";
        }
        if (isNotNull && hasNotSpace && isLengthCorrect)
        {
            tipStr = "√";
            res = true;
        }
        else
        {
            res = false;
        }
        detectInfo = tipStr;
        return res;
    }

    public static bool EmailDetect(string str, out string detectInfo)       //邮箱格式检测
    {
        //检测邮箱格式
        int len = str.Length;
        int indexOfAt = str.IndexOf("@");
        bool res = false;
        string tipStr = "";
        if (indexOfAt == -1 || indexOfAt == len - 1 || indexOfAt == 0 || indexOfAt != str.LastIndexOf("@"))
        {
            tipStr = "电子邮箱格式错误";
            res = false;
        }
        else
        {
            tipStr = "√";
            res = true;
        }
        detectInfo = tipStr;
        return res;
    }
}
