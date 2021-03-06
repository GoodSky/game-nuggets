﻿using System;
using UI;

namespace Common
{
    /// <summary>
    /// Static tracker of the object that is selected.
    /// </summary>
    public static class SelectionManager
    {
        public static Selectable Selected { get { return globalSelection; } }

        private static Selectable globalSelection = null;

        private static object globalSelectionLock = new object();

        /// <summary>
        /// Updates which object is currently selected.
        /// </summary>
        /// <param name="selection">The Selectable component of the new selected GameObject.</param>
        public static void UpdateSelection(Selectable selection)
        {
            lock (globalSelectionLock)
            {
                if (selection == globalSelection)
                    return;

                var oldSelection = globalSelection;
                globalSelection = selection;

                if (oldSelection != null)
                {
                    try
                    {
                        oldSelection.Deselect();
                    }
                    catch (Exception e)
                    {
                        GameLogger.Warning("Exception during Deselect. Object = {0}. Ex = {1}.", oldSelection.GetType().Name, e);
                    }
                }

                if (globalSelection != null)
                {
                    try
                    {
                        globalSelection.Select();
                    }
                    catch (Exception e)
                    {
                        GameLogger.Warning("Exception during Select. Object = {0}. Ex = {1}.", oldSelection.GetType().Name, e);
                    }
                }

                TooltipManager.PopDown();
            }
        }
    }
}
