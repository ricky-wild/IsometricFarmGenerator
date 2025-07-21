using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DoomBreakers;
using wildlogicgames;
using UnityEngine.Profiling;

namespace IsometricFarmGenerator
{
	public enum FieldGridCellID //Order of _prefabGridCellObjects references.
	{

		FieldGridCell_Soil_Peat = 0,
		FieldGridCell_Soil_Light = 1,
		FieldGridCell_Soil_Heavy = 2,
		FieldGridCell_Grass_Tall = 3,
	};
	public enum SurfaceGridCellID
	{
		Surface_Type_None = 0,
		Surface_Type_Wheat = 1,
		Surface_Type_TallGrass = 2,
		Surface_Type_Tree_A = 3,
		Surface_Type_Tree_B = 4,
		Surface_Type_Tree_C = 5,
		Surface_Type_Tree_D = 6,
		Surface_Type_Hedge_A = 7,
		Surface_Type_Fence_A = 8,
		Surface_Type_OilSeedRape = 9,
		Surface_Type_Beans = 10,
		Surface_Type_SugarBeat = 11,
	};
	public enum TillageGridCellID
    {
		Tillage_Type_None = 0,
		Tillage_Type_Standard = 1,
		Tillage_Type_NoTill = 2,
    }
	
	public struct FieldGridSize
    {
		public int START_X;
		public int START_Y;
		public int X;
		public int Y;
		public float CELL_SIZE;// = 0.5f;
		//public CellObjType CELL_TYPE;
		public FieldGridCellID SOIL_TYPE;
		public SurfaceGridCellID SURFACE_TYPE;
		public TillageGridCellID TILLAGE_TYPE;
	};
	
	public class FieldGenerator : MonoBehaviour
	{
		private Transform _transform;
		private bool _processUpdateLoop;
		private bool _initFieldGeneration;

		[Header("Camera Manager Ref")]
		public CameraManager _cameraManager;


		[Header("BaseGridCellPrefab")]
		public GameObject _prefabGridCellBase;
		public Dictionary<FieldGridCellID, List<GameObject>> _baseGridCellObjDict;
		private const int _maxBaseGridCellsX = 50;
		private const int _maxBaseGridCellsY = 50;
		private int _nextGridCellStartX;
		private int _nextGridCellStartY;
		private CellObj[,] _baseCellObjsCached = new CellObj[50, 50];
		private FieldGridSize _baseGrid;
		private float _cellWidth;

		[Header("CellMaterialTypes")]
		[SerializeField] public Material[] _cellMaterials; //Order of CellObjType enum for reference index'

		[Header("GridCellPrefabTypes")]
		public GameObject[] _prefabGridCellObjects; //Order of FieldGridCellID enum for reference index'

		[Header("SurfacePrefabTypes")]
		public GameObject[] _prefabSurfaceObjects; //Order of SurfaceGridCellID enum for reference index'

		//[Header("Optimized Crop Rendering Ref")]
		//public OPTMeshRenderer _optMeshRenderer;


		private FieldGridCellID _gridCellType;
		private int _fieldCount;

		public Dictionary<FieldGridCellID, List<GameObject>> _fieldGridCellObjDict;

		private const int _maxGridCellsX = 30;
		private const int _maxGridCellsY = 30;
		private Dictionary<int, FieldGridSize> _fieldGrids;


		private SurfaceGridCellID _surfaceType;
		public Dictionary<SurfaceGridCellID, List<GameObject>> _surfaceGridCellObjDict;

		private Vector3 _originalPosition, _startPosition, _prevPosition, _curPosition, _centerPoint, _surfaceOffset, _baseStartPosition;
		private Vector3[] _fieldGridCenterPointsCached = new Vector3[4];

		private bool _includeCrop;
		private bool _hasBeenReset;
		private bool _isOptimized;

		private Action[] _actionListener = new Action[5];

