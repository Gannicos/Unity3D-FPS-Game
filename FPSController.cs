using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    [Header("Player External")]
    public CharacterController controller;
    public Animator anim;
    public Transform playerBody;
    public Transform playerCamera;
    public Transform groundCheck;
    public LayerMask groundMask;

    [Header("Player Controller")]
    public float walkSpeed = 4f;
    public float runSpeed = 8f;
    private float speed;

    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public float groundDistance = 0.4f;
    
    Vector3 velocity;
    public bool isGrounded;

    [Header("Camera Controller")]
    public float mouseSensitivity = 1000f;
    float xRotation = 0f;

    [Header("Weapon External")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Weapon Controller")]
    public float bulletVelocity = 200f;
    public float bulletPrefabLife = 3f;

    float x;
    float z;
    float mouseX;
    float mouseY;

    void Start(){
        speed = walkSpeed;
        anim.SetBool("isIdle", true);
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update(){
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
        Movement();

        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        View();
    }

    public void Movement(){
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0){
            velocity.y = -2f;
        }

        if(isGrounded){
            anim.SetBool("isJumping", false);
            anim.SetBool("isIdle", true);
            if(x != 0 || z != 0){
                    anim.SetBool("isIdle", false);
                    anim.SetBool("isWalking", true);
                    if (Input.GetKey(KeyCode.LeftShift)){
                        speed = runSpeed;
                        anim.SetBool("isRunning", true);
                    }else{
                        speed = walkSpeed;
                        anim.SetBool("isRunning", false);
                        if(Input.GetKeyDown(KeyCode.Mouse0)){
                            Fire();
                        }
                        if (Input.GetButtonDown("Jump")){
                            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                        }
                    }
                    if (Input.GetKey(KeyCode.LeftShift) && Input.GetButtonDown("Jump")){
                            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                    }
            }else{
                if (Input.GetButtonDown("Jump")){
                    velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                }else{
                    anim.SetBool("isWalking", false);
                    if(Input.GetKeyDown(KeyCode.Mouse0)){
                        Fire();
                    }
                }
            }
        }else{
            anim.SetBool("isJumping", true);
        }

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void View(){
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
        
    }

    private void Fire(){
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().AddForce(firePoint.forward.normalized * bulletVelocity, ForceMode.Impulse);
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLife));
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay){
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }

}
