using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDimensionalAnimationStateController : MonoBehaviour
{

    Animator animator;
    float velocityZ = 0.0f;
    float velocityX = 0.0f;
    public float acceleration = 2f;
    public float decceleration = 2f;
    public float MaxWalk = 0.5f;
    public float MaxRun = 2.0f;


    //hashing for performance
    int velocityZHash;
    int velocityXHash;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        velocityZHash = Animator.StringToHash("Velocity Z");
        velocityXHash = Animator.StringToHash("Velocity X");
    }


    void changeVelocity(bool forwardPressed, bool rightPressed, bool leftPressed, bool backPressed, bool runPressed, float currentMaxVelocity)
    {
        //going forward, allows sprint
        if (forwardPressed && velocityZ < currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * acceleration;
        }

        //going left, to allow sprint remove the !runpressed
        if (leftPressed && velocityX > -currentMaxVelocity && !runPressed)
        {
            velocityX -= Time.deltaTime * acceleration;
        }

        //going back, to allow sprint remove the !runpressed
        if (backPressed && velocityZ > -currentMaxVelocity && !runPressed)
        {
            velocityZ -= Time.deltaTime * acceleration;
        }
        //going right, to allow sprint remove the !runpressed
        if (rightPressed && velocityX < currentMaxVelocity && !runPressed)
        {
            velocityX += Time.deltaTime * acceleration;
        }

        //decllerating fter going forward
        if (!forwardPressed && velocityZ > 0.0f)
        {
            velocityZ -= Time.deltaTime * decceleration;
        }
        //decllerating fter going backward
        if (!backPressed && velocityZ < 0.0f)
        {
            velocityZ += Time.deltaTime * decceleration;
        }

        //decllerating fter going right
        if (!rightPressed && velocityX > 0.0f)
        {
            velocityX -= Time.deltaTime * decceleration;
        }

        //decllerating fter going left
        if (!leftPressed && velocityX < 0.0f)
        {
            velocityX += Time.deltaTime * decceleration;
        }
    }

    void lockOrResetVelocity(bool forwardPressed, bool rightPressed, bool leftPressed, bool backPressed, bool runPressed, float currentMaxVelocity)
    {
        if (!rightPressed && !leftPressed && velocityX != 0 && (velocityX > -0.05f && velocityX < 0.05))
        {
            velocityX = 0.0f;
        }


        if (!forwardPressed && !backPressed && velocityZ != 0 && (velocityZ > -0.05f && velocityZ < 0.05))
        {
            velocityZ = 0.0f;
        }

        if (forwardPressed && runPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ = currentMaxVelocity;
        }
        else if (forwardPressed && velocityZ > currentMaxVelocity)
        {


            velocityZ -= Time.deltaTime * decceleration;

            if (velocityZ > currentMaxVelocity && velocityZ < (currentMaxVelocity + 0.05f))
            {
                velocityZ = currentMaxVelocity;
            }

        }
        else if (forwardPressed && velocityZ < currentMaxVelocity && velocityZ > (currentMaxVelocity - 0.05f))
        {
            velocityZ = currentMaxVelocity;
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool forwardPressed = Input.GetKey(KeyCode.W);
        bool leftPressed = Input.GetKey(KeyCode.A);
        bool rightPressed = Input.GetKey(KeyCode.D);
        bool backPressed = Input.GetKey(KeyCode.S);
        bool runPressed = Input.GetKey(KeyCode.LeftShift);


        float currentMaxVelocity = runPressed ? MaxRun : MaxWalk;

        changeVelocity(forwardPressed, rightPressed, leftPressed, backPressed, runPressed, currentMaxVelocity);
        lockOrResetVelocity(forwardPressed, rightPressed, leftPressed, backPressed, runPressed, currentMaxVelocity);





        animator.SetFloat(velocityZHash, velocityZ);
        animator.SetFloat(velocityXHash, velocityX);
    }
}
