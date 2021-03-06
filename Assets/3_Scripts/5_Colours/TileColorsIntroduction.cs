﻿using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileColorsIntroduction : MonoBehaviour
{
    
    private const float TIME_TO_CHECK_AGAIN = 0.06f; // Waiting time for Coroutines


    /* ------------------------------ COLOUR INDICATIONS OF LIST TILES AT MAP START ------------------------------  */
    [SerializeField] private bool cameraFollowCrackedTiles;
    [SerializeField] private float crackedTilesColorStartTime;
    [SerializeField] private float crackedTilesColorTime;
    [SerializeField] private float crackedTimeBeforeFading;
    [SerializeField] private float crackedTilesColorFading;
    [SerializeField] private Color crackedTilesColor;


    [SerializeField] private bool cameraFollowPathTiles;
    [SerializeField] private float pathTilesColorStartTime;
    [SerializeField] private float pathTilesColorTime;
    [SerializeField] private float pathTimeBeforeFading;
    [SerializeField] private float pathTilesColorFading;
    [SerializeField] private Color pathTilesColor;    
    
    
    [SerializeField] private bool cameraFollowDistractionTiles;
    [SerializeField] private float distractionTilesColorStartTime;
    [SerializeField] private float distractionTilesColorTime;
    [SerializeField] private float distractionTimeBeforeFading;
    [SerializeField] private float distractionTilesColorFading;
    [SerializeField] private Color distractionTilesColor;    


    [SerializeField] private bool cameraFollowCheckpointTiles;
    [SerializeField] private float checkpointTilesColorStartTime;
    [SerializeField] private float checkpointTilesColorTime;
    [SerializeField] private float checkpointTimeBeforeFading;
    [SerializeField] private float checkpointTilesColorFading;
    [SerializeField] private Color checkpointTilesColor;


    [SerializeField] private bool cameraFollowSpecialTiles;
    [SerializeField] private float specialTilesColorStartTime;
    [SerializeField] private float specialTilesColorTime;
    [SerializeField] private float specialTimeBeforeFading;
    [SerializeField] private float specialTilesColorFading;
    [SerializeField] private Color specialTilesColor;


    [SerializeField] private bool cameraFollowMovingTiles;
    [SerializeField] private float movingTilesColorStartTime;
    [SerializeField] private float movingTilesColorTime;
    [SerializeField] private float movingTimeBeforeFading;
    [SerializeField] private float movingTilesColorFading;
    [SerializeField] private Color movingTilesColor;


    [SerializeField] private bool cameraFollowStartingTiles;
    [SerializeField] private float startingTilesColorStartTime;
    [SerializeField] private float startingTilesColorTime;
    [SerializeField] private float startingTimeBeforeFading;
    [SerializeField] private float startingTilesColorFading;
    [SerializeField] private Color startingTilesColor;


    [SerializeField] private bool cameraFollowWinningTiles;
    [SerializeField] private float winningTilesColorStartTime;
    [SerializeField] private float winningTilesColorTime;
    [SerializeField] private float winningTimeBeforeFading;
    [SerializeField] private float winningTilesColorFading;
    [SerializeField] private Color winningTilesColor;

    
    private Tiles tiles;
    private SkipButton skipButton;
    private List<IntroductionTilesManager> colors = new List<IntroductionTilesManager>();
    private IntroductionTilesManager checkpoints;
    private bool checkpointsHasBeenMarked = false;
    Dictionary<Hexagon, Color> tileColors;

    // Filling the array and setting up the tiles
    public void GetStarted(Dictionary<Hexagon, Color> tileColors)
    {
        tiles = GetComponent<Tiles>();
        this.tileColors = tileColors;

        colors.Add(new IntroductionTilesManager(crackedTilesColorStartTime, crackedTilesColorTime, crackedTimeBeforeFading, crackedTilesColorFading, crackedTilesColor, tiles.GetCrackedTiles(), cameraFollowCrackedTiles));
        colors.Add(new IntroductionTilesManager(distractionTilesColorStartTime, distractionTilesColorTime, distractionTimeBeforeFading, distractionTilesColorFading, distractionTilesColor, tiles.GetDistractionTiles(), cameraFollowDistractionTiles));
        colors.Add(new IntroductionTilesManager(specialTilesColorStartTime, specialTilesColorTime, specialTimeBeforeFading, specialTilesColorFading, specialTilesColor, tiles.GetSpecialTiles(), cameraFollowSpecialTiles));
        colors.Add(new IntroductionTilesManager(movingTilesColorStartTime, movingTilesColorTime, movingTimeBeforeFading, movingTilesColorFading, movingTilesColor, tiles.GetMovingTiles(), cameraFollowMovingTiles));
        colors.Add(new IntroductionTilesManager(startingTilesColorStartTime, startingTilesColorTime, startingTimeBeforeFading, startingTilesColorFading, startingTilesColor, tiles.GetStartingTiles(), cameraFollowStartingTiles));
        colors.Add(new IntroductionTilesManager(pathTilesColorStartTime, pathTilesColorTime, pathTimeBeforeFading, pathTilesColorFading, pathTilesColor, tiles.GetPathTiles(), cameraFollowPathTiles));
        colors.Add(new IntroductionTilesManager(winningTilesColorStartTime, winningTilesColorTime, winningTimeBeforeFading, winningTilesColorFading, winningTilesColor, tiles.GetWinningTiles(), cameraFollowWinningTiles));
        
        this.checkpoints = new IntroductionTilesManager(checkpointTilesColorStartTime, checkpointTilesColorTime, checkpointTimeBeforeFading, checkpointTilesColorFading, checkpointTilesColor, tiles.GetCheckpointTiles(), cameraFollowCheckpointTiles);
        colors.Add(checkpoints);
    }

    public void DisplayTiles(bool chooseCheckPoints, SkipButton skipButton, CameraFollow cam)
    {
        this.skipButton = skipButton;
        if(chooseCheckPoints)
        {
            colors.Remove(checkpoints);
        }

        for(int i = 0; i < colors.Count; i++)
        {
            StartCoroutine(SetColor(colors[i], chooseCheckPoints, cam));
        }
    }

    /*  Tutorial: https://answers.unity.com/questions/1604527/instantiate-an-array-of-gameobjects-with-a-time-de.html
     *  This method will colour all the incoming tiles
     *  Works : tiles are colored in with a delay
    **/
    private IEnumerator SetColor(IntroductionTilesManager tileOptions, bool waitForChoosingCheckPoints, CameraFollow cam)
    {        
        float stopTime = Time.fixedTime + tileOptions.GetStartingTime(); // wait for the specified seconds
        while(stopTime > Time.fixedTime && !skipButton.IsButtonPressed()) // if button was pressed, then continue instantly
        {            
            yield return new WaitForSeconds(TIME_TO_CHECK_AGAIN);
        }
        
        Dictionary<int, List<Hexagon>> colorList = tileOptions.GetTiles();        
        int numberOfLists = colorList.Count;
        int countActualSteps = 0;        

        for(int i = 0; i < numberOfLists; i++)
        {
            if(colorList.TryGetValue(i, out List<Hexagon> hexagonList)) // if the key is available, then just procceed
            {
                
                List<Hexagon> hexagons = hexagonList;
            
                for(int k = 0; k < hexagons.Count; k++)
                {                    
                    hexagons[k].SetColor(tileOptions.GetColor());
                    if(tileOptions.CameraShouldFollow())
                    {
                        cam.SetTarget(hexagons[k].transform);
                    }
                    
                    if(countActualSteps == colorList.Count - 1)
                    {                        
                        cam.GetBackInPosition();                        
                        while(!cam.GetCameraReachedFinalPosition() && !skipButton.IsButtonPressed())  // wait for the user to finish watching the introduction screen
                        {
                            yield return new WaitForSeconds(TIME_TO_CHECK_AGAIN);
                        }
                    }
                }

                stopTime = Time.fixedTime + tileOptions.GetTimeToNextTile(); // wait for the specified seconds
                while(stopTime > Time.fixedTime && !skipButton.IsButtonPressed()) // if button was pressed, then continue instantly
                {
                    yield return new WaitForSeconds(TIME_TO_CHECK_AGAIN);
                }
                countActualSteps++;             
            }
            else
            {
                numberOfLists++;    // if this key doesn't exist, increase the number to keep looking for every available list
            }
        }        
        tileOptions.SetReadyForCheckpoints(true);

        while(!IsReadyForCheckpoints())
        {
            yield return new WaitForSeconds(TIME_TO_CHECK_AGAIN);
        }

        if(waitForChoosingCheckPoints)
        {
            while(!checkpointsHasBeenMarked)
            {                
                yield return new WaitForSeconds(TIME_TO_CHECK_AGAIN);
            }
        }

        StartCoroutine(MakePathDisappear(tileOptions, cam)); //start the disappering backwards
    }

    /* Tutorial: https://answers.unity.com/questions/1604527/instantiate-an-array-of-gameobjects-with-a-time-de.html
    * Method goes trough the Dictionary in the opposite ordner and coulors them back to the old colour
    */
    private IEnumerator MakePathDisappear(IntroductionTilesManager tileOptions, CameraFollow cam)
    {        
        Dictionary<int, List<Hexagon>> colorList = tileOptions.GetTiles();

        float stopTime = Time.fixedTime + tileOptions.GetTimeBeforeFadingStarts(); // wait for the specified seconds
        while(stopTime > Time.fixedTime && !skipButton.IsButtonPressed()) // if button was pressed, then continue instantly
        {
            yield return new WaitForSeconds(TIME_TO_CHECK_AGAIN);
        }
      
      
        int highestKey = 0;
        
        try
        {
            highestKey = (int) colorList.Keys.Max();
        }
        catch
        {
            // find an idea how to deal with the problem of tiles.Keys.Max(), if there's no value at all
        }

        for(int i = highestKey; i >= 0 ; i--)
        {
            if(colorList.TryGetValue(i, out List<Hexagon> hexagonList)) // if the key is available, then procceed
            {
                for(int k = 0; k < hexagonList.Count; k++)
                {
                    Hexagon hexagon = hexagonList[k];
                    
                    if(tileOptions.CameraShouldFollow())
                    {
                        cam.SetTarget(hexagon.transform);
                    } 

                    if(!hexagon.IsCheckpointTile())  // if there was no choosing of checkpoints or the hexagon is not a checkpoint anyway, then get it's colour back
                    {
                        if(tileColors.TryGetValue(hexagon, out Color hexagonColor))
                        {
                            hexagon.SetColor(hexagonColor);
                        }
                    }                    
                }

                stopTime = Time.fixedTime + tileOptions.GetTimeForEachTileFading(); // wait for the specified seconds
                while(stopTime > Time.fixedTime && !skipButton.IsButtonPressed()) // if button was pressed, then continue instantly
                {
                    yield return new WaitForSeconds(TIME_TO_CHECK_AGAIN);
                }                
            }
        }
        tileOptions.SetFinished(true);
    }


    /**
      * Method is checking, if all the coroutines have finished
     */
    public bool IsFinished()
    {
        for(int i = 0; i < colors.Count; i++)
        {
            if(!colors[i].IsFinished())
            {
                return false;
            }
        }
        return true;
    }

    public bool IsReadyForCheckpoints()
    {
        for(int i = 0; i < colors.Count; i++)
        {
            if(!colors[i].IsReadyForCheckpoints())
            {
                return false;
            }
        }
        return true;
    }


    public void Finish()
    {
        checkpointsHasBeenMarked = true;
    }


    /**
      * Abstract data class for an easier way of managing the multiple coroutines above
     */
    private class IntroductionTilesManager
    {
        private float startingTime;
        private float timeToNextTile;
        private float timeBeforeFadingStarts;
        private float timeForEachTileFading;
        private Color color;
        private bool letCameraFollow;
        private Dictionary<int, List<Hexagon>> tiles;
        private bool finished = false;
        private bool readyForCheckpoints = false;


        public IntroductionTilesManager (float startingTime, float timeToNextTile, float timeBeforeFadingStarts, 
                                float timeForEachTileFading, Color color, Dictionary<int, List<Hexagon>> tiles, bool letCameraFollow)
        {
            this.startingTime = startingTime;
            this.timeToNextTile = timeToNextTile;
            this.timeBeforeFadingStarts = timeBeforeFadingStarts;
            this.timeForEachTileFading = timeForEachTileFading;
            this.letCameraFollow = letCameraFollow;
            this.color = color; 
            this.tiles = tiles;
        }

        public float GetStartingTime()
        {
            return startingTime;
        }

        public float GetTimeToNextTile()
        {
            return timeToNextTile;
        }

        public float GetTimeBeforeFadingStarts()
        {
            return timeBeforeFadingStarts;
        }

        public float GetTimeForEachTileFading()
        {
            return timeForEachTileFading;
        }

        public Color GetColor()
        {
            return color;
        }

        public Dictionary<int, List<Hexagon>> GetTiles()
        {
            return tiles;
        }

        public bool IsFinished()
        {
            return finished;
        }

        public bool CameraShouldFollow()
        {
            return letCameraFollow;
        }

        public void SetFinished(bool status)
        {
            finished = status;
        }        

        public void SetReadyForCheckpoints(bool status)
        {
            readyForCheckpoints = status;            
        }

        public bool IsReadyForCheckpoints()
        {
            return readyForCheckpoints;
        }
    }
}