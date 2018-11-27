/* Pison Gesture Manager
 * A class for basic gestures using the Pison device in Unity. 
 * Gestures include click (full activation of finger up and down), hold activation, hold and roll right,
 * hold and roll left, hold and swipe up and hold and swipe down. 
 * Hold and swipe left and right can also be implemented using con.GetYEuler() in CheckLeft() or CheckRight()
 */

using System.Collections;
using UnityEngine;

public class GestureManager : MonoBehaviour
{
    #region Variables
    [HideInInspector]
    public enum GestureState { NONE, CLICK, RIGHT, LEFT, BACK, DOWN }
    [HideInInspector]
    public enum Period { RESTING, ACTIVE, GESTURE }

    public Period State = Period.RESTING;
    public GestureState Gesture;

    public Controller con;

    public float x;
    public float z;
    public float holdTime;

    public int activeCount;

    public float initZR;
    public float initZL;
    public float currZR;
    public float currZL;
    public float deltaZR;
    public float deltaZL;
    public float zR;
    public float zL;
    private float rightLowerThreshold = 20;
    private float rightUpperThreshold = 180;
    private float leftLowerThreshold = -16;
    private float leftUpperThreshold = -160;

    public float xR;
    public float initXR;
    public float initX;
    public float currX;
    public float deltaX;
    private float upLowerThreshold = 15;
    private float upUpperThreshold = 190;

    public float xD;
    public float initXD;
    public float currXD;
    public float deltaXD;
    private float downLowerThreshold = -190;
    private float downUpperThreshold = -15;

    public bool activeOnce;
    public bool performedGesture;
    public bool freezeZR;
    public bool freezeZL;
    public bool freezeX;
    public bool freezeXD;
    public bool performUp;
    public bool performClick;
    public bool doingGesture;

    #endregion
  

    #region Unity Callbacks
    void Start()
    {
        con = FindObjectOfType<Controller>();
      
    }

    // Update is called once per frame
    void Update()
    {
        x = con.objectRotation.eulerAngles.x;
        z = con.objectRotation.eulerAngles.z;
        
        if (x < 30 || x > 80)
        {
            //hand level or below state, not event
        }
        else if (x > 30 && x < 80)
        {
            //hand up state, not event
        }

        if (con.startEnd == "START" && !activeOnce) //NORMAL ACTIVATION
        {
            State = Period.ACTIVE;
            activeCount++;
            if (activeCount == 1)
            {
                State = Period.GESTURE;
            }
            activeOnce = true;
        }
        else if (con.startEnd == "END") //NORMAL DEACTIVATION
        {
            if (activeCount > 0)
            {
                activeCount++;
                activeOnce = false;
            }

            if (activeCount == 2 && !doingGesture)//CHECK FOR A CLICK EVENT WITH NO GESTURE
            {
                Gesture = GestureState.CLICK;
            }
            else if (activeCount == 2 && doingGesture)//CHECK FOR A CLICK EVENT WITH GESTURE
            {
                ResetGestures();
            }
        }

        if (Gesture == GestureState.CLICK)
        {
            //Click separated currently to click with hand up and click with any other hand position.
            /*In order to check for a normal click, comment out the if-else statement below, then uncomment out
            * the next two lines and insert you click event logic*/

            //doingGesture = true;
            //ResetGestures

            if (x < 30 || x > 80)
            {
                //CLICK EVENT
                doingGesture = true;
                ResetGestures();
            }
            else if (x > 30 && x < 80)
            {
                //CLICK WITH HAND UP EVENT
                doingGesture = true;
                ResetGestures();
            }
        }

        if (State == Period.GESTURE)
        {
            //ONCE ACTIVATION IS HELD, GESTURE DETECTION BEGINS
            if (!performedGesture)
            {
                CheckLeft();
                CheckRight();
                CheckUp();
                CheckDown();
            }

        }

        if (State == Period.GESTURE && Gesture == GestureState.NONE && !performedGesture)
        {
            holdTime += Time.deltaTime;
            if (holdTime > 1.0f) //TIME FOR GESTURE TO BE RECOGNIZED AS A HOLD EVENT
            {
                //HOLD EVENT
            }
        }

        if (Gesture == GestureState.RIGHT && !performedGesture)
        {
            //HOLD ROTATE RIGHT ONCE EVENT
            doingGesture = true;
            performedGesture = true;
        }
        else if (Gesture == GestureState.LEFT && !performedGesture)
        {
            //HOLD ROTATE LEFT ONCE EVENT
            doingGesture = true;
            performedGesture = true;
        }
        if (Gesture == GestureState.RIGHT)
        {
            //HOLD RIGHT CONTINUOUS EVENT

            doingGesture = true;
        }
        else if (Gesture == GestureState.LEFT)
        {
            //HOLD CONTINUOUS LEFT EVENT 

            doingGesture = true;
        }
        else if (Gesture == GestureState.BACK && !performedGesture)
        {
            //HOLD RAISE UP ONCE EVENT

            doingGesture = true;
            performedGesture = true;
        }
        else if (Gesture == GestureState.DOWN && !performedGesture)
        {
            //HOLD LOWER ARM ONCE EVENT

            doingGesture = true;
            performedGesture = false;
        }
    }

