using UnityEngine;
using System.Collections;

public class Tile : Entity {
	public int x;
	public int y;
	
	public Tile parent;
	public int pathCost;
	
	public void Start () {
		transform.parent = managerMap.currentMission.transform;
		x = Mathf.RoundToInt (transform.position.x / CompressedMap.TILE_SIZE);
		y = Mathf.RoundToInt (transform.position.z / CompressedMap.TILE_SIZE);
		managerMap.currentMission.mapTiles [x, y] = this; 
	}
	
	public MapTileType mapTileType;
}

public enum MapTileType {
	Door,
	Wall,
	Floor
}
