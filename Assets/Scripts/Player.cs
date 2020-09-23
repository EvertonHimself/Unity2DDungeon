using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Importa o namespace CrossPlatformInput.
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    Rigidbody2D myRigidBody;
    [SerializeField] float runSpeed = 5f;
    Animator myAnimator;
    bool isAlive = true;
    [SerializeField] float jumpSpeed = 5f;
    //Collider2D myCollider2D;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeet;
    [SerializeField] float climbSpeed = 5f;
    // Para desligar a gravidade ao subir escadas.
    float gravityScaleAtStart;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        // Salva a escala de gravidade atual.
        gravityScaleAtStart = myRigidBody.gravityScale;
        myFeet = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Executa "Run" 60fps.
        Run();
        FlipSprite();
        Jump();
        ClimbLadder();
    }
    
    // Faz o personagem correr.
    private void Run()
    {
        // Direção na esquerda e direita.
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal"); // Valor é entre -1 e 1.
        // Velocidade com direção.
        Vector2 playerVelocity = new Vector2(controlThrow * runSpeed, myRigidBody.velocity.y);
        myRigidBody.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("Running", playerHasHorizontalSpeed);
    }

    // Gira o sprite na horizontal.
    private void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidBody.velocity.x), 1f);
        }
    }

    private void Jump()
    {
        //if (!myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            return;
        }

        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            myRigidBody.velocity += jumpVelocityToAdd;
        }
    }

    private void ClimbLadder()
    {
        //if (!myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            // Ajuste para parar a animação.
            myAnimator.SetBool("Climbing", false);
            myRigidBody.gravityScale = gravityScaleAtStart;
            return;
        }

        float controlThrow = CrossPlatformInputManager.GetAxis("Vertical");
        Vector2 climbVelocity = new Vector2(myRigidBody.velocity.x, controlThrow * climbSpeed);
        myRigidBody.velocity = climbVelocity;
        myRigidBody.gravityScale = 0f;

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("Climbing", playerHasVerticalSpeed);
    }
}
