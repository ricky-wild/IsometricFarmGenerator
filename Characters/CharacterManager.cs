using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DoomBreakers;
using wildlogicgames;


namespace IsometricFarmGenerator
{
    public enum CharacterDialogueID
    {
        Character_Dialogue_None = 0,
        Character_Dialogue_Welcome = 1,
        Character_Dialogue_Speech_A = 2,
        Character_Dialogue_Speech_B = 3,
        Character_Dialogue_Speech_C = 4,
        Character_Dialogue_Speech_D = 5,
        Character_Dialogue_Speech_E = 6,
        Character_Dialogue_Speech_F = 7,
        Character_Dialogue_Speech_G = 8,
        Character_Dialogue_Ending = 9,
        Character_Dialogue_Exit = 10,

        Character_Dialogue_Hedgerow_Full_Speech_A = 11,
        Character_Dialogue_Hedgerow_Full_Speech_B = 12,
    };
    public enum CharacterPrefabID //_characterPrefabs
    {
        Character_Human_Farmer_A = 0,
        Character_Human_Farmer_B = 1,
    };

    public class CharacterManager : MonoBehaviour
    {
        //[Header("Character Camera")]
        //public static Transform _characterCameraTransform;

        [Header("Character Prefabs")]
        [Tooltip("Must be populated in order of CharacterPrefabID")]
        public GameObject[] _characterPrefabs;
        private static GameObject[] _characterPrefab = new GameObject[2];
        private static Dictionary<CharacterPrefabID, List<GameObject>> _characterObjsDict;

        private static CharacterDialogueID _characterDialogueID;

        private static Vector3[] _fieldGridCenterPointsCached = new Vector3[4];
        private static Vector3 _currentCharacterPos = new Vector3();

        public static CharacterManager _instance = null;
        private static Transform _transform;

        private void Awake() => Setup();
        private void Setup()
        {
            if (_instance == null)
            {
                _instance = this;
                _transform = this.transform;

                _characterDialogueID = CharacterDialogueID.Character_Dialogue_None;

                for (int i = 0; i < _characterPrefabs.Length; i++)
                {
                    _characterPrefab[i] = _characterPrefabs[i];
                }

                if (_characterObjsDict == null) _characterObjsDict = new Dictionary<CharacterPrefabID, List<GameObject>>();

                _characterObjsDict.Add(CharacterPrefabID.Character_Human_Farmer_A, new List<GameObject>());
                _characterObjsDict.Add(CharacterPrefabID.Character_Human_Farmer_B, new List<GameObject>());
                AddCharacter(CharacterPrefabID.Character_Human_Farmer_A);
                AddCharacter(CharacterPrefabID.Character_Human_Farmer_B);

            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }
        private static void AddCharacter(CharacterPrefabID characterID)
        {
            GameObject newGameObject = (GameObject)Instantiate(_characterPrefab[(int)characterID]);
            newGameObject.transform.parent = _transform;
            newGameObject.name = "Farmer Character Obj";
            newGameObject.SetActive(false);

            if (_characterObjsDict.TryGetValue(characterID, out List<GameObject> listPlatforms)) listPlatforms.Add(newGameObject);

        }
        private static GameObject GetCharacter(CharacterPrefabID characterID)
        {
            if (_characterObjsDict == null) return null;

            List<GameObject> list;
            for (int i = 0; i < _characterObjsDict.Count; i++)
            {
                list = _characterObjsDict[characterID];
                for (int j = 0; j < list.Count; j++)
                {
                    if (!_characterObjsDict[characterID][j].activeInHierarchy)
                    {
                        return _characterObjsDict[characterID][j];
                    }
                }

            }
            return null;
        }


        public static void SpawnCharacterPosition(CharacterPrefabID characterPrefabID, int fieldId)//Vector3 position)
        {
            if (_characterObjsDict == null) return;

            GameObject obj = GetCharacter(characterPrefabID);
            if (obj == null) return;

            obj.transform.position = _fieldGridCenterPointsCached[fieldId];//position;
            obj.SetActive(true);

            _currentCharacterPos = obj.transform.position;

            //Vector3 v = obj.transform.position;
            //v.z -= 2.0f;
            //_characterCameraTransform.position = v;

            //_characterObjsDict[characterPrefabID]
            //_characterPrefabs[(int)characterPrefabID].transform.position = position;
        }
        public static void SetFieldGridCenterPosition(int fieldId, Vector3 pos)
        {
            pos.y += 0.075f;// 0.125f;
            _fieldGridCenterPointsCached[fieldId] = pos;
        }
        public static Vector3 GetCurrentCharacterSpawnPos() => _currentCharacterPos;

        public static CharacterDialogueID GetCharacterDialogueID() => _characterDialogueID;
        public static void SetCharacterDialogueID(CharacterDialogueID id) => _characterDialogueID = id;
    }
}
