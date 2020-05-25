﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  
 *  Class purpose: Giving each ball (respectively player) values and behaviour 
**/
public class Ball : MonoBehaviour
{    
    private Rigidbody rb;
    private HexagonBehaviour occupiedTile;
    private Hexagon lastSpawnPosition;
    private Timer timer;
    private List<Vector3> positions = new List<Vector3>();
    private int playerNumber;
    private float loseHeight = -10;
    private int replayPositionCounter = 0;



    /* ------------------------------ METHODS FOR DIFFERENT STATES BEGINN ------------------------------  */



                    /* --------------- STATUS: SCENE LOADED, PLAYER GETS PREPARED ---------------  */
    /*  
     *  Preparing the ball by saving some of it's components or getting values from the map
     */

    public void GetStarted(int playerNumber)
    {
        rb = GetComponent<Rigidbody>();
        timer = this.GetComponentInChildren<Timer>();
        this.playerNumber = playerNumber;

        GameObject loseTile = GameObject.Find("Map/LoseHeight");
        loseHeight = loseTile.transform.position.y;

        StartCoroutine(Introduction());
    }



                    /* --------------- STATUS: INTRODUCTION, PLAYER HAS TO WAIT ---------------  */
    /*
     *  All the colours of the non-standard tiles will be shown
     *  When all colours have faded, then the game starts
     */
    IEnumerator Introduction()
    {
        GameObject tiles = GameObject.Find("Map/Tiles");
        TileColorsIntroduction tileColorsIntroduction = tiles.GetComponent<TileColorsIntroduction>();
        tileColorsIntroduction.DisplayTiles();
        
        // Just wait for the tiles to finish 
        while(!tileColorsIntroduction.IsFinished())
        {
            yield return new WaitForSeconds(0.2f);
        }
        GameStarts();
    }

                /* --------------- STATUS: GAME STARTED, PLAYER CAN DO SOMETHING ---------------  */
    /*  
     *  This method is called when the game starts
     *  The player gets its controls and the timer will show up
     */
    void GameStarts()
    {
        ActivatePlayerControls();
        timer.Show();
        StartCoroutine(CheckLoseCondition());
    }

                /* --------------- STATUS: PLAYER WON, PLAYER REACHED A FINISH-TILE ---------------  */
    private void PlayerWon()
    {
        
    }


                /* --------------- STATUS: PLAYER LOST, PLAYER MET A LOSE CONDITION ---------------  */
    private void PlayerLost()
    {
        GoToSpawnPosition(lastSpawnPosition);
    }



    /* ------------------------------ UPDATING AND WAITING FOR INPUT METHODS ------------------------------  */

    /*       
     *  So far just testing stuff
    **/
    void FixedUpdate()
    {
        // Save current position, not used yet
        positions.Add(transform.position);

        // Start ghost/replay --- JUST A TEST SO FAR ---
        if(Input.GetKeyDown(KeyCode.R))
        {                     
            transform.position = positions[replayPositionCounter];
            replayPositionCounter++;
        }     
    }


    /* ------------------------------ CHECKING AND ANALYSING ENVIRONMENT ------------------------------  */

    /*  
     *  The player checks, if it is standing on a tile; if true, then save the currentTile and tell the current and former tile
    **/
    void OnCollisionEnter(Collision collision)
    {        
        GameObject tile = collision.gameObject;

        if(tile.tag == "Tile")
        {
            HexagonBehaviour currentTile = tile.GetComponent<HexagonBehaviour>();
        
            if(occupiedTile != currentTile)         // Check if the former occupiedTile has changed   
            {
                if(occupiedTile != null)            // Prevent a NullReferenceException
                {
                    occupiedTile.GotUnoccupied(this);   // Tell the former occupiedTile, that this ball left
                }                    
                currentTile.GotOccupied(this);          // Tell the currentTile, that this player stands on it
                occupiedTile = currentTile;         // Save the current tile

                AnalyseHexagon(occupiedTile.GetComponent<Hexagon>());
            }            
        }
    }


    private void AnalyseHexagon(Hexagon hexagon)
    {

        if(hexagon.IsStartingTile())
        {
            timer.StartTiming();
            Debug.Log("Timer started/reseted");
            Debug.Log("Record to beat: " + timer.GetBestTime());
        }
        else if(hexagon.IsWinningTile())
        {
            timer.StopTiming();

            Debug.Log("Finish time: " + timer.GetLastFinishTime());

            if(timer.IsNewBestTime())
            {
                Debug.Log("New record");
            }
            else
            {
                Debug.Log("No new record");
            }
        }
    }


    /* 
     *  This is constantly checking if a lose condition (ball fell to deep) has met
     *  It's packed in a coroutine, so it is not called every single frame -> saves performance
     */
    IEnumerator CheckLoseCondition()
    {
        // Lose condition through falling
        for(;;)
        {
            if(loseHeight > transform.position.y)
            {
                PlayerLost();
            }
            yield return new WaitForSeconds(0.2f);
        }            
    }



    /* ------------------------------ BEHAVIOUR METHODS ------------------------------  */

    /*  
     *  Let the player spawn above the desired tile
    **/
    public void GoToSpawnPosition(Hexagon spawnTile)
    {
        float distanceAboveTile = 1f; // Should go later to a central place for all settings
        transform.position = new Vector3(spawnTile.transform.position.x, spawnTile.transform.position.y + distanceAboveTile, spawnTile.transform.position.z);
        lastSpawnPosition = spawnTile;
    }


    /*
     *  Deactivates the player attached scripts "Ball" and "AccelerometerMovement". Hence all the effects and manipulations caused by them will be absent.
    **/
    void DeactivatePlayerControls()
    {
        GetComponent<BallControls>().enabled = false;
        GetComponent<AccelorometerMovement>().enabled = false;
    }


    /*
     * Activates the player attached scripts "Ball" and "AccelerometerMovement"
     **/
    void ActivatePlayerControls()
    {
        GetComponent<BallControls>().enabled = true;
        GetComponent<AccelorometerMovement>().enabled = true;
    }


} // CLASS END