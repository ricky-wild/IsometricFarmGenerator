using UnityEngine.UI;
using UnityEngine;
using wildlogicgames;

namespace IsometricFarmGenerator
{
    //Input.GetMouseButtonDown(0); // Left click
    //Input.GetMouseButtonDown(1); // Right click
    //Input.GetMouseButtonDown(2); // Middle click
    //public enum MouseButtonDownInputID
    //{
    //    Left_Click = 0,
    //    Right_Click = 1,
    //    Middle_Click = 2,
    //};
    public class CursorController : MonoBehaviour
    {
        [Header("Camera Manager Ref")]
        public CameraManager _cameraManager;


        [Header("Cursor Sprites")]
        [Tooltip("in order of MouseClickStateID enum found in eventmanager.cs")]
        public Sprite[] _cursorSprites; //in order of MouseClickStateID enum found in eventmanager.cs

        private RectTransform _cursorRectTrans;
        private Vector3 _cachedPosition;
        private float _cursorImgWidth, _cursorImgHeight;
        private Image _cursorImage;
        private bool _process;

        private float _minZoomDistance = 2.0f;
        private float _maxZoomDistance = 26.0f;//28.0f;
        private float _zoomSpeed = 10.0f;//3.5f;//2.0f;
        private float _mouseDownRoationSpeed = 2.0f;//1.25f;//0.5f;
        private bool _isCamRotation, _isCamPanning, _isCamZooming;
        private Vector3 _mousePointWorldPos;
        private Vector3 _mousePositionCached;

        private MouseClickStateID _cursorStateID, _prevCursorStateID;




        private float _touchDoubleTapTime = 0.3f;
        private float _touchLastTapTime = 0f;
        private int _touchCount;// = Input.touchCount;



        private void Awake() => Setup();
        private void Setup()
        {
            if (_cursorRectTrans == null) 
                _cursorRectTrans = this.gameObject.GetComponent<RectTransform>();
            if (_cursorImage == null) 
                _cursorImage = this.gameObject.GetComponent<Image>();

            _process = false;

            _isCamRotation = false;
            _isCamPanning = false;
            _isCamZooming = false;
            _mousePointWorldPos = new Vector3();
            _mousePositionCached = new Vector3();
            _cachedPosition = new Vector3();
            _mousePositionCached = Vector3.zero;
            _cursorStateID = MouseClickStateID.OnClick_Nothing;


            _cursorImgWidth = 24.0f /2;// _cursorImage.minWidth;
            _cursorImgHeight = 24.0f /2;// _cursorImage.minHeight;
        }
        public void Process(bool b, MouseClickStateID stateID) //=> _process = b;
        {
            _process = b;
            _cursorStateID = stateID;
            _prevCursorStateID = _cursorStateID;
            SetSpriteToCursor(_cursorStateID);
        }
        public bool GetMouseMiddleClick()
        {
            if (Input.GetMouseButtonDown(2)) return true; // Middle click

            return false;
        }
        public bool GetMouseFirstClick()
        {
            if (Input.GetMouseButtonUp(0)) return true; // Middle click

            return false;
        }

        public bool GetESCKeyPress()
		{
            if (Input.GetKeyDown(KeyCode.Escape)) return true;

            return false;
        }
        private void SetSpriteToCursor(MouseClickStateID stateID)// => _cursorImage.sprite = _cursorSprites[(int)stateID];
        {
            if (_cursorImage == null) _cursorImage = this.gameObject.GetComponent<Image>();
            _cursorImage.sprite = _cursorSprites[(int)stateID];
        }
        
        private void Update()
        {
            if (!_process) return;
            if (EventManager.GetEventFlag() == EventFlagID.EventFlag_BadgeReward_Process) return;
            if (EventManager.GetEventFlag() == EventFlagID.EventFlag_Hedgerow_FullPlacement_Character_Dialogue_Init) return;

            InputUpdate();

            _cachedPosition = Input.mousePosition;
            _cachedPosition.x += _cursorImgWidth;// / 2;// 0.1f;
            _cachedPosition.y += _cursorImgHeight;// / 2;// 0.1f;
            _cursorRectTrans.position = _cachedPosition;
        }

