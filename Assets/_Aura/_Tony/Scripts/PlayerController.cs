using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]float moveSpeed;
    [SerializeField]float moveSpeedMultiplier;
    [SerializeField] float lookSensitivity;
    [SerializeField]float lookSensitivityMultiplier;
    [SerializeField] float maxLookUp, maxLookDown;
    [SerializeField] Transform eyePosition;
    [SerializeField] GameObject impactVFX;

    CharacterController characterController;
    Vector2 inputFromKeyboard;
    Vector2 inputFromMouse;

    Vector3 moveDirection;
    Vector3 moveDirectionLocal;
    float headRotStore;
    float bodyRotStore;

    [SerializeField] float fireRate;
    float fireTimer;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        characterController = GetComponent<CharacterController>();
    }
    private void Update()
    {
        fireTimer -= Time.deltaTime;
        if (Input.GetMouseButton(0) && fireTimer < 0)
        {
            ShootGun();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        Look();

        Move();
    }

    private void ShootGun()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f,0f));
        if (Physics.Raycast(ray,out RaycastHit hitInfo, 1000))
        {
            var vfx = Instantiate(impactVFX,hitInfo.point,Quaternion.identity);
            Destroy(vfx,0.5f);
        }
        fireTimer = fireRate;
    }
    private void Look()
    {
        inputFromMouse = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        bodyRotStore += inputFromMouse.x * lookSensitivity * lookSensitivityMultiplier * Time.deltaTime;
        Quaternion bodyRotation = Quaternion.Euler(Vector3.up * bodyRotStore);
        transform.rotation = bodyRotation;

        headRotStore -= inputFromMouse.y * lookSensitivity * lookSensitivityMultiplier * Time.deltaTime;
        headRotStore = Mathf.Clamp(headRotStore, maxLookUp, maxLookDown);
        Quaternion headRotation = Quaternion.Euler(new Vector3(headRotStore, 0f, 0f));
        eyePosition.localRotation = headRotation;
    }

    private void Move()
    {
        inputFromKeyboard = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        moveDirection = new Vector3(inputFromKeyboard.x, 0f, inputFromKeyboard.y);

        moveDirectionLocal = (transform.forward * moveDirection.z + transform.right * moveDirection.x).normalized * moveSpeed * moveSpeedMultiplier;

        characterController.Move(moveDirectionLocal * Time.deltaTime);
    }
}