		private void Update()
		{
			if (!_processUpdateLoop) return;

			//OnClickUpdate();

			//long totalMemory = System.GC.GetTotalMemory(false);
			//Debug.Log($"[MEMORY] GC memory: {totalMemory / (1024 * 1024)} MB, Physical: {SystemInfo.systemMemorySize} MB");
			//print($"[MEMORY] GC memory: {totalMemory / (1024 * 1024)} MB, Physical: {SystemInfo.systemMemorySize} MB");

			//long monoMemory = Profiler.GetMonoUsedSizeLong();
			//long totalReserved = Profiler.GetTotalReservedMemoryLong();
			//long totalAllocated = Profiler.GetTotalAllocatedMemoryLong();
			//print($"[MEMORY] monoMemory: {monoMemory} MB");


			//InitFieldGeneration(); //Call from outside of this Class.
		}
		private void OnDestroy() => Unload();
		private void OnDisable() => Unload();
		public void Unload()
		{
			EventManager.Unsubscribe("Report_OnClick_Hedgerow", _actionListener[0]);
			EventManager.Unsubscribe("Report_OnClick_Fence", _actionListener[1]);
			EventManager.Unsubscribe("Report_OnClick_Tree", _actionListener[2]);
			EventManager.Unsubscribe("Report_OnClick_Wildflower", _actionListener[3]);
			EventManager.Unsubscribe("Report_OnClick_Waterpool", _actionListener[4]);

			//if (_baseGridCellObjDict.Count == 0) return;
			//if (_fieldGridCellObjDict != null) return;

			ClearUnUsedGridObjs();
			ClearUnUsedCropObjs();


			_initFieldGeneration = false;

			GC.Collect();
			Resources.UnloadUnusedAssets(); //expensive.
			GC.Collect();

			_hasBeenReset = true;
			//Destroy(this.gameObject);
		}
		public void ClearUnUsedGridObjs()//FieldGridCellID gridCellId)
		{
			if (_fieldGridCellObjDict == null) return;

			List<GameObject> list;

			FieldGridCellID gridCellId = _gridCellType;

			// Collect items to remove
			List<GameObject> itemsToRemove = new List<GameObject>();

			if (_fieldGridCellObjDict.TryGetValue(gridCellId, out list))
			{
				for (int j = 0; j < list.Count; j++)
				{
					if (!list[j].activeInHierarchy)
					{
						itemsToRemove.Add(list[j]);
					}
					else
					{
						list[j].SetActive(false);
						itemsToRemove.Add(list[j]);
					}
				}

				// Remove the items outside the loop
				foreach (var itemToRemove in itemsToRemove)
				{
					list.Remove(itemToRemove);
				}

				// If the list is empty, remove the entry from the dictionary
				if (list.Count == 0)
				{
					_fieldGridCellObjDict.Remove(gridCellId);
				}
			}
			_fieldGridCellObjDict.Clear();
			_baseGridCellObjDict.Clear();
			//_initFieldGeneration = false;
		}
		public void ClearUnUsedCropObjs()
		{
			if (_surfaceGridCellObjDict == null) return;

			List<GameObject> list;

			SurfaceGridCellID cropCellId = _surfaceType;

			// Collect items to remove
			List<GameObject> itemsToRemove = new List<GameObject>();

			if (_surfaceGridCellObjDict.TryGetValue(cropCellId, out list))
			{
				for (int j = 0; j < list.Count; j++)
				{
					if (list[j] != null)
					{
						if (!list[j].activeInHierarchy)
						{
							if (itemsToRemove != null)
								itemsToRemove.Add(list[j]);
						}
						else
						{
							list[j].SetActive(false);
							if (itemsToRemove != null)
								itemsToRemove.Add(list[j]);
						}
					}

				}

				// Remove the items outside the loop
				foreach (var itemToRemove in itemsToRemove)
				{
					list.Remove(itemToRemove);
				}

				// If the list is empty, remove the entry from the dictionary
				if (list.Count == 0)
				{
					_surfaceGridCellObjDict.Remove(cropCellId);
				}
			}
			_surfaceGridCellObjDict.Clear();
			//_initFieldGeneration = false;
		}
		public void SetOptimizationFlag(bool enabled) => _isOptimized = enabled;
		private void Awake() => Setup();
		private void Start()
		{

		}
		private void Setup()
		{
			_transform = this.transform;
			_processUpdateLoop = true;
			_initFieldGeneration = false;

			//if (_boxCollider == null) _boxCollider = this.GetComponent<BoxCollider>();

			_baseGrid.X = _maxBaseGridCellsX;
			_baseGrid.Y = _maxBaseGridCellsY;
			_baseGrid.CELL_SIZE = 0.5f;
			_fieldGrids = new Dictionary<int, FieldGridSize>();

			_gridCellType = FieldGridCellID.FieldGridCell_Soil_Peat;

			_surfaceType = SurfaceGridCellID.Surface_Type_None;

			//_startPosition = new Vector3(-2.0f, -0.66f, -2.26f);
			_startPosition = new Vector3(-10.0f, -0.66f, -10.0f);
			_baseStartPosition = _startPosition;

			_includeCrop = true;
			_hasBeenReset = false;
			_isOptimized = false;

			PopulateBaseGridCellsDict();
			//PopulateFieldGridCellsDict();
			PopulateSurfaceGridCellsDict();

			_actionListener[0] = new Action(AddHedgerowToSelectedCell);//DelegateMethod()
			_actionListener[1] = new Action(AddFenceToSelectedCell);//DelegateMethod()
			_actionListener[2] = new Action(AddTreeToSelectedCell);//DelegateMethod()
			_actionListener[3] = new Action(AddWildflowerToSelectedCell);//DelegateMethod()
			_actionListener[4] = new Action(AddWaterpoolToSelectedCell);//DelegateMethod()


			EventManager.Subscribe("Report_OnClick_Hedgerow", _actionListener[0]);
			EventManager.Subscribe("Report_OnClick_Fence", _actionListener[1]);
			EventManager.Subscribe("Report_OnClick_Tree", _actionListener[2]);
			EventManager.Subscribe("Report_OnClick_Wildflower", _actionListener[3]);
			EventManager.Subscribe("Report_OnClick_Waterpool", _actionListener[4]);


			Resources.UnloadUnusedAssets(); //expensive.
			GC.Collect();
		}
		private void AddWaterpoolToSelectedCell()
        {
			if (RulescopeManager._instance.GetRulescope().GetCoinCount() <= RulescopeManager._instance.GetRulescope().GetFenceCost() * 2) return;

			int x = 0;
			int y = 0;

			x = EventManager.GetGridCellClicked_X();
			y = EventManager.GetGridCellClicked_Y();

			if (_baseCellObjsCached[x, y].GetCellCurrentSurfaceObj() == CellSurfObjType.Cell_Surface_Waterpool)
				return;

			

			AudioManager.PlaySFX(AudioID.Audio_Placement_SFX);

			Vector3 cellClickedPos = _baseCellObjsCached[x, y].GetCellPosition();
			_baseCellObjsCached[x, y].SetCellCurrentSurfaceObj(CellSurfObjType.Cell_Surface_Waterpool);

			Quaternion q = Quaternion.Euler(0f, 0f, 0f);
			Transform t = _baseCellObjsCached[x, y].transform;

			GameObject obj = FieldObjPooler._instance.InstantiateObj(PooledObjID.Waterpool_Placement_Obj, t, q);

			if (_gridCellType == FieldGridCellID.FieldGridCell_Soil_Light)
				_baseCellObjsCached[x, y].GetComponent<CellObj>().ApplyWaterbodySpace(CellObjType.Cell_Soil_Light);
			if (_gridCellType == FieldGridCellID.FieldGridCell_Soil_Peat)
				_baseCellObjsCached[x, y].GetComponent<CellObj>().ApplyWaterbodySpace(CellObjType.Cell_Soil_Peat);
			if (_gridCellType == FieldGridCellID.FieldGridCell_Soil_Heavy)
				_baseCellObjsCached[x, y].GetComponent<CellObj>().ApplyWaterbodySpace(CellObjType.Cell_Soil_Heavy);


			_baseCellObjsCached[x, y].SetCellSurfaceObj(ref obj, CellSurfObjType.Cell_Surface_Waterpool, false);//, 0);


			int randLilypadChance = wildlogicgames.Utilities.GetRandomNumberInt(0, 100);

			if (randLilypadChance > 85) //> 85 = 15% chance of lilypad surface addition to each water placement.
            {
				// t = obj.transform;
				GameObject objB = FieldObjPooler._instance.InstantiateObj(PooledObjID.LilypadPond_Placement_Obj, t, q);
				_baseCellObjsCached[x, y].SetCellSurfaceObj(ref objB, CellSurfObjType.Cell_Surface_LilypadPond, true);//, 0);
			}


			if (!EventManager.HasInitCharacterDialogueComplete()) return;

			FieldObjPooler._instance.InstantiateFX(PooledObjID.Default_Placement_FX, t);
		}
		private void AddWildflowerToSelectedCell()//DelegateMethod()
		{
			if (RulescopeManager._instance.GetRulescope().GetCoinCount() <= RulescopeManager._instance.GetRulescope().GetFenceCost() * 2) return;

			int x = 0;
			int y = 0;

			x = EventManager.GetGridCellClicked_X();
			y = EventManager.GetGridCellClicked_Y();

			if (_baseCellObjsCached[x, y].GetCellCurrentSurfaceObj() == CellSurfObjType.Cell_Surface_Wildflower)
				return;

			AudioManager.PlaySFX(AudioID.Audio_Placement_SFX);

			Vector3 cellClickedPos = _baseCellObjsCached[x, y].GetCellPosition();
			_baseCellObjsCached[x, y].SetCellCurrentSurfaceObj(CellSurfObjType.Cell_Surface_Wildflower);

			Quaternion q = Quaternion.Euler(0f, 0f, 0f);
			Transform t = _baseCellObjsCached[x, y].transform;

			GameObject obj = FieldObjPooler._instance.InstantiateObj(PooledObjID.Wildflower_Placement_Obj, t, q);
			_baseCellObjsCached[x, y].SetCellSurfaceObj(ref obj, CellSurfObjType.Cell_Surface_Wildflower, false);//, 0);

			if (!EventManager.HasInitCharacterDialogueComplete()) return;

			FieldObjPooler._instance.InstantiateFX(PooledObjID.Default_Placement_FX, t);
		}
		private void AddTreeToSelectedCell()//DelegateMethod()
		{
			if (RulescopeManager._instance.GetRulescope().GetCoinCount() <= RulescopeManager._instance.GetRulescope().GetFenceCost()*3) return;

			int x = 0;
			int y = 0;

			x = EventManager.GetGridCellClicked_X();
			y = EventManager.GetGridCellClicked_Y();

			if (_baseCellObjsCached[x, y].GetCellCurrentSurfaceObj() == CellSurfObjType.Cell_Surface_Tree)
				return;

			AudioManager.PlaySFX(AudioID.Audio_Placement_SFX);

			Vector3 cellClickedPos = _baseCellObjsCached[x, y].GetCellPosition();
			_baseCellObjsCached[x, y].SetCellCurrentSurfaceObj(CellSurfObjType.Cell_Surface_Tree);

			Quaternion q = Quaternion.Euler(0f, 0f, 0f);
			Transform t = _baseCellObjsCached[x, y].transform;

			GameObject obj = FieldObjPooler._instance.InstantiateObj(PooledObjID.Tree_Placement_Obj, t, q);
			_baseCellObjsCached[x, y].SetCellSurfaceObj(ref obj, CellSurfObjType.Cell_Surface_Tree, false);//, 0);

			if (!EventManager.HasInitCharacterDialogueComplete()) return;

			FieldObjPooler._instance.InstantiateFX(PooledObjID.Default_Placement_FX, t);
		}
		private void AddFenceToSelectedCell()//DelegateMethod()
        {
			//if (_ruleScope == null) return;
			//if (_ruleScope.GetCoinCount() <= _ruleScope.GetFenceCost()) return;
			if (RulescopeManager._instance.GetRulescope().GetCoinCount() <= RulescopeManager._instance.GetRulescope().GetFenceCost()) return;


			int x = 0;
			int y = 0;

			x = EventManager.GetGridCellClicked_X();
			y = EventManager.GetGridCellClicked_Y();

            if (_baseCellObjsCached[x, y].GetCellCurrentSurfaceObj() == CellSurfObjType.Cell_Surface_Fence)
                return;

            AudioManager.PlaySFX(AudioID.Audio_Placement_SFX);

			Vector3 cellClickedPos = _baseCellObjsCached[x, y].GetCellPosition();
			_baseCellObjsCached[x, y].SetCellCurrentSurfaceObj(CellSurfObjType.Cell_Surface_Fence);

			int selectedFieldId = RulescopeManager._instance.GetRulescope().GetSelectedField();
			Quaternion q = Quaternion.Euler(0f, 0f, 0f);
			Transform t = _baseCellObjsCached[x, y].transform;

			if (_baseCellObjsCached[x, y].GetCellBorderType(selectedFieldId) == CellBorderType.Cell_Border_Top) q = Quaternion.Euler(0f, 90f, 0f);
			if (_baseCellObjsCached[x, y].GetCellBorderType(selectedFieldId) == CellBorderType.Cell_Border_Bottom) q = Quaternion.Euler(0f, 90f, 0f);
			if (_baseCellObjsCached[x, y].GetCellBorderType(selectedFieldId) == CellBorderType.Cell_Border_Left) q = Quaternion.Euler(0f, 0f, 0f);
			if (_baseCellObjsCached[x, y].GetCellBorderType(selectedFieldId) == CellBorderType.Cell_Border_Right) q = Quaternion.Euler(0f, 0f, 0f);

			GameObject obj = FieldObjPooler._instance.InstantiateObj(PooledObjID.Fence_Placement_Obj, t, q);
			_baseCellObjsCached[x, y].SetCellSurfaceObj(ref obj, CellSurfObjType.Cell_Surface_Fence, false);//, 0);

			if (!EventManager.HasInitCharacterDialogueComplete()) return;
			
			FieldObjPooler._instance.InstantiateFX(PooledObjID.Default_Placement_FX, t);

			//ActivateSurfaceCells(cellClickedPos, SurfaceGridCellID.Surface_Type_Fence_A, x, y, CellSurfObjType.Cell_Surface_Fence, false);
		}
		private void AddHedgerowToSelectedCell()//DelegateMethod()
        {
			//if (_ruleScope == null) return;
			//if (_ruleScope.GetCoinCount() <= _ruleScope.GetHedgerowCost()) return;
			if (RulescopeManager._instance.GetRulescope().GetCoinCount() <= 
				RulescopeManager._instance.GetRulescope().GetHedgerowCost()) return;

			int x = 0;
			int y = 0;

			x = EventManager.GetGridCellClicked_X();
			y = EventManager.GetGridCellClicked_Y();

			//print("\nx = " + x.ToString());
			//print("\ny = " + y.ToString());

			if (_baseCellObjsCached[x, y].GetCellCurrentSurfaceObj() == CellSurfObjType.Cell_Surface_Hedge)// && _baseCellObjsCached[x, y].GetCellID() != CellObjType.Cell_Empty_Blank)
				return; //We don't need to set again, already been assigned this surface obj.

			AudioManager.PlaySFX(AudioID.Audio_Placement_SFX);

			Vector3 cellClickedPos = _baseCellObjsCached[x, y].GetCellPosition();
			_baseCellObjsCached[x, y].SetCellCurrentSurfaceObj(CellSurfObjType.Cell_Surface_Hedge);


			//ActivateSurfaceCells(cellClickedPos, SurfaceGridCellID.Surface_Type_Hedge_A, x, y, CellSurfObjType.Cell_Surface_Hedge, false);

			int selectedFieldId = RulescopeManager._instance.GetRulescope().GetSelectedField();
			//_surfaceOffset = new Vector3(18.32f, 0.5f, 2.1f);
			//_surfaceOffset = Vector3.zero;
			//_surfaceOffset.y += 0.175f;//0.25f;

			//Vector3 v = cellClickedPos + _surfaceOffset;
			Quaternion q = Quaternion.Euler(0f, 0f, 0f);
			Transform t = _baseCellObjsCached[x, y].transform;

			if (_baseCellObjsCached[x, y].GetCellBorderType(selectedFieldId) == CellBorderType.Cell_Border_Top) q = Quaternion.Euler(0f, 90f, 0f);
			if (_baseCellObjsCached[x, y].GetCellBorderType(selectedFieldId) == CellBorderType.Cell_Border_Bottom) q = Quaternion.Euler(0f, 90f, 0f);
			if (_baseCellObjsCached[x, y].GetCellBorderType(selectedFieldId) == CellBorderType.Cell_Border_Left) q = Quaternion.Euler(0f, 0f, 0f);
			if (_baseCellObjsCached[x, y].GetCellBorderType(selectedFieldId) == CellBorderType.Cell_Border_Right) q = Quaternion.Euler(0f, 0f, 0f);

			//FieldObjPooler._instance.InstantiateObj(PooledObjID.Hedgerow_Placement_Obj, t, q);
			//SurfaceObj tmp = obj.GetComponent<SurfaceObj>();//.EnablePlacementDustcloudFX();
			//if (tmp != null) tmp.EnablePlacementDustcloudFX();

			GameObject obj = FieldObjPooler._instance.InstantiateObj(PooledObjID.Hedgerow_Placement_Obj, t, q);

			_baseCellObjsCached[x, y].SetCellSurfaceObj(ref obj, CellSurfObjType.Cell_Surface_Hedge, false);//, 0);
			FieldObjPooler._instance.InstantiateFX(PooledObjID.Default_Placement_FX, t);

			RulescopeManager._instance.GetRulescope().HasCorrectHedgerowPlacementOccured(_baseCellObjsCached, x, y);


			
		}

