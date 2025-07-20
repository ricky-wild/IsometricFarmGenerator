using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using wildlogicgames;


namespace IsometricFarmGenerator
{
    public enum EventFlagID
    {
        EventFlag_Idle,
        EventFlag_Initial_Character_Camera_Process,
        EventFlag_Initial_Character_Dialogue_Init,
        EventFlag_Hedgerow_FullPlacement_Character_Dialogue_Init,
        EventFlag_BadgeReward_Process,
    };
    public enum MouseClickStateID
    {
        OnClick_Nothing = 0,
        OnClick_Paint_Cell_Soil_Peat = 1,
        OnClick_Paint_Cell_Soil_Light = 2,
        OnClick_Paint_Cell_Soil_Heavy = 3,
        OnClick_Paint_Cell_Hedgerow = 4,

        OnClick_Tool_Selection = 6,

        OnClick_Paint_Cell_Fence = 5,

        OnClick_Camera_Manual_Rotate = 7,
        OnClick_Camera_Manual_Pan = 8,
        OnClick_Camera_Manual_Zoom = 9,

        OnClick_Paint_Cell_Tree = 10,
        OnClick_Paint_Cell_Wildflower = 11,
        OnClick_Paint_Cell_Waterpool = 12,
    };
    public class EventManager : MonoBehaviour
    {
        private Dictionary<string, Action> _eventDictionary;
        private static EventManager _eventManager;
        private static MouseClickStateID _mouseClickStateID;
        private static int _clickedCell_X;
        private static int _clickedCell_Y;

        //private static int _currentSelectedFieldId;


        private static EventFlagID _eventState;
        private static bool _hasInitCharcaterDialogue, _isPerformanceMode;

        //private static int _coinCount;

        //private static RuleScope _ruleScopeScoring;

        public static EventManager _instance
        {
            get //When we access our instance from another place, we'll setup as appropriate if required.
            {
                if (!_eventManager)
                {
                    //FindObjectOfType isn't a cheap call but we only do this once, if not at all.
                    _eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                    if (!_eventManager)
                    {
                        //print("\nEventManager= You need an active EventManager script attached to a GameObject within the scene!");
                    }
                    else
                        _eventManager.Setup();
                }

                return _eventManager;
            }
        }
        public static void Subscribe(string eventName, Action listener)
        {
            Action thisEvent;

            //out: differs from the ref keyword in that it does not require parameter variables to be
            //initialized before they are passed to a method. Must be explicitly declared in the method
            //definition​ as well as in the calling method.
            if (_instance._eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                //Add another event to the existing ones.
                thisEvent += listener;

                //Update the dictionary.
                _instance._eventDictionary[eventName] = thisEvent;
            }
            else
            {
                //Add the event to the dictionary for the first time.
                thisEvent += listener;
                _instance._eventDictionary.Add(eventName, thisEvent);
            }
        }
        public static void Unsubscribe(string eventName, Action listener)
        {
            if (_instance == null) return;

            Action thisEvent;
            if (_instance._eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                //Remove the event from the existing ones.
                thisEvent -= listener;

                //Now update the dictionary.
                _instance._eventDictionary[eventName] = thisEvent;
            }
        }
        public static void TriggerEvent(string eventName)
        {
            Action thisEvent = null;
            if (_instance._eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                _instance._eventDictionary[eventName]();
                //thisEvent.Invoke();
            }
        }









        private void Setup()
        {
            if (_eventDictionary == null) _eventDictionary = new Dictionary<string, Action>();
            //if (_ruleScopeScoring == null) _ruleScopeScoring = new RuleScope();

            //_mouseClickStateID = MouseClickStateID.OnClick_Paint_Cell_Soil_Peat;


            _eventState = EventFlagID.EventFlag_Idle;

            _hasInitCharcaterDialogue = false;
            _isPerformanceMode = false;
        }
        private void OnDisable() => FreeMemory();
        public void FreeMemory()
        {
            if (_eventDictionary != null) _eventDictionary.Clear();
        }
        public static MouseClickStateID GetMouseClickStateID() => _mouseClickStateID;
        public static void SetMouseClickStateID(MouseClickStateID clickStateID) => _mouseClickStateID = clickStateID;

        public static void SetGridCellIDClicked(int x, int y)
        {
            _clickedCell_X = x;
            _clickedCell_Y = y;
            //EventManager.TriggerEvent("Report_OnClick_Hedgerow");
        }
        public static int GetGridCellClicked_X() => _clickedCell_X;
        public static int GetGridCellClicked_Y() => _clickedCell_Y;

        public static void SetEventFlag(EventFlagID eventFlagID)//, bool b)
        {
            _eventState = eventFlagID;

            if (_eventState == EventFlagID.EventFlag_Initial_Character_Dialogue_Init) _hasInitCharcaterDialogue = true;
        }
        public static EventFlagID GetEventFlag()//EventFlagID eventFlagID)
        {
            return _eventState;
        }
        public static bool HasInitCharacterDialogueComplete() => _hasInitCharcaterDialogue;
        public static void SetPerformanceMode(bool isPerformance) => _isPerformanceMode = isPerformance;
        public static bool IsPerformanceMode() => _isPerformanceMode;
    }
}
