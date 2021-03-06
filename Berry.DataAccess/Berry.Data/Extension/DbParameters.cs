﻿using Berry.Code;
using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace Berry.Data.Extension
{
    /// <summary>
    /// SQL参数化
    /// </summary>
    public static class DbParameters
    {
        /// <summary>
        /// 根据配置文件中所配置的数据库类型
        /// 来获取命令参数中的参数符号Oracle为":",SqlServer为"@"
        /// </summary>
        /// <returns></returns>
        public static string CreateDbParmCharacter()
        {
            string character;
            switch (DbHandlingCenter.DbType)
            {
                case DatabaseType.SqlServer:
                    character = "@";
                    break;

                case DatabaseType.Oracle:
                    character = ":";
                    break;

                case DatabaseType.MySql:
                    character = "?";
                    break;

                //case DatabaseType.SqLite:
                //    character = "@";
                //    break;

                default:
                    throw new Exception("数据库类型目前不支持！");
            }
            return character;
        }

        /// <summary>
        /// 根据配置文件中所配置的数据库类型
        /// 来创建相应数据库的参数对象
        /// </summary>
        /// <returns></returns>
        public static DbParameter CreateDbParameter()
        {
            DbParameter parameter;
            switch (DbHandlingCenter.DbType)
            {
                case DatabaseType.SqlServer:
                    parameter = new SqlParameter();
                    break;

                //case DatabaseType.Oracle:
                //    parameter = new OracleParameter();
                //    break;

                //case DatabaseType.MySql:
                //    parameter = new MySqlParameter();
                //    break;

                default:
                    parameter = new SqlParameter();
                    break;
            }
            return parameter;
        }

        /// <summary>
        /// 根据配置文件中所配置的数据库类型
        /// 来创建相应数据库的参数对象
        /// </summary>
        /// <returns></returns>
        public static DbParameter CreateDbParameter(string paramName, object value)
        {
            DbParameter param = DbParameters.CreateDbParameter();
            param.ParameterName = paramName;
            param.Value = value;
            return param;
        }

        /// <summary>
        /// 根据配置文件中所配置的数据库类型
        /// 来创建相应数据库的参数对象
        /// </summary>
        /// <returns></returns>
        public static DbParameter CreateDbParameter(string paramName, object value, DbType dbType)
        {
            DbParameter param = DbParameters.CreateDbParameter();
            param.DbType = dbType;
            param.ParameterName = paramName;
            param.Value = value;
            return param;
        }

        /// <summary>
        /// 转换对应的数据库参数
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        public static DbParameter[] ToDbParameter(DbParameter[] parameter)
        {
            int i = 0;
            int size = parameter.Length;
            DbParameter[] dbParameter = null;
            switch (DbHandlingCenter.DbType)
            {
                case DatabaseType.SqlServer:
                    dbParameter = new DbParameter[size];
                    while (i < size)
                    {
                        dbParameter[i] = new SqlParameter(parameter[i].ParameterName, parameter[i].Value);
                        i++;
                    }
                    break;
                //case DatabaseType.MySql:
                //    dbParameter = new DbParameter[size];
                //    while (i < size)
                //    {
                //        dbParameter[i] = new MySqlParameter(dbParameter[i].ParameterName, dbParameter[i].Value);
                //        i++;
                //    }
                //    break;
                //case DatabaseType.Oracle:
                //    dbParameter = new DbParameter[size];
                //    while (i < size)
                //    {
                //        dbParameter[i] = new OracleParameter(dbParameter[i].ParameterName, dbParameter[i].Value);
                //        i++;
                //    }
                //    break;
                //case DatabaseType.Access:
                //    dbParameter = new DbParameter[size];
                //    while (i < size)
                //    {
                //        dbParameter[i] = new OleDbParameter(dbParameter[i].ParameterName, dbParameter[i].Value);
                //        i++;
                //    }
                //    break;
                //case DatabaseType.SqLite:
                //    dbParameter = new DbParameter[size];
                //    while (i < size)
                //    {
                //        dbParameter[i] = new SQLiteParameter(dbParameter[i].ParameterName, dbParameter[i].Value);
                //        i++;
                //    }
                //    break;
                default:
                    throw new Exception("数据库类型目前不支持！");
            }
            return dbParameter;
        }

        /// <summary>
        /// DbParameter转成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbParameter"></param>
        /// <returns></returns>
        public static T DbParameterToObject<T>(this DbParameter[] dbParameter) where T : new()
        {
            T t = new T();
            if (dbParameter != null)
            {
                Type type = typeof(T);
                PropertyInfo[] props = type.GetProperties();

                foreach (DbParameter parameter in dbParameter)
                {
                    if (parameter.Value != null)
                    {
                        foreach (PropertyInfo prop in props)
                        {
                            // 判断此属性是否有Setter  
                            if (!prop.CanWrite) continue;//该属性不可写，直接跳出  

                            if (prop.Name == parameter.ParameterName.Replace(DbParameters.CreateDbParmCharacter(), ""))
                            {
                                prop.SetValue(t, parameter.Value, null);
                            }
                        }
                    }
                }
            }
            return t;
        }
    }
}