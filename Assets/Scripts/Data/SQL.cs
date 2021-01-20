using System;
using System.Collections.Generic;

namespace CoronaGame.Data
{
    /// <summary>
    /// Static class that contains prepared
    /// SQL statements in this custom system.
    /// </summary>
    public static class SQL_STATEMENTS
    {
        public static readonly IReadOnlyDictionary<Type, string> SELECT = new Dictionary<Type, string>() 
        {
            { typeof(SaveModel), "SELECT * FROM save" },
            { typeof(EnemyModel), "SELECT * FROM enemy"},
            { typeof(ItemModel), "SELECT * FROM item"},
            { typeof(PlayerModel), "SELECT * FROM player"}
        };

        public static readonly IReadOnlyDictionary<Type, string> BULK_DELETE = new Dictionary<Type, string>()
        {
            { typeof(EnemyModel), "DELETE FROM enemy WHERE save_id = $save_id" },
            { typeof(ItemModel), "DELETE FROM item WHERE save_id = $save_id"},
            { typeof(PlayerModel), "DELETE FROM player WHERE save_id = $save_id"}
        };

        public static readonly IReadOnlyDictionary<Type, string> DELETE = new Dictionary<Type, string>()
        {
            { typeof(SaveModel), "DELETE FROM save WHERE id = $id" },
            { typeof(EnemyModel), "DELETE FROM enemy WHERE id = $id" },
            { typeof(ItemModel), "DELETE FROM item WHERE id = $id"},
            { typeof(PlayerModel), "DELETE FROM player WHERE id = $id"}
        };

        public static readonly IReadOnlyDictionary<Type, string> UPDATE = new Dictionary<Type, string>()
        {
            {
                typeof(SaveModel),
                @"UPDATE save
                    SET
                        level_index = $level_index,
                        name = $name
                    WHERE id = $id"
            },
            {
                typeof(EnemyModel),
                @"UPDATE enemy
                    SET
                        health = $health,
                        is_dead = $is_dead,
                        x = $x,
                        y = $y
                    WHERE id = $id"
            },
            {
                typeof(ItemModel),
                @"UPDATE item
                    SET
                        collected = $collected,
                        x = $x,
                        y = $y
                    WHERE id = $id"
            },
            {
                typeof(PlayerModel),
                @"UPDATE player
                    SET
                        facemask_count = $facemask_count,
                        ammo_count = $ammo_count,
                        has_gun = $has_gun,
                        x = $x,
                        y = $y,
                        spawn_x = $spawn_x,
                        spawn_y = $spawn_y
                    WHERE id = $id"
            }
        };
        public static readonly IReadOnlyDictionary<Type, string> INSERT = new Dictionary<Type, string>()
        {
            {
                typeof(SaveModel),
                @"INSERT INTO save
                    (level_index, name, timestamp)
                VALUES
                    ($level_index, $name, $timestamp)"
            },
            {
                typeof(EnemyModel),
                @"INSERT INTO enemy
                    (ingame_id, save_id, level_index, health, is_dead, x, y)
                VALUES
                    ($ingame_id, $save_id, $level_index, $health, $is_dead, $x, $y)"
            },
            {
                typeof(ItemModel),
                @"INSERT INTO item
                    (ingame_id, save_id, level_index, collected, x, y)
                VALUES
                    ($ingame_id, $save_id, $level_index, $collected, $x, $y)"
            },
            {
                typeof(PlayerModel),
                @"INSERT INTO player
                    (save_id, facemask_count, ammo_count, has_gun, x, y, spawn_x, spawn_y)
                VALUES
                    ($save_id, $facemask_count, $ammo_count, $has_gun, $x, $y, $spawn_x, $spawn_y)"
            }
        };
    }
}

