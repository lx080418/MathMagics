using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitboxGrid : MonoBehaviour
{
    public static HitboxGrid Singleton;
    [SerializeField] private GameObject hitboxGridHolder;

    [Header("3x3 Hitbox Images")]
    [SerializeField] private Image[] images;



    //* --------- Internal -------- */
    private Image[,] hitboxImages = new Image[3,3];
    private Vector2Int[] directionVectors = new Vector2Int[] {Vector2Int.down, Vector2Int.left, Vector2Int.right, Vector2Int.up};


    public Pair currentPosition = new Pair(0, 0);
    private int[,] currentWeaponGrid; 

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        SeedHitboxArray();
        //Initialize(new int[3,3] { {0, 0, 0}, {1, 0, 0}, {0, 0, 0}});
        //SelectLocation(currentPosition);
    }

    public void Initialize(int[,] ints)
    {
        int width = ints.GetLength(0);
        int height = ints.GetLength(1);
        
        //First, disable everything just to be sure.
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                hitboxImages[i,j].enabled = false;
            }

        }

        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                
                if(ints[i,j] == 0 && !hitboxImages[i,j].enabled == true) //if this spot is a zero, AND hasn't been enabled yet, disable it
                {
                    //hitboxImages[i,j].color = Color.red;
                    hitboxImages[i,j].enabled = false;
                }
                else if (ints[i,j] == 1)
                {
                    //Enable all adjacent images that are in bounds and not already enabled
                    hitboxImages[i,j].enabled = true;
                    hitboxImages[i,j].color = Color.black;
                    foreach(Vector2Int vec in directionVectors)
                    {
                        //If the proposed new position would be out of bounds, continue
                        if(i + vec.x < 0 || i + vec.x >= width || j + vec.y < 0 || j + vec.y >= height) continue;
                        hitboxImages[i+vec.x, j+vec.y].enabled = true;
                        hitboxImages[i+vec.x, j+vec.y].color = Color.grey;
                    }
                    
                }
            }
        }
        currentWeaponGrid = ints;
        hitboxGridHolder.SetActive(true);
    }

    private void SeedHitboxArray()
    {
        int width = hitboxImages.GetLength(0);
        int height = hitboxImages.GetLength(1);

        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                hitboxImages[i,j] = images[i*height+j];
            }
        }
    }

    public void Clear()
    {
        hitboxGridHolder.SetActive(false);
        currentPosition = new Pair(0,0);
        currentWeaponGrid = null;
    }

    private bool IsValidPosition(Vector2Int pos)
    {
        // Only positions adjacent to a 1 are selectable (not the 1s themselves)
        if(currentWeaponGrid == null) return false;
        if(currentWeaponGrid[pos.x, pos.y] == 1) return false;

        int width = hitboxImages.GetLength(0);
        int height = hitboxImages.GetLength(1);
        foreach(Vector2Int vec in directionVectors)
        {
            
            if(pos.x + vec.x < 0 || pos.x + vec.x >= width || pos.y + vec.y < 0 || pos.y + vec.y >= height) continue;
            if(currentWeaponGrid[pos.x + vec.x, pos.y + vec.y] == 1) return true;
            else continue;
        }
        return false;
    }

    public void Move(Vector2 dir)
    {
        Debug.Log($"[HitboxGrid] Moving with {dir}. + {currentPosition.x}, {currentPosition.y}");

        //Adjust for 2d array [row, column] alignment, correctedDir is the "2d array version" of the movement.
        Vector2 correctedDir = new Vector2Int((int)dir.y*-1, (int)dir.x);
        int width = hitboxImages.GetLength(0);
        int height = hitboxImages.GetLength(1);

        int row = currentPosition.x;
        int col = currentPosition.y;
        Vector2Int? targetPos = null;

        // Search along the pressed direction in the current row/column for the closest valid position
        if (correctedDir.x != 0)
        {
            int step = correctedDir.x < 0 ? -1 : 1;
            for (int r = row + step; r >= 0 && r < width; r += step)
            {
                var pos = new Vector2Int(r, col);
                if (IsValidPosition(pos)) { targetPos = pos; break; }
            }
        }
        else if (correctedDir.y != 0)
        {
            int step = correctedDir.y < 0 ? -1 : 1;
            for (int c = col + step; c >= 0 && c < height; c += step)
            {
                var pos = new Vector2Int(row, c);
                if (IsValidPosition(pos)) { targetPos = pos; break; }
            }
        }

        // If nothing found, scan the entire grid top-left to bottom-right for the first valid position
        if (!targetPos.HasValue)
        {
            for (int r = 0; r < width && !targetPos.HasValue; r++)
            {
                for (int c = 0; c < height; c++)
                {
                    var pos = new Vector2Int(r, c);
                    if ((pos.x != row || pos.y != col) && IsValidPosition(pos)) { targetPos = pos; break; }
                }
            }
        }

        if (targetPos.HasValue)
        {
            hitboxImages[currentPosition.x, currentPosition.y].color = Color.grey;
            currentPosition.x = targetPos.Value.x;
            currentPosition.y = targetPos.Value.y;
            Debug.Log($"{currentPosition.x}, {currentPosition.y}");
            SelectLocation(currentPosition);
        }
    }

    private void SelectLocation(Pair loc)
    {
        hitboxImages[loc.x, loc.y].color = Color.green;
    }


    

}

public struct Pair
    {
        public int x;
        public int y;

        public Pair(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
