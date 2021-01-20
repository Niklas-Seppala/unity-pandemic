using Mono.Data.SqliteClient;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoronaGame.Data
{
    /// <summary>
    /// Class that can access the database.
    /// </summary>
    public static class DbAccess
    {
        private static string connStr = string.Empty;

        /// <summary>
        /// Sets the connection string.
        /// </summary>
        /// <param name="str">connection string</param>
        public static void SetConnectionString(string str) => connStr = str;

        /// <summary>
        /// Static constructor sets default connection string.
        /// </summary>
        static DbAccess() => SetConnectionString($"URI=file:{Application.dataPath}/saves.db");

        /// <summary>
        /// Dictionary that holds the database table names
        /// of each model type.
        /// </summary>
        /// <typeparam name="Type">Type of the table</typeparam>
        /// <typeparam name="string">Table name in db</typeparam>
        /// <returns></returns>
        private static readonly IReadOnlyDictionary<Type, string> SQLiteSeq = new Dictionary<Type, string>()
        {
            { typeof(SaveModel), "save" },
            { typeof(EnemyModel), "enemy" },
            { typeof(ItemModel), "item" },
            { typeof(PlayerModel), "player" }
        };

        /// <summary>
        /// Gets the next autoincrement id for a table.
        /// </summary>
        /// <typeparam name="T">Type of the model</typeparam>
        /// <returns>next id if the table, -1 if failed</returns>
        public static int GetNextId<T>()
        {
            string sql = @"SELECT seq
                            FROM sqlite_sequence
                            WHERE name = $name";
            using (var dbConnection = new SqliteConnection(connStr))
            using (var dbCommand = dbConnection.CreateCommand())
            {
                if (SQLiteSeq.TryGetValue(typeof(T), out string name))
                {
                    dbCommand.Parameters.Add(new SqliteParameter("$name", name));
                    dbConnection.Open();
                    dbCommand.CommandText = sql;
                    using (var reader = dbCommand.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            return reader.GetInt32(0)+1;
                        }
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Updates the specified model in the database. Returns the
        /// affected row count.
        /// </summary>
        /// <param name="model">model to be updated</param>
        /// <typeparam name="T">Type of the model</typeparam>
        /// <returns>Affected row count</returns>
        public static int UpdateModel<T>(T model) where T : IDbModel
        {
            using (var dbConnection = new SqliteConnection(connStr))
            using (var dbCommand = dbConnection.CreateCommand())
            {
                dbConnection.Open();
                if (SQL_STATEMENTS.UPDATE.TryGetValue(typeof(T), out string query))
                {
                    dbCommand.CommandText = query;
                    model.SetUpdateParameters(dbCommand);
                    return dbCommand.ExecuteNonQuery();
                }
            }
            return 0;
        }

        /// <summary>
        /// Deletes the specified model from the database.
        /// Returns the affected row count.
        /// </summary>
        /// <param name="model">model to be deleted</param>
        /// <typeparam name="T">Type of model</typeparam>
        /// <returns>Affected row count</returns>
        public static int DeleteModel<T>(T model) where T : IDbModel
        {
            using (var dbConnection = new SqliteConnection(connStr))
            using (var dbCommand = dbConnection.CreateCommand())
            {
                dbConnection.Open();
                if (SQL_STATEMENTS.DELETE.TryGetValue(typeof(T), out string query))
                {
                    dbCommand.CommandText = query;
                    dbCommand.Parameters.Add(new SqliteParameter("$id", model.Id));
                    return dbCommand.ExecuteNonQuery();
                }
            }
            return 0;
        }

        /// <summary>
        /// Performs a bulk delete of type T that are related
        /// to specified save id.
        /// </summary>
        /// <param name="saveId">Save id</param>
        /// <typeparam name="T">Type of model</typeparam>
        /// <returns>Affected row count</returns>
        public static int DeleteModels<T>(int saveId)
        {
            using (var dbConnection = new SqliteConnection(connStr))
            using (var dbCommand = dbConnection.CreateCommand())
            {
                dbConnection.Open();
                if (SQL_STATEMENTS.BULK_DELETE.TryGetValue(typeof(T), out string query))
                {
                    dbCommand.CommandText = query;
                    dbCommand.Parameters.Add(new SqliteParameter("$save_id", saveId));
                    return dbCommand.ExecuteNonQuery();
                }
            }
            return 0;
        }

        /// <summary>
        /// Inserts specified model to database.
        /// </summary>
        /// <param name="model">Model to be inserted</param>
        /// <typeparam name="T">Model type</typeparam>
        /// <returns>Affected row count</returns>
        public static int InsertModel<T>(T model) where T: IDbModel
        {
            using (var dbConnection = new SqliteConnection(connStr))
            using (var dbCommand = dbConnection.CreateCommand())
            {
                dbConnection.Open();
                if (SQL_STATEMENTS.INSERT.TryGetValue(typeof(T), out string query))
                {
                    dbCommand.CommandText = query;
                    model.SetInsertParameters(dbCommand);
                    return dbCommand.ExecuteNonQuery();
                }
            }
            return 0;
        }

        /// <summary>
        /// Gets a single model of specified type from database.
        /// (First)
        /// </summary>
        /// <typeparam name="T">Type of the model</typeparam>
        public static T GetModel<T>(string where="") where T : class, IDbModel, new()
        {
            using (var dbConnection = new SqliteConnection(connStr))
            using (var dbCommand = dbConnection.CreateCommand())
            {
                dbConnection.Open();
                if (SQL_STATEMENTS.SELECT.TryGetValue(typeof(T), out string query))
                {
                    dbCommand.CommandText = query + where;            // Set SQL command
                    using (var reader = dbCommand.ExecuteReader())
                    {
                        while(reader.Read())                          //While there are rows
                        {
                            var model = new T();                      // Create new empty generic object
                            model.CreateFromRow(reader);              // Fill the model object with it's data
                            return model;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets a collection of model by specified type.
        /// </summary>
        /// <typeparam name="T">model type</typeparam>
        public static IReadOnlyList<T> GetModels<T>(string where="") where T : class, IDbModel, new()
        {                                                           // Constraints for type parameter T.
            var results = new List<T>();
            using (var dbConnection = new SqliteConnection(connStr))
            using (var dbCommand = dbConnection.CreateCommand())
            {
                dbConnection.Open();
                if (SQL_STATEMENTS.SELECT.TryGetValue(typeof(T), out string query)) // Get SQL command related to T.
                {
                    dbCommand.CommandText = query + where;               // Set SQL command.
                    using (var reader = dbCommand.ExecuteReader())
                    {
                        while(reader.Read())                             // While there are rows to read.
                        {
                            var model = new T();                         // Create new empty generic object.
                            model.CreateFromRow(reader);                 // Fill the model object with it's data.
                            results.Add(model);                          // Add to results and continue.
                        }
                    }
                }
            }
            return results;
        }
    }
}

