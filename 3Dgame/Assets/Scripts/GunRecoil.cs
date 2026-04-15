using UnityEngine;

public class GunRecoil : MonoBehaviour
{
    [Header("Ateş Etme")]
    public float recoilAmount = 0.15f;
    public float recoilSpeed = 15f;
    public float returnSpeed = 8f;

    [Header("Yürüyüş Sallanması")]
    public float walkBobSpeed = 8f;
    public float walkBobAmountX = 0.02f;
    public float walkBobAmountY = 0.02f;

    [Header("Dönme (Rotation)")]
    public float rotationAmount = 5f;

    private Vector3 originalPosition;
    private Vector3 targetPosition;

    private Quaternion originalRotation;

    private float bobTimer;

    void Start()
    {
        originalPosition = transform.localPosition;
        targetPosition = originalPosition;

        originalRotation = transform.localRotation;
    }

    void Update()
    {
        bool isMoving = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;

        Vector3 walkOffset = Vector3.zero;
        Quaternion rotationOffset = Quaternion.identity;

        if (isMoving)
        {
            bobTimer += Time.deltaTime * walkBobSpeed;

            float offsetX = Mathf.Sin(bobTimer) * walkBobAmountX;
            float offsetY = Mathf.Cos(bobTimer * 2f) * walkBobAmountY;

            walkOffset = new Vector3(offsetX, offsetY, 0f);

            float rotZ = Mathf.Sin(bobTimer) * rotationAmount;
            rotationOffset = Quaternion.Euler(0f, 0f, rotZ);
        }
        else
        {
            bobTimer = 0f;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        Vector3 finalTarget = targetPosition + walkOffset;

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            finalTarget,
            Time.deltaTime * recoilSpeed
        );

        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            originalRotation * rotationOffset,
            Time.deltaTime * recoilSpeed
        );

        targetPosition = Vector3.Lerp(
            targetPosition,
            originalPosition,
            Time.deltaTime * returnSpeed
        );
    }

    void Shoot()
    {
        targetPosition = originalPosition + new Vector3(0f, 0.02f, -recoilAmount);
    }
}