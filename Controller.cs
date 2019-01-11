using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// 
/// </summary>

public class Controller : MonoBehaviour {
    // State variables
    private int level = 1; // {1,2,3} else gameOver
    private Vector3 level1Position;
    private Vector3 level2Position;
    private Vector3 level3Position;
    public Canvas ui; // The canvas that follows the player.
    public RawImage copper; // Medals that show up on achievements.
    public RawImage silver;
    public RawImage gold;
    private bool cooldown = false; // activates when showing a medal for 3 seconds.
    private Stopwatch stopWatch = new Stopwatch();
    private Stopwatch uiWatcherLvl1 = new Stopwatch(); // When player looks at a button, this starts, after 3sec gaze it triggers.
    private bool cdLvl1 = false;
    private Stopwatch uiWatcherLvl2 = new Stopwatch();
    private bool cdLvl2 = false;
    private Stopwatch uiWatcherLvl3 = new Stopwatch();
    private bool cdLvl3 = false;
    private Stopwatch uiWatcherExit = new Stopwatch();
    private bool cdExit = false;


    void Move()
    {
        // To move only forward just do -.5f on Z axis.
        //transform.position += Camera.main.transform.forward * .5f * Time.deltaTime;
        Vector3 forward = transform.position;
        forward[2] += .5f * Time.deltaTime;
        transform.position = forward;
        Vector3 newUIPos = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + 4f);
        ui.transform.position = newUIPos;
    }

    // Initialization
    void Start () {
        // Set starting poition
        level1Position = new Vector3(0f, 1.9f, -62f);
        level2Position = new Vector3(0f, 1.9f, 0f);
        level3Position = new Vector3(0f, 1.9f, 45f);
        // Disable medals
        copper.enabled = false;
        silver.enabled = false;
        gold.enabled = false;


    }

    // Update is called once per frame.
    void Update () {
        /// GAME PROGRESSION
        // if game on, if checkpoint 1/2/3 if angle of head is right go forward or push z, if goal
        // show medal for 3 sec:
        // if !started: start
        // else: if delta > 3: stop

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0f));

        //Vector3 frwd = new Vector3(transform.position.x + 100f, transform.position.y + 100f, transform.position.z + 100f);
        //Ray ray = new Ray(transform.position, frwd);
        RaycastHit hit;

        switch (level)
        {
            case 1:
                // Check if facing an object with "Go1" in its name and go forward if such is the case.
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.name.Contains("Go1"))
                    {
                        Move();
                    }
                }

                // If end of level 1
                if (transform.position.z > -4)
                {
                    if (!cooldown)
                    {
                        cooldown = true;
                        copper.enabled = true;
                        stopWatch.Start();
                    }
                    else // cooldown = true (means medal is showing)
                    {
                        if (stopWatch.ElapsedMilliseconds > 3000)
                        {
                            cooldown = false;
                            stopWatch.Stop();
                            stopWatch.Reset();
                            copper.enabled = false;
                            level += 1;
                        }
                    }
                }
                break;
            case 2:
                // Check if facing an object with "Go1" in its name and go forward if such is the case.
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.name.Contains("Go1"))
                    {
                        Move();
                    }
                }

                // If end of level 1
                if (transform.position.z > 45f)
                {
                    if (!cooldown)
                    {
                        cooldown = true;
                        silver.enabled = true;
                        stopWatch.Start();
                    } else // cooldown = true (means medal is showing)
                    {
                        if (stopWatch.ElapsedMilliseconds > 3000)
                        {
                            cooldown = false;
                            stopWatch.Stop();
                            stopWatch.Reset();
                            silver.enabled = false;
                            level += 1;
                        }
                    }
                }
                break;
            case 3:
                // First go up to the edge, then once a jump is made, go forward untill you reach final point.
                if (transform.position.z < 52.5f) // If haven't reached the edge, go forward.
                {
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider.name.Contains("Go1"))
                        {
                            Move();
                        }
                    }
                }
                else if (transform.position.z < 60f) // At edge go forward by looking down.
                {
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider.name.Contains("FinalJump") || hit.collider.name.Contains("Slide") || hit.collider.name.Contains("FinalJump2"))
                        {
                            Move();
                        }
                    }
                }
                else if (transform.position.z < 82f) // Player has jumped. Go to platform center.
                {
                    Move();
                    if (transform.position.z > 78f)
                    {
                        gold.enabled = true;
                    }
                }
                else // At center of platform. Game has finished.
                {
                    if (!cooldown)
                    {
                        cooldown = true;
                        stopWatch.Start();
                    }
                    else // cooldown = true (means medal is showing)
                    {
                        if (stopWatch.ElapsedMilliseconds > 4000) // Show gold medal for 4 sec more when final destination.
                        {
                            cooldown = false;
                            stopWatch.Stop();
                            stopWatch.Reset();
                            gold.enabled = false;
                            level += 1;
                        }
                    }
                }
                break;
            default: // Game has ended.
                // Show a GG message.
                break;
        }


        /// UI INTERACTION:
        if (Physics.Raycast(ray, out hit))
        {
            /// User is looking to transport to Level 1.
            if (hit.collider.name.Contains("Level 1"))
            {
                if (!cdLvl1) // if he just started
                {
                    cdLvl1 = true;
                    uiWatcherLvl1.Start(); // start counting
                }
                else // he was looking before
                {
                    if (uiWatcherLvl1.ElapsedMilliseconds > 3000) // it has been 3 seconds
                    {
                        cdLvl1 = false;
                        uiWatcherLvl1.Stop();
                        uiWatcherLvl1.Reset();
                        transform.position = level1Position; // Move user. UI will follow.
                        Vector3 newPos = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + 4f);
                        ui.transform.position = newPos;
                        level = 1;
                    }
                }
            } else // he looked somewhere else than button 1
            {
                cdLvl1 = false; // reset cd on button 1
                uiWatcherLvl1.Stop(); // also stop timer
                uiWatcherLvl1.Reset(); // and reset
            }

            /// User is looking to transport to level 2.
            if (hit.collider.name.Contains("Level 2"))
            {
                if (!cdLvl2) // if he just started
                {
                    cdLvl2 = true;
                    uiWatcherLvl2.Start(); // start counting
                }
                else // he was looking before
                {
                    if (uiWatcherLvl2.ElapsedMilliseconds > 3000) // it has been 3 seconds
                    {
                        cdLvl2 = false;
                        uiWatcherLvl2.Stop();
                        uiWatcherLvl2.Reset();
                        transform.position = level2Position; // Move user. UI will follow.
                        Vector3 newPos = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + 4f);
                        ui.transform.position = newPos;
                        level = 2; // set level
                    }
                }
            }
            else // he looked somewhere else than button 2
            {
                cdLvl2 = false; // reset cd on button 2
                uiWatcherLvl2.Stop(); // also stop timer
                uiWatcherLvl2.Reset(); // and reset
            }

            /// User is looking to transport to level 3.
            if (hit.collider.name.Contains("Level 3"))
            {
                if (!cdLvl3) // if he just started
                {
                    cdLvl3 = true;
                    uiWatcherLvl3.Start(); // start counting
                }
                else // he was looking before
                {
                    if (uiWatcherLvl3.ElapsedMilliseconds > 3000) // it has been 3 seconds
                    {
                        cdLvl3 = false;
                        uiWatcherLvl3.Stop();
                        uiWatcherLvl3.Reset();
                        transform.position = level3Position; // Move user. UI will follow.
                        Vector3 newPos = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + 4f);
                        ui.transform.position = newPos;
                        level = 3; // set level
                    }
                }
            }
            else // he looked somewhere else than button 3
            {
                cdLvl3 = false; // reset cd on button 3
                uiWatcherLvl3.Stop(); // also stop timer
                uiWatcherLvl3.Reset(); // and reset
            }
        }


    }
}
