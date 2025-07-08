using System.Collections.Generic;
using UnityEngine;

namespace Kartografowie.General
{
    public static class Generals
    {

        public static Dictionary<CellType, Color> CellTypeColors = new()
        {
            { CellType.Default, new Color(0.4528302f, 0.432272f, 0.254183f, 1) }, //gold/sand
            { CellType.Village, new Color(0.6037736f, 0, 0.005363896f, 1) }, //red
            { CellType.Field, new Color(1f, 0.8873908f, 0, 1) }, //yellow
            { CellType.Forest, new Color(0, 0.6509434f, 0.03363523f, 1) }, //green
            { CellType.Mountain, new Color(0.3f, 0.15f, 0, 1) }, //brown
            { CellType.Water, new Color(0, 0.4603219f, 1, 1) }, //blue
            { CellType.Chasm, new Color(0, 0, 0, 1) }, //black
            { CellType.Monster, new Color(0.3293156f, 0.1520559f, 0.6320754f, 1) }, //purple
        };

        public static List<Vector2Int> Directions = new()
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };


        public static string EdictName(this Edicts edict)
        {
            return edict switch
            {
                Edicts.Edict_A => "A",
                Edicts.Edict_B => "B",
                Edicts.Edict_C => "C",
                Edicts.Edict_D => "D",
                Edicts.Monesters => "M",
                Edicts.Coins => "P",
                _ => "Unknown Edict"
            };
        }
    }


    public enum Edicts
    {
        Edict_A,
        Edict_B,
        Edict_C,
        Edict_D,
        Monesters,
        Coins
    }
}