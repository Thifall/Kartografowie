using System.Collections.Generic;
using UnityEngine;

public class Generals
{

    public static Dictionary<CellType, Color> CellTypeColors = new()
    {
        { CellType.Default, new Color(0.4528302f, 0.432272f, 0.254183f, 1) }, //gold/sand
        { CellType.Vilage, new Color(0.6037736f, 0, 0.005363896f, 1) }, //red
        { CellType.Field, new Color(1f, 0.8873908f, 0, 1) }, //yellow
        { CellType.Forest, new Color(0, 0.6509434f, 0.03363523f, 1) }, //green
        { CellType.Mountain, new Color(0.5490196f, 0.2745098f, 0, 1) }, //brown
        { CellType.Water, new Color(0, 0.4603219f, 1, 1) }, //blue
        { CellType.Chasm, new Color(0, 0, 0, 1) }, //black
        { CellType.Monster, new Color(0.3293156f, 0.1520559f, 0.6320754f, 1) }, //purple
    };
}
