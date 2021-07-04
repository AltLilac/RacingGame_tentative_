using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class CarController : MonoBehaviour
{
    private const string _Horizontal = "Horizontal";
    private const string _Vertical = "Vertical";

	// Inputs
    private float _horizontalInput;
    private float _verticalInput;

    private float _currentSteerAngle;	
    private float _currentBreakForce;	

    private bool _isBreaking;	// ブレーキボタンを押しているか

    [SerializeField] private float motorForce;		// 車のパワー
    [SerializeField] private float breakForce;      // ブレーキ力
	[SerializeField] private float maxSteerAngle;   // ステア角

	// タイヤのコライダー
	[SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

	// 強制停止用に公開
	public float FrontLeftWheelBreakTorque { get { return frontLeftWheelCollider.brakeTorque; } set { frontLeftWheelCollider.brakeTorque = value; } }
	public float FrontRightWheelBreakTorque { get { return frontRightWheelCollider.brakeTorque; } set { frontRightWheelCollider.brakeTorque = value; } }

	// タイヤの位置
    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheeTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    private static float _currentSpeed;		// HUD シーンへ送る速度の情報

	private Rigidbody _rb;					// 速度計算用 Rigidbody
	public Rigidbody GetCarRigidbody => _rb;

    // m/s から km/h へ変換
    private const float _ConvertValue = 3.6f;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

		this.FixedUpdateAsObservable()
			.Where(_ => CarManager.IsCarInputEnabled)
			.Subscribe(_ =>
			{
				GetInput();

				HandleMotor();

				HandleSteering();

				UpdateWheels();

				CalculateSpeed();
			});
    }

	// HUD シーンへ速度の情報を送る
    public static float GetSpeed()
    {
        return _currentSpeed;
    }

    // スピードの計算
    private void CalculateSpeed()
    {
        _currentSpeed = Mathf.Round(_rb.velocity.magnitude * _ConvertValue);
    }

    private void GetInput()
    {
        _horizontalInput = Input.GetAxis(_Horizontal);
        _verticalInput = Input.GetAxis(_Vertical);
        _isBreaking = Input.GetKey(KeyCode.Space);
    }

    // アクセル
    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = _verticalInput * motorForce;
        frontRightWheelCollider.motorTorque = _verticalInput * motorForce;

        _currentBreakForce = _isBreaking ? breakForce : 0f;
        ApplyBreaking();
    }

    // ブレーキ
    private void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = _currentBreakForce;
        frontLeftWheelCollider.brakeTorque = _currentBreakForce;
        rearLeftWheelCollider.brakeTorque = _currentBreakForce;
        rearRightWheelCollider.brakeTorque = _currentBreakForce;
    }

    // ハンドル回転
    private void HandleSteering()
    {
        _currentSteerAngle = maxSteerAngle * _horizontalInput;

        frontLeftWheelCollider.steerAngle = _currentSteerAngle;
        frontRightWheelCollider.steerAngle = _currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheeTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    // タイヤの更新
    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
}
