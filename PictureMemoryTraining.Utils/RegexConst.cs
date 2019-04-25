using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureMemoryTraining.Utils
{
    /// <summary>
    /// 正则表达式常量
    /// </summary>
    public class RegexConst
    {
        /// <summary>
        /// 验证手机号
        /// </summary>
        public const string PhoneNumber = @"^(13[0-9]|14[56789]|15[0-3,5-9]|166|17[0135678]|18[0-9]|19[89])\d{8}$";
        /// <summary>
        /// 禁止非数字
        /// </summary>
        public const string NumbersOnly = "[^0-9]+";

        /// <summary>
        /// 禁止非数字与.
        /// </summary>
        public const string NumbersPointOnly = "[^0-9.]+";

        /// <summary>
        /// 禁止文件名不合法字符\:/*?\"
        /// </summary>
        public const string IllegalCharactes = "[\\\\:/*?\"<>|]+";

        /// <summary>
        /// 英文字母与数字
        /// </summary>
        public const string LettersAndNumbers = "^[a-zA-Z0-9]+$";
    }
}
