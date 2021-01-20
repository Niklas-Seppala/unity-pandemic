#pragma warning disable CS0649

using UnityEngine;
using UnityEngine.SceneManagement;
using CoronaGame.UI;
using CoronaGame.Units;
using Cinemachine;
using CoronaGame.Data;
using System;

namespace CoronaGame
{
    /// <summary>
    /// Game manager that manages different game events.
    /// Handles operations after scene loading. The "brain" of the game.
    /// Uses singleton pattern: use static Instance - object reference.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private ViewManager viewManager;
        [SerializeField] private Player playerPrefab;
        [SerializeField] private SaveManager saveManager;
        public SaveManager SaveManager => this.saveManager;

        public static GameManager Instance { get; private set; }

        public bool OnPause { get; private set; }
        public bool ActiveAI { get; private set; }
        public bool ActiveGame { get; private set; }
        public int LevelIndex { get; private set; }
        public LevelManager LevelManager { get; private set; }
        public SaveModel CurrentSave { get; set; }

        public bool LoadSave { get; private set; }

        private CinemachineVirtualCamera cam;
        private Player player;
        public Player Player => this.player;

        private void Awake()
        {
            if (GameManager.Instance == null)
            {
                GameManager.Instance = this;
                DontDestroyOnLoad(this.gameObject);
                SceneManager.sceneLoaded += SceneLoaded;
            }
            else
            {
                Destroy(this.gameObject);
            }
            AudioManager.Instance.Play("Theme");
        }

        /// <summary>
        /// Eventhandler for SceneManager scene loaded event.
        /// Updates fields related to scene and builds it dynamically,
        /// if necessary.
        /// </summary>
        /// <param name="scene">Scene that got loaded</param>
        /// <param name="mode">Loading mode</param>
        private void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            this.LevelIndex = scene.buildIndex;
            if (scene.buildIndex > 0 ) // Game scene
            {
                // Try to find the player from the scene
                this.player = GameObject.FindGameObjectWithTag("Player")
                    ?.GetComponent<Player>();

                // If player was not found, instantiate from prefab.
                if (this.player == null)
                    this.player = Instantiate(playerPrefab);
                
                // Add eventhandlers
                player.PlayerDeath.AddListener(PlayerDeath);

                this.InitGameScene(scene);
                this.ActiveGame = true;
            }
            else // Main menu scene
            {
                this.ActiveGame = false;
            }
            this.UnpauseGame();
        }

        /// <summary>
        /// Initializes the game scene. Loads the save is
        /// Current save is active.
        /// </summary>
        /// <param name="scene">Loaded scene</param>
        private void InitGameScene(Scene scene)
        {
            // Game level setup
            this.cam = FindObjectOfType<CinemachineVirtualCamera>();
            this.LevelManager = FindObjectOfType<LevelManager>();
            this.player.SpawnPoint = this.LevelManager.InitialSpawnPoint;

            // Are we loading a save? If so create the scene dynamically from it.
            bool? loaded = CurrentSave?.Loaded;
            if (loaded.HasValue && !loaded.Value)
            {
                this.LevelManager.LoadEnemyTemplateFromSave(CurrentSave.Enemies);
                this.LevelManager.LoadItemTemplateFromSave(CurrentSave.Items);
                this.LevelManager.LoadPlayerFromSave(CurrentSave.Player);

                // Mark the load saved, so on the next scene load ignores it.
                this.CurrentSave.Loaded = true;

                ViewManager.Instance.FlashMessage("Game Loaded", duration: 0.5f);
            }
            // If the scene is not the first play scene, we need to add
            // the player and perform some setup
            else if (scene.buildIndex >= 1)
            {
                this.player.Spawn();
                this.cam.Follow = player.transform;
                this.cam.ForceCameraPosition(player.transform.position, Quaternion.identity);
            }
        }

        /// <summary>
        /// Player death eventhandler.
        /// After player death, Death View will be
        ///  displayed. Disables AI and Active game.
        /// </summary>
        private void PlayerDeath()
        {
            AudioManager.Instance.Play("PlayerDeath");
            this.viewManager.Open<DeathView>();
            this.ActiveAI = false;
            this.ActiveGame = false;
        }

        /// If game was not on pause, changes time scale
        /// to 0, and toggles game state fields
        /// to represent paused game.
        public void PauseGame()
        {
            this.OnPause = true;
            this.ActiveAI = false;
            Time.timeScale = 0f;
        }

        /// <summary>
        /// If game was on pause, changes game time
        /// back to 1, and toggles game state fields
        /// to represent unpaused game.
        /// </summary>
        public void UnpauseGame()
        {
            this.OnPause = false;
            this.ActiveAI = true;
            Time.timeScale = 1f;
        }

        /// <summary>
        /// Loads the save from passed
        /// SaveModl. Starts scene transition.
        /// </summary>
        /// <param name="save">Save model to be loaded</param>
        public void LoadGame(SaveModel save)
        {
            this.CurrentSave = save;
            this.CurrentSave.Loaded = false;
            SceneController.Instance.ChangeScene(this.CurrentSave.LevelIndex);
        }

        /// <summary>
        /// Overrides current active save. Sets new save
        /// as active save.
        /// </summary>
        public void OverwriteCurrentSave()
        {
            try
            {
                var newSave = this.LevelManager.Export(from: this.CurrentSave);

                DbAccess.UpdateModel<SaveModel>(newSave);
                DbAccess.UpdateModel<PlayerModel>(newSave.Player);

                if (newSave.LevelIndex != this.CurrentSave.LevelIndex)
                {
                    // Level has changed, all enemies and items in current save are dirty.
                    DbAccess.DeleteModels<ItemModel>(this.CurrentSave.Id);
                    DbAccess.DeleteModels<EnemyModel>(this.CurrentSave.Id);

                    // Insert new versions of enemies and items
                    foreach (var item in newSave.Items) DbAccess.InsertModel<ItemModel>(item);
                    foreach (var enemy in newSave.Enemies) DbAccess.InsertModel<EnemyModel>(enemy);
                }
                else
                {   // Update will do for in this case.
                    // Update data than might have changed (positon, health.. etc)
                    foreach (var item in newSave.Items) DbAccess.UpdateModel<ItemModel>(item);
                    foreach (var enemy in newSave.Enemies) DbAccess.UpdateModel<EnemyModel>(enemy);
                }
                this.CurrentSave = newSave;
                this.CurrentSave.Loaded = true;
                this.viewManager.FlashMessage("Game Saved!");
            }
            catch (Exception)
            {
                this.viewManager.FlashMessage("Game Save Failed!");
                this.CurrentSave = null;
            }
        }

        /// <summary>
        /// Creates new save from current game state.
        /// </summary>
        /// <param name="saveName">name of the save</param>
        public void SaveGame(string saveName)
        {
            try
            {
                var save = this.LevelManager.Export(saveName);

                DbAccess.InsertModel<SaveModel>(save);
                DbAccess.InsertModel<PlayerModel>(save.Player);
                foreach (var item in save.Items) // bulk insert ?? :D, nope too busy
                    DbAccess.InsertModel<ItemModel>(item);
                foreach (var enemy in save.Enemies)
                    DbAccess.InsertModel<EnemyModel>(enemy);

                this.CurrentSave = save;
                this.CurrentSave.Loaded = true;
                viewManager.FlashMessage("Game saved!");
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                viewManager.FlashMessage("Save failed!");
            }
        }

        /// <summary>
        /// Respawns the player. Enemies are reset, but items
        /// remain as they are.
        /// </summary>
        public void RespawnPlayer()
        {
            // If player is on boss fight, reset its state.
            var bossFightManager = FindObjectOfType<BossFightManager>();
            bossFightManager?.RestartPlayerState();

            // For cool looking fade effect.
            this.viewManager.ScreenFade(middleAction: () => {
                this.viewManager.Close<DeathView>();
                this.player.Spawn();
                this.cam.ForceCameraPosition(player.SpawnPoint, Quaternion.identity);
                this.cam.Follow = this.player.transform;
                this.LevelManager.CreateEnemies();
                this.ActiveAI = true;
                this.ActiveGame = true;
            });
        }

        /// <summary>
        /// Loads the next save.
        /// </summary>
        public void NextLevel()
        {
            SceneController.Instance.ChangeScene(this.LevelIndex+1);
        }

        /// <summary>
        /// Changes the scene to main menu. Toggles
        /// game to inactive.
        /// </summary>
        public void ExitToMainMenu()
        {
            this.ActiveGame = false;
            Destroy(this.player.gameObject);
            SceneController.Instance.ChangeScene(0);
        }

        /// <summary>
        /// Updates player's facemask and ammo count
        /// to UI.
        /// </summary>
        public void UpdatePlayerDataToUI()
        {
            ViewManager.Instance.UpdateFacemaskCount(this.player.FaceMaskCount);
            ViewManager.Instance.UpdateAmmoCount(this.player.AmmoCount);
        }
    }
}

