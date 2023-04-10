using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float playerSpeed;
    bool paused = false;
    Vector3 movement;
    public Joystick movementJoystick;
    public PlayerScript playerScript;

    private void Start()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        movementJoystick.gameObject.SetActive(false);
#endif
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (paused) return;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        playerScript.PlayAnimation((movement.magnitude > 0) ? PlayerScript.ANIMATIONS.Walk : PlayerScript.ANIMATIONS.Idle);
        playerScript.SetWalkAnimation();
#endif
#if UNITY_IOS || UNITY_ANDROID || UNITY_IPHONE

        //Matthew: Prioritising input manager axis instead of joystick so itd be easier 
        //for testing
        if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        {
            movement = new Vector3(movementJoystick.Horizontal, 0, movementJoystick.Vertical);
            playerScript.PlayAnimation((movement.magnitude > 0) ? PlayerScript.ANIMATIONS.Walk : PlayerScript.ANIMATIONS.Idle);
            playerScript.SetWalkAnimation();
        }
#endif
        transform.position += movement * Time.deltaTime * playerSpeed;

    }

    public Vector2 GetMovementJoyStickDirection()
    {
        return movementJoystick.Direction;
    }

    public void UpdateMovemnentSpeed(float _playerSpeed)
    {
        playerSpeed = _playerSpeed;
    }

    public void SetPaused(bool _pause)
    {
        paused = _pause;
    }
}
