using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using wildlogicgames;

namespace IsometricFarmGenerator
{
	public enum PooledObjID
    {
		Default_Placement_FX = 0,
		Correct_Placement_Award_FX = 1,
		Default_Removal_FX = 2,
		Grass_Growth_FX = 3,
		Hedgerow_Placement_Obj = 4,
		Fence_Placement_Obj = 5,
		Tree_Placement_Obj = 6,
		Wildflower_Placement_Obj = 7,
		Waterpool_Placement_Obj = 8,
		LilypadPond_Placement_Obj = 9,
	};

    public class FieldObjPooler : MonoBehaviour
    {
        public static FieldObjPooler _instance;

		[Header("Prefabs To Pool")]
		[Tooltip("The order must corrispond to enum PooledObjID")]
		public GameObject[] _prefabGameObjects;

		//[Header("Typical Pool Size")]
		//[Tooltip("The starting size of each pool.")]
		private const int _poolSizeHedgerowPlacementObj = 256;
		private const int _poolSizeFencePlacementObj = 256;
		private const int _poolSizeTreePlacementObj = 64;//128;
		private const int _poolSizeWildflowerPlacementObj = 128;
		private const int _poolSizeWaterpoolPlacementObj = 128;
		private const int _poolSizeLilypadPondPlacementObj = _poolSizeWaterpoolPlacementObj/4;

		private const int _poolSizeDefaultPlacementFX = 8;
		private const int _poolSizeRemovalPlacementFX = 8;
		private const int _poolSizeGrowthPlacementFX = 8;
		private const int _poolSizeCorrectPlacementFX = 23;


		private const int _maxPoolSizePerType = 10;
		private Transform _transform;
		private Vector3 _vectorOffset;
		private float _incrementZ = 0f;

		public Dictionary<PooledObjID, List<GameObject>> _pooledObjsDict;

        private void OnDisable()
        {
			_pooledObjsDict.Clear();
			_pooledObjsDict = null;
			_instance = null;
			Destroy(gameObject);
		}

        private void Awake() //=> Setup();
		{
			if (_instance == null)
			{
				_instance = this;
				_transform = this.transform;
				Setup();
			}
			else if (_instance != this)
			{
				Destroy(gameObject);
			}
			DontDestroyOnLoad(gameObject);


		}
		private void Setup()
        {
			if (_prefabGameObjects == null) return;

			for (int i = 0; i < _prefabGameObjects.Length; i++)
            {
				_prefabGameObjects[i].SetActive(false);
			}

			if (_pooledObjsDict == null) _pooledObjsDict = new Dictionary<PooledObjID, List<GameObject>>();

			_pooledObjsDict.Add(PooledObjID.Default_Placement_FX, new List<GameObject>());
			_pooledObjsDict.Add(PooledObjID.Correct_Placement_Award_FX, new List<GameObject>());
			_pooledObjsDict.Add(PooledObjID.Default_Removal_FX, new List<GameObject>());
			_pooledObjsDict.Add(PooledObjID.Grass_Growth_FX, new List<GameObject>());

			_pooledObjsDict.Add(PooledObjID.Hedgerow_Placement_Obj, new List<GameObject>());
			_pooledObjsDict.Add(PooledObjID.Fence_Placement_Obj, new List<GameObject>());
			_pooledObjsDict.Add(PooledObjID.Tree_Placement_Obj, new List<GameObject>());
			_pooledObjsDict.Add(PooledObjID.Wildflower_Placement_Obj, new List<GameObject>());
			_pooledObjsDict.Add(PooledObjID.Waterpool_Placement_Obj, new List<GameObject>());
			_pooledObjsDict.Add(PooledObjID.LilypadPond_Placement_Obj, new List<GameObject>());

			for (int i = 0; i < _poolSizeDefaultPlacementFX; i++) AddGameObject(PooledObjID.Default_Placement_FX);
			for (int i = 0; i < _poolSizeRemovalPlacementFX; i++) AddGameObject(PooledObjID.Default_Removal_FX);
			for (int i = 0; i < _poolSizeGrowthPlacementFX; i++) AddGameObject(PooledObjID.Grass_Growth_FX);
			for (int i = 0; i < _poolSizeCorrectPlacementFX; i++) AddGameObject(PooledObjID.Correct_Placement_Award_FX);

			for (int i = 0; i < _poolSizeHedgerowPlacementObj; i++) AddGameObject(PooledObjID.Hedgerow_Placement_Obj);
			for (int i = 0; i < _poolSizeFencePlacementObj; i++) AddGameObject(PooledObjID.Fence_Placement_Obj);
			for (int i = 0; i < _poolSizeTreePlacementObj; i++) AddGameObject(PooledObjID.Tree_Placement_Obj);
			for (int i = 0; i < _poolSizeWildflowerPlacementObj; i++) AddGameObject(PooledObjID.Wildflower_Placement_Obj);
			for (int i = 0; i < _poolSizeWaterpoolPlacementObj; i++) AddGameObject(PooledObjID.Waterpool_Placement_Obj);
			for (int i = 0; i < _poolSizeLilypadPondPlacementObj; i++) AddGameObject(PooledObjID.LilypadPond_Placement_Obj);


		}

