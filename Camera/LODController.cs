using System;
using UnityEngine;
using wildlogicgames;


namespace IsometricFarmGenerator
{
    [RequireComponent(typeof(LODGroup))]
    public class LODController : MonoBehaviour
    {
        [Tooltip("Distances at which each LOD activates. Index 0 = LOD0, 1 = LOD1, etc.")]
        private static readonly float[][] _LODOptions = new float[][]
        {
            new float[] { 18f, 26f, 22f, 39f }, // V Low
            new float[] { 18f, 29f, 36f, 44f }, // Low
            new float[] { 22f, 32f, 40f, 45f }, // Med
            new float[] { 24f, 36f, 50f, 60f }, // High
        };
        private float[] _LODDistances = new float[] { 20f, 30f, 40f, 50f };

        //performance modes
        // LOW { 15f, 25f, 30f, 45f };
        // MED { 20f, 30f, 40f, 50f };
        // HIGH { 20f, 30f, 45f, 60f };

        public bool _hasChildRenderers;

        private float[] _screenHeightThresholds = { 0.02f, 0.015f, 0.01f }; //{ 0.0175f, 0.0150f, 0.0125f };
        private Camera _camera;
        //private Renderer _renderer;
        //private Renderer[] _renderers;
        //private Bounds _bounds, _combinedBounds;// = renderer.bounds;
        //private Vector3 _boundsCenter;// = bounds.center;
        //private Vector3 _extents;// = bounds.extents;

        private LODGroup _LODGroup;
        private Transform _transform, _cameraTransform;
        private float _dist;//, _screenHeightPercentage;
        private int _LODLevel;
        //private bool _hasBeenPerformancedAdjusted;
        //private CameraManager _cameraManager;

        private void Setup()
        {
            _LODGroup = this.gameObject.GetComponent<LODGroup>();
            if (Camera.main != null)
                _cameraTransform = Camera.main.transform;
            else
                Debug.LogWarning("ManualLODController: No main camera found.");

            _transform = this.transform;

            //if (_hasChildRenderers)
            //    _renderers = this.gameObject.GetComponentsInChildren<Renderer>();
            //else
            //{
            //    _renderers = new Renderer[1];
            //    _renderers[0] = this.gameObject.GetComponent<Renderer>();
            //}

            //else
            //    _renderer = this.gameObject.GetComponent<Renderer>();

            //if (_renderers == null)
            //{

            //    print("\n" + this.gameObject.name);
            //    _renderers[0] = this.gameObject.GetComponent<Renderer>();
            //}

            //if (_cameraManager == null)
            //{
            //_cameraManager = FindObjectOfType<CameraManager>();
            //_cameraTransform.gameObject.GetComponent<CameraManager>();

            //if (_cameraManager == null)
            //    _cameraManager = FindObjectOfType<CameraManager>();

            //}
            //_hasBeenPerformancedAdjusted = false;
            //_screenHeightPercentage = 0.0f;

        }
        void Start() => Setup();

        //void Update()
        //{
        //    //if (!this.gameObject.activeSelf) return;
        //    if (!_hasBeenPerformancedAdjusted)
        //    {
        //        if (EventManager.IsPerformanceMode())
        //        {
        //            //_screenHeightThresholds = { 0.02f, 0.015f, 0.01f }; //{ 0.0175f, 0.0150f, 0.0125f };
        //            _screenHeightThresholds[0] = 0.03f;
        //            _screenHeightThresholds[1] = 0.025f;
        //            _screenHeightThresholds[2] = 0.0175f;
        //            _hasBeenPerformancedAdjusted = true;
        //        }
        //    }


        //    _combinedBounds = CalculateCombinedBounds(_renderers);

        //    _screenHeightPercentage = CalculateScreenHeightPercent();// _renderer, _camera);
        //    _LODLevel = CalculateLODLevel(_screenHeightPercentage);

        //    if (!_hasChildRenderers)
        //        _LODGroup.ForceLOD(0);
        //    else
        //        _LODGroup.ForceLOD(_LODLevel);

        //}
        //private Bounds CalculateCombinedBounds(Renderer[] renderers)
        //{
        //    if (renderers == null || renderers.Length == 0)
        //        return new Bounds(transform.position, Vector3.zero);

        //    Bounds bounds = renderers[0].bounds;
        //    for (int i = 1; i < renderers.Length; i++)
        //    {
        //        bounds.Encapsulate(renderers[i].bounds);
        //    }

        //    return bounds;
        //}
        //private float CalculateScreenHeightPercent()//Renderer renderer, Camera camera)
        //{
        //    Vector3 center = _combinedBounds.center;
        //    float extentY = _combinedBounds.extents.y;

        //    Vector3 top = _camera.WorldToScreenPoint(center + Vector3.up * extentY);
        //    Vector3 bottom = _camera.WorldToScreenPoint(center - Vector3.up * extentY);

        //    if (top.z < 0 || bottom.z < 0)
        //        return 0f; // Behind camera

        //    float pixelHeight = Mathf.Abs(top.y - bottom.y);
        //    return Mathf.Clamp01(pixelHeight / Screen.height);
        //}

        //private int CalculateLODLevel(float screenHeightPercent)
        //{
        //    for (int i = 0; i < _screenHeightThresholds.Length; i++)
        //    {
        //        if (screenHeightPercent >= _screenHeightThresholds[i])
        //            return i;
        //    }

        //    //if (!_hasChildRenderers)
        //    //    return _screenHeightThresholds[2];

        //    return _screenHeightThresholds.Length;
        //}



















        private void SetPerformanceMode(int modeIndex)
        {
            if (_LODDistances == null || _LODDistances.Length < 4)
                _LODDistances = new float[4];



            for (int i = 0; i < 4; i++)
                _LODDistances[i] = _LODOptions[modeIndex][i];
        }








        private void Update()
        {
            if (_cameraTransform == null) return;

            //_dist = Vector3.Distance(_cameraTransform.position, this.transform.position);
            //_LODLevel = CalculateLODLevel(_dist);

            //_LODLevel = CalculateLODLevelIsometric();


            //if (!_cameraManager.GetHoldCameraMoveProcessFlag())
            //{
            //    _LODLevel = CalculateLODLevelIsometric();
            //    print("\nusing LODLevelIsometric()");
            //}
            //else
            //{
            //    switch (_cameraManager.GetCurrentCameraBehaviour())
            //    {
            //        case CameraBehaviour.TravelFromAPointToBPoint:
            //        case CameraBehaviour.TravelToCharacterPoint:
            //            float dist = Vector3.Distance(_cameraTransform.position, _transform.position);
            //            _LODLevel = CalculateLODLevel(dist);
            //            print("\nusing LODLevelDistance()");
            //            break;
            //    }
            //}

            //if (CharacterManager.GetCharacterDialogueID() == CharacterDialogueID.Character_Dialogue_Exit)
            //{
            //    switch (_cameraManager.GetCurrentCameraBehaviour())
            //    {

            //        case CameraBehaviour.CircleAroundAndZoomIn:
            //        case CameraBehaviour.CircleAroundAndZoomOut:
            //        case CameraBehaviour.CircleAroundPivot:
            //        case CameraBehaviour.DefaultStandby:
            //            _LODLevel = CalculateLODLevelIsometric();
            //            //print("\nusing LODLevelIsometric()");
            //            break;


            //        case CameraBehaviour.TravelFromAPointToBPoint:
            //        case CameraBehaviour.TravelToCharacterPoint:
            //            _dist = Vector3.Distance(_cameraTransform.position, _transform.position);
            //            _LODLevel = CalculateLODLevel(_dist);
            //            //print("\nusing LODLevelDistance()");
            //            break;
            //    }
            //}
            //if (CharacterManager.GetCharacterDialogueID() != CharacterDialogueID.Character_Dialogue_Exit)
            //{
            //    float dist = Vector3.Distance(_cameraTransform.position, _transform.position);
            //    _LODLevel = CalculateLODLevel(dist);
            //}

            _dist = Vector3.Distance(_cameraTransform.position, _transform.position);
            _LODLevel = CalculateLODLevel(_dist);
            _LODGroup.ForceLOD(_LODLevel);
        }

        int CalculateLODLevel(float distance)
        {

            if (EventManager.IsPerformanceMode())
            {
                //print("\nEventManager.IsPerformanceMode()");
                SetPerformanceMode(1);
            }
            if (!EventManager.IsPerformanceMode())
            {
                //print("\nQuality Mode");
                SetPerformanceMode(2);
            }

            for (int i = 0; i < _LODDistances.Length; i++)
            {
                if (distance < _LODDistances[i])
                    return i;
            }

            // If distance exceeds all thresholds, use the last LOD (e.g., LOD2 or LOD3)
            return _LODDistances.Length;
        }
        int CalculateLODLevelIsometric()
        {

            if (EventManager.IsPerformanceMode())
            {
                //print("\nEventManager.IsPerformanceMode()");
                SetPerformanceMode(1);
            }
            if (!EventManager.IsPerformanceMode())
            {
                //print("\nQuality Mode");
                SetPerformanceMode(2);
            }


            Vector3 cameraToObj = _transform.position - _cameraTransform.position;
            float projectedDist = Vector3.Dot(cameraToObj, _cameraTransform.forward);

            for (int i = 0; i < _LODDistances.Length; i++)
            {
                if (projectedDist < _LODDistances[i])
                    return i;
            }

            return _LODDistances.Length;
        }
    }
}
