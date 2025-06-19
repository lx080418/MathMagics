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
    public TileBase grassTile;
    public TileBase wallTile;


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

    private void Start()
    {
        //TEMP, should call this from GameManager or likewise
        NewLevel(1);
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


        currentX = 1;
        currentY = 0;
        previousRoom = GenerateNewRoom(currentX, currentY);
        Tilemap previousRoomTilemap = previousRoom.transform.GetChild(1).GetComponent<Tilemap>();
        Tilemap previousRoomBackground = previousRoom.transform.GetChild(0).GetComponent<Tilemap>();
        RoomPreset spawnedPreset = previousRoom.GetComponent<RoomPreset>();

        //Destroy west walls on first room since Spawn room is always to the left
        previousRoomTilemap.SetTile(new Vector3Int(-spawnedPreset.roomWidth / 2, -1), null);
        previousRoomTilemap.SetTile(new Vector3Int(-spawnedPreset.roomWidth / 2, 0), null);
        previousRoomTilemap.SetTile(new Vector3Int(-spawnedPreset.roomWidth / 2, 1), null);

        previousRoomBackground.SetTile(new Vector3Int(-spawnedPreset.roomWidth / 2, -1), grassTile);
        previousRoomBackground.SetTile(new Vector3Int(-spawnedPreset.roomWidth / 2, 0), grassTile);
        previousRoomBackground.SetTile(new Vector3Int(-spawnedPreset.roomWidth / 2, 1), grassTile);

        Tilemap enemyTilemap = previousRoom.transform.GetChild(2).GetComponent<Tilemap>();
        //now can get all tiles on this map, each one will be an enemy spawn location.

        //Spawn the enemies at each tile in the Enemy tilemap on the first preset
        BoundsInt bounds = enemyTilemap.cellBounds;

        // TileBase[] enemySpawnTiles = enemyTilemap.GetTilesBlock(bounds);
        // for (int x = -spawnedPreset.roomWidth/2; x < bounds.size.x/2; x++) {
        //     for (int y = -spawnedPreset.roomHeight/2; y < bounds.size.y/2; y++) {
        //         TileBase tile = enemySpawnTiles[x + y * bounds.size.x/2];
        //         if (tile != null) {
        //             //here, x and y should be enemy spawn position
        //             GameObject enemy = Instantiate(enemies[UnityEngine.Random.Range(0, enemies.Count)], new Vector2(currentX*cellWidth + x-1.5f, currentY*cellWidth+y-.5f), Quaternion.identity);
        //             toBeDestroyedOnReset.Add(enemy);
        //         } 
        //     }
        // }      

        numRooms -= 1;
        while (numRooms > 0)
        {
            if (numRooms == 1)
            {
                //BOSS ROOM SPAWNING
                //boss room always spawns up.

                if (previousRoom != null)
                {
                    Tilemap prevRoomTilemap = previousRoom.transform.GetChild(1).GetComponent<Tilemap>();
                    Tilemap roomBackground = previousRoom.transform.GetChild(0).GetComponent<Tilemap>();
                    RoomPreset previousPreset = previousRoom.GetComponent<RoomPreset>();

                    prevRoomTilemap.SetTile(new Vector3Int(-1, previousPreset.roomHeight / 2), null);
                    prevRoomTilemap.SetTile(new Vector3Int(0, previousPreset.roomHeight / 2), null);
                    prevRoomTilemap.SetTile(new Vector3Int(1, previousPreset.roomHeight / 2), null);
                    
                    roomBackground.SetTile(new Vector3Int(-1, previousPreset.roomHeight/2), grassTile);
                    roomBackground.SetTile(new Vector3Int(0, previousPreset.roomHeight/2), grassTile);
                    roomBackground.SetTile(new Vector3Int(1, previousPreset.roomHeight/2), grassTile);
                }
                currentY++;
                //GenerateBossRoom(currentX, currentY, GameManager.instance.GetCurrentLevel());
                //! TEMP
                GenerateBossRoom(currentX, currentY, 1);
                return;
            }

            int num = UnityEngine.Random.Range(0, 2);
            if (num == 0 && gridPositions[currentX, currentY + 1] != 1) //up
            {
                if (previousRoom != null)
                {
                    RoomPreset prevRoomPreset = previousRoom.GetComponent<RoomPreset>();
                    Debug.Log($"Destroying tiles on tilemap {previousRoom.name} at positions \n {-1}, {prevRoomPreset.roomHeight - 1} \n {0}, {prevRoomPreset.roomHeight}\n {1},{prevRoomPreset.roomHeight}");

                    Tilemap prevRoomTilemap = previousRoom.transform.GetChild(1).GetComponent<Tilemap>();
                    Tilemap roomBackgrnd = previousRoom.transform.GetChild(0).GetComponent<Tilemap>();

                    //Destroy North 3 walls
                    prevRoomTilemap.SetTile(new Vector3Int(-1, prevRoomPreset.roomHeight / 2), null);
                    prevRoomTilemap.SetTile(new Vector3Int(0, prevRoomPreset.roomHeight / 2), null);
                    prevRoomTilemap.SetTile(new Vector3Int(1, prevRoomPreset.roomHeight / 2), null);

                    //Draw the bridge
                    for (int i = -2; i <= 2; i++)
                    {
                        for (int j = prevRoomPreset.roomHeight / 2; j <= cellWidth / 2; j++)
                        {
                            if (i == -2 || i == 2)
                            {
                                prevRoomTilemap.SetTile(new Vector3Int(i, j), wallTile);
                            }
                            else
                            {
                                roomBackgrnd.SetTile(new Vector3Int(i, j), grassTile);
                            }
                        }
                    }
                    

                    
                    //Set top 3 walls to grass
                    roomBackgrnd.SetTile(new Vector3Int(-1, prevRoomPreset.roomHeight/2), grassTile);
                    roomBackgrnd.SetTile(new Vector3Int(0, prevRoomPreset.roomHeight/2), grassTile);
                    roomBackgrnd.SetTile(new Vector3Int(1, prevRoomPreset.roomHeight/2), grassTile);
                }

                currentY++;


                //generate new room here
                //have it return a reference to itself, then we can access it's enemy tilemap
                room = GenerateNewRoom(currentX, currentY);
                RoomPreset roomPreset = room.GetComponent<RoomPreset>();
                //delete three tiles on the bottom
                Tilemap roomTilemap = room.transform.GetChild(1).GetComponent<Tilemap>();
                Tilemap roomBackground = room.transform.GetChild(0).GetComponent<Tilemap>();

                roomTilemap.SetTile(new Vector3Int(-1, -roomPreset.roomHeight/2), null);
                roomTilemap.SetTile(new Vector3Int(0, -roomPreset.roomHeight/2), null);
                roomTilemap.SetTile(new Vector3Int(1, -roomPreset.roomHeight/2), null);

                for (int i = -2; i <= 2; i++)
                {
                    for (int j = -roomPreset.roomHeight / 2; j >= -cellWidth / 2; j--)
                    {
                        if (i == -2 || i == 2)
                        {
                            roomTilemap.SetTile(new Vector3Int(i, j), wallTile);
                        }
                        else
                        {
                            roomBackground.SetTile(new Vector3Int(i, j), grassTile);
                        }
                    }
                }

                roomBackground.SetTile(new Vector3Int(-1, -roomPreset.roomHeight/2), grassTile);
                roomBackground.SetTile(new Vector3Int(0, -roomPreset.roomHeight/2), grassTile);
                roomBackground.SetTile(new Vector3Int(1, -roomPreset.roomHeight/2), grassTile);

                //create the bridge 

                enemyTilemap = room.transform.GetChild(2).GetComponent<Tilemap>();
                //now can get all tiles on this map, each one will be an enemy spawn location.\

                bounds = enemyTilemap.cellBounds;

                // enemySpawnTiles = enemyTilemap.GetTilesBlock(bounds);
                // for (int x = 0; x < bounds.size.x; x++)
                // {
                //     for (int y = 0; y < bounds.size.y; y++)
                //     {
                //         TileBase tile = enemySpawnTiles[x + y * bounds.size.x];
                //         if (tile != null)
                //         {
                //             //here, x and y should be enemy spawn position
                //             GameObject enemy = Instantiate(enemies[UnityEngine.Random.Range(0, enemies.Count)], new Vector2(currentX * roomWidth + x - 1.5f, currentY * roomWidth + y - .5f), Quaternion.identity);
                //             toBeDestroyedOnReset.Add(enemy);
                //             GameManager.instance.numOfEnemy++;
                //         }
                //     }
                // }
                numRooms -= 1;
                //check if left is a valid option, if it is spawn a room here
                //if its not occupied
                //if its not on the edge
            }
            else if (num == 1 && gridPositions[currentX + 1, currentY] != 1) //right
            {

                if (previousRoom != null)
                {
                    Tilemap prevRoomTilemap = previousRoom.transform.GetChild(1).GetComponent<Tilemap>();
                    Tilemap roomBagd = previousRoom.transform.GetChild(0).GetComponent<Tilemap>();
                    RoomPreset prevRoomPreset = previousRoom.GetComponent<RoomPreset>();
                    Debug.Log($"Destroying tiles on tilemap {previousRoom.name} at positions \n {-1}, {prevRoomPreset.roomHeight - 1} \n {0}, {prevRoomPreset.roomHeight}\n {1},{prevRoomPreset.roomHeight}");


                    prevRoomTilemap.SetTile(new Vector3Int(prevRoomPreset.roomWidth / 2, 1), null);
                    prevRoomTilemap.SetTile(new Vector3Int(prevRoomPreset.roomWidth / 2, 0), null);
                    prevRoomTilemap.SetTile(new Vector3Int(prevRoomPreset.roomWidth / 2, -1), null);

                    for (int j = -2; j <= 2; j++)
                    {
                        for (int i = prevRoomPreset.roomHeight / 2; i <= cellWidth / 2; i++)
                        {
                            if (j == -2 || j == 2)
                            {
                                prevRoomTilemap.SetTile(new Vector3Int(i, j), wallTile);
                            }
                            else
                            {
                                roomBagd.SetTile(new Vector3Int(i, j), grassTile);
                            }
                        }
                    }
                    
                    roomBagd.SetTile(new Vector3Int(prevRoomPreset.roomWidth/2, 1), grassTile);
                    roomBagd.SetTile(new Vector3Int(prevRoomPreset.roomWidth/2, 0), grassTile);
                    roomBagd.SetTile(new Vector3Int(prevRoomPreset.roomWidth/2, -1), grassTile);
                }
                currentX++;
                //generate new room here
                //have it return a reference to itself, then we can access it's enemy tilemap
                room = GenerateNewRoom(currentX, currentY);
                RoomPreset roomPreset = room.GetComponent<RoomPreset>();
                Debug.Log($"Destroying tiles on tilemap {previousRoom.name} at positions \n {-1}, {roomPreset.roomHeight/2-1} \n {0}, {roomPreset.roomHeight/2-1}\n {1},{roomPreset.roomHeight/2-1}");


                Tilemap roomTilemap = room.transform.GetChild(1).GetComponent<Tilemap>();
                Tilemap roomBackground = room.transform.GetChild(0).GetComponent<Tilemap>();

                roomTilemap.SetTile(new Vector3Int(-roomPreset.roomWidth/2, 1), null);
                roomTilemap.SetTile(new Vector3Int(-roomPreset.roomWidth/2, 0), null);
                roomTilemap.SetTile(new Vector3Int(-roomPreset.roomWidth/2, -1), null);

                for (int j = -2; j <= 2; j++)
                {
                    for (int i = -roomPreset.roomHeight / 2; i >= -cellWidth / 2; i--)
                    {
                        if (j == -2 || j == 2)
                        {
                            roomTilemap.SetTile(new Vector3Int(i, j), wallTile);
                        }
                        else
                        {
                            roomBackground.SetTile(new Vector3Int(i, j), grassTile);
                        }
                    }
                }

                roomBackground.SetTile(new Vector3Int(-roomPreset.roomWidth/2, 1), grassTile);
                roomBackground.SetTile(new Vector3Int(-roomPreset.roomWidth/2, 0), grassTile);
                roomBackground.SetTile(new Vector3Int(-roomPreset.roomWidth/2, -1), grassTile);


                enemyTilemap = room.transform.GetChild(2).GetComponent<Tilemap>();
                Tilemap wallTilemap = room.transform.GetChild(1).GetComponent<Tilemap>();
                //now can get all tiles on this map, each one will be an enemy spawn location.\

                bounds = enemyTilemap.cellBounds;

                // enemySpawnTiles = enemyTilemap.GetTilesBlock(bounds);
                // for (int x = 0; x < bounds.size.x; x++)
                // {
                //     for (int y = 0; y < bounds.size.y; y++)
                //     {
                //         TileBase tile = enemySpawnTiles[x + y * bounds.size.x];
                //         if (tile != null)
                //         {
                //             //here, x and y should be enemy spawn position
                //             GameObject enemy = Instantiate(enemies[UnityEngine.Random.Range(0, enemies.Count)], new Vector2(currentX * roomWidth + x - 1.5f, currentY * roomWidth + y - .5f), Quaternion.identity);
                //             toBeDestroyedOnReset.Add(enemy);
                //             GameManager.instance.numOfEnemy++;
                //         }
                //     }
                // }
                numRooms -= 1;

                //check if left is a valid option, if it is spawn a room here
                //if its not occupied
                //if its not on the edge
            }
            else
            {
                continue;
            }
            previousRoom = room;
            // for (int i = currentX * roomWidth; i < (currentX * roomWidth) + roomWidth; i++)
            // {
            //     for (int j = currentY * roomWidth; j < (currentY * roomWidth) + roomWidth; j++)
            //     {
            //         //i and j should be positions of the tiles
            //         //remove tiles in the boundary map.
            //         boundaryTilemap.SetTile(new Vector3Int(i, j), null);
            //     }
            // }
        }

    }



    public void GenerateSpawnRoom(int x, int y)
    {
        //instantiate the preset at the given position
        //set the preset's parent to the grid object
        GameObject spawned = Instantiate(spawnRoom.gameObject, new Vector2(x * cellWidth, y * cellWidth), Quaternion.identity);
        RoomPreset spawnedPreset = spawned.GetComponent<RoomPreset>();
        spawned.transform.SetParent(mainGrid.transform);
        toBeDestroyedOnReset.Add(spawned);


        Tilemap roomTilemap = spawned.transform.GetChild(1).GetComponent<Tilemap>();
        roomTilemap.SetTile(new Vector3Int(spawnedPreset.roomWidth / 2, -1), null);
        roomTilemap.SetTile(new Vector3Int(spawnedPreset.roomWidth / 2, 0), null);
        roomTilemap.SetTile(new Vector3Int(spawnedPreset.roomWidth / 2, 1), null);

    }

    public GameObject GenerateNewRoom(int x, int y)
    {
        //instantiate the preset at the given position
        //set the preset's parent to the grid object
        if (roomPresets.Count > 0)
        {
            GameObject spawned = Instantiate(roomPresets[UnityEngine.Random.Range(0, roomPresets.Count)].gameObject, new Vector2(x * cellWidth, y * cellWidth), Quaternion.identity);
            RoomPreset spawnedPreset = spawned.GetComponent<RoomPreset>();

            spawned.transform.SetParent(mainGrid.transform);
            toBeDestroyedOnReset.Add(spawned); //add to be destroyed on game load
            //set this grid position to 1
            gridPositions[x, y] = 1;
            return spawned;
        }
        else
        {
            Debug.Log("Error, no room presets found");
            return null;
        }

    }
    
    private void GenerateBossRoom(int x, int y, int level)
    {
        RoomPreset bossPreset = bossRoom.GetComponent<RoomPreset>();
        GameObject bossRoomObj = Instantiate(bossRoom.gameObject, new Vector2(x * cellWidth, y * cellWidth), Quaternion.identity, mainGrid.transform);
        //GameObject boss = Instantiate(level1Boss, new Vector2(x * bossPreset.roomWidth + bossPreset.roomWidth/2, y * roomWidth + roomWidth/2), Quaternion.identity);

        string bossHealth;
        switch(level){
            case 1:
                bossHealth = "66";
                break;
            case 2:
                bossHealth = "-77";
                break;
            case 3:
                bossHealth = "101/105";
                break;
            case 4:
                bossHealth = "999999";
                break;
            default:
                bossHealth = "100";
                break;
        }

        //boss.GetComponent<DinoBoss>().health = bossHealth;
        
        
        toBeDestroyedOnReset.Add(bossRoomObj);
        //toBeDestroyedOnReset.Add(boss);

        //level1:  70
        //level2:  -101
        //level3:  101/105
        //level4:  999999
    }
}
