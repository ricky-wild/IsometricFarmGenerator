using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DoomBreakers;
using wildlogicgames;

namespace IsometricFarmGenerator
{
    public enum CellObjType 
    {
        Cell_Empty_Blank = 0,
        Cell_Soil_Peat = 1,
        Cell_Soil_Light = 2,
        Cell_Soil_Heavy = 3,
        Cell_Grass_Tall = 4,
    };

    public enum CellSurfObjType //Order of _prefabCellSurfObjs references.
    {
        Cell_Surface_Empty = 0,
        Cell_Surface_Wheat = 1,
        Cell_Surface_Grass = 2,
        Cell_Surface_Tree = 3,
        Cell_Surface_Hedge = 4,
        Cell_Surface_Fence = 5,
        Cell_Surface_OilSeedRape = 6,
        Cell_Surface_Beans = 7,
        Cell_Surface_SugarBeat = 8,
        Cell_Surface_Wildflower = 9,
        Cell_Surface_Waterpool = 10,
        Cell_Surface_LilypadPond = 11,
    };

    public enum CellBorderType //Used to determine which side of a field is N, S, E or West.
    {
        Cell_Border_Unspecified,
        Cell_Border_Top,
        Cell_Border_Bottom,
        Cell_Border_Left,
        Cell_Border_Right,
    };


    public class CellObj : MonoBehaviour
    {
        private int _cellID;
        private int _assignedFieldID;

        //[Header("CellMaterialTypes")]
        //[SerializeField] 
        private Material[] _cellMaterials; //Order of CellObjType for reference index'
                                          //We swap out material when needed for different cell type display.

        [Header("CellSurfacePrefabTypes")]
        public GameObject[] _prefabCellSurfObjs; //Order of CellSurfObjType for reference index'
                                                 //We instantiate cell surface obj on top of this cell, as appropriate.


        private Transform _transform;
        private Vector3 _assignedGridPosition;

        private Renderer _renderer;

        private CellObjType _cachedCellID;

        private int _gridPos_X = -1;
        private int _gridPos_Y = -1;
        //private int temp = 0;

        public GameObject[] _surfaceObjCached = new GameObject[3];
        private SurfaceObj[] _surfObjCached = new SurfaceObj[3];

        private const float _maxCellHeightDistThreshold = 0.1f;
        private float _totalCellHeightDistTravelled = 0f;
        private float _cellHeightMovement = 0f;//0.5f;
        private float _startCellYPos;

        private CellSurfObjType _currentPlacedSurfaceObj;
        private CellBorderType[] _cellBorderType = new CellBorderType[4];


        private bool _markedForAutoFencePlacement;


        private GameObject _timerObj;
        private WTimer _wTimer;
        private const float _waitTime = 3.0f;

        private Action[] _actionListener = new Action[5];

        private void OnDisable()
        {
            for (int i = 0; i < _surfaceObjCached.Length; i++)
            {
                _surfaceObjCached[i] = null;// new GameObject();
                _surfObjCached[i] = null;
            }
            if (_timerObj != null)
            {
                Destroy(_timerObj);
                _timerObj = null;
            }

            EventManager.Unsubscribe("Report_Left_Hedgerow_Award", _actionListener[0]);
            EventManager.Unsubscribe("Report_Right_Hedgerow_Award", _actionListener[1]);
            EventManager.Unsubscribe("Report_Top_Hedgerow_Award", _actionListener[2]);
            EventManager.Unsubscribe("Report_Bottom_Hedgerow_Award", _actionListener[3]);
        }
        private void Awake() => Setup();
        private void Setup()
        {
            _transform = this.transform;
            _assignedGridPosition = new Vector3();
            _renderer = this.gameObject.GetComponent<Renderer>();
            _cachedCellID = CellObjType.Cell_Grass_Tall;
            _currentPlacedSurfaceObj = CellSurfObjType.Cell_Surface_Empty;
            for (int i = 0; i < 4; i ++) _cellBorderType[i] = CellBorderType.Cell_Border_Unspecified;
            //_cellBorderType = CellBorderType.Cell_Border_Unspecified;

            for (int i = 0; i < _surfaceObjCached.Length; i++)
            {
                _surfaceObjCached[i] = null;// new GameObject();
                _surfObjCached[i] = null;
            }

            _timerObj = new GameObject();
            _timerObj.AddComponent<WTimer>();
            _wTimer = _timerObj.GetComponent<WTimer>();

            _assignedFieldID = -1;
            _markedForAutoFencePlacement = false;

            _actionListener[0] = new Action(ProcessHedgeLeftAwardFX);//DelegateMethod()
            _actionListener[1] = new Action(ProcessHedgeRightAwardFX);//DelegateMethod()
            _actionListener[2] = new Action(ProcessHedgeTopAwardFX);//DelegateMethod()
            _actionListener[3] = new Action(ProcessHedgeBotAwardFX);//DelegateMethod()
            _actionListener[4] = new Action(ProcessDefaultAutoFencePlacements);//DelegateMethod()

            //EventManager.Subscribe("Report_Left_Hedgerow_Award", _actionListener[0]);
            //EventManager.Subscribe("Report_Right_Hedgerow_Award", _actionListener[1]);
            //EventManager.Subscribe("Report_Top_Hedgerow_Award", _actionListener[2]);
            //EventManager.Subscribe("Report_Bottom_Hedgerow_Award", _actionListener[3]);
            //EventManager.Subscribe("Report_AllSides_Hedgerow_Award", _actionListener[4]);
        }
        private void ProcessHedgeAwardFXBase(CellBorderType cellBorderType, int fieldId)
        {

			if (IsCellBorderHedgeAward(cellBorderType, fieldId))
			{             
				EnablePlacementCorrectAwardFX(cellBorderType);
			}
			//EnablePlacementCorrectAwardFX();
		}
        private void ProcessHedgeLeftAwardFX() => ProcessHedgeAwardFXBase(CellBorderType.Cell_Border_Left, _assignedFieldID);
        private void ProcessHedgeRightAwardFX() => ProcessHedgeAwardFXBase(CellBorderType.Cell_Border_Right, _assignedFieldID);
        private void ProcessHedgeTopAwardFX() => ProcessHedgeAwardFXBase(CellBorderType.Cell_Border_Top, _assignedFieldID);
        private void ProcessHedgeBotAwardFX() => ProcessHedgeAwardFXBase(CellBorderType.Cell_Border_Bottom, _assignedFieldID);

        private void Update()
        {
            if (_wTimer.HasTimerFinished(true))
            {
                ApplyCellType(CellObjType.Cell_Grass_Tall);
                FieldObjPooler._instance.InstantiateFX(PooledObjID.Grass_Growth_FX, this.transform);
            }



        }
        
        
        
        
        
        
        public void PopulateSharedMaterialReference(Material[] cellMaterials)
        {
            _cellMaterials = cellMaterials;
        }
        public void SetCellSurfaceObj(ref GameObject surfaceObj, CellSurfObjType cellSurfObjType, bool ignoreInitRemove)//, int curFieldId)//, SurfaceObj surfObj) //=> _surfaceObjCached = surfaceObj;
        {

            //...ect

        }

        private void OnEnable()
        {
            //This grid cell would've had its specific position in space assigned by this point.
            //Now we cache that value.
            _assignedGridPosition = this.gameObject.transform.position;
            _startCellYPos = _assignedGridPosition.y;
        }

        private void ReportOnClickHedgerow()
        {
            _cellHeightMovement = 0f;
            EventManager.SetGridCellIDClicked(_gridPos_X, _gridPos_Y);
            //print("\nx = " + _gridPos_X.ToString());
            //print("\ny = " + _gridPos_Y.ToString());
            EventManager.TriggerEvent("Report_OnClick_Hedgerow");
        }
        private void ReportOnClickTree()
        {
            _cellHeightMovement = 0f;
            EventManager.SetGridCellIDClicked(_gridPos_X, _gridPos_Y);
            EventManager.TriggerEvent("Report_OnClick_Tree");
        }
        private void ReportOnClickFence()
        {
            _cellHeightMovement = 0f;
            EventManager.SetGridCellIDClicked(_gridPos_X, _gridPos_Y);
            EventManager.TriggerEvent("Report_OnClick_Fence");
        }
        private void ReportOnClickWildflower()
        {
            _cellHeightMovement = 0f;
            EventManager.SetGridCellIDClicked(_gridPos_X, _gridPos_Y);
            EventManager.TriggerEvent("Report_OnClick_Wildflower");
        }
        private void ReportOnClickWaterpool()
        {
            //AdjustCellY(true);
            _cellHeightMovement = 0.05f;
            EventManager.SetGridCellIDClicked(_gridPos_X, _gridPos_Y);
            EventManager.TriggerEvent("Report_OnClick_Waterpool");
            
        }
        private void RemoveSurfaceObj()
        {
            //...ect

        }
        public void ApplySurfaceObjState(bool disable)
		{
            //...ect
        }
        private void OnMouseDown()
        {
            //CellOnClick();
        }

        public void CellOnClick()
        {
            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Nothing) return;


            MouseDownSoilPeat();
            MouseDownSoilLight();
            MouseDownSoilHeavy();
            MouseDownHedgerow();
            MouseDownFence();
            MouseDownTree();
            MouseDownWildflower();
            MouseDownWaterpool();

            //_renderer.material = _cellMaterials[1];
            //print("\n" + name + " was clicked! Position = " + _assignedGridPosition);
            //_assignedGridPosition.y -= 0.05f;
            //this.gameObject.transform.position = _assignedGridPosition;
        }




        private bool IsYieldDeduction(MouseClickStateID removalMethodID)
        {

            

            //Don't account for another field when focused on the current.
            if (_assignedFieldID != RulescopeManager._instance.GetRulescope().GetSelectedField()) return false;

            for (int i = 0; i < _surfObjCached.Length; i++)
            {
                if (_surfObjCached[i] != null)
                {
                    if (removalMethodID == MouseClickStateID.OnClick_Paint_Cell_Soil_Peat ||
                        removalMethodID == MouseClickStateID.OnClick_Paint_Cell_Soil_Light ||
                        removalMethodID == MouseClickStateID.OnClick_Paint_Cell_Soil_Heavy ||
                        removalMethodID == MouseClickStateID.OnClick_Paint_Cell_Tree ||
                        removalMethodID == MouseClickStateID.OnClick_Paint_Cell_Waterpool ||
                        removalMethodID == MouseClickStateID.OnClick_Paint_Cell_Wildflower)
                    {
                        if (_surfObjCached[i]._cellSurfaceObjType == CellSurfObjType.Cell_Surface_OilSeedRape ||
                            _surfObjCached[i]._cellSurfaceObjType == CellSurfObjType.Cell_Surface_Wheat ||
                            _surfObjCached[i]._cellSurfaceObjType == CellSurfObjType.Cell_Surface_Beans ||
                            _surfObjCached[i]._cellSurfaceObjType == CellSurfObjType.Cell_Surface_SugarBeat)
                        {
                            return true;
                        }
                    }

                }

            }

            return false;
        }
        private void MouseDownSoilPeat()
        {
            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Paint_Cell_Soil_Peat)
            {
                _cellHeightMovement = 0.05f;
                //Adjust height back before proceeding if waterbody.
                if (this._currentPlacedSurfaceObj == CellSurfObjType.Cell_Surface_Waterpool ||
                    this._currentPlacedSurfaceObj == CellSurfObjType.Cell_Surface_LilypadPond) AdjustCellY(false);//, _cellHeightMovement);// 0.10f);

                //TriggerDeductYeildProcess();
                if (IsYieldDeduction(EventManager.GetMouseClickStateID()))
                {
                    RulescopeManager._instance.GetRulescope().DeductYieldScore(_assignedFieldID);
                    EventManager.TriggerEvent("Report_OnClick_Spade_Removal");
                }

                RemoveSurfaceObj();
                ApplyCellType(CellObjType.Cell_Soil_Peat);
                _wTimer.StartTimer(_waitTime);
                AudioManager.PlaySFX(AudioID.Audio_Dig_SFX);
                FieldObjPooler._instance.InstantiateFX(PooledObjID.Default_Removal_FX, this.transform);

                

                return;
            }
        }
        private void MouseDownSoilLight()
        {
            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Paint_Cell_Soil_Light)
            {
                //...ect
            }
        }
        private void MouseDownSoilHeavy()
        {
            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Paint_Cell_Soil_Heavy)
            {
                //...ect
            }
        }
        private void MouseDownHedgerow()
        {
            if (RulescopeManager._instance.GetRulescope().GetCoinCount() <= RulescopeManager._instance.GetRulescope().GetHedgerowCost()) return;

            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Paint_Cell_Hedgerow)
            {
                if (_surfaceObjCached == null) return;

                int nullCount = 0;

                for (int j = 0; j < _surfObjCached.Length; j++)
                {
                    if (_surfObjCached[j] != null)
                    {
                        if (IsBannedHedgerowPlacement(j)) 
                            return;
                        else
                        {
                            //TriggerDeductYeildProcess();
                            if (IsYieldDeduction(EventManager.GetMouseClickStateID()))
                            {
                                RulescopeManager._instance.GetRulescope().DeductYieldScore(_assignedFieldID);
                                EventManager.TriggerEvent("Report_OnClick_Spade_Removal");
                            }

                            _surfObjCached[j]._cellSurfaceObjType = CellSurfObjType.Cell_Surface_Empty;
                            _surfaceObjCached[j].SetActive(false);
                            _currentPlacedSurfaceObj = CellSurfObjType.Cell_Surface_Empty;
                            _surfaceObjCached[j] = null;
                            ReportOnClickHedgerow();//eventually will call SetCellSurfaceObj() here, externally from fieldgen.
                            return;
                        }
                    }
                    if (_surfObjCached[j] == null)
                    {
                        nullCount++;
                        //Yes but we may have a hedgerow present in the other two remaining array elements!
                        //ReportOnClickHedgerow();//eventually will call SetCellSurfaceObj() here, externally from fieldgen.
                        //return;
                    }
                    if (nullCount == _surfObjCached.Length)
                    {
                        if (_surfaceObjCached[0] != null) _surfaceObjCached[0].SetActive(false);
                        if (_surfaceObjCached[1] != null) _surfaceObjCached[1].SetActive(false);
                        if (_surfaceObjCached[2] != null) _surfaceObjCached[2].SetActive(false);
                        ReportOnClickHedgerow();
                        return;
                    }
                }
                
            }
        }
        private void MouseDownFence()
        {
            //...ect
        }

        private void MouseDownTree()
        {
            //...ect
        }
        private void MouseDownWildflower()
        {
            //...ect
        }

        private void MouseDownWaterpool()
        {
            //...ect
        }

        private bool IsBannedFencePlacement(int i)
        {
            if (_surfObjCached[i]._cellSurfaceObjType == CellSurfObjType.Cell_Surface_Fence) return true;
            if (_surfObjCached[i]._cellSurfaceObjType == CellSurfObjType.Cell_Surface_OilSeedRape) return true;
            if (_surfObjCached[i]._cellSurfaceObjType == CellSurfObjType.Cell_Surface_Beans) return true;
            if (_surfObjCached[i]._cellSurfaceObjType == CellSurfObjType.Cell_Surface_SugarBeat) return true;
            if (_surfObjCached[i]._cellSurfaceObjType == CellSurfObjType.Cell_Surface_Wheat) return true;
            if (_surfObjCached[i]._cellSurfaceObjType == CellSurfObjType.Cell_Surface_Empty) return true;

            return false;
        }
        private bool IsBannedHedgerowPlacement(int i)
        {
            //...ect
        }
        private bool IsBannedTreePlacement(int i)
        {
            //...ect
        }
        private bool IsBannedWildflowerPlacement(int i)
        {
            //...ect
        }
        private bool IsBannedWaterpoolPlacement(int i)
        {
            //...ect
        }


        public void ApplyParentFieldID(int fieldId) => _assignedFieldID = fieldId;
        public int GetParentFieldID() => _assignedFieldID;
        public void ApplyCellType(CellObjType cellObjID)
        {
            if (_cachedCellID == cellObjID) return;

            //float dist = _cellHeightMovement;
            bool lowerCellHeight = true;

            _cachedCellID = cellObjID;

            if (this._currentPlacedSurfaceObj == CellSurfObjType.Cell_Surface_Waterpool ||
                this._currentPlacedSurfaceObj == CellSurfObjType.Cell_Surface_LilypadPond)
            {
                //dist = 0.00f;
                lowerCellHeight = false;
            }

            if (_renderer != null)
            {
                if (_cachedCellID == CellObjType.Cell_Empty_Blank)
                {
                    //_renderer.material = _cellMaterials[0];
                    _renderer.sharedMaterial = _cellMaterials[0];
                }
                if (_cachedCellID == CellObjType.Cell_Soil_Peat)
                {
                    //_renderer.material = _cellMaterials[1];
                    _renderer.sharedMaterial = _cellMaterials[1];
                    //AudioManager.PlaySFX(AudioID.Audio_Dig_SFX);
                    AdjustCellY(lowerCellHeight);//, _cellHeightMovement);
                }
                //...ect
            }

        }
        private void AdjustCellY(bool lowerHeight)//, float amount)
        {
            if (Mathf.Abs(_totalCellHeightDistTravelled) >= _maxCellHeightDistThreshold) //This prevents cell height. Mathf.Abs will test against both negative and psoitive vals.
            {
                //Reset
                _totalCellHeightDistTravelled = 0f;
                _cellHeightMovement = 0f;
                return;
            }

            _assignedGridPosition = this.gameObject.transform.position;
            if (lowerHeight)
            {
                _assignedGridPosition.y -= _cellHeightMovement;
                _totalCellHeightDistTravelled -= _cellHeightMovement;
            }
            else
            {
                _assignedGridPosition.y += _cellHeightMovement;
                _totalCellHeightDistTravelled += _cellHeightMovement;
            }
            this.gameObject.transform.position = _assignedGridPosition;

            
            print("\n_totalCellHeightDistTravelled=" + _totalCellHeightDistTravelled.ToString());
        }
        public void ApplyWaterbodySpace(CellObjType cellObjID)
        {

            if (_renderer == null) return;

            //float amount = 0.05f;

            this._currentPlacedSurfaceObj = CellSurfObjType.Cell_Surface_Waterpool;

            //...ect
        }










        private bool IsCellBorderHedgeAward(CellBorderType cellBorderType, int fieldID)
        {

            int x = EventManager.GetGridCellClicked_X();
            int y = EventManager.GetGridCellClicked_Y();

            //print("\ncellBorderType=" + cellBorderType.ToString());

            if (RulescopeManager._instance.GetRulescope().GetCellBorderTypeByID(x, y, fieldID) == cellBorderType)
                return true;
            //if (this._cellBorderType[_assignedFieldID] == cellBorderType)
            //    return true;

            return false;
        }
        private void EnablePlacementCorrectAwardFX(CellBorderType cellBorderType)
        {
            int curSelectedFieldId = RulescopeManager._instance.GetRulescope().GetSelectedField();
            if (_assignedFieldID != curSelectedFieldId) return;

            int x = EventManager.GetGridCellClicked_X();
            int y = EventManager.GetGridCellClicked_Y();

            if (this._cellBorderType[_assignedFieldID] == cellBorderType)
                FieldObjPooler._instance.InstantiateFX(PooledObjID.Correct_Placement_Award_FX, this.transform);

        }
        
        
        private void ProcessDefaultAutoFencePlacements()//EventManager listener. So we execute after full grid cell init has complete.
        {
            //EventManager.TriggerEvent("Process_Default_Auto_Fence_Placements");
            if (this._markedForAutoFencePlacement)
            {
                RulescopeManager._instance.GetRulescope().SetSelectedField(_assignedFieldID);
                ReportOnClickFence();
                this._markedForAutoFencePlacement = false;
            }
        }
        public void SetCellBorderType(CellBorderType cellBorderType, int fieldId, bool isDefaultSetup, int x, int y) //=> _cellBorderType = cellBorderType;
        {
            if (_cellBorderType[fieldId] == CellBorderType.Cell_Border_Unspecified)
            {
                this._cellBorderType[fieldId] = cellBorderType;
                _gridPos_X = x;
                _gridPos_Y = y;
            }

            _assignedFieldID = fieldId;

            if (_cellBorderType[fieldId] == CellBorderType.Cell_Border_Unspecified) return;

            EventManager.Subscribe("Report_Left_Hedgerow_Award", _actionListener[0]);
            EventManager.Subscribe("Report_Right_Hedgerow_Award", _actionListener[1]);
            EventManager.Subscribe("Report_Top_Hedgerow_Award", _actionListener[2]);
            EventManager.Subscribe("Report_Bottom_Hedgerow_Award", _actionListener[3]);

            //In default setup mode we fill these borders with fences intially.
            if (!isDefaultSetup) return;

            this._markedForAutoFencePlacement = true;

            EventManager.Subscribe("Process_Default_Auto_Fence_Placements", _actionListener[4]);

        }
        public CellBorderType GetCellBorderType(int fieldId) => _cellBorderType[fieldId];//_cellBorderType;// _cellBorderType[fieldId];
        public void SetCellID(int id) => _cellID = id;
        public void SetCellCurrentSurfaceObj(CellSurfObjType currentPlacedSurfaceObj) => _currentPlacedSurfaceObj = currentPlacedSurfaceObj;
        public CellSurfObjType GetCellCurrentSurfaceObj() => _currentPlacedSurfaceObj;
        public CellObjType GetCellID() => _cachedCellID;
        public void SetCellPosition(Vector3 v) => _assignedGridPosition = v;
        public void SetCellPosition(int x, int y)
        {
            if (_gridPos_X == -1) 
                _gridPos_X = x;
            if (_gridPos_Y == -1) 
                _gridPos_Y = y;
        }

        public int GetCellPosX() => _gridPos_X;
        public int GetCellPosY() => _gridPos_Y;
        public Vector3 GetCellPosition() => _assignedGridPosition;



    }
}
