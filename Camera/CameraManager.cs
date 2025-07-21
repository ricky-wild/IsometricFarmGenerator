using System;
using UnityEngine;
using wildlogicgames;


namespace IsometricFarmGenerator
{
	public enum CameraBehaviour
	{
		DefaultStandby = -1,
		CircleAroundPivot = 0,
		ShootAndLandTowardPivot = 1,
		CircleAroundAndZoomOut = 2,
		CircleAroundAndZoomIn = 3,


		TravelToCharacterPoint = 4,

		TravelFromAPointToBPoint = 5,
	};
	public enum CameraTimerID //Identifies array usage of timers and all associated variables
	{
		Timer_CicrleAroundPivot = 0,
		Timer_ShootAndLandTowardPivot = 1,
		Timer_CircleAroundAndZoomOut = 2,
		Timer_CircleAroundAndZoomIn = 3,
	};

	public enum CameraDistanceID
	{
		CamDist_Extra_Short = 0,
		CamDist_Short = 1,
		CmaDist_Med = 2,
		CamDist_Far = 3,
		CamDist_Extra_Far = 4,
	};

	public class CameraManager : MonoBehaviour
	{
		[Header("Field Gen Ref Plugin")]
		public FieldGenerator _fieldGenerator;

		//[Header("Isometric Angle Objs")]
		//public Transform[] _isometricViews;

		[Header("Max FOV")]
		public float _maxFOV;
		private float _altFOV;
		//15 extra small
		//28 small
		//40 med
		//48 large


		private CameraBehaviour _cameraBehaviour;
		private CameraDistanceID _cameraDistanceSettings;

		private float _baseTimeSpeed = 4.0f;//5.0f;
		private float _orbitSpeed = 25f;
		private Vector3 _orbitAxis = Vector3.up;
		private float _distance = 5f;
		private float _smoothTime = 0.3f;
		private Vector3 _isOffset;
		private Vector3 _currentVelocity;
		private Vector3 _pivotPoint, _allFieldsCenterPoint;
		private Vector3 _characterTargetPoint;
		private Vector3 _startPosition;
		private Vector3 _pointA, _pointB, _centerPoint;
		private Vector3[] _fieldGridCenterPointsCached = new Vector3[4];
		private Quaternion _startRotation, _mouseDownRotation;
		private FieldGridSize _curFieldGridSizeRef;

		private int _lastAppliedFieldId;
		private int _lastAppliedFieldWidth;

		private const int _timerCount = 1;
		private GameObject[] _timerObjs = new GameObject[1];
		private WTimer[] _timers = new WTimer[1];
		private float[] _waitTimes = new float[1];// = 10.0f;

		private bool _holdCamMovProcess;
		private bool _initCamBehaviours;
		private bool _initFOVZoomOut;
		private bool _usingDefaultSetup;
		private bool _process;

		private void Awake() => Setup();
		private void Setup()
		{
			_process = false;
			_initFOVZoomOut = false;
			_initCamBehaviours = false;
			_holdCamMovProcess = false;
			_usingDefaultSetup = false;

			_altFOV = _maxFOV / 2;

			_startPosition = this.transform.position;
			_startRotation = this.transform.rotation;

			_isOffset = new Vector3();
			_pointA = new Vector3();
			_pointB = new Vector3();

			_timerObjs[0] = new GameObject();
			_timerObjs[0].AddComponent<WTimer>();
			_timers[0] = _timerObjs[0].GetComponent<WTimer>();
			_waitTimes[0] = 8.0f;// 10.0f;

			_cameraBehaviour = CameraBehaviour.DefaultStandby;

		}

		private void InitZoomOutFOVUpdate()
        {
			if (!_initFOVZoomOut) return;

			if (_usingDefaultSetup)
			{
				if (_maxFOV != 28)
                {
					_maxFOV = 28;
					_altFOV = _maxFOV / 2;
				}

			}

			if (Camera.main.fieldOfView < _maxFOV)
			{
				Camera.main.fieldOfView += 0.5f;

			}
			else
			{
				Camera.main.fieldOfView = _maxFOV;
				_initFOVZoomOut = false;
				if (!_holdCamMovProcess) _initCamBehaviours = true;
			}
		}

		private void Update()
		{
			if (!_process) return;

			InitZoomOutFOVUpdate();
			CameraMovementUpdate();


		}





















		public void EnableToolboxVisuals(bool b)
        {
			//...ect
        }

