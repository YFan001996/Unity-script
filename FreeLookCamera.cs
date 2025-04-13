using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FreeLookCamera : MonoBehaviour
{
    [Header("移动设置")]
    [Tooltip("正常移动速度")]
    public float moveSpeed = 5.0f;
    [Tooltip("冲刺时的移动速度")]
    public float sprintSpeed = 10.0f;
    [Tooltip("移动时的加速度")]
    public float moveAcceleration = 10.0f;
    [Tooltip("移动时的减速度")]
    public float moveDeceleration = 10.0f;

    [Header("旋转设置")]
    [Tooltip("鼠标控制旋转的灵敏度")]
    public float mouseSensitivity = 2.0f;
    [Tooltip("纵向旋转的最小角度")]
    public float pitchMin = -90.0f;
    [Tooltip("纵向旋转的最大角度")]
    public float pitchMax = 90.0f;

    [Header("其他设置")]
    [Tooltip("是否启用鼠标锁定")]
    public bool lockMouse = true;

    private float currentMoveSpeed;
    private float yaw; // 绕Y轴旋转角度
    private float pitch; // 绕X轴旋转角度
    private Vector3 moveVelocity; // 移动速度
    private Vector3 targetMoveVelocity; // 目标移动速度

    private void Start()
    {
        // 初始化
        currentMoveSpeed = moveSpeed;
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;

        // 锁定鼠标光标
        if (lockMouse)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        // 处理旋转
        HandleRotation();

        // 处理移动
        HandleMovement();
    }

    private void HandleRotation()
    {
        // 获取鼠标移动量
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // 更新绕Y轴旋转角度
        yaw += mouseX;

        // 更新绕X轴旋转角度，并限制在pitchMin到pitchMax之间
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        // 应用旋转
        transform.eulerAngles = new Vector3(pitch, yaw, 0);
    }

    private void HandleMovement()
    {
        // 计算目标移动速度
        float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
        targetMoveVelocity = CalculateMoveVelocity(targetSpeed);

        // 平滑改变当前移动速度到目标移动速度
        moveVelocity = Vector3.Lerp(moveVelocity, targetMoveVelocity, moveAcceleration * Time.deltaTime);

        // 如果没有按键，减速到停止
        if (Input.GetKeyUp(KeyCode.W) && Input.GetKeyUp(KeyCode.S) && Input.GetKeyUp(KeyCode.A) && Input.GetKeyUp(KeyCode.D))
        {
            moveVelocity = Vector3.Lerp(moveVelocity, Vector3.zero, moveDeceleration * Time.deltaTime);
        }

        // 应用移动
        transform.Translate(moveVelocity * Time.deltaTime, Space.World);
    }

    private Vector3 CalculateMoveVelocity(float speed)
    {
        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDirection += -transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection += -transform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += transform.right;
        }

        moveDirection.Normalize();
        return moveDirection * speed;
    }
}