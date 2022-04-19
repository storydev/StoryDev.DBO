﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Oracle.ManagedDataAccess.Client;

namespace StoryDev.DBO.PostgreSQL
{
    public class DBObject : IDBObject
    {

        private static DBManager manager;
        public static DBManager Manager
        {
            get
            {
                if (manager == null)
                    manager = new DBManager();
                return manager;
            }
        }

        public int ID;

        public void Delete()
        {
            var name = GetType().Name;
            var query = Utils.GenerateDelete(name, ID);

            if (!Manager.IsBuilding)
            {
                var connection = (OracleConnection)Manager.OpenConnection(Manager.ConnectionInfo);
                var command = new OracleCommand(query, connection);
                command.ExecuteNonQuery();
                Manager.CloseConnection(connection);
            }
            else
            {
                BulkModifier.AppendQuery(query);
            }
        }

        public void Insert()
        {
            var name = GetType().Name;
            var fields = GetType().GetFields();
            var query = Utils.GenerateInsert(name, fields, true);

            var connection = new OracleConnection(Manager.ConnectionInfo);
            var command = new OracleCommand(query);
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                if (field.Name == "ID")
                    continue;
                command.Parameters.Add("@" + field.Name, field.GetValue(this));
            }

            if (!Manager.IsBuilding)
            {
                connection.Open();
                command.Connection = connection;
                Manager.LastInsertedID = (int)command.ExecuteScalar();
                Manager.CloseConnection(connection);
            }
            else
            {
                BulkModifier.AppendQuery(command.CommandText);
            }
        }

        public void Update(DBFilter[] filters = null)
        {
            var name = GetType().Name;
            var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            var query = Utils.GenerateUpdate(name, fields, ID, filters);

            var connection = new OracleConnection(Manager.ConnectionInfo);
            var command = new OracleCommand(query);
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                if (field.Name == "ID" || field.IsInitOnly)
                    continue;
                command.Parameters.Add("@" + field.Name, field.GetValue(this));
            }

            if (!Manager.IsBuilding)
            {
                connection.Open();
                command.Connection = connection;
                command.ExecuteNonQuery();
                Manager.CloseConnection(connection);
            }
            else
            {
                BulkModifier.AppendQuery(command.CommandText);
            }
        }
    }
}
