using System.Collections.Generic;
using UnityEngine;

namespace IsometricFarmGenerator
{
    public class OPTMeshRenderer : MonoBehaviour
    {
        public Mesh _mesh;
        public Material _material;

        private List<Matrix4x4[]> _matrixBatches = new List<Matrix4x4[]>();

        private int _fieldCount = 4;
        private int _totalCount = 484; // Example: 22x22 = 484
        private Vector3[] _positions = new Vector3[1936];
        private Vector3 _scale = Vector3.one;
        private Quaternion _rotation = Quaternion.identity;

        private bool _scaleSet, _rotationSet, _positionsSet;
        private bool _allowRender = false;

        private void Awake() => Setup();

        private void Setup()
        {
            _scaleSet = false;
            _rotationSet = false;
            _positionsSet = false;
        }

        public void InitializeMatrices()
        {
            _matrixBatches.Clear();

            const int batchSize = 1023;
            int totalInstanceCount = _totalCount * _fieldCount;

            int fullBatches = totalInstanceCount / batchSize;
            int remainder = totalInstanceCount % batchSize;

            int index = 0;

            for (int b = 0; b < fullBatches; b++)
            {
                Matrix4x4[] batch = new Matrix4x4[batchSize];
                for (int i = 0; i < batchSize; i++, index++)
                {
                    if (index >= _positions.Length)
                    {
                        print("\nIndex exceeded _positions length!");
                        break;
                    }

                    batch[i] = Matrix4x4.TRS(_positions[index], _rotation, _scale);
                }
                _matrixBatches.Add(batch);
            }

            if (remainder > 0)
            {
                Matrix4x4[] finalBatch = new Matrix4x4[remainder];
                for (int i = 0; i < remainder; i++, index++)
                {
                    if (index >= _positions.Length)
                    {
                        print("\nIndex exceeded _positions length in final batch!");
                        break;
                    }

                    finalBatch[i] = Matrix4x4.TRS(_positions[index], _rotation, _scale);
                }
                _matrixBatches.Add(finalBatch);
            }
        }

        public void SetScale(Vector3 scale)
        {
            _scaleSet = true;
            _scale = scale;
        }

        public void SetRotation(Quaternion rotation)
        {
            _rotationSet = true;
            _rotation = rotation;
        }

        public void PopulatePositions(Vector3 pos, int i)
        {
            _positionsSet = true;
            _positions[i] = pos;
        }

        public void EnableRendering()
        {
            if (!_scaleSet || !_rotationSet || !_positionsSet) return;
            if (!_mesh || !_material) return;

            //InitializeMatrices();
            _allowRender = true;
        }

        public void DisableRendering()
        {
            _allowRender = false;
        }

        void OnRenderObject()
        {
            if (!_allowRender) return;

            foreach (var batch in _matrixBatches)
            {
                Graphics.DrawMeshInstanced(_mesh, 0, _material, batch);
            }
        }
    }
}