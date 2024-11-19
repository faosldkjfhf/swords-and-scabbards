using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDimensionalAnimationStateController : MonoBehaviour
{
    [Header("Controller")]
    public PlayerController playerController;


    [Header("IKController")]
    public IKConstraintController armController;

    [Header("Animator")]
    Animator animator;
    float velocityZ = 0.0f;
    float velocityX = 0.0f;
    public float acceleration = 2f;
    public float decceleration = 2f;
    public float MaxWalk = 0.5f;
    public float MaxRun = 2.0f;
    public float weight = 0;


    [SerializeField]
    private RuntimeAnimatorController animationStyle;
    bool isLightAttacking = false;
    bool isHeavyAttacking = false;
    bool isSpecialAttacking = false;
    bool isBlocking = false;

    //hashing for performance
    int velocityZHash;
    int velocityXHash;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        animator = GetComponent<Animator>();
        armController = GetComponent<IKConstraintController>();
        


        velocityZHash = Animator.StringToHash("Velocity Z");
        velocityXHash = Animator.StringToHash("Velocity X");

        if (!playerController)
        {
            Debug.LogError("PlayerController not found");
        }

        //gripWeapon();
        getAnimationStyle();
        weight = playerController.weight;
        Debug.LogError("YADAYDA");



    }

    void getAnimationStyle()
    {
        animator.runtimeAnimatorController = playerController.animationStyle;
        animationStyle = animator.runtimeAnimatorController;
    }

    void gripWeapon()
    {
        //armController.rightHandTarget = playerController.rightHandGrip;
        //if (playerController.leftHandGrip == null)
        //{
           // armController.leftHandGrabWeapon.enabled = false;
       // } else
       // {
            //armController.leftHandTarget = playerController.rightHandGrip;
       // }
        
    }

    void changeVelocity(
        bool forwardPressed,
        bool rightPressed,
        bool leftPressed,
        bool backPressed,
        bool runPressed,
        float currentMaxVelocity
    )
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

    void lockOrResetVelocity(
        bool forwardPressed,
        bool rightPressed,
        bool leftPressed,
        bool backPressed,
        bool runPressed,
        float currentMaxVelocity
    )
    {
        if (
            !rightPressed
            && !leftPressed
            && velocityX != 0
            && (velocityX > -0.05f && velocityX < 0.05)
        )
        {
            velocityX = 0.0f;
        }

        if (
            !forwardPressed
            && !backPressed
            && velocityZ != 0
            && (velocityZ > -0.05f && velocityZ < 0.05)
        )
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
        else if (
            forwardPressed
            && velocityZ < currentMaxVelocity
            && velocityZ > (currentMaxVelocity - 0.05f)
        )
        {
            velocityZ = currentMaxVelocity;
        }
    }

    public bool attacking()
    {
        return isLightAttacking && isHeavyAttacking && isSpecialAttacking && isBlocking;
    }

    // Is the player doing any attack?
    public bool isAttacking()
    {
        return isLightAttacking || isHeavyAttacking || isSpecialAttacking;
    }

    void handleAttackAnimation()
    {
        bool lightAttackPressed = playerController.GetLightAttack();
        bool heavyAttackPressed = playerController.GetHeavyAttack();
        bool weaponArtPressed = playerController.GetSpecialAttack();
        bool blockPressed = playerController.GetBlocking();

        //bool attack1Finished = stateInfo.IsName("Attacks.GreatSwordSlash") && stateInfo.normalizedTime >= 1.0f;
        //bool attack2Finished = stateInfo.IsName("Attacks.greatSwordSlash2") && stateInfo.normalizedTime >= 1.0f;
        //bool attack3Finished = stateInfo.IsName("Attacks.GreatSwordPommelStrike") && stateInfo.normalizedTime >= 1.0f;
        //bool blockFinished = stateInfo.IsName("Attacks.GreatSwordBlock2") && stateInfo.normalizedTime >= 1.0f;


        Dictionary<string, string> animationBoolMap = new Dictionary<string, string>()
        {
            { "Attack1", "isAttacking1" },
            { "Attack2", "isAttacking2" },
            { "Attack3", "isAttacking3" },
            { "Block", "isBlocking" }
        };

        // right bumper - light attack - PRIORITY
        // right trigger - heavy attack - PRIORITY
        // left trigger - weapon art


        if (lightAttackPressed && !attacking())
        {
            animator.SetBool("isAttacking1", true);
        }

        if (heavyAttackPressed && !attacking())
        {
            animator.SetBool("isAttacking2", true);
        }

        if (weaponArtPressed && !attacking())
        {
            animator.SetBool("isAttacking3", true);
        }

        if (blockPressed && !attacking())
        {
            animator.SetBool("isBlocking", true);
        }

        foreach (var entry in animationBoolMap)
        {
            string animationName = entry.Key;
            string boolParameter = entry.Value;

            // Get the state info of the current animation on the base layer
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            // Check if the animation is playing and has finished (normalizedTime >= 1.0f)
            if (stateInfo.IsName($"Attacks.{animationName}") && stateInfo.normalizedTime >= 1.0f)
            {
                animator.SetBool(boolParameter, false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(animationStyle == null)
        {
            getAnimationStyle();
        }
        isLightAttacking = animator.GetBool("isAttacking1");
        isHeavyAttacking = animator.GetBool("isAttacking2");
        isSpecialAttacking = animator.GetBool("isAttacking3");
        isBlocking = animator.GetBool("isBlocking");

        handleAttackAnimation();
        bool forwardPressed = playerController.GetMoveDirection().y > 0;
        bool leftPressed = playerController.GetMoveDirection().x < 0;
        bool rightPressed = playerController.GetMoveDirection().x > 0;
        bool backPressed = playerController.GetMoveDirection().y < 0;
        bool runPressed = playerController.GetSprinting();

        float currentMaxVelocity = runPressed ? MaxRun : MaxWalk;

        changeVelocity(
            forwardPressed,
            rightPressed,
            leftPressed,
            backPressed,
            runPressed,
            currentMaxVelocity
        );
        lockOrResetVelocity(
            forwardPressed,
            rightPressed,
            leftPressed,
            backPressed,
            runPressed,
            currentMaxVelocity
        );

        animator.SetFloat(velocityZHash, velocityZ);
        animator.SetFloat(velocityXHash, velocityX);
        UpdateAnimationSpeed();
    }
    private void UpdateAnimationSpeed()
    {
        if (animator != null)
        {
            weight = playerController.weight;
            // Set a float parameter in the Animator to influence specific animations
            animator.SetFloat("AttackSpeed", weight);
        }
    }

}
