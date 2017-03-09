using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using System;
using System.Collections;

public class GameController : MonoBehaviour {
    public enum TimerStates { NotStarted, Started, Ended };
    public enum GameStates { NotStarted, Started, Lost, Won };
    public enum PlayerStates { Normal, Injured }
    public enum FallingStates { Standing, Climbing, Falling, Gliding};

    //probably don't want this
    public Blink BlinkMask;
    public float InjuryBlinkSpeed;

    public LavaController Lava;
    public CanvasGroup GameOverScreen;
    public CanvasGroup GameWinScreen;

    //I don't like this. It seems like this should be an enum, since these are mutually exclusive, but I can't think of a way to switch state atomicaly
    public bool PlayerIsGripping = false;
    public bool PlayerIsTouchingGround = false;


    //Falling variables
    public float MaxFallTime;
    public VignetteAndChromaticAberration FallingFilter;
    public float VignettingSpeed;
    public float BlurLevel;
    private Coroutine FallingActionCoroutine;
    public Teleporter TeleporterInstance;


    public TimerStates TimerState;
    public GameStates GameState;
    public PlayerStates PlayerState;
    public FallingStates FallingState = FallingStates.Standing;

    public float StartTime = 0; // time in seconds after the game loaded when lava was started
    public float CurrentTime = 0;


    // Use this for initialization
    void Start()
    {
        TimerState = TimerStates.NotStarted;
        GameState = GameStates.NotStarted;
        PlayerState = PlayerStates.Normal;

        GameOverScreen.alpha = 0.0f;
        GameWinScreen.alpha = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void FixedUpdate()
    {
        /*
        if (FallingState == FallingStates.Falling)
        {
            if (PlayerIsGripping)
            {
                FallingState = FallingStates.Climbing;
                ExitFallingState();
            }
            else if (!PlayerIsGripping && PlayerIsTouchingGround)
            {
                FallingState = FallingStates.Standing;
                ExitFallingState();
            }
        }

        else if (FallingState == FallingStates.Climbing)
        {
            if (!PlayerIsGripping && !PlayerIsTouchingGround)
            {
                FallingState = FallingStates.Falling;
                EnterFallingState();
            }
            else if (!PlayerIsGripping && PlayerIsTouchingGround)
            {
                FallingState = FallingStates.Standing;
            }
        }

        else if (FallingState == FallingStates.Standing)
        {
            if (PlayerIsGripping)
            {
                FallingState = FallingStates.Climbing;
            }
            else if (!PlayerIsGripping && !PlayerIsTouchingGround)
            {
                FallingState = FallingStates.Falling;
                EnterFallingState();
            }
        }
        */

    }

    public void StartTimer()
    {
        TimerState = TimerStates.Started;
        StartTime = Time.time;
    }

    public float TimeElapsed()
    {

        if (TimerState == TimerStates.Started)
        {
            CurrentTime = Time.time - StartTime;
            return Mathf.Round(CurrentTime);
        }
        else if (TimerState == TimerStates.Ended)
        {
            return Mathf.Round(CurrentTime);
        }
        else if (TimerState == TimerStates.NotStarted)
        {
            return 0;
        }
        else
        {
            return -1;
        }

    }

    public void StopTimer()
    {
        TimerState = TimerStates.Ended;
        CurrentTime = Time.time - StartTime;
    }



    public void StartGame()
    {
        if (GameState == GameStates.NotStarted)
        {
            GameState = GameStates.Started;
            StartTimer();
           // Lava.lavaEnabled = true;
            //Lava.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public void LoseGame()
    {
        if (GameState == GameStates.Started)
        {
            GameState = GameStates.Lost;
            StopTimer();
            GameOverScreen.alpha = 1.0f;
            Lava.lavaEnabled = false;
        }

    }

    public void WinGame()
    {
        if (GameState == GameStates.Started) {
            GameState = GameStates.Won;
            StopTimer();
            //Lava.lavaEnabled = false;
            TimeSpan finalTime = TimeSpan.FromSeconds(TimeElapsed());
            GameWinScreen.GetComponentInChildren<Text>().text = "You Win: " + finalTime.ToString();
            GameWinScreen.alpha = 1.0f;
        }
    }

    public void PlayerInjured()
    {
        StartCoroutine("PlayerInjuredTimer");

        Color flashColor = new Color(1f, 0f, 0f, 1f);
        float flashSpeed = InjuryBlinkSpeed;
        BlinkMask.BlinkScreen(flashColor, flashSpeed);
    }


    IEnumerator PlayerInjuredTimer()
    {
        Debug.Log("coroutine started");
        PlayerState = PlayerStates.Injured;
        Debug.Log("player injured");
        yield return new WaitForSecondsRealtime(2.0f);
        PlayerState = PlayerStates.Normal;
        Debug.Log("Player recovered");
        yield return null;
    }

    private void EnterFallingState()
    {
        FallingActionCoroutine = StartCoroutine(FallingAction());
    }

    private void ExitFallingState()
    {
        StopCoroutine(FallingActionCoroutine);
        FallingFilter.enabled = false;
    }

    IEnumerator FallingAction()
    {

        yield return new WaitForSecondsRealtime(0.25f);
        FallingFilter.enabled = true;

        float elapsedTime = 0;
        FallingFilter.intensity = 0.01f;
        FallingFilter.blurDistance = .01f;

     
        while(elapsedTime < MaxFallTime)
        {
            FallingFilter.intensity = Mathf.Min(FallingFilter.intensity + VignettingSpeed * Time.deltaTime / 10f, 1f);
            FallingFilter.blurDistance = Mathf.Min(FallingFilter.blurDistance + BlurLevel * Time.deltaTime / 10f, 1f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        TeleporterInstance.Teleport();


    }


}
