using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DoomBreakers;
using wildlogicgames;

namespace IsometricFarmGenerator
{
    public class SurfaceObj : MonoBehaviour
    {

        public CellSurfObjType _cellSurfaceObjType;
        //public ParticleSystem _placementDustcloudFX;
        //public ParticleSystem _placementCorrectAwardFX;

        private void Awake() => Setup();
        private void Setup()
        {
            //_cellSurfaceObjType = CellSurfObjType.Cell_Surface_Empty;

            //if(_placementDustcloudFX != null)
            //{
            //    _placementDustcloudFX.gameObject.SetActive(false);
                //_placementDustcloudFX.Stop();
            //}
            //if (_placementCorrectAwardFX != null)
            //{
            //    _placementCorrectAwardFX.Stop();
            //}
        }

        public void EnablePlacementDustcloudFX()
        {
            FieldObjPooler._instance.InstantiateFX(PooledObjID.Default_Placement_FX, this.transform);


            //if (_placementDustcloudFX == null) return;

            //if (!_placementDustcloudFX.gameObject.activeSelf)
            //    _placementDustcloudFX.gameObject.SetActive(true);
            //_placementDustcloudFX.Stop();
            //_placementDustcloudFX.Play();
        }

        //public void EnablePlacementCorrectAwardFX()
        //{
        //    if (_placementCorrectAwardFX == null) return;

        //    _placementCorrectAwardFX.Stop();
        //    _placementCorrectAwardFX.Play();
        //}
    }
}