    #endregion

    #region Functions
    //FUNCTION TO CHECK FOR RIGHT GESTURE
    void CheckRight()
    {
        zR = con.GetZEuler();
        if (zR >= 0 && zR < 250)
            currZR = 360 + zR;
        else
            currZR = zR;

        if (!freezeZR)
        {
            initZR = currZR;

            freezeZR = true;
        }
        deltaZR = initZR - currZR;

        if (deltaZR > rightLowerThreshold && deltaZR < rightUpperThreshold)
        {
            Gesture = GestureState.RIGHT;
        }
        else if (Gesture != GestureState.LEFT && Gesture != GestureState.BACK && Gesture != GestureState.DOWN)
        {
            Gesture = GestureState.NONE;
        }

    }

    //FUNCTION TO CHECK FOR LEFT GESTURE
    void CheckLeft()
    {
        zL = con.GetZEuler();
        if (zL >= 0 && zL < 250)
            currZL = 360 + zL;
        else
            currZL = zL;

        if (!freezeZL)
        {
            initZL = currZL;

            freezeZL = true;
        }
        deltaZL = initZL - currZL;

        if (deltaZL < leftLowerThreshold && deltaZL > leftUpperThreshold)
            Gesture = GestureState.LEFT;
        else if (Gesture != GestureState.RIGHT && Gesture != GestureState.BACK && Gesture != GestureState.DOWN)
        {
            Gesture = GestureState.NONE;
        }

    }

    //FUNCTION TO CHECK FOR UP GESTURE
    void CheckUp()
    {
        xR = con.GetXEuler();
        if (xR >= 0 && xR < 250)
            currX = 360 + xR;
        else
            currX = xR;

        if (!freezeX)
        {
            initX = currX;

            freezeX = true;
        }
        deltaX = currX - initX;

        if (deltaX > upLowerThreshold && deltaX < upUpperThreshold)
        {
            Gesture = GestureState.BACK;
        }
        else if (Gesture != GestureState.RIGHT && Gesture != GestureState.LEFT && Gesture != GestureState.DOWN)
        {
            Gesture = GestureState.NONE;
        }

    }

    //FUNCTION TO CHECK FOR DOWN GESTURE
    void CheckDown()
    {
        xD = con.GetXEuler();
        if (xD >= 0 && xD < 250)
            currXD = 360 + xD;
        else
            currXD = xD;

        if (!freezeXD)
        {
            initXD = currXD;

            freezeXD = true;
        }
        deltaXD = currXD - initXD;

        if (deltaXD > downLowerThreshold && deltaXD < downUpperThreshold)
        {
            Gesture = GestureState.DOWN;
        }
        else if (Gesture != GestureState.RIGHT && Gesture != GestureState.LEFT && Gesture != GestureState.BACK)
        {
            Gesture = GestureState.NONE;
        }
    }

    //FUNCTION TO RESET GESTURE RECOGNITION
    void ResetGestures()
    {
        activeCount = 0;
        holdTime = 0;
        StartCoroutine("Reset");
        Gesture = GestureState.NONE;
        State = Period.RESTING;
        freezeZR = false;
        freezeZL = false;
        freezeX = false;
        freezeXD = false;
    }


    //CREATE PAUSE IN BETWEEN GESTURES TO PREVENT RAPID FALSE ACTIVATIONS
    IEnumerator Reset()
    {
        yield return new WaitForSeconds(.5f);
        performedGesture = false;
        doingGesture = false;
    }
    #endregion
}

