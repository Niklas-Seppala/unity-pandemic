#pragma warning disable CS0649

using Cinemachine;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using CoronaGame.Units.Enemies;
using CoronaGame.Items;
using CoronaGame.Data;
using CoronaGame.Units;

namespace CoronaGame
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private LevelObjects objects;
        [SerializeField] private int levelIndex;
        [SerializeField] private Transform initialSpawnPoint;
        public Vector3 InitialSpawnPoint => this.initialSpawnPoint.position;
        
        private CinemachineVirtualCamera cam;
        
        public GameObject EnemyTemplate => objects.EnemyTemplate;
        public GameObject ItemTemplate => objects.ItemsTemplate;
        
        public IReadOnlyCollection<string> DefEnemyIds { get; set; }
        public IReadOnlyCollection<string> DefItemIds { get; set; }

        public GameObject CurrentItemsInstance { get; private set; }
        public GameObject CurrentEnemiesInstance { get; private set; }

        /// <summary>
        /// Loads the player data from the save model
        /// and updates player game object accordingly.
        /// </summary>
        /// <param name="model">player save model</param>
        public void LoadPlayerFromSave(PlayerModel model)
        {
            var player = GameManager.Instance.Player;
            player.AmmoCount = model.AmmoCount;
            player.FaceMaskCount = model.FacemaskCount;
            player.transform.position = model.Position;
            player.SpawnPoint = model.SpawnPoint;
            if (model.HasGun)
                player.PickupShotgun();
            this.cam.Follow = player.transform;
            GameManager.Instance.UpdatePlayerDataToUI();
        }

        /// <summary>
        /// Loads the items from the save and updates
        /// level's item template based on the save.
        /// </summary>
        /// <param name="itemModels">Collection of item models</param>
        public void LoadItemTemplateFromSave(IReadOnlyCollection<ItemModel> itemModels)
        {
            foreach (var model in itemModels)
            {
                if (model.LevelIndex != this.levelIndex) throw new Exception("Wrong level index");
                var enemy = this.CurrentItemsInstance.transform.Find(model.InGameId);
                if (model.Collected)
                {
                    Destroy(enemy.gameObject);
                    continue;
                }
            }
        }

        /// <summary>
        /// Loads the enemies from the save and updates
        /// level's enemy template based on the save.
        /// </summary>
        /// <param name="enemyModels">Collection of enemy models</param>
        public void LoadEnemyTemplateFromSave(IReadOnlyCollection<EnemyModel> enemyModels)
        {
            foreach (var model in enemyModels)
            {
                if (model.LevelIndex != this.levelIndex) throw new Exception("Wrong level index");
                var enemy = this.CurrentEnemiesInstance.transform.Find(model.InGameId)
                    .GetComponent<Enemy>();

                if (model.IsDead)
                {
                    Destroy(enemy.gameObject);
                    continue;
                }
                enemy.transform.position = model.Position;
                enemy.Health = model.Health;
            }
        }

        /// <summary>
        /// Stores the default ingame ids of items and
        /// enemies.
        /// </summary>
        private void Awake()
        {
            this.DefEnemyIds = FindObjectsOfType<Enemy>()
                .Select(enemy => enemy.name)
                .ToArray();
            this.DefItemIds = FindObjectsOfType<WorldItem>()
                .Select(item => item.name)
                .ToArray();

            this.CurrentEnemiesInstance = GameObject.FindGameObjectWithTag("EnemyTree");
            this.CurrentItemsInstance = GameObject.FindGameObjectWithTag("ItemTree");
            this.cam = FindObjectOfType<CinemachineVirtualCamera>();
        }

        /// <summary>
        /// Creates enemies by deleting the old.
        /// </summary>
        public void CreateEnemies()
        {
            Destroy(this.CurrentEnemiesInstance);
            var newEnemies = Instantiate(this.EnemyTemplate, Vector3.zero, Quaternion.identity);
            this.CurrentEnemiesInstance = newEnemies;
        }

        /// <summary>
        /// Exports the save based on previous save.
        /// (model db ids are copied).
        /// </summary>
        /// <param name="from">Save to override</param>
        /// <returns>Overridden save</returns>
        public SaveModel Export(SaveModel from)
        {
            var save = this.Export(from.Name, id: from.Id);
            
            // Transfer the database ids to new models
            save.Id = from.Id;
            save.Player.Id = from.Player.Id;
            if (this.levelIndex == from.LevelIndex)
            {   // This stinks, but no time to fix :(
                foreach (var item in save.Items)
                {
                    item.Id = from.Items
                        .Where(i => i.InGameId == item.InGameId)
                        .Select(i => i.Id)
                        .First();
                }
                foreach (var enemy in save.Enemies)
                {
                    enemy.Id = from.Enemies
                        .Where(i => i.InGameId == enemy.InGameId)
                        .Select(i => i.Id)
                        .First();
                }
            }
            return save;
        }

        /// <summary>
        /// Export the level as a save model.
        /// </summary>
        /// <param name="name">save name</param>
        /// <param name="id">save database id (optional)</param>
        /// <returns>Exported save</returns>
        public SaveModel Export(string name, int id=-1)
        {
            // If we override previous save, id param should be set.
            int saveId;
            if (id <= 0)
                // otherwise, get next autoincrement id
                saveId = DbAccess.GetNextId<SaveModel>();
            else
                saveId = id;

            var playerModel = (PlayerModel)FindObjectOfType<Player>();
            playerModel.SaveId = saveId;

            // Collect the item data to item models.
            var itemModels = new List<ItemModel>();
            var itemsStillInGame = FindObjectsOfType<WorldItem>();
            foreach (var defItemId in this.DefItemIds)
            {
                var match = Array.Find<WorldItem>(itemsStillInGame, i => i.name == defItemId);
                if (match == null)
                {
                    // Destroyed
                    var model = new ItemModel();
                    model.InGameId = defItemId;
                    model.SaveId = saveId;
                    model.LevelIndex = this.levelIndex;
                    model.Collected = true;
                    itemModels.Add(model);
                }
                else
                {
                    // Still there
                    var model = (ItemModel)match;
                    model.SaveId = saveId;
                    model.LevelIndex = this.levelIndex;
                    model.Collected = false;
                    itemModels.Add(model);
                }
            }

            // Collect the enemy data to enemy models
            var enemyModels = new List<EnemyModel>();
            var enemiesStillInGame = FindObjectsOfType<Enemy>();
            foreach (var defEnemyId in this.DefEnemyIds)
            {
                var match = Array.Find<Enemy>(enemiesStillInGame, i => i.name == defEnemyId);
                if (match == null)
                {
                    var model = new EnemyModel();
                    model.InGameId = defEnemyId;
                    model.IsDead = true;
                    model.LevelIndex = this.levelIndex;
                    model.SaveId = saveId;
                    enemyModels.Add(model);
                }
                else
                {
                    var model = (EnemyModel)match;
                    model.IsDead = false;
                    model.SaveId = saveId;
                    model.LevelIndex = this.levelIndex;
                    enemyModels.Add(model);
                }
            }

            // Gather level data to save model object and ship it.
            return new SaveModel()
            {
                LevelIndex = this.levelIndex,
                Timestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                Player = playerModel,
                Items = itemModels,
                Enemies = enemyModels,
                Name = name
            };
        }
    }
}
