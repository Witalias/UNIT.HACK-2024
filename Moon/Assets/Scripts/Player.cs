using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class Player : MonoBehaviour
{
    private Animator animator;
    private CharacterController charController;
    private float fallSpeed;
    private float originalStepOffset;

    // for coyote jump 
    private float jumpCoyoteTime = 0.2f;
    private float coyoteCounter;

    //jump Buffering
    private float jumpBufferTime = 0.1f;
    private float jumpBufferCounter;

    //Player
    public float speed;
    public float fastspeed; //shift
    private float speednow;
    public float rotationSpeed;
    public float jumpSpeed;

    //Shoot
    private float bulletTime;
    public GameObject heroBullet;
    public Transform spawnHBullets;
    public float bulletSpeed;

    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private GameObject _aimCamera;
    [SerializeField] private float _aimMovementOffset;
    [SerializeField] private float _shootDelay;

    private bool _isAim;
    private bool _canShoot = true;

    //Ladder
    // public Transform chTransform;
    // bool inside = false;
    // public float speedUpDown = 3.2f;
    // public RaycastHit raycastHit;
    // public bool isClimbingLadder;
    // private Vector3 lastGrabLadderDirection;

    void Start()
    {
        charController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        originalStepOffset = charController.stepOffset;

        speednow = speed;

        //inside = false;
    }

    private void OnEnable()
    {
        Garden.OnInteract += PlayInteractAnimation;
        ItemObject.OnPicked += PlayInteractAnimation;
        HealthBar.OnEated += PlayInteractAnimation;
        Garden.PlayerIsAim += GetIsAim;
        InteractableWithInfo.PlayerIsAim += GetIsAim;
    }

    private void OnDisable()
    {
        Garden.OnInteract -= PlayInteractAnimation;
        ItemObject.OnPicked -= PlayInteractAnimation;
        HealthBar.OnEated -= PlayInteractAnimation;
        Garden.PlayerIsAim -= GetIsAim;
        InteractableWithInfo.PlayerIsAim -= GetIsAim;
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticallInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticallInput);
        float magnitude = Mathf.Clamp01(movementDirection.magnitude) * speednow; //for gamepads
        var offset = 0.0f;
        if (_isAim)
            offset = _aimMovementOffset;
        movementDirection = Quaternion.AngleAxis(_cameraTransform.eulerAngles.y - offset, Vector3.up) * movementDirection;
        movementDirection.Normalize();

        fallSpeed += Physics.gravity.y * Time.deltaTime;

        //jump buffer time
        if (Input.GetButtonDown("Jump"))
        {
            animator.SetBool("jumping", true);
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // coyote time
        if (charController.isGrounded)
        {
            coyoteCounter = jumpCoyoteTime;
            charController.stepOffset = originalStepOffset;
            fallSpeed = -0.5f;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
            charController.stepOffset = 0;
        }

        if (coyoteCounter > 0f && jumpBufferCounter > 0f)
        {
            fallSpeed = jumpSpeed;
            jumpBufferCounter = 0f;
        }


        Vector3 velocity = movementDirection * magnitude;
        velocity.y = fallSpeed;

        charController.Move(velocity * Time.deltaTime);

        if (movementDirection != Vector3.zero)  //Turn
        {
            animator.SetBool("Walk", true);

            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("Walk", false);
        }

        if (Input.GetButtonDown("Fire1") && _isAim)
        {
            Shoot();
        }

        if (_isAim)
        {
            transform.eulerAngles = new Vector3(0.0f, _cameraTransform.eulerAngles.y, 0.0f);
        }
        if (Input.GetButtonDown("Fire2"))
        {
            //var angle = Quaternion.LookRotation(_cameraTransform.position - transform.position).eulerAngles;
            //transform.eulerAngles = new Vector3(0.0f, _cameraTransform.eulerAngles.y, 0.0f);
            _isAim = true;
            animator.SetBool("Aim", true);
            _aimCamera.SetActive(true);
        }
        if (Input.GetButtonUp("Fire2"))
        {
            _isAim = false;
            animator.SetBool("Aim", false);
            _aimCamera.SetActive(false);
        }

        SetRunState(Input.GetKey(KeyCode.LeftShift));
    }

    public void PlayStep()
    {
        AudioManager.Instanse.Play(AudioType.Step);
    }

    void Shoot()
    {
        if (_canShoot)
        {
            var direction = spawnHBullets.forward;
            var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            if (Physics.Raycast(ray, out var hit, 100.0f))
            {
                direction = hit.point - spawnHBullets.position;
            }
            var bullet = Instantiate(heroBullet, spawnHBullets.position, spawnHBullets.rotation);
            bullet.GetComponent<Rigidbody>().velocity = direction * bulletSpeed;
            animator.SetTrigger("Shoot");
            AudioManager.Instanse.Play(AudioType.Shoot);
            _canShoot = false;
            DOVirtual.DelayedCall(_shootDelay, () => _canShoot = true);
        }
    }

    void SetRunState(bool value)
    {
        animator.SetBool("Run", value);
        speednow = value ? fastspeed : speed;
    }

    private void PlayInteractAnimation()
    {
        animator.SetTrigger("Interact");
    }

    private bool GetIsAim() => _isAim;

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