		private void PopulateBaseGridCellsDict()
        {
			if (_baseGridCellObjDict == null) _baseGridCellObjDict = new Dictionary<FieldGridCellID, List<GameObject>>();

			//Profiler.BeginSample("AddBaseGridCellsObject");

			_baseGridCellObjDict.Add(FieldGridCellID.FieldGridCell_Grass_Tall, new List<GameObject>());

			for (int x = 0; x < _maxBaseGridCellsX; x++)
			{
				for (int y = 0; y < _maxBaseGridCellsX; y++)
				{
					AddBaseGridCellsObject(FieldGridCellID.FieldGridCell_Grass_Tall, x, y);
				}
			}
		}
		private void AddBaseGridCellsObject(FieldGridCellID fieldGridCellID, int x, int y)
		{
			GameObject newGameObject = (GameObject)Instantiate(_prefabGridCellBase);

			//_baseCellObjsCached.Add(newGameObject.GetComponent<CellObj>());
			_baseCellObjsCached[x,y] = newGameObject.GetComponent<CellObj>();

			//int index = _baseCellObjsCached.Count;
			//_baseCellObjsCached[index-1]._cellMaterials = this._cellMaterials;
			//_baseCellObjsCached[x, y]._cellMaterials = this._cellMaterials;

			newGameObject.transform.SetParent(_transform, false); // False to avoid world space recalculations
			//newGameObject.transform.parent = _transform;
			newGameObject.SetActive(false);

			if (_baseGridCellObjDict.TryGetValue(fieldGridCellID, out List<GameObject> listPlatforms)) listPlatforms.Add(newGameObject);

		}
		private GameObject GetBaseGridCellObject(FieldGridCellID fieldGridCellID)
		{
			if (_baseGridCellObjDict == null) return null;

			List<GameObject> list;
			for (int i = 0; i < _baseGridCellObjDict.Count; i++)
			{
				list = _baseGridCellObjDict[fieldGridCellID];
				for (int j = 0; j < list.Count; j++)
				{
					if (!_baseGridCellObjDict[fieldGridCellID][j].activeInHierarchy)
					{
						return _baseGridCellObjDict[fieldGridCellID][j];
					}
				}

			}
			return null;
		}

