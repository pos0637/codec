using DBHelper;
using System.Data.SqlClient;

namespace Repository.Pool
{
    /// <summary>
    /// 数据库连接池
    /// </summary>
    public sealed class DBConnectionPool : ObjectPool
    {
        public DBConnectionPool() { }

        private string mConnectionString;

        public string ConnectionString {
            get {
                return mConnectionString;
            }
            set {
                mConnectionString = value;
            }
        }

        protected override object Create()
        {
            IDbHelper conn = DbFactory.Instance.GetDbHelper(ConnectionString, DBType.SQLite);
            conn.OpenConnection();
            return conn;
        }

        protected override bool Validate(object o)
        {
            try {
                IDbHelper conn = (IDbHelper)o;
                return conn.ConnectionIsOpen;
            }
            catch (SqlException) {
                return false;
            }
        }

        protected override void Expire(object o)
        {
            try {
                IDbHelper conn = (IDbHelper)o;
                conn.CloseConnection();
            }
            catch (SqlException) { }
        }

        public IDbHelper GetDBConnection()
        {
            try {
                return (IDbHelper)base.GetObjectFromPool();
            }
            catch {
                return null;
            }
        }

        public void ReturnDBConnection(IDbHelper conn)
        {
            base.ReturnObjectToPool(conn);
        }
    }
}
