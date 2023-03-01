using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float speed, maxSpeed, jumpPower, groundedFriction, aerialFriction;
    float currentSpeed;
    [SerializeField] PlayerAnimator playerAnimator;
    [SerializeField] Transform reflectionTrfm;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Attack[] attackColliders;

    [SerializeField] OnGround onGround;

    bool facingDirection, isOnGround, doubleJumpReady;
    const bool LEFT = true, RIGHT = false;

    int movementLock;

    [SerializeField] int attackPhase, attackCooldown, attackHitboxTimer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        if (PlayerInput.JumpPressed())
        {
            if (onGround.isOnGround)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            }
            else if (doubleJumpReady)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpPower);
                doubleJumpReady = false;
            }
        }
    }

    void FixedUpdate()
    {
        ProcessGroundedState();
        ProcessCooldowns();

        if (PlayerInput.AttackHeld() && attackCooldown < 1)
        {
            if (attackPhase == 0) //light attack 1
            {
                playerAnimator.QueAnimation(playerAnimator.LightAttack2, 12);
                LockMovement(10);

                attackCooldown = 13;
                attackHitboxTimer = 11;

                if (PlayerInput.LeftHeld() || PlayerInput.RightHeld()) { ForwardAttackShift(); }
                else { NeutralAttackShift(); }
                attackPhase = 1;
            }
            else if (attackPhase == 1) //light attack 2
            {
                playerAnimator.QueAnimation(playerAnimator.LightAttack1, 15);
                LockMovement(10);

                attackCooldown = 13;
                attackHitboxTimer = 11;

                if (PlayerInput.LeftHeld() || PlayerInput.RightHeld()) { ForwardAttackShift(); }
                else { NeutralAttackShift(); }
                attackPhase = 2;
            }
            else if (attackPhase == 2) //heavy attack
            {
                playerAnimator.QueAnimation(playerAnimator.HeavyAttack, 26);
                LockMovement(27);

                attackCooldown = 30;
                attackHitboxTimer = 14;
                if (PlayerInput.LeftHeld() || PlayerInput.RightHeld()) { ForwardAttackShift(); }
                else { NeutralAttackShift(); }
                attackPhase = 0;
            }
        }

        ProcessMovement();
    }

    void ProcessMovement()
    {
        if (movementLock > 0) { return; }
        if (PlayerInput.LeftHeld())
        {
            if (!PlayerInput.RightHeld())
            {
                AddXVelocity(-currentSpeed, -maxSpeed);
                playerAnimator.RequestAnimatorState(playerAnimator.Run);

                if (facingDirection == RIGHT)
                {
                    reflectionTrfm.localScale = new Vector3(-2.2f, 2.2f, 1);
                    facingDirection = LEFT;
                }
            }
            else
            {
                NoMovementInput();
            }
        }
        else if (PlayerInput.RightHeld())
        {
            AddXVelocity(currentSpeed, maxSpeed);
            playerAnimator.RequestAnimatorState(playerAnimator.Run);

            if (facingDirection == LEFT)
            {
                reflectionTrfm.localScale = new Vector3(2.2f, 2.2f, 1);
                facingDirection = RIGHT;
            }
        }
        else
        {
            NoMovementInput();
        }
    }

    void NoMovementInput()
    {
        playerAnimator.RequestAnimatorState(playerAnimator.Idle);
    }

    void NeutralAttackShift()
    {
        AddForwardXVelocity(speed * 5, speed * 5);
    }

    void ForwardAttackShift()
    {
        AddForwardXVelocity(speed * 6, speed * 6);
    }

    void ProcessGroundedState()
    {
        if (isOnGround != onGround.isOnGround)
        {
            isOnGround = onGround.isOnGround;

            if (isOnGround)
            {
                doubleJumpReady = true;
            }
        }

        if (isOnGround)
        {
            currentSpeed = speed;
            ApplyXFriction(groundedFriction);
        }
        else
        {
            if (rb.velocity.y > 0)
            {
                playerAnimator.QueAnimation(playerAnimator.Jump, 2) ;
            }
            else
            {
                playerAnimator.QueAnimation(playerAnimator.Fall, 2);
            }
            currentSpeed = speed * .3f;
            ApplyXFriction(aerialFriction);
        }
    }

    void ProcessCooldowns()
    {
        if (attackCooldown > -10)
        {
            if (attackCooldown == -9)
            {
                attackPhase = 0;
            }
            attackCooldown--;
        }

        if (attackHitboxTimer > 0)
        {
            if (attackHitboxTimer == 9)
            {
                if (attackPhase > 0)
                {
                    attackColliders[0].Activate(facingDirection);
                }
                else
                {
                    attackColliders[1].Activate(facingDirection);
                }
            }

            if (attackHitboxTimer == 2)
            {
                attackColliders[0].Deactivate();
                attackColliders[1].Deactivate();
            }
            attackHitboxTimer--;
        }

        if (movementLock > 0) { movementLock--; }
    }

    Vector2 vect2; //cached Vector2 to avoid declaring 'new'
    public void AddForwardXVelocity(float amount, float max)
    {
        if (facingDirection == LEFT) { AddXVelocity(-amount, -max); }
        else { AddXVelocity(amount, max); }
    }

    void AddXVelocity(float amount, float max)
    {
        vect2 = rb.velocity;

        if (amount > 0)
        {
            if (vect2.x > max)
            {
                return;
            }
            else
            {
                vect2.x += amount;
                if (vect2.x > max)
                {
                    vect2.x = max;
                }
            }
        }
        else
        {
            if (vect2.x < max)
            {
                return;
            }
            else
            {
                vect2.x += amount;
                if (vect2.x < max)
                {
                    vect2.x = max;
                }
            }
        }

        rb.velocity = vect2;
    }
    protected void ApplyXFriction(float amount)
    {
        vect2 = rb.velocity;

        if (vect2.x > 0)
        {
            vect2.x -= amount;
            if (vect2.x < 0)
            {
                vect2.x = 0;
            }
        }
        else
        {
            vect2.x += amount;
            if (vect2.x > 0)
            {
                vect2.x = 0;
            }
        }

        rb.velocity = vect2;
    }

    void LockMovement(int duration)
    {
        if (movementLock < duration) { movementLock = duration; }
    }
}