		private void PopulateFieldGridCellsDict()
		{
			//...ect
		}
		private void AddFieldGridCellsObject(FieldGridCellID fieldGridCellID)
		{
			GameObject newGameObject = (GameObject)Instantiate(_prefabGridCellObjects[(int)fieldGridCellID]);
			newGameObject.transform.parent = _transform;
			newGameObject.SetActive(false);

			if (_fieldGridCellObjDict.TryGetValue(fieldGridCellID, out List<GameObject> listPlatforms)) listPlatforms.Add(newGameObject);

		}
		private GameObject GetFieldGridCellObject(FieldGridCellID fieldGridCellID)
		{
			if (_fieldGridCellObjDict == null) return null;

			List<GameObject> list;
			for (int i = 0; i < _fieldGridCellObjDict.Count; i++)
			{
				list = _fieldGridCellObjDict[fieldGridCellID];
				for (int j = 0; j < list.Count; j++)
				{
					if (!_fieldGridCellObjDict[fieldGridCellID][j].activeInHierarchy)
					{
						return _fieldGridCellObjDict[fieldGridCellID][j];
					}
				}

			}
			return null;
		}
		private float GetGridCellWidth(FieldGridCellID fieldGridCellID)
		{
			float width = 0f;
			if (_prefabGridCellObjects != null)
			{
				width = _prefabGridCellObjects[(int)fieldGridCellID].GetComponent<MeshRenderer>().bounds.size.x;
			}
			return width;
		}
		private float GetGridCellHeight(FieldGridCellID fieldGridCellID)
		{
			float height = 0f;
			if (_prefabGridCellObjects != null)
			{
				height = _prefabGridCellObjects[(int)fieldGridCellID].GetComponent<MeshRenderer>().bounds.size.y;
			}
			return height;
		}

