﻿using Common;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace GameData
{
    /// <summary>
    /// Colors that I've picked out to use.
    /// Intended to be loaded from GameData.
    /// </summary>
    public static class ColorPalette
    {
        private static readonly Dictionary<string, Color> Palette = new Dictionary<string, Color>()
        {
            /* Toolbar and UI Grays */
            { "DarkestGray",    new Color32(31, 31, 31, 255) },
            { "DarkerGray",     new Color32(43, 43, 43, 255) },
            { "DarkGray",       new Color32(57, 57, 57, 255) },
            { "Gray",           new Color32(118, 118, 118, 255) },
            { "LightGray",      new Color32(204, 204, 204, 255) },
            { "LighterGray",    new Color32(230, 230, 230, 255) },
            { "LightestGray",   new Color32(242, 242, 242, 255) },
        };

        /// <summary>
        /// Boldly retrieve a color that you are certain exists. 
        /// Throws exception if the color doesn't exist.
        /// </summary>
        /// <param name="name">Name of the color.</param>
        /// <returns>The color. Or an exception to the face.</returns>
        public static Color GetColor(string name)
        {
            Color color = Color.magenta; // default to a garish color
            if (!Palette.TryGetValue(name, out color))
            {
                GameLogger.FatalError("Configuration attempted to load unrecognized color name '{0}'", name);
            }

            return color;
        }
    }

    /// <summary>
    /// Attribute to allow the <see cref="GameDataLoader{T}"/> to load colors from the palette.
    /// </summary>
    public class ColorPaletteAttribute : XmlIgnoreAttribute
    {
        public ColorPaletteAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; set; }
    }
}
