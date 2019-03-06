using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Manager2 : MonoBehaviour
{
    //references
    public Tiles2 tiles;
    private Draw2 draw;
    private Resources_Loader loader;
    public Send_Event[,] sendevents;
    public SpriteRenderer[,] sprrnd;
    [SerializeField]
    private ButtonInput input;
    [SerializeField]
    private Text time_indicator;
    [SerializeField]
    private Text parret_indicator;
    [SerializeField]
    private Text turn_indicator;
    [SerializeField]
    private Text resourceL_indicator;
    [SerializeField]
    private Text resourceR_indicator;

    //states
    public States state;
    private bool action_enabled;

    //time
    private float time;
    [SerializeField]
    private float cooltime;
    private float cooltimeL;
    private float cooltimeR;
    [SerializeField]
    private float process_time;
    [SerializeField]
    private float select_time;
    [SerializeField]
    private float playerLs_time;
    [SerializeField]
    private float playerRs_time;
    [SerializeField]
    private float time_gain_rate;

    //
    private int draw_state;

    //mode select
    [SerializeField]
    private bool is_discrete;

    public enum States
    {
        Wait, PlayerL, PlayerR, WinnerL, WinnerR
    }

    private void Awake()
    {
        loader = GetComponent<Resources_Loader>();
        loader.Load();
    }

    // Use this for initialization
    void Start()
    {
        tiles = GetComponent<Tiles2>();
        draw = GetComponent(typeof(Draw2)) as Draw2;
        state = States.Wait;
        tiles.Cell_init();
        tiles.Init_rules();        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        Process();
        SetText();
    }

    void CellClicked()
    {
        if (is_discrete)
        {
            draw_state = input.GetSelectedState();

            for (int _x = 0; _x < tiles.CELL_SIZE_X; _x++)
            {
                for (int _y = 0; _y < tiles.CELL_SIZE_Y; _y++)
                {
                    if(state == States.PlayerL)
                    {
                        if (sendevents[_x, _y].fire == true && action_enabled == true && tiles.GetCost(0, draw_state) <= tiles.resources[0])
                        {
                            tiles.Consume(0, tiles.cells[_x, _y], draw_state);
                            tiles.Set_Cell(tiles.cells, _x, _y, draw_state);
                            sendevents[_x, _y].fire = false;
                            action_enabled = false;
                            draw.Draw();
                        }
                    } else if (state == States.PlayerR)
                    {
                        if (sendevents[_x, _y].fire == true && action_enabled == true && tiles.GetCost(1, draw_state) <= tiles.resources[1])
                        {
                            tiles.Consume(1, tiles.cells[_x, _y], draw_state);
                            tiles.Set_Cell(tiles.cells_another, _x, _y, draw_state);
                            sendevents[_x, _y].fire = false;
                            action_enabled = false;
                            draw.Draw();
                        }
                    } 
                }
            }
        } else
        {
            for (int _x = 0; _x < tiles.CELL_SIZE_X; _x++)
            {
                for (int _y = 0; _y < tiles.CELL_SIZE_Y; _y++)
                {
                    if (state == States.PlayerL)
                    {
                        if (sendevents[_x, _y].fire == true && action_enabled == true)
                        {
                            if (tiles.CanSetPattern(_x, _y, 0, tiles.RANGE))
                            {
                                tiles.SetPattern(_x, _y, 0, draw_state);
                                sendevents[_x, _y].fire = false;
                                action_enabled = false;
                                draw.Draw();
                            }
                        }
                    } else if (state == States.PlayerR)
                    {
                        if (sendevents[_x, _y].fire == true && action_enabled == true)
                        {
                            if (tiles.CanSetPattern(_x, _y, 1, tiles.RANGE))
                            {
                                tiles.SetPattern(_x, _y, 1, draw_state);
                                sendevents[_x, _y].fire = false;
                                action_enabled = false;
                                draw.Draw();
                            }
                        }
                    }
                }
            }
        }
    }

    public void SetState(States state_in)
    {
        state = state_in;
        time = 0;
        ClearClickEvent();
        action_enabled = true;
    }

    public void Process()
    {
        switch (state)
        {
            case States.Wait:
                playerLs_time += Time.deltaTime * time_gain_rate;
                playerRs_time += Time.deltaTime * time_gain_rate;
                cooltimeL += Time.deltaTime;
                cooltimeR += Time.deltaTime;

                GetKeyWait();
                if (time >= process_time)
                {
                    tiles.Next_life1();
                    draw.Draw();
                    JudgeWinner();
                    time = 0f;
                }
                break;

            case States.PlayerL:
                if (time <= select_time && action_enabled == true && playerLs_time > 0)
                {
                    playerLs_time += -Time.deltaTime;
                    GetKeyPlayer();
                    CellClicked();
                } else
                {
                    cooltimeL = -cooltime;
                    SetState(States.Wait);
                }
                break;

            case States.PlayerR:
                if (time <= select_time && action_enabled == true && playerRs_time > 0)
                {
                    playerRs_time += -Time.deltaTime;
                    GetKeyPlayer();
                    CellClicked();
                } else
                {
                    cooltimeR = -cooltime;
                    SetState(States.Wait);
                }
                break;

            case States.WinnerL:
                if (Input.anyKeyDown)
                {
                    SceneManager.LoadScene(0);
                } 
                break;
            case States.WinnerR:
                if (Input.anyKeyDown)
                {
                    SceneManager.LoadScene(0);
                }
                break;
            default:
                break;
        }
    }

    public void GetKeyWait()
    {
        if (Input.GetKeyDown(KeyCode.Z) && playerLs_time > 0 && cooltimeL > 0)
        {
            draw.IndicateTeritory(0);
            SetState(States.PlayerL);
        } else if (Input.GetKeyDown(KeyCode.Backslash) && playerRs_time > 0 && cooltimeR > 0)
        {
            draw.IndicateTeritory(1);
            SetState(States.PlayerR);
        }
    }

    public void GetKeyPlayer()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            draw_state = 1;
        }else if(Input.GetKeyDown(KeyCode.Alpha2)){
            draw_state = 2;
        }else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            draw_state = 3;
        }else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            draw_state = 4;
        }else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            draw_state = 5;
        }
    }

    public void ClearClickEvent()
    {
        for (int _x = 0; _x < tiles.CELL_SIZE_X; _x++)
        {
            for (int _y = 0; _y < tiles.CELL_SIZE_Y; _y++)
            {
                sendevents[_x, _y].fire = false;
            }
        }
    }

    public void SetText()
    {
        if (state == States.PlayerL || state == States.PlayerR || state == States.Wait) turn_indicator.text = state.ToString() + "'sTrun";
        else turn_indicator.text = state.ToString();
        parret_indicator.text = "State" + draw_state + "is selected";
        if (state == States.PlayerL) time_indicator.text = playerLs_time.ToString();
        else if (state == States.PlayerR) time_indicator.text = playerRs_time.ToString();
        resourceL_indicator.text = "L's resource : " + tiles.resources[0];
        resourceR_indicator.text = "R's resource : " + tiles.resources[1];
    }

    public void JudgeWinner()
    {
        if (tiles.Judge() == 1) SetState(States.WinnerL);
        else if (tiles.Judge() == 2) SetState(States.WinnerR);
    }
}