		public void EnableStartPivotPoint(Vector3 pivotPoint, Vector3[] fieldGridCenterPoints)
		{
			_process = true;
			_initFOVZoomOut = true;

			_allFieldsCenterPoint = pivotPoint;
			_pivotPoint = pivotPoint;

			_cameraBehaviour = CameraBehaviour.CircleAroundPivot;
			////this.transform.rotation = _isometricViews[2].rotation;
			//Vector3 offset = new Vector3(0, 20, -20); // height and distance
			//Camera.main.transform.position = _pivotPoint + offset;
			//Camera.main.transform.LookAt(_pivotPoint);

			float desiredDistance = _maxFOV;//40f;// 20f;
			Vector3 isoOffset = new Vector3(-1, 1, -1).normalized * desiredDistance;
			Camera.main.transform.position = _pivotPoint + isoOffset;
			Camera.main.transform.LookAt(_pivotPoint);

			//15 extra small
			//28 small
			//40 med
			//48 large


			Camera.main.fieldOfView = 0.01f;
			//_timers[0].StartTimer(_waitTimes[0]);
			Array.Copy(fieldGridCenterPoints, _fieldGridCenterPointsCached, fieldGridCenterPoints.Length);
		}
		public void EnableStartFieldViewsPoint()
        {
			//...ect
		}
		public void ChangePivotPoint(int fieldId)//Vector3 pivotPoint)
        {
			//...ect
		}
		public void EnableFieldStartPivotPoint(int fieldId, int fieldWidth, bool usingDefaultSetup)
		{
			//...ect
		}
		public void EnableTravelToCharacterPoint(Vector3 targetPoint, FieldGridSize fieldGridSize)
		{
			_curFieldGridSizeRef = fieldGridSize;
			_holdCamMovProcess = false;
			_initCamBehaviours = true;
			_characterTargetPoint = targetPoint;
			_characterTargetPoint.y += 0.25f;
			_cameraBehaviour = CameraBehaviour.TravelToCharacterPoint;
		}
		public void EnableTravelFromAPointToBPoint(Vector3 startPos, Vector3 endPos)
        {
			//...ect
		}
		public void MouseDownRotate(ref Vector3 pos, float rotationSpeed)
        {
			//_mousePositionCached.z = Camera.main.nearClipPlane;
			//_mousePointWorldPos = Camera.main.ScreenToWorldPoint(_mousePositionCached);


			Vector3 delta = Input.mousePosition - pos;

			//delta.z = Camera.main.nearClipPlane;

			//Cache these vars.
			float yaw = delta.x * rotationSpeed * Time.deltaTime;
			float pitch = -delta.y * rotationSpeed * Time.deltaTime;

			//// Apply rotation around Y (yaw) and X (pitch)
			//transform.eulerAngles += new Vector3(pitch, yaw, 0);
			//lastMousePosition = Input.mousePosition;

			//_mouseDownRotation.x = pitch;
			//_mouseDownRotation.y = yaw;
			//_mouseDownRotation.z = 0f;

			Camera.main.transform.eulerAngles += new Vector3(pitch, yaw, 0);
		}



		private void CameraMovementUpdate()
		{
			if (_holdCamMovProcess) return;
			if (!_initCamBehaviours) return;

			switch (_cameraBehaviour)
			{
				case CameraBehaviour.CircleAroundPivot:
					CircleAroundCenterPointUpdate();
					break;
				case CameraBehaviour.ShootAndLandTowardPivot:
					ShootUpAndLandTowardTargetUpdate();
					break;
				case CameraBehaviour.CircleAroundAndZoomOut:
					CircleAroundAndZoomOutUpdate();
					break;
				case CameraBehaviour.CircleAroundAndZoomIn:
					CircleAroundAndZoomInUpdate();
					break;

				case CameraBehaviour.TravelToCharacterPoint:
					TravelToCharacterPointUpdate();
					break;

				case CameraBehaviour.TravelFromAPointToBPoint:
					TravelFromAPointToBPoint();
					break;
			}
		}
		private void TravelFromAPointToBPoint()
        {
			float horizontalInput = (Time.deltaTime * (_baseTimeSpeed / 2.25f));
			transform.position = Vector3.MoveTowards(transform.position, _pointB, _orbitSpeed * horizontalInput * Time.deltaTime);

			float freqA = 0f;
			float freqB = 0f;
			float endDist = 0.25f;// 1.5f;

			_centerPoint = (_pointA + _pointB) / 2;
			transform.LookAt(_centerPoint);

			if (Vector3.Distance(transform.position, _pointB) < endDist)//< 2.44f
			{
				_holdCamMovProcess = true;
				_initCamBehaviours = false;
				this.EnableFieldStartPivotPoint(_lastAppliedFieldId, _lastAppliedFieldWidth, _usingDefaultSetup);
				_cameraBehaviour = CameraBehaviour.DefaultStandby;
				return;
			}

		}
		private void TravelToCharacterPointUpdate()
        {
			//...ect
		}
		private void CircleAroundCenterPointUpdate()
		{
			float horizontalInput = (Time.deltaTime * (_baseTimeSpeed/2.25f));
			transform.RotateAround(_pivotPoint, _orbitAxis, _orbitSpeed * horizontalInput * Time.deltaTime);
		}
		private void ShootUpAndLandTowardTargetUpdate()
		{
			//...ect
		}
		private void CircleAroundAndZoomOutUpdate()
		{
			//...ect
		}
		public void ResetCameraToStart()
		{
			this.transform.position = _startPosition;
			this.transform.rotation = _startRotation;
		}

		public Vector3 GetPivotPoint() => _pivotPoint;
		public bool GetDefaultSetupFlag() => _usingDefaultSetup;
		public void SetDefaultSetupFlag(bool b) => _usingDefaultSetup = b;
		public CameraBehaviour GetCurrentCameraBehaviour() => _cameraBehaviour;

		public bool GetHoldCameraMoveProcessFlag() => _holdCamMovProcess;
	}
}