        private void InputUpdate()
        {
            //Input.GetMouseButtonDown(0); // Left click
            //Input.GetMouseButtonDown(1); // Right click
            //Input.GetMouseButtonDown(2); // Middle click



            InputCameraPanning();
            InputCameraRotation();
            InputCameraZooming();
        }
        private void InputCameraZooming()
        {
            float scrollInput = Input.mouseScrollDelta.y;

            if (Mathf.Abs(scrollInput) > 0.01f)
            {
                _isCamZooming = true;
                SetSpriteToCursor(MouseClickStateID.OnClick_Camera_Manual_Zoom);
                
            }
            else
            {
                if (_isCamZooming)
                {
                    _isCamZooming = false;
                    SetSpriteToCursor(_prevCursorStateID);
                }
            }
            if (_isCamZooming)
            {
                MouseWheelZoomInOut(scrollInput);
            }
        }
        private void InputCameraPanning()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _isCamPanning = true;
                SetSpriteToCursor(MouseClickStateID.OnClick_Camera_Manual_Pan);
                _mousePositionCached = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (_isCamPanning)
                {
                    _isCamPanning = false;
                    SetSpriteToCursor(_prevCursorStateID);
                }

            }
            if (_isCamPanning)
            {
                MouseDownRotate(ref _mousePositionCached, _mouseDownRoationSpeed);
                //MouseDownRotateAround(ref _mousePositionCached, _cameraManager.GetPivotPoint(), _mouseDownRoationSpeed);
            }
        }
        private void InputCameraRotation()
        {
            if (Input.GetMouseButtonDown(1))
            {
                _isCamRotation = true;
                SetSpriteToCursor(MouseClickStateID.OnClick_Camera_Manual_Rotate);
                _mousePositionCached = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(1))
            {
                if (_isCamRotation)
                {
                    _isCamRotation = false;
                    SetSpriteToCursor(_prevCursorStateID);
                }

            }
            if (_isCamRotation)
            {
                //MouseDownRotate(ref _mousePositionCached, _mouseDownRoationSpeed);
                MouseDownRotateAround(ref _mousePositionCached, _cameraManager.GetPivotPoint(), _mouseDownRoationSpeed);
            }
        }
        private void MouseDownRotate(ref Vector3 pos, float rotationSpeed)
        {
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 delta = currentMousePosition - pos;

            float yaw = delta.x * rotationSpeed * Time.deltaTime;
            float pitch = -delta.y * rotationSpeed * Time.deltaTime;

            // Apply rotation
            Camera.main.transform.eulerAngles += new Vector3(pitch, yaw, 0);

            // Update reference position so rotation is smooth
            pos = currentMousePosition;
        }
        private void MouseDownRotateAround(ref Vector3 pos, Vector3 pivot, float rotationSpeed)
        {
            Vector3 delta = Input.mousePosition - pos;
            float yaw = delta.x * rotationSpeed * Time.deltaTime;
            float pitch = -delta.y * rotationSpeed * Time.deltaTime;

            // Rotate around pivot
            Camera.main.transform.RotateAround(pivot, Vector3.up, yaw);
            Camera.main.transform.RotateAround(pivot, transform.right, pitch);

            // Reorient toward pivot
            Camera.main.transform.LookAt(pivot);

            pos = Input.mousePosition;
        }
        private void MouseWheelZoomInOut(float scrollInput)
        {
            Vector3 direction = Camera.main.transform.forward;
            float distance = Vector3.Distance(Camera.main.transform.position, _cameraManager.GetPivotPoint());

            // Zoom in/out
            Vector3 newPosition = Camera.main.transform.position + direction * scrollInput * _zoomSpeed * Time.deltaTime;

            // Clamp distance
            float newDistance = Vector3.Distance(newPosition, _cameraManager.GetPivotPoint());
            if (newDistance > _minZoomDistance && newDistance < _maxZoomDistance)
            {
                Camera.main.transform.position = newPosition;
            }
        }


























        //private SpriteRenderer _spriteRenderer;
        //private bool _process;
        //private Vector3 _mousePositionCached;

        //private void Awake() => Setup();
        //private void Setup()
        //{
        //    if (_spriteRenderer == null) _spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();

        //    _process = false;
        //    _mousePositionCached = new Vector3();
        //    _mousePositionCached = Vector3.zero;
        //}
        //public void Process(bool b) => _process = b;
        //private void Update()
        //{
        //    if (!_process) return;

        //    Vector3 mousePosition = Input.mousePosition;
        //    mousePosition.z = 10f;

        //    _mousePositionCached = Camera.main.ScreenToWorldPoint(mousePosition);
        //    _mousePositionCached.z = Camera.main.nearClipPlane + 1;//-14.0f;// 0f;

        //    this.transform.forward = Camera.main.transform.forward;

        //    this.transform.position = _mousePositionCached;
        //}
        //private void OnMouseDown()
        //{
        //    //print("\n" + name + " was clicked!");
        //}


    }
}
