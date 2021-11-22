using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class XMLFormatter 
{
    public class TileData
    {
        public HexCoordinates m_coords;
        public TerrainType m_terrain;
        public HexMatrics m_matrics;
	}
    /*  (tiles positions)
     * 
     */
    public static void SendMapData(List<Tile> _allTiles)
    {
        List<TileData> allTilesData = new List<TileData>();
        foreach(Tile t in _allTiles)
        {
            TileData tData = new TileData();
            tData.m_coords = t.Coordinates;
            tData.m_matrics = t.Matrics;
            tData.m_terrain = t.Terrain;
            allTilesData.Add(tData);
		}

        string filePath = Path.Combine(Application.persistentDataPath, "filename.xml"); 
        var serializer = new XmlSerializer(typeof(List<TileData>));
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            serializer.Serialize(stream, allTilesData);
        }
	}
}
