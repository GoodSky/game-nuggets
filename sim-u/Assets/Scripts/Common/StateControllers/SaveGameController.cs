﻿using GameData;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Common
{
    /// <summary>
    /// Game Controller that runs during the SaveGame state.
    /// </summary>
    [StateController(HandledState = GameState.SavingGame)]
    internal class SaveGameController : GameStateMachine.Controller
    {
        public SaveGameController()
        {
        }

        /// <summary>
        /// Called by Game State Machine.
        /// </summary>
        /// <param name="_">Not used.</param>
        public override void TransitionIn(object _)
        {
            string saveGameDirectory = Path.Combine(Application.persistentDataPath, "Saved Games");
            Directory.CreateDirectory(saveGameDirectory);

            string path = EditorUtility.SaveFilePanel("Save Game", saveGameDirectory, "saved-game", "svg");

            if (!string.IsNullOrEmpty(path))
            {
                GameLogger.Info("Saving game at '{0}'.", path);

                GameSaveState state = new GameSaveState
                {
                    Version = GameSaveState.CurrentVersion,
                    Campus = Accessor.CampusManager.SaveGameState(),
                };

                SavedGameLoader.WriteToDisk(path, state);
            }

            Accessor.StateMachine.StopDoing();
        }

        /// <summary>
        /// Called by Game State Manager.
        /// </summary>
        public override void TransitionOut()
        {
        }

        /// <summary>
        /// Called each step of this state.
        /// </summary>
        public override void Update()
        {
        }
    }
}
