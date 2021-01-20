using System;
using System.Data;
using System.Data.Common;
using Mono.Data.SqliteClient;
using UnityEngine;
using CoronaGame.Units.Enemies;

namespace CoronaGame.Data
{
    public class EnemyModel : IDbModel
    {
        public int Id { get; set; }
        public string InGameId { get; set; }
        public int SaveId { get; set; }
        public int LevelIndex { get; set; }
        public int Health { get; set; }
        public bool IsDead { get; set; }
        public Vector3 Position { get; set; }

        /// <summary>
        /// Creates an instance of the model from
        /// database reader row.
        /// </summary>
        /// <param name="reader">database reader</param>
        public void CreateFromRow(IDataReader reader)
        {
            this.Id = reader.GetInt32(0);
            this.InGameId = reader.GetString(1);
            this.SaveId = reader.GetInt32(2);
            this.LevelIndex = reader.GetInt32(3);
            this.Health = reader.GetInt32(4);
            this.IsDead = Convert.ToBoolean(reader.GetInt32(5));
            this.Position = new Vector3(
                reader.GetFloat(6),
                reader.GetFloat(7),
                0
            );
        }

        /// <summary>
        /// Set parameters to db command object when inserting
        /// model to database.
        /// </summary>
        /// <param name="dbCommand">db command to be executed.</param>
        public void SetInsertParameters(DbCommand dbCommand)
        {
            dbCommand.Parameters.Add(new SqliteParameter("$ingame_id", this.InGameId));
            dbCommand.Parameters.Add(new SqliteParameter("$save_id", this.SaveId));
            dbCommand.Parameters.Add(new SqliteParameter("$level_index", this.LevelIndex));
            dbCommand.Parameters.Add(new SqliteParameter("$health", this.Health));
            dbCommand.Parameters.Add(new SqliteParameter("$is_dead", Convert.ToInt32(this.IsDead)));
            dbCommand.Parameters.Add(new SqliteParameter("$x", this.Position.x));
            dbCommand.Parameters.Add(new SqliteParameter("$y", this.Position.y));
        }

        /// <summary>
        /// Set parameters to db command object when updating model
        /// to database.
        /// </summary>
        /// <param name="dbCommand">db command to be executed.</param>
        public void SetUpdateParameters(DbCommand dbCommand)
        {
            dbCommand.Parameters.Add(new SqliteParameter("$id", this.Id));
            dbCommand.Parameters.Add(new SqliteParameter("$health", this.Health));
            dbCommand.Parameters.Add(new SqliteParameter("$is_dead", Convert.ToInt32(this.IsDead)));
            dbCommand.Parameters.Add(new SqliteParameter("$x", this.Position.x));
            dbCommand.Parameters.Add(new SqliteParameter("$y", this.Position.y));
        }

        /// <summary>
        /// Convert Enemy to EnemyModel transfering
        /// VARIABLE DATA.
        /// </summary>
        /// <param name="enemy"></param>
        public static explicit operator EnemyModel(Enemy enemy)
        {
            var model = new EnemyModel()
            {
                Health = enemy.Health,
                InGameId = enemy.name,
                Position = enemy.transform.position,
            };
            return model;
        }
    }
}

