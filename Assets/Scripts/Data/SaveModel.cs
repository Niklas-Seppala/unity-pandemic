using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System;
using Mono.Data.SqliteClient;

namespace CoronaGame.Data
{
    public class SaveModel : IDbModel
    {
        public bool Loaded { get; set; } = false;
        public int Id { get; set; } = -1;
        public int LevelIndex { get; set; }
        public string Name { get; set; }
        public int Timestamp { get; set; }

        public IReadOnlyList<EnemyModel> Enemies { get; set; }
        public IReadOnlyList<ItemModel> Items { get; set; }
        public PlayerModel Player { get; set; }

        /// <summary>
        /// Creates an instance of the model from
        /// database reader row.
        /// </summary>
        /// <param name="reader">database reader</param>
        public void CreateFromRow(IDataReader reader)
        {
            this.Id = reader.GetInt32(0);
            this.LevelIndex = reader.GetInt32(1);
            this.Name = reader.GetString(2);
            this.Timestamp = reader.GetInt32(3);
        }

        /// <summary>
        /// Gathers the rest of the data from the database
        /// that are related to this save.
        /// </summary>
        public void GatherRelatedData()
        {
            if (this.Id < 0) throw new Exception("Save you are trying to access is uninitialized!");

            string where = $" WHERE save_id = {this.Id}";
            this.Enemies = DbAccess.GetModels<EnemyModel>(where);
            this.Items = DbAccess.GetModels<ItemModel>(where);
            this.Player = DbAccess.GetModel<PlayerModel>(where);
        }

        /// <summary>
        /// Set parameters to db command object when inserting
        /// model to database.
        /// </summary>
        /// <param name="dbCommand">db command to be executed.</param>
        public void SetInsertParameters(DbCommand dbCommand)
        {
            dbCommand.Parameters.Add(new SqliteParameter("$level_index", this.LevelIndex));
            dbCommand.Parameters.Add(new SqliteParameter("$name", this.Name));
            dbCommand.Parameters.Add(new SqliteParameter("$timestamp", this.Timestamp));
        }

        /// <summary>
        /// Set parameters to db command object when updating model
        /// to database.
        /// </summary>
        /// <param name="dbCommand">db command to be executed.</param>
        public void SetUpdateParameters(DbCommand dbCommand)
        {
            dbCommand.Parameters.Add(new SqliteParameter("$level_index", this.LevelIndex));
            dbCommand.Parameters.Add(new SqliteParameter("$name", this.Name));
            dbCommand.Parameters.Add(new SqliteParameter("$id", this.Id));
        }
    }
}