		private void AddGameObject(PooledObjID pooledObjID)
		{
			//Reference to the on-disk asset. Don't do anything to that,Instead, use Instantiate<GameObject>() and use the copy.

			GameObject newGameObject = (GameObject)Instantiate(_prefabGameObjects[(int)pooledObjID]);
			newGameObject.transform.parent = _transform;
			newGameObject.SetActive(false);

			if (_pooledObjsDict.TryGetValue(pooledObjID, out List<GameObject> listPlayer)) listPlayer.Add(newGameObject);

		}

		private GameObject GetPooledObject(PooledObjID pooledObjID)
		{
			if (_pooledObjsDict == null) return null;

			if (_pooledObjsDict.TryGetValue(pooledObjID, out List<GameObject> list))
			{
				for (int i = 0; i < list.Count; i++)
				{
					GameObject obj = list[i];
					if (obj != null && !obj.activeInHierarchy)
					{
						obj.SetActive(true); // Enable the object
						return obj;
					}
				}

				// No inactive object found, instantiate a new one
				//Debug.Log("Instantiating from obj pool!");
				//GameObject newGameObject = Instantiate(_prefabGameObjects[(int)pooledObjID]);
				//list.Add(newGameObject);
				//return newGameObject;
			}

			return null;
		}


		public GameObject InstantiateObj(PooledObjID pooledObjID, Transform positionToSpawn, Quaternion q)
		{

			GameObject obj = GetPooledObject(pooledObjID);

			if (obj == null) return null;
			if (positionToSpawn == null) return null;

			_vectorOffset = positionToSpawn.position;

			Vector3 v = Vector3.zero;

			if (pooledObjID == PooledObjID.Hedgerow_Placement_Obj)
			{			
				v.y += 0.175f;//0.25f;
			}
			if (pooledObjID == PooledObjID.Fence_Placement_Obj)
			{
				v.x += 0.01f;
				v.y += 0.075f;
			}
			if (pooledObjID == PooledObjID.Tree_Placement_Obj)
			{
				v.y += 0.175f;//0.25f;
			}
			if (pooledObjID == PooledObjID.Wildflower_Placement_Obj)
			{
				v.x = 18.32f;//, 0.5f, 2.3f);
				v.y = 0.475f;
				//v.y += 0.175f;//0.25f;
				v.z = 2.3f;
			}
			if (pooledObjID == PooledObjID.Waterpool_Placement_Obj)
			{
				//v.y += 0.025f;//0.5f;
				v.y -= 0.0005f;//0.001f;
			}
			if (pooledObjID == PooledObjID.LilypadPond_Placement_Obj)
			{
				v.x = 18.32f;//, 0.5f, 2.3f);
				v.y = 0.475f;
				//v.y -= 0.0006f;//0.001f;
				v.z = 2.155f;// 2.3f;
			}


			obj.transform.position = _vectorOffset + v;
			obj.transform.rotation = q;
			obj.transform.parent = _transform;
			obj.SetActive(true);

			return obj;
		}

		public void InstantiateFX(PooledObjID pooledObjID, Transform positionToSpawn)
		{
			//if (pooledObjID == PooledObjID.Correct_Placement_Award_FX) return;

			GameObject obj = GetPooledObject(pooledObjID);

			if (obj == null) return;
			if (positionToSpawn == null) return;

			_vectorOffset = positionToSpawn.position;


            if (pooledObjID == PooledObjID.Hedgerow_Placement_Obj)
            {

				Vector3 v = Vector3.zero;
				v.y += 0.175f;//0.25f;

				Quaternion q = Quaternion.Euler(0f, 0f, 0f);
			}



            obj.transform.position = _vectorOffset;
			//obj.transform.rotation = positionToSpawn.rotation;
			obj.transform.parent = _transform;
			obj.SetActive(true);

			if (pooledObjID == PooledObjID.Default_Placement_FX) obj.GetComponent<ParticleSystem>().Play();
			if (pooledObjID == PooledObjID.Correct_Placement_Award_FX) 
				obj.GetComponent<ParticleSystem>().Play();
			if (pooledObjID == PooledObjID.Default_Removal_FX) obj.GetComponent<ParticleSystem>().Play();
			if (pooledObjID == PooledObjID.Grass_Growth_FX) obj.GetComponent<ParticleSystem>().Play();
		}



	}
}
