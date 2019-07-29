using Common;
using DBHelper;
using Repository.Pool;
using System;
using System.Configuration;
using System.IO;

namespace Repository
{
    class DBConnection : Singleton<DBConnection>
    {
        private String mConnectionString = String.Format("Data Source={0}\\{1}", Directory.GetCurrentDirectory(), ConfigurationManager.AppSettings["DatabaseName"]);

        private DBConnectionPool mPool = new DBConnectionPool();

        private DBConnection()
        {
            mPool.ConnectionString = mConnectionString;
        }

        public IDbHelper GetConnection()
        {
            return mPool.GetDBConnection();
        }

        public void ReturnDBConnection(IDbHelper conn)
        {
            mPool.ReturnDBConnection(conn);
        }
    }
}
