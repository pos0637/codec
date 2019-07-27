using System;

namespace Common
{
    /// <summary>
    /// 定义错误码
    /// </summary>
    public class ARESULT
    {
        public static ARESULT S_OK = new ARESULT(0);
        public static ARESULT E_FAIL = new ARESULT(-1);
        public static ARESULT E_TERMINATED = new ARESULT(-2);
        public static ARESULT E_OUTOFMEMORY = new ARESULT(-3);
        public static ARESULT E_INVALIDARG = new ARESULT(-4);
        public static ARESULT E_TIMEOUT = new ARESULT(-5);
        public static ARESULT E_NOIMPL = new ARESULT(-6);
        public static ARESULT E_ALREADY_EXISTS = new ARESULT(-7);
        public static ARESULT E_INVALID_OPERATION = new ARESULT(-8);

        public ARESULT(Int32 code)
        {
            mCode = code;
        }

        private ARESULT()
        {
        }

        public static Boolean ASUCCEEDED(ARESULT value)
        {
            return (value.Equals(S_OK));
        }

        public static Boolean AFAILED(ARESULT value)
        {
            return (!value.Equals(S_OK));
        }

        private Int32 mCode;
    }
}