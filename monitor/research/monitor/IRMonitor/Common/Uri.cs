using System;
using System.Collections;
using System.Collections.Generic;

namespace Common
{
    /// <summary>
    /// 定义统一资源标识符,适用于设备或资源描述
    /// 系统URI格式:
    /// <scheme name> : <hierarchical part> [ ? <query> ] [ # <fragment> ]
    /// 使用自定义URI格式:
    /// <scheme name> :// <domain> / <request> [ ? <arguments> ]
    /// </summary>
    public class Uri
    {
        private static readonly Char[] ARGUMENTS_SPLITTER = new Char[] { '&', '=' };

        public Uri(String uri)
        {
            mUri = uri;

            if (String.IsNullOrEmpty(mUri))
                return;

            Int32 pos1 = mUri.IndexOf("://");
            if (pos1 < 0)
                return;

            mScheme = mUri.Substring(0, pos1);

            Int32 pos2 = mUri.IndexOf('/', pos1 + 3);
            if (pos2 < 0)
                return;

            mDomain = mUri.Substring(pos1 + 3, pos2 - pos1 - 3);

            Int32 pos3 = mUri.IndexOf('?', pos2 + 1);
            if (pos3 < 0) {
                mRequest = mUri.Substring(pos2 + 1);
            }
            else {
                mRequest = mUri.Substring(pos2 + 1, pos3 - pos2 - 1);
                mArguments = mUri.Substring(pos3 + 1);
            }

            mSpec = mScheme + "://" + mDomain + "/" + mRequest;

            if (mArguments != null) {
                mArgumentTable = new Hashtable();
                String[] list = mArguments.Split(ARGUMENTS_SPLITTER);
                if ((list == null) || ((list.Length & 0x01) != 0))
                    return;

                for (Int32 i = 0; i < list.Length; i += 2) {
                    mArgumentTable.Add(list[i], list[i + 1]);
                }
            }
        }

        /// <summary>
        /// 协议
        /// </summary>
        public String Scheme
        {
            get { return mScheme; }
        }

        /// <summary>
        /// 域
        /// </summary>
        public String Domain
        {
            get { return mDomain; }
        }

        /// <summary>
        /// 请求
        /// </summary>
        public String Request
        {
            get { return mRequest; }
        }

        /// <summary>
        /// 参数
        /// </summary>
        public String Arguments
        {
            get { return mArguments; }
        }

        /// <summary>
        /// 参数
        /// </summary>
        public Hashtable ArgumentTable
        {
            get { return mArgumentTable; }
        }

        /// <summary>
        /// 统一资源标识符
        /// </summary>
        public String Content
        {
            get { return mUri; }
        }

        /// <summary>
        /// URI列表中是否有匹配项
        /// </summary>
        /// <param name="uris">URI列表</param>
        /// <param name="uri">指定URI</param>
        /// <returns>是否有匹配项</returns>
        public static Boolean IsMatch(List<Common.Uri> uris, Common.Uri uri)
        {
            if (uris == null)
                return false;

            return (uris.Find(item => item.mSpec.Equals(uri.mSpec)) != null);
        }

        /// <summary>
        /// URI列表中是否有匹配项
        /// </summary>
        /// <param name="uris">URI列表</param>
        /// <param name="uri">指定URI</param>
        /// <returns>是否有匹配项</returns>
        public static Boolean IsMatch(Common.Uri[] uris, Common.Uri uri)
        {
            if (uris == null)
                return false;

            foreach (Common.Uri item in uris) {
                if (item.mSpec.Equals(uri.mSpec))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 获取URI列表中匹配项索引
        /// </summary>
        /// <param name="uris"></param>
        /// <param name="uri"></param>
        /// <returns>匹配项索引,错误返回-1</returns>
        public static Int32 GetMatchId(List<Common.Uri> uris, Common.Uri uri)
        {
            if (uris == null)
                return -1;

            for (Int32 i = 0; i < uris.Count; ++i) {
                if (uris[i].mSpec.Equals(uri.mSpec))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// 获取URI列表中匹配项索引
        /// </summary>
        /// <param name="uris"></param>
        /// <param name="uri"></param>
        /// <returns>匹配项索引,错误返回-1</returns>
        public static Int32 GetMatchId(Common.Uri[] uris, Common.Uri uri)
        {
            if (uris == null)
                return -1;

            for (Int32 i = 0; i < uris.Length; ++i) {
                if (uris[i].mSpec.Equals(uri.mSpec))
                    return i;
            }

            return -1;
        }

        public override Int32 GetHashCode()
        {
            return mUri.GetHashCode();
        }

        public override Boolean Equals(System.Object obj)
        {
            if (obj == null)
                return false;

            Common.Uri p = obj as Common.Uri;
            if ((Object)p == null)
                return false;

            return mUri.Equals(p.mUri);
        }

        public Boolean Equals(Common.Uri obj)
        {
            if (obj == null)
                return false;

            return mUri.Equals(obj.mUri);
        }

        public static Boolean operator ==(Common.Uri l, Common.Uri r)
        {
            if (System.Object.ReferenceEquals(l, r))
                return true;

            if (((Object)l == null) && ((Object)r == null))
                return true;
            else if ((Object)l == null)
                return r.Equals(l);
            else
                return l.Equals(r);
        }

        public static Boolean operator !=(Common.Uri l, Common.Uri r)
        {
            if (System.Object.ReferenceEquals(l, r))
                return false;

            if (((Object)l == null) && ((Object)r == null))
                return true;
            else if ((Object)l == null)
                return !r.Equals(l);
            else
                return !l.Equals(r);
        }

        private String mUri;
        private String mScheme;
        private String mDomain;
        private String mRequest;
        private String mSpec;
        private String mArguments;
        private Hashtable mArgumentTable;
    }
}