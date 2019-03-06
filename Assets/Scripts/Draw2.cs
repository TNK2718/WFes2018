using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw2 : MonoBehaviour
{
    //private float time;
    //public float time_out;

    GameObject[,] tiles;
    public GameObject cells_prefab;

    Manager2 manager;


    // Use this for initialization
    void Start()
    {
        manager = GetComponent(typeof(Manager2)) as Manager2;
        manager.tiles = GetComponent(typeof(Tiles2)) as Tiles2;
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        /*time += Time.deltaTime;
        if (time >= time_out)
        {
            draw();
            time = 0f;
        }*/
    }

    void Init()
    {
        tiles = new GameObject[manager.tiles.CELL_SIZE_X, manager.tiles.CELL_SIZE_Y];
        manager.sprrnd = new SpriteRenderer[manager.tiles.CELL_SIZE_X, manager.tiles.CELL_SIZE_Y];
        manager.sendevents = new Send_Event[manager.tiles.CELL_SIZE_X, manager.tiles.CELL_SIZE_Y];

        Vector3 tmp_vec = Vector3.back;//initialization
        for (int y = 0; y < manager.tiles.CELL_SIZE_Y; y++)
        {
            for (int x = 0; x < manager.tiles.CELL_SIZE_X; x++)
            {
                tiles[x, y] = Instantiate(cells_prefab);
                manager.sprrnd[x, y] = tiles[x, y].gameObject.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
                manager.sendevents[x, y] = tiles[x, y].gameObject.GetComponent(typeof(Send_Event)) as Send_Event;

                tmp_vec.x = x + transform.position.x;
                tmp_vec.y = y + transform.position.y;
                tiles[x, y].transform.position = tmp_vec;

            }
        }
    }

    public void Draw()
    {
        for (int y = 0; y < manager.tiles.CELL_SIZE_Y; y++)
        {
            for (int x = 0; x < manager.tiles.CELL_SIZE_X; x++)
            {
                if (manager.tiles.cells[x, y] == 1)
                {
                    manager.sprrnd[x, y].color = new Color(0, 1f, 0);
                } else if (manager.tiles.cells[x, y] == 2)
                {
                    manager.sprrnd[x, y].color = new Color(1f, 0, 0);
                } else if (manager.tiles.cells[x, y] == 3)
                {
                    manager.sprrnd[x, y].color = new Color(1f, 1f, 0);
                } else if (manager.tiles.cells[x, y] == 4)
                {
                    manager.sprrnd[x, y].color = new Color(0.5f, 1f, 0);
                }
                if (manager.tiles.cells_another[x, y] == 1)
                {
                    manager.sprrnd[x, y].color = new Color(0, 0, 1f);
                } else if (manager.tiles.cells_another[x, y] == 2)
                {
                    manager.sprrnd[x, y].color = new Color(0, 1f, 1f);
                } else if (manager.tiles.cells_another[x, y] == 3)
                {
                    manager.sprrnd[x, y].color = new Color(0, 0.5f, 1f);
                } else if (manager.tiles.cells_another[x, y] == 4)
                {
                    manager.sprrnd[x, y].color = new Color(0, 1f, 0.5f);
                } else if (manager.tiles.cells[x, y] == 0 && manager.tiles.cells_another[x, y] == 0)
                {
                    manager.sprrnd[x, y].color = new Color(0, 0, 0);
                }

            }
        }
    }

    public void IndicateTeritory(int player)
    {
        for (int y = 0; y < manager.tiles.CELL_SIZE_Y; y++)
        {
            for (int x = 0; x < manager.tiles.CELL_SIZE_X; x++)
            {
                if(manager.tiles.cells[x,y] != 0 && player == 0)
                {
                    for(int i = -manager.tiles.RANGE; i <= manager.tiles.RANGE; i++)
                    {
                        for(int j = -manager.tiles.RANGE; j <= manager.tiles.RANGE; j++)
                        {
                            if(manager.tiles.Get_cell(manager.tiles.cells,x+i,y+j) == 0 &&
                                manager.tiles.Get_cell(manager.tiles.cells_another, x+i, y+j) == 0 && x+i >=0 && x+i < manager.tiles.CELL_SIZE_X && y+j>=0&&y+j<manager.tiles.CELL_SIZE_Y) manager.sprrnd[x + i, y + j].color = new Color(0.8f, 1, 0.8f);//TODO
                        }
                    }
                }
                if (manager.tiles.cells_another[x, y] != 0 && player == 1)
                {
                    for (int i = -manager.tiles.RANGE; i <= manager.tiles.RANGE; i++)
                    {
                        for (int j = -manager.tiles.RANGE; j <= manager.tiles.RANGE; j++)
                        {
                            if (manager.tiles.Get_cell(manager.tiles.cells, x + i, y + j) == 0 &&
                                manager.tiles.Get_cell(manager.tiles.cells_another, x + i, y + j) == 0 && x + i >= 0 && x + i < manager.tiles.CELL_SIZE_X && y + j >= 0 && y + j < manager.tiles.CELL_SIZE_Y) manager.sprrnd[x + i, y + j].color = new Color(0.8f,0.8f,1);//TODO
                        }
                    }
                }
            }

        }
    }
}