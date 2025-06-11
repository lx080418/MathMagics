using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapSetup : MonoBehaviour
{

    //* --------- Singleton ------- */
    public static TilemapSetup Instance;

    //* --------- Inspector References ---------- */
    [Header("Main Grid")]
    public GameObject mainGrid;
    public Tilemap boundaryTilemap;


    [Header("Room Presets")]
    public RoomPreset spawnRoom;
    public List<RoomPreset> roomPresets;
    public RoomPreset bossRoom;


    [Header("Enemies")]
    public List<GameObject> enemies;
    public GameObject level1Boss;
    public GameObject level2Boss;
    public GameObject level3Boss;
    public GameObject level4Boss;


    //* ------------------ Generation Settings ---------------- */
    [Header("Generation Settings")]
    [Tooltip("The width of each cell on the world. Should be odd.")]
    public int cellWidth;

    [Header("Tiles")]
    public TileBase boundaryTile;


    //* ----------------- Internal Usage ----------------- */
    private int currentX;
    private int currentY;
    private int[,] gridPositions;
    private int numRooms;
    private List<GameObject> toBeDestroyedOnReset;
    private int maxNumRooms;
    private int maxRoomsLength;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        toBeDestroyedOnReset = new List<GameObject>();
        maxNumRooms = numRooms;
    }


    public void NewLevel(int level)
    {
        InitializeGridPositions(level);
        ClearPreviousLevel(level);
        Generate();
    }

    private void InitializeGridPositions(int level)
    {
        maxRoomsLength = 5 + level * 2;
        gridPositions = new int[maxRoomsLength, maxRoomsLength]; // values default to 0
    }

    private void ClearPreviousLevel(int level)
    {
        //destroys everything added to this list. Will be enemies and room presets
        foreach (GameObject go in toBeDestroyedOnReset)
        {
            Destroy(go);
        }
        maxNumRooms = 5 + level * 2;
        numRooms = maxNumRooms;
    }

    private void Generate()
    {
        GameObject room;
        GameObject previousRoom;

        // First, generate the border of 1's on our grid
        //this prevents us from generating rooms west or south
        for (int i = 0; i < maxRoomsLength; i++)
        {
            gridPositions[0, i] = 1;
            gridPositions[maxRoomsLength - 1, i] = 1;
        }

        for (int i = 0; i < maxRoomsLength; i++)
        {
            gridPositions[i, 0] = 1;
            gridPositions[i, maxRoomsLength - 1] = 1;
        }
        GenerateSpawnRoom(0, 0);
    }
    


    public void GenerateSpawnRoom(int x, int y)
    {
        //instantiate the preset at the given position
        //set the preset's parent to the grid object
        GameObject spawned = Instantiate(spawnRoom.gameObject, new Vector2(x*cellWidth, y * cellWidth), Quaternion.identity);
        RoomPreset spawnedPreset = spawned.GetComponent<RoomPreset>();
        spawned.transform.SetParent(mainGrid.transform);
        toBeDestroyedOnReset.Add(spawned);


        Tilemap roomTilemap = spawned.transform.GetChild(1).GetComponent<Tilemap>();
        roomTilemap.SetTile(new Vector3Int(spawnedPreset.roomWidth/2, -1), null);
        roomTilemap.SetTile(new Vector3Int(spawnedPreset.roomWidth/2, 0), null);
        roomTilemap.SetTile(new Vector3Int(spawnedPreset.roomWidth/2,0), null);
        
        for(int i = x*cellWidth; i < (x*cellWidth)+cellWidth; i++)
        {
            for(int j = y*cellWidth; j < (y*cellWidth)+cellWidth; j++)
            {
                //i and j should be positions of the tiles
                //remove tiles in the boundary map.
                boundaryTilemap.SetTile(new Vector3Int(i, j), null);
            }
        }

    }
}
