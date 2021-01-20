using UnityEngine;
using System.Data;
using System.Data.Common;
using System;
using Mono.Data.SqliteClient;
using CoronaGame.Units;

namespace CoronaGame.Data
{
    public class PlayerModel : IDbModel
    {
        public int Id { get; set; }
        public int SaveId { get; set; }
        public int FacemaskCount { get; set; }
        public int AmmoCount { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 SpawnPoint { get; set; }
        public bool HasGun { get; set; }


        /// <summary>
        /// Creates an instance of the model from
        /// database reader row.
        /// </summary>
        /// <param name="reader">database reader</param>
        public void CreateFromRow(IDataReader reader)
        {
            this.Id = reader.GetInt32(0);
            this.SaveId = reader.GetInt32(1);
            this.FacemaskCount = reader.GetInt32(2);
            this.AmmoCount = reader.GetInt32(3);
            this.HasGun = Convert.ToBoolean(reader.GetInt32(4));
            this.Position = new Vector3(
                reader.GetFloat(5),
                reader.GetFloat(6),
                0f
            );
            this.SpawnPoint = new Vector3(
                reader.GetFloat(7),
                reader.GetFloat(8),
                0f
            );
        }

        /// <summary>
        /// Set parameters to db command object when inserting
        /// model to database.
        /// </summary>
        /// <param name="dbCommand">db command to be executed.</param>
        public void SetInsertParameters(DbCommand dbCommand)
        {
            dbCommand.Parameters.Add(new SqliteParameter("$save_id", this.SaveId));
            dbCommand.Parameters.Add(new SqliteParameter("$facemask_count", this.FacemaskCount));
            dbCommand.Parameters.Add(new SqliteParameter("$ammo_count", this.AmmoCount));
            dbCommand.Parameters.Add(new SqliteParameter("$has_gun", Convert.ToInt32(this.HasGun)));
            dbCommand.Parameters.Add(new SqliteParameter("$x", this.Position.x));
            dbCommand.Parameters.Add(new SqliteParameter("$y", this.Position.y));
            dbCommand.Parameters.Add(new SqliteParameter("$spawn_x", this.SpawnPoint.x));
            dbCommand.Parameters.Add(new SqliteParameter("$spawn_y", this.SpawnPoint.y));
        }

        /// <summary>
        /// Set parameters to db command object when updating model
        /// to database.
        /// </summary>
        /// <param name="dbCommand">db command to be executed.</param>
        public void SetUpdateParameters(DbCommand dbCommand)
        {
            dbCommand.Parameters.Add(new SqliteParameter("$id", this.Id));
            dbCommand.Parameters.Add(new SqliteParameter("$facemask_count", this.FacemaskCount));
            dbCommand.Parameters.Add(new SqliteParameter("$ammo_count", this.AmmoCount));
            dbCommand.Parameters.Add(new SqliteParameter("$has_gun", Convert.ToInt32(this.HasGun)));
            dbCommand.Parameters.Add(new SqliteParameter("$x", this.Position.x));
            dbCommand.Parameters.Add(new SqliteParameter("$y", this.Position.y));
            dbCommand.Parameters.Add(new SqliteParameter("$spawn_x", this.SpawnPoint.x));
            dbCommand.Parameters.Add(new SqliteParameter("$spawn_y", this.SpawnPoint.y));
        }

        /// <summary>
        /// Convert Player object to ItemModel object transfering
        /// VARIABLE DATA.
        /// </summary>
        /// <param name="player"></param>
        public static explicit operator PlayerModel(Player player)
        {
            var model = new PlayerModel()
            {
                FacemaskCount = player.FaceMaskCount,
                AmmoCount = player.AmmoCount,
                Position = player.transform.position,
                HasGun = player.HasGun,
                SpawnPoint = player.SpawnPoint
            };
            return model;
        }
    }
}