		public int GetGridX(int fieldGridId)
        {
			return _fieldGrids[fieldGridId].X;
		}
		public int GetGridY(int fieldGridId)
		{
			return _fieldGrids[fieldGridId].Y;
		}
		public int GetGridStartX(int fieldGridId)
		{
			return _fieldGrids[fieldGridId].START_X;
		}
		public int GetGridStartY(int fieldGridId)
		{
			return _fieldGrids[fieldGridId].START_Y;
		}

		public int GetMaxGridX() => _maxBaseGridCellsX;
		public int GetMaxGridY() => _maxBaseGridCellsY;
		public CellObj GetGridCellObj(int x, int y)
        {
			return _baseCellObjsCached[x, y];

		}
		public Vector3 GetGridStartPosition() => _baseStartPosition;
		public float GetCellWidth() => _cellWidth;
		public void DisableUnselectedFieldCrops(int currentSelectedFieldID)
		{
			for (int x = 0; x < _maxBaseGridCellsX; x++)
			{
				for (int y = 0; y < _maxBaseGridCellsY; y++)
				{
					if (_baseCellObjsCached[x, y].GetParentFieldID() != currentSelectedFieldID)
					{
						//Then only cells assigned a paraent field ID will hold a crop.
						//Unless it's -1 so check for that too.
						if (_baseCellObjsCached[x, y].GetParentFieldID() != -1)
						{
							if (_baseCellObjsCached[x, y].GetCellCurrentSurfaceObj() == CellSurfObjType.Cell_Surface_Wheat ||
								_baseCellObjsCached[x, y].GetCellCurrentSurfaceObj() == CellSurfObjType.Cell_Surface_OilSeedRape ||
								_baseCellObjsCached[x, y].GetCellCurrentSurfaceObj() == CellSurfObjType.Cell_Surface_Beans ||
								_baseCellObjsCached[x, y].GetCellCurrentSurfaceObj() == CellSurfObjType.Cell_Surface_Grass)
								_baseCellObjsCached[x, y].ApplySurfaceObjState(false);
						}
					}
				}
			}

			//_optMeshRenderer.EnableRendering();
		}
		public void RenableAllFieldCrops()
		{
			for (int x = 0; x < _maxBaseGridCellsX; x++)
			{
				for (int y = 0; y < _maxBaseGridCellsY; y++)
				{
					//if (_baseCellObjsCached[x, y].GetParentFieldID() != currentSelectedFieldID)
					//{
					//Then only cells assigned a paraent field ID will hold a crop.
					//Unless it's -1 so check for that too.
					if (_baseCellObjsCached[x, y].GetParentFieldID() != -1)
					{
						if (_baseCellObjsCached[x, y].GetCellCurrentSurfaceObj() == CellSurfObjType.Cell_Surface_Wheat ||
							_baseCellObjsCached[x, y].GetCellCurrentSurfaceObj() == CellSurfObjType.Cell_Surface_OilSeedRape ||
							_baseCellObjsCached[x, y].GetCellCurrentSurfaceObj() == CellSurfObjType.Cell_Surface_Beans ||
							_baseCellObjsCached[x, y].GetCellCurrentSurfaceObj() == CellSurfObjType.Cell_Surface_Grass)
							_baseCellObjsCached[x, y].ApplySurfaceObjState(true);
					}
					//}
				}
			}

			//_optMeshRenderer.DisableRendering();
		}


		private void PopulateSurfaceGridCellsDict()
		{
			if (_surfaceGridCellObjDict == null) _surfaceGridCellObjDict = new Dictionary<SurfaceGridCellID, List<GameObject>>();


			_surfaceGridCellObjDict.Add(SurfaceGridCellID.Surface_Type_None, new List<GameObject>());
			_surfaceGridCellObjDict.Add(SurfaceGridCellID.Surface_Type_Wheat, new List<GameObject>());
			_surfaceGridCellObjDict.Add(SurfaceGridCellID.Surface_Type_OilSeedRape, new List<GameObject>());
			//...ect

			int defaultMax_X = 24*2;//(_maxGridCellsX*4)
			int defaultMax_Y = 24*2;//(_maxGridCellsY*4)

			for (int x = 0; x < defaultMax_X; x++)
			{
				for (int y = 0; y < defaultMax_Y; y++)
				{
					AddSurfaceGridCellsObject(SurfaceGridCellID.Surface_Type_None);
					AddSurfaceGridCellsObject(SurfaceGridCellID.Surface_Type_Wheat);
					AddSurfaceGridCellsObject(SurfaceGridCellID.Surface_Type_OilSeedRape);
					//...ect
				}
			}

		}
		private bool IsTree(SurfaceGridCellID SurfaceGridCellID)
        {
			if (SurfaceGridCellID == SurfaceGridCellID.Surface_Type_Tree_A)
				return true;
			if (SurfaceGridCellID == SurfaceGridCellID.Surface_Type_Tree_B)
				return true;
			if (SurfaceGridCellID == SurfaceGridCellID.Surface_Type_Tree_C)
				return true;
			if (SurfaceGridCellID == SurfaceGridCellID.Surface_Type_Tree_D)
				return true;

			return false;
        }
		private bool IsTallGrass(SurfaceGridCellID SurfaceGridCellID)
		{
			if (SurfaceGridCellID == SurfaceGridCellID.Surface_Type_TallGrass)
				return true;

			return false;
		}
		private bool IsHedgerow(SurfaceGridCellID SurfaceGridCellID)
		{
			if (SurfaceGridCellID == SurfaceGridCellID.Surface_Type_Hedge_A)
				return true;


			return false;
		}
		private bool IsFence(SurfaceGridCellID SurfaceGridCellID)
		{
			if (SurfaceGridCellID == SurfaceGridCellID.Surface_Type_Fence_A)
				return true;


			return false;
		}
		private void AddSurfaceGridCellsObject(SurfaceGridCellID SurfaceGridCellID)
		{
			GameObject newGameObject = (GameObject)Instantiate(_prefabSurfaceObjects[(int)SurfaceGridCellID]);
			newGameObject.transform.parent = _transform;

			bool ignoreScaling = false;

			if (!ignoreScaling) ignoreScaling = IsTree(SurfaceGridCellID);
			if (!ignoreScaling) ignoreScaling = IsHedgerow(SurfaceGridCellID);
			if (!ignoreScaling) ignoreScaling = IsFence(SurfaceGridCellID);

			if (!ignoreScaling) newGameObject.transform.localScale = Vector3.one;


			newGameObject.transform.SetParent(null, true);
			//newGameObject.GetComponent<CellObj>().SetCellID(0);
			newGameObject.SetActive(false);

			if (_surfaceGridCellObjDict.TryGetValue(SurfaceGridCellID, out List<GameObject> listPlatforms)) listPlatforms.Add(newGameObject);

		}
		private GameObject GetSurfaceGridCellObject(SurfaceGridCellID SurfaceGridCellID)
		{
			if (_surfaceGridCellObjDict == null) return null;

			List<GameObject> list;
			for (int i = 0; i < _surfaceGridCellObjDict.Count; i++)
			{
				list = _surfaceGridCellObjDict[SurfaceGridCellID];
				for (int j = 0; j < list.Count; j++)
				{
                    if (!_surfaceGridCellObjDict[SurfaceGridCellID][j].activeInHierarchy)
                    {
                        return _surfaceGridCellObjDict[SurfaceGridCellID][j];
                    }
                }

			}
			return null;
		}























		private void InitBaseGridGeneration()
        {
			if (_hasBeenReset)
			{
				_cameraManager.ResetCameraToStart();
				_hasBeenReset = false;
			}

			float cellWidth = GetGridCellWidth(FieldGridCellID.FieldGridCell_Grass_Tall);
			float curFieldWidth = cellWidth * _baseGrid.X;

			for (int x = 0; x < _baseGrid.X; x++)
			{
				for (int y = 0; y < _baseGrid.Y; y++)
				{
					if (x == 0 && y == 0)//Start Position
					{
						_curPosition = _startPosition;
						_baseStartPosition = _startPosition;
						_cellWidth = cellWidth;
					}
					else
						_curPosition = _startPosition + new Vector3(x * _baseGrid.CELL_SIZE, 0,
							y * _baseGrid.CELL_SIZE);

					if (x == _baseGrid.X / 2 && y == _baseGrid.Y / 2)
					{
						_centerPoint = _curPosition;

					}

					GameObject obj = GetBaseGridCellObject(FieldGridCellID.FieldGridCell_Grass_Tall);
					if (obj == null) return;

					

					obj.transform.position = _curPosition;
					obj.SetActive(true); //CellObj class fires OnEnable to retrieve assigned grid position at this point. 

					obj.GetComponent<CellObj>().SetCellPosition(x, y);
					obj.GetComponent<CellObj>().PopulateSharedMaterialReference(_cellMaterials);
					_baseCellObjsCached[x, y].PopulateSharedMaterialReference(_cellMaterials);

				}
			}




            for (int i = 0; i < _fieldCount; i++)
            {
				ActivateFieldCells(i);
				
			}
			//_optMeshRenderer.InitializeMatrices();



			//...ect
		}

		private void ActivateFieldCells(int fieldID)
        {
			int cellStartX = 5;
			int cellStartY = 5;
			int cellWidth = _fieldGrids[fieldID].X;
			int cellHeight = _fieldGrids[fieldID].Y;

			//...ect
		}
		private void ApplyBorderIfValid(int x, int y, CellBorderType side, int fieldID)
		{
			// Bounds check
			if (x >= 0 && x < _baseCellObjsCached.GetLength(0) &&
				y >= 0 && y < _baseCellObjsCached.GetLength(1))
			{
				//var cell = _baseCellObjsCached[x, y];

				//if (fieldID > 0)
				//	print("\nbegin checking issue here. ApplyBorderIfValid() FieldGenerator.cs");
				//if (cell.GetCellBorderType(fieldID) != CellBorderType.Cell_Border_Unspecified) return;

				RulescopeManager._instance.GetRulescope().SetFieldCellBorder(fieldID, x, y, side);
				bool isDefaultSetup = _cameraManager.GetDefaultSetupFlag();

				switch (side)
				{
					case CellBorderType.Cell_Border_Top:

						//cell.SetCellBorderType(CellBorderType.Cell_Border_Top, fieldID, isDefaultSetup);
						_baseCellObjsCached[x, y].SetCellBorderType(CellBorderType.Cell_Border_Top, fieldID, isDefaultSetup, x, y);

						//RulescopeManager._instance.GetRulescope().SetCorrectHedgerowPlacementStartPoint(x, y, CellBorderType.Cell_Border_Top);

						//cell.ApplyCellType(CellObjType.Cell_Soil_Light);
						break;
					case CellBorderType.Cell_Border_Bottom:
						_baseCellObjsCached[x, y].SetCellBorderType(CellBorderType.Cell_Border_Bottom, fieldID, isDefaultSetup, x, y);
						//cell.ApplyCellType(CellObjType.Cell_Soil_Light);
						break;
					case CellBorderType.Cell_Border_Left:
						_baseCellObjsCached[x, y].SetCellBorderType(CellBorderType.Cell_Border_Left, fieldID, isDefaultSetup, x, y);
						//cell.ApplyCellType(CellObjType.Cell_Soil_Light);
						break;
					case CellBorderType.Cell_Border_Right:
						_baseCellObjsCached[x, y].SetCellBorderType(CellBorderType.Cell_Border_Right, fieldID, isDefaultSetup, x, y);
						//cell.ApplyCellType(CellObjType.Cell_Soil_Light);
						break;
				}

				//_baseCellObjsCached[x, y].GetComponent<CellObj>() = cell;
			}
		}
		private void ActivateSurfaceCells(Vector3 spawnPosition, SurfaceGridCellID SurfaceGridCellID, int x, int y, CellSurfObjType cellSurfObjType, bool ignoreInitRemove)
        {
			GameObject obj = GetSurfaceGridCellObject(SurfaceGridCellID);
			if (obj == null) return;

			int selectedFieldId = RulescopeManager._instance.GetRulescope().GetSelectedField();

			//...ect
		}
		private void ActivateCropCells(Vector3 spawnPosition, int fieldID, int x, int y, CellSurfObjType cellSurfObjType, int i)
        {
			if (!_includeCrop) return;

			SurfaceGridCellID _tempCropId = SurfaceGridCellID.Surface_Type_None;

			if (_fieldGrids[fieldID].SURFACE_TYPE == SurfaceGridCellID.Surface_Type_None)
				_tempCropId = SurfaceGridCellID.Surface_Type_None;
			if (_fieldGrids[fieldID].SURFACE_TYPE == SurfaceGridCellID.Surface_Type_Wheat)
				_tempCropId = SurfaceGridCellID.Surface_Type_Wheat;
			if (_fieldGrids[fieldID].SURFACE_TYPE == SurfaceGridCellID.Surface_Type_OilSeedRape)
				_tempCropId = SurfaceGridCellID.Surface_Type_OilSeedRape;
			if (_fieldGrids[fieldID].SURFACE_TYPE == SurfaceGridCellID.Surface_Type_Beans)
				_tempCropId = SurfaceGridCellID.Surface_Type_Beans;
			if (_fieldGrids[fieldID].SURFACE_TYPE == SurfaceGridCellID.Surface_Type_SugarBeat)
				_tempCropId = SurfaceGridCellID.Surface_Type_SugarBeat;
			//if (_fieldGrids[fieldID].SURFACE_TYPE == SurfaceGridCellID.Surface_Type_TallGrass)
			//	_tempCropId = SurfaceGridCellID.Surface_Type_TallGrass;


			GameObject obj = GetSurfaceGridCellObject(_tempCropId);
			if (obj == null) return;


			//print("\ni = " + i.ToString());
			_surfaceOffset = new Vector3(18.32f, 0.5f, 2.3f);
			//_optMeshRenderer.PopulatePositions(spawnPosition, i);
			//if (i == 0)
			//{
			//	_optMeshRenderer.SetScale(obj.transform.lossyScale);
			//	_optMeshRenderer.SetRotation(obj.transform.rotation);
			//}



			obj.transform.position = spawnPosition + _surfaceOffset;

			//obj.transform.Find("Row").position = _curPosition;
			//obj.transform.position = _curPosition;

			obj.SetActive(true);
			_baseCellObjsCached[x, y].SetCellSurfaceObj(ref obj, cellSurfObjType, false);//, 0);
		}





















		private void RunInitGenerationProcess()
		{
			if (_hasBeenReset)
			{
				_cameraManager.ResetCameraToStart();
				_hasBeenReset = false;
				//PopulateFieldGridCellsDict();
				//PopulateSurfaceGridCellsDict();
			}
			for (int i = 0; i < _fieldCount; i++)
            {
				GenerateFieldGrid(i,_gridCellType);
			}

			InitCameraStartView();
		}
		private void InitCameraStartView()
        {
			_centerPoint = Vector3.zero;

			if (_fieldCount == 1)
				_centerPoint = _fieldGridCenterPointsCached[0];
			if (_fieldCount == 2)
				_centerPoint = (_fieldGridCenterPointsCached[0] + _fieldGridCenterPointsCached[1]) / 2;
			if (_fieldCount == 3)
				_centerPoint = (_fieldGridCenterPointsCached[0] + _fieldGridCenterPointsCached[1] + _fieldGridCenterPointsCached[2]) / 3;
			if (_fieldCount == 4)
				_centerPoint = (_fieldGridCenterPointsCached[0] + _fieldGridCenterPointsCached[1] + _fieldGridCenterPointsCached[2] + _fieldGridCenterPointsCached[3]) / 4;

			_cameraManager.EnableStartPivotPoint(_centerPoint, _fieldGridCenterPointsCached);


			for (int i = 0; i < _fieldCount; i++)
            {
				CharacterManager.SetFieldGridCenterPosition(i, _fieldGridCenterPointsCached[i]);
			}
			//CharacterManager.SetCharacterPosition(CharacterPrefabID.Character_Human_Farmer, _fieldGridCenterPointsCached[0]);
		}
		private void AdjustForNextFieldGridGen(int fieldGridID, FieldGridCellID fieldGridCellID, float curFieldWidth, float cellWidth)
        {
			if (fieldGridID == 0)
			{
				//This is too help align the 4th field iteration.
				_originalPosition = _startPosition;
				_originalPosition.z += curFieldWidth + cellWidth;

				_startPosition.x += curFieldWidth + cellWidth;
			}
			//...ect
		}
		private void GenerateFieldGrid(int fieldGridID, FieldGridCellID fieldGridCellID)
		{


			float cellWidth = GetGridCellWidth(fieldGridCellID);
			float curFieldWidth = cellWidth * _fieldGrids[fieldGridID].X;

			

			for (int x = 0; x < _fieldGrids[fieldGridID].X; x++)
			{
				for (int y = 0; y < _fieldGrids[fieldGridID].Y; y++)
				{
					GenerateCell(fieldGridCellID, x, y, fieldGridID);
				}
			}

			AdjustForNextFieldGridGen(fieldGridID, fieldGridCellID, curFieldWidth, cellWidth);

		}
		private void GenerateCell(FieldGridCellID fieldGridCellID, int x, int y, int fieldGridID)
		{
			if (x == 0 && y == 0)//Start Position
				_curPosition = _startPosition;
			else
				_curPosition = _startPosition + new Vector3(x * _fieldGrids[fieldGridID].CELL_SIZE, 0, y * _fieldGrids[fieldGridID].CELL_SIZE);


			GameObject obj = GetBaseGridCellObject(FieldGridCellID.FieldGridCell_Grass_Tall);
			if (obj == null) return;

			//...ect
		}
		private void GenerateCrop(SurfaceGridCellID SurfaceGridCellID, int x, int y, ref Vector3 currentPos, int fieldGridID)
		{
			if (!_includeCrop) return;

			if (x == 0 && y == 0)//Start Position
				_curPosition = _startPosition;
			else
				_curPosition = _startPosition + new Vector3(x * _fieldGrids[fieldGridID].CELL_SIZE, 0, y * _fieldGrids[fieldGridID].CELL_SIZE);

			GameObject obj = GetSurfaceGridCellObject(SurfaceGridCellID);
			if (obj == null) return;


			//...ect
		}











		public void InitFieldGeneration() //Call from outside of this Class.
		{
			if (!_initFieldGeneration)
			{
				InitBaseGridGeneration();
				//RunInitGenerationProcess();
				_initFieldGeneration = true;
				EventManager.TriggerEvent("Process_Default_Auto_Fence_Placements");
			}
		}
		public void SetFieldCount(int count)
		{
			_fieldCount = count;
		}
		public void AddFieldGrid(int id, FieldGridSize fieldGrid)
		{
			if (_fieldGrids == null) return;
			_fieldGrids.Add(id, fieldGrid);

		}
		public void SetFieldType(FieldGridCellID fieldSoilTypeID)
		{
			_gridCellType = fieldSoilTypeID;
		}
		public void SetCropType(SurfaceGridCellID SurfaceGridCellID)
		{
			_surfaceType = SurfaceGridCellID;
		}
		public FieldGridCellID GetFieldType() => _gridCellType;
		public Vector3 GetGridCenterPosition(int fieldId)
		{
			return _fieldGridCenterPointsCached[fieldId];
			//return _centerPoint;
		}
		public int GetFieldCount() => _fieldCount;
		public string GetFieldGridSize(int fieldGridId)
		{
			if (_fieldGrids[fieldGridId].X == 5 && _fieldGrids[fieldGridId].Y == 5) return "Extra Small";
			if (_fieldGrids[fieldGridId].X == 10 && _fieldGrids[fieldGridId].Y == 10) return "Small";
			if (_fieldGrids[fieldGridId].X == 15 && _fieldGrids[fieldGridId].Y == 15) return "Medium";
			if (_fieldGrids[fieldGridId].X == 20 && _fieldGrids[fieldGridId].Y == 20) return "Large";
			if (_fieldGrids[fieldGridId].X == 25 && _fieldGrids[fieldGridId].Y == 25) return "Extra Large";
			if (_fieldGrids[fieldGridId].X == 30 && _fieldGrids[fieldGridId].Y == 30) return "Massive";
			//if (_gridSizeX == 5 && _gridSizeY == 5) return "Extra Small";
			//if (_gridSizeX == 10 && _gridSizeY == 10) return "Small";
			//if (_gridSizeX == 15 && _gridSizeY == 15) return "Medium";
			//if (_gridSizeX == 20 && _gridSizeY == 20) return "Medium";
			//if (_gridSizeX == 25 && _gridSizeY == 25) return "Large";
			//if (_gridSizeX == 30 && _gridSizeY == 30) return "Extra Large";

			return "";
		}
		public string GetFieldSoilType()
		{
			return "";
		}

		public void EnableCropLayer(bool isAllowed)
		{
			_includeCrop = isAllowed;

			if (_includeCrop)
			{
				return;
			}
			if (!_includeCrop)
			{
				return;
			}
		}
	}
}
