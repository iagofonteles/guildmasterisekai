using System;
using System.Linq;
using Drafts.Database;
using TileGrid.Tiles;
using UnityEditor;
using UnityEngine;

namespace TileGrid.Editor
{
    public static class PalettSetup
    {
        [MenuItem("Game/Tiles/Setup Walls Palette")]
        public static void SetupWallPalettes()
        {
            var palette = Selection.objects.OfType<GameObject>()
                .FirstOrDefault(go => go.GetComponent<Grid>())?
                .GetComponent<Grid>();

            var db = Selection.objects.OfType<IDatabase>().FirstOrDefault();
            if (!palette || db == null) throw new Exception("Select database and palette");

            var walls = db.Items.OfType<ITileEntry>().Where(t => t.Roof != null).ToArray();
            var roofs = walls.Select(w => w.Roof).ToArray();

            // set palette tiles, target aspect 1.5/1
            // order: roof[i] on top of wall[i], one spacing on x and y
            //RXRXR
            //WXWXW
            //XXXXX
            //RXRXR
            //WXWXW
        }

        [MenuItem("Game/Tiles/Setup Floors Palette")]
        public static void SetupFloorPalettes()
        {
            var palette = Selection.objects.OfType<GameObject>()
                .FirstOrDefault(go => go.GetComponent<Grid>())?
                .GetComponent<Grid>();

            var db = Selection.objects.OfType<IDatabase>().FirstOrDefault();
            if (!palette || db == null) throw new Exception("Select database and palette");

            var walls = db.Items.OfType<ITileEntry>().Where(t => t.Roof != null).ToArray();
            var roofs = walls.Select(w => w.Roof).ToArray();
            var floors = db.Items.OfType<ITileEntry>().Except(walls).Except(roofs);

            // set palette tiles, target aspect 1.5/1
            // order: alternate, one spacing on x and y
            //FXFXF
            //XFXFX
            //FXFXF
            //XFXFX
        }
    }
}