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

    public enum RemovalMethodID
    {
        Removal_With_Spade = 0,
        Removal_With_Placement = 1
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

        //[Header("CellPlacementAwardFX")]
        //public ParticleSystem _placementCorrectAwardFX;

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
            //if (_assignedFieldID == -1) return;
            //if (_cellBorderType[_assignedFieldID] == CellBorderType.Cell_Border_Unspecified) return;
            //if (_cellBorderType[_assignedFieldID] == CellBorderType.Cell_Border_Unspecified) return;


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

            //if (_placementCorrectAwardFX != null)
            //{
            //    _placementCorrectAwardFX.gameObject.SetActive(false);
            //_placementCorrectAwardFX.Stop();
            //}

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

            if (!ignoreInitRemove) RemoveSurfaceObj();

            if (surfaceObj == null) return;//_surfObjCached

            for (int i = 0; i < _surfaceObjCached.Length; i++)
            {
                if (_surfaceObjCached[i] == null)
                {
                    _surfaceObjCached[i] = surfaceObj;

                    _surfObjCached[i] = _surfaceObjCached[i].GetComponent<SurfaceObj>();
                    if (_surfObjCached[i] != null) 
                        _surfObjCached[i]._cellSurfaceObjType = cellSurfObjType;

                    _currentPlacedSurfaceObj = cellSurfObjType;

                     

                    i = _surfaceObjCached.Length;//return; //break loop.
                }
            }

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
            for (int i = 0; i < _surfObjCached.Length; i++)
            {
                if (_surfObjCached[i] != null)
                {
                    _surfObjCached[i]._cellSurfaceObjType = CellSurfObjType.Cell_Surface_Empty;
                    _currentPlacedSurfaceObj = CellSurfObjType.Cell_Surface_Empty;
                    _surfObjCached[i] = null;
                    //_surfObjCached[i]._cellSurfaceObjType
                }

            }
            for (int i = 0; i < _surfaceObjCached.Length; i++)
            {
                if (_surfaceObjCached[i] != null)
                {
                    _surfaceObjCached[i].SetActive(false);
                    _surfaceObjCached[i] = null;
                }
            }

        }
        public void ApplySurfaceObjState(bool disable)
		{
            //int indexToIgnore = RulescopeManager._instance.GetRulescope().GetSelectedField();
            for (int i = 0; i < _surfaceObjCached.Length; i++)
            {
                if (_surfaceObjCached[i] != null)
                {
                    _surfaceObjCached[i].SetActive(disable);
                    //if (i == indexToIgnore)
                    //{

                    //}
                    //else
                    //{
                    //    _surfaceObjCached[i].SetActive(disable);
                    //}
                }
            }
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



        private void TriggerDeductYeildProcess()
        {


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
                    //if (removalMethodID == MouseClickStateID.OnClick_Paint_Cell_Tree ||
                    //    removalMethodID == MouseClickStateID.OnClick_Paint_Cell_Waterpool ||
                    //    removalMethodID == MouseClickStateID.OnClick_Paint_Cell_Wildflower)
                    //{
                    //    return true;
                    //}
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
                ApplyCellType(CellObjType.Cell_Soil_Light);
                _wTimer.StartTimer(_waitTime);
                AudioManager.PlaySFX(AudioID.Audio_Dig_SFX);
                FieldObjPooler._instance.InstantiateFX(PooledObjID.Default_Removal_FX, this.transform);

                //TriggerDeductYeildProcess();

                return;
            }
        }
        private void MouseDownSoilHeavy()
        {
            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Paint_Cell_Soil_Heavy)
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
                ApplyCellType(CellObjType.Cell_Soil_Heavy);
                _wTimer.StartTimer(_waitTime);
                AudioManager.PlaySFX(AudioID.Audio_Dig_SFX);
                FieldObjPooler._instance.InstantiateFX(PooledObjID.Default_Removal_FX, this.transform);

                //TriggerDeductYeildProcess();

                return;
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
            if (RulescopeManager._instance.GetRulescope().GetCoinCount() <= RulescopeManager._instance.GetRulescope().GetFenceCost()) return;

            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Paint_Cell_Fence)
            {
                //if (_surfaceObjCached == null) return;

                int nullCount = 0;

                for (int j = 0; j < _surfObjCached.Length; j++)
                {
                    if (_surfObjCached[j] != null)
                    {
                        if (IsBannedFencePlacement(j))
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
                            ReportOnClickFence();//eventually will call SetCellSurfaceObj() here, externally from fieldgen.
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
                        ReportOnClickFence();
                        return;
                    }
                }
            }
        }

        private void MouseDownTree()
        {
            if (RulescopeManager._instance.GetRulescope().GetCoinCount() <= RulescopeManager._instance.GetRulescope().GetFenceCost()*3) return;

            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Paint_Cell_Tree)
            {
                if (_surfaceObjCached == null) return;

                int nullCount = 0;

                for (int j = 0; j < _surfObjCached.Length; j++)
                {
                    if (_surfObjCached[j] != null)
                    {
                        if (IsBannedTreePlacement(j))
                            return;
                        else
                        {
                            //TriggerDeductYeildProcess();
                            if (IsYieldDeduction(EventManager.GetMouseClickStateID()))
                            {
                                for (int i = 0; i < 20; i++) 
                                    RulescopeManager._instance.GetRulescope().DeductYieldScore(_assignedFieldID);
                                EventManager.TriggerEvent("Report_OnClick_Spade_Removal");
                            }

                            _surfObjCached[j]._cellSurfaceObjType = CellSurfObjType.Cell_Surface_Empty;
                            _surfaceObjCached[j].SetActive(false);
                            _currentPlacedSurfaceObj = CellSurfObjType.Cell_Surface_Empty;
                            _surfaceObjCached[j] = null;


                            ReportOnClickTree();//eventually will call SetCellSurfaceObj() here, externally from fieldgen.
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
                        ReportOnClickTree();
                        return;
                    }
                }

            }
        }
        private void MouseDownWildflower()
        {
            if (RulescopeManager._instance.GetRulescope().GetCoinCount() <= RulescopeManager._instance.GetRulescope().GetFenceCost() * 2) return;

            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Paint_Cell_Wildflower)
            {
                if (_surfaceObjCached == null) return;

                int nullCount = 0;

                for (int j = 0; j < _surfObjCached.Length; j++)
                {
                    if (_surfObjCached[j] != null)
                    {
                        if (IsBannedWildflowerPlacement(j))
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
                            ReportOnClickWildflower();//eventually will call SetCellSurfaceObj() here, externally from fieldgen.
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
                        ReportOnClickWildflower();
                        return;
                    }
                }

            }
        }

        private void MouseDownWaterpool()
        {
            if (RulescopeManager._instance.GetRulescope().GetCoinCount() <= RulescopeManager._instance.GetRulescope().GetFenceCost() * 8) return;

            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Paint_Cell_Waterpool)
            {
                if (_surfaceObjCached == null) return;

                int nullCount = 0;

                for (int j = 0; j < _surfObjCached.Length; j++)
                {
                    if (_surfObjCached[j] != null)
                    {
                        if (IsBannedWaterpoolPlacement(j))
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



                            ReportOnClickWaterpool();//eventually will call SetCellSurfaceObj() here, externally from fieldgen.
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
                        ReportOnClickWaterpool();
                        return;
                    }
                }

            }
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
            if (_surfObjCached[i]._cellSurfaceObjType == CellSurfObjType.Cell_Surface_Hedge) return true;
            if (_surfObjCached[i]._cellSurfaceObjType == CellSurfObjType.Cell_Surface_OilSeedRape) return true;
            if (_surfObjCached[i]._cellSurfaceObjType == CellSurfObjType.Cell_Surface_Beans) return true;
            if (_surfObjCached[i]._cellSurfaceObjType == CellSurfObjType.Cell_Surface_SugarBeat) return true;
            if (_surfObjCached[i]._cellSurfaceObjType == CellSurfObjType.Cell_Surface_Wheat) return true;
            if (_surfObjCached[i]._cellSurfaceObjType == CellSurfObjType.Cell_Surface_Empty) return true;

            return false;
        }
        private bool IsBannedTreePlacement(int i)
        {
            //if (_surfObjCached[i]._cellSurfaceObjType == CellSurfObjType.Cell_Surface_Hedge) return true;
            //if (_surfObjCached[i]._cellSurfaceObjType == CellSurfObjType.Cell_Surface_Fence) return true;
            //if (_surfObjCached[i]._cellSurfaceObjType == CellSurfObjType.Cell_Surface_OilSeedRape) return true;
            //if (_surfObjCached[i]._cellSurfaceObjType == CellSurfObjType.Cell_Surface_Beans) return true;
            //if (_surfObjCached[i]._cellSurfaceObjType == CellSurfObjType.Cell_Surface_SugarBeat) return true;
            //if (_surfObjCached[i]._cellSurfaceObjType == CellSurfObjType.Cell_Surface_Wheat) return true;
            if (_surfObjCached[i]._cellSurfaceObjType == CellSurfObjType.Cell_Surface_Empty) return true;
            if (_surfObjCached[i]._cellSurfaceObjType == CellSurfObjType.Cell_Surface_Tree) return true;

            return false;
        }
        private bool IsBannedWildflowerPlacement(int i)
        {

            if (_surfObjCached[i]._cellSurfaceObjType == CellSurfObjType.Cell_Surface_Empty) return true;
            if (_surfObjCached[i]._cellSurfaceObjType == CellSurfObjType.Cell_Surface_Wildflower) return true;

            return false;
        }
        private bool IsBannedWaterpoolPlacement(int i)
        {

            if (_surfObjCached[i]._cellSurfaceObjType == CellSurfObjType.Cell_Surface_Empty) return true;
            if (_surfObjCached[i]._cellSurfaceObjType == CellSurfObjType.Cell_Surface_Waterpool) return true;
            if (_surfObjCached[i]._cellSurfaceObjType == CellSurfObjType.Cell_Surface_LilypadPond) return true;

            return false;
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
                if (_cachedCellID == CellObjType.Cell_Soil_Light)
                {
                    //_renderer.material = _cellMaterials[2];
                    _renderer.sharedMaterial = _cellMaterials[2];
                    //AudioManager.PlaySFX(AudioID.Audio_Dig_SFX);
                    AdjustCellY(lowerCellHeight);//, _cellHeightMovement);
                }
                if (_cachedCellID == CellObjType.Cell_Soil_Heavy)
                {
                    //_renderer.material = _cellMaterials[3];
                    _renderer.sharedMaterial = _cellMaterials[3];
                    //AudioManager.PlaySFX(AudioID.Audio_Dig_SFX);
                    AdjustCellY(lowerCellHeight);//, _cellHeightMovement);
                }
                if (_cachedCellID == CellObjType.Cell_Grass_Tall)
                {
                    lowerCellHeight = false;
                    //_renderer.material = _cellMaterials[4];
                    _renderer.sharedMaterial = _cellMaterials[4];
                    _assignedGridPosition.y = _startCellYPos;
                    this.gameObject.transform.position = _assignedGridPosition;
                    //AdjustCellY(lowerCellHeight);//, _cellHeightMovement);
                }
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

            if (_cachedCellID == CellObjType.Cell_Soil_Peat ||
                _cachedCellID == CellObjType.Cell_Soil_Light ||
                _cachedCellID == CellObjType.Cell_Soil_Heavy)
            {
                //Return the heigh back before processing waterpool body.
                AdjustCellY(false);//, _cellHeightMovement);
            }

            if (_cachedCellID == CellObjType.Cell_Empty_Blank)
            {
                _renderer.sharedMaterial = _cellMaterials[0];
            }
            if (_cachedCellID == CellObjType.Cell_Soil_Peat)
            {
                _cellHeightMovement = 0.10f;
                _renderer.sharedMaterial = _cellMaterials[1];
                AdjustCellY(true);//, amount);
            }
            if (_cachedCellID == CellObjType.Cell_Soil_Light)
            {
                _cellHeightMovement = 0.10f;
                _renderer.sharedMaterial = _cellMaterials[2];
                AdjustCellY(true);//, amount);
            }
            if (_cachedCellID == CellObjType.Cell_Soil_Heavy)
            {
                _cellHeightMovement = 0.10f;
                _renderer.sharedMaterial = _cellMaterials[3];
                AdjustCellY(true);//, amount);
            }
            if (_cachedCellID == CellObjType.Cell_Grass_Tall)
            {
                _cellHeightMovement = 0.10f;
                _renderer.sharedMaterial = _cellMaterials[4];
                AdjustCellY(true);//, amount);
            }
            _cellHeightMovement = 0f;
        }










        private bool IsCellBorderHedgeAward(CellBorderType cellBorderType, int fieldID)
        {
            //Issue here is these values below are always 1
            //which identifies fact we aren't on the last placed cell.
            //perhaps this is reason why _cellBorderType isn't what it is expected to be?
            //in this case, left border we are testing.(its top for values x =1 y = 1)
            //_gridPos_X = x;
            //_gridPos_Y = y;
            //this is being processed for every singlke cell that exists. not good it needs pin pointing and called!
            //temp++;
            //print("\n_cellBorderType check visits made = " + temp.ToString());
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

            //RulescopeManager._instance.GetRulescope().GetCellPositionByID(x, y);

            if (this._cellBorderType[_assignedFieldID] == cellBorderType)
                FieldObjPooler._instance.InstantiateFX(PooledObjID.Correct_Placement_Award_FX, this.transform);

            //FieldObjPooler._instance.Instantiate(PooledObjID.Correct_Placement_Award_FX, 
            //    RulescopeManager._instance.GetRulescope().GetCellTransformByID(x, y));


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
            //_cellBorderType = cellBorderType;
            _assignedFieldID = fieldId;

            //If we set the border type to one of the four sides that matter, then we subscribe to appropriate events for later use.

            if (_cellBorderType[fieldId] == CellBorderType.Cell_Border_Unspecified) return;
            //if (_cellBorderType == CellBorderType.Cell_Border_Unspecified) return;

            //if (_cellBorderType[fieldId] == CellBorderType.Cell_Border_Left)
            //{
            //    if (fieldId == 0) RulescopeManager._instance.GetRulescope().field_1_left_count++;
            //    if (fieldId == 1) RulescopeManager._instance.GetRulescope().field_2_left_count++;
            //    if (fieldId == 2) RulescopeManager._instance.GetRulescope().field_3_left_count++;
            //    if (fieldId == 3) RulescopeManager._instance.GetRulescope().field_4_left_count++;
            //}
            //if (_cellBorderType[fieldId] == CellBorderType.Cell_Border_Right)
            //{
            //    if (fieldId == 0) RulescopeManager._instance.GetRulescope().field_1_right_count++;
            //    if (fieldId == 1) RulescopeManager._instance.GetRulescope().field_2_right_count++;
            //    if (fieldId == 2) RulescopeManager._instance.GetRulescope().field_3_right_count++;
            //    if (fieldId == 3) RulescopeManager._instance.GetRulescope().field_4_right_count++;
            //}
            //if (_cellBorderType[fieldId] == CellBorderType.Cell_Border_Top)
            //{
            //    Vector3 v;
            //    v = transform.position;
            //    v.y += 5;
            //    transform.position = v;
            //    if (fieldId == 0) RulescopeManager._instance.GetRulescope().field_1_top_count++;
            //    if (fieldId == 1) RulescopeManager._instance.GetRulescope().field_2_top_count++;
            //    if (fieldId == 2) RulescopeManager._instance.GetRulescope().field_3_top_count++;
            //    if (fieldId == 3) RulescopeManager._instance.GetRulescope().field_4_top_count++;
            //}
            //if (_cellBorderType[fieldId] == CellBorderType.Cell_Border_Bottom)
            //{
            //    if (fieldId == 0) RulescopeManager._instance.GetRulescope().field_1_bot_count++;
            //    if (fieldId == 1) RulescopeManager._instance.GetRulescope().field_2_bot_count++;
            //    if (fieldId == 2) RulescopeManager._instance.GetRulescope().field_3_bot_count++;
            //    if (fieldId == 3) RulescopeManager._instance.GetRulescope().field_4_bot_count++;
            //}

            //string SIDE = "";
            //if (_cellBorderType == CellBorderType.Cell_Border_Left) SIDE = "Left";
            //if (_cellBorderType == CellBorderType.Cell_Border_Right) SIDE = "Right";
            //if (_cellBorderType == CellBorderType.Cell_Border_Top) SIDE = "Top";
            //if (_cellBorderType == CellBorderType.Cell_Border_Bottom) SIDE = "Bottom";
            //print("\nFIELD " + _assignedFieldID.ToString() + " SIDE = " + SIDE); 

            //switch (_assignedFieldID)
            //{
            //    case 0:
            //        if (_cellBorderType == CellBorderType.Cell_Border_Left) RulescopeManager._instance.GetRulescope().field_1_left_count++;
            //        if (_cellBorderType == CellBorderType.Cell_Border_Right) RulescopeManager._instance.GetRulescope().field_1_right_count++;
            //        if (_cellBorderType == CellBorderType.Cell_Border_Top) RulescopeManager._instance.GetRulescope().field_1_top_count++;
            //        if (_cellBorderType == CellBorderType.Cell_Border_Bottom) RulescopeManager._instance.GetRulescope().field_1_bot_count++;
            //        break;
            //    case 1:
            //        if (_cellBorderType == CellBorderType.Cell_Border_Left) RulescopeManager._instance.GetRulescope().field_2_left_count++;
            //        if (_cellBorderType == CellBorderType.Cell_Border_Right) RulescopeManager._instance.GetRulescope().field_2_right_count++;
            //        if (_cellBorderType == CellBorderType.Cell_Border_Top) RulescopeManager._instance.GetRulescope().field_2_top_count++;
            //        if (_cellBorderType == CellBorderType.Cell_Border_Bottom) RulescopeManager._instance.GetRulescope().field_2_bot_count++;
            //        break;
            //    case 2:
            //        if (_cellBorderType == CellBorderType.Cell_Border_Left) RulescopeManager._instance.GetRulescope().field_3_left_count++;
            //        if (_cellBorderType == CellBorderType.Cell_Border_Right) RulescopeManager._instance.GetRulescope().field_3_right_count++;
            //        if (_cellBorderType == CellBorderType.Cell_Border_Top) RulescopeManager._instance.GetRulescope().field_3_top_count++;
            //        if (_cellBorderType == CellBorderType.Cell_Border_Bottom) RulescopeManager._instance.GetRulescope().field_3_bot_count++;
            //        break;
            //    case 3:
            //        if (_cellBorderType == CellBorderType.Cell_Border_Left) RulescopeManager._instance.GetRulescope().field_4_left_count++;
            //        if (_cellBorderType == CellBorderType.Cell_Border_Right) RulescopeManager._instance.GetRulescope().field_4_right_count++;
            //        if (_cellBorderType == CellBorderType.Cell_Border_Top) RulescopeManager._instance.GetRulescope().field_4_top_count++;
            //        if (_cellBorderType == CellBorderType.Cell_Border_Bottom) RulescopeManager._instance.GetRulescope().field_4_bot_count++;
            //        break;
            //}

            EventManager.Subscribe("Report_Left_Hedgerow_Award", _actionListener[0]);
            EventManager.Subscribe("Report_Right_Hedgerow_Award", _actionListener[1]);
            EventManager.Subscribe("Report_Top_Hedgerow_Award", _actionListener[2]);
            EventManager.Subscribe("Report_Bottom_Hedgerow_Award", _actionListener[3]);

            //In default setup mode we fill these borders with fences intially.
            if (!isDefaultSetup) return;

            //ReportOnClickHedgerow();
            //ReportOnClickFence();
            this._markedForAutoFencePlacement = true;

            EventManager.Subscribe("Process_Default_Auto_Fence_Placements", _actionListener[4]);
            //EventManager.TriggerEvent("Process_Default_Auto_Fence_Placements");
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
