using System;
using UnityEngine;

namespace Resat.Behaviours
{
    public class ThrownBottle : MonoBehaviour
    {
        public Renderer? Renderer => _renderer;
        public Collider? Collider => _collider;
        
        [NonSerialized]
        public bool Seen = false;

        [SerializeField]
        private Renderer? _renderer;
        
        [SerializeField]
        private Collider? _collider;
        
        [SerializeField]
        private float _rotationAcceleration;

        [SerializeField]
        private float _maxRotationSpeed;

        [SerializeField]
        private float _startRotationSpeed;

        [SerializeField]
        private float _moveSpeed;

        [SerializeField]
        private float _angle;
        
        [SerializeField]
        private float _sideAngle;

        [SerializeField]
        private float _moveSpeedDropOff;

        [SerializeField]
        private float _angleDropOff;
        
        private float _actualRotationSpeed;
        private float _actualMoveSpeed;
        private float _actualAngle;
        
        private void Start()
        {
            _actualRotationSpeed = _startRotationSpeed;
            _actualMoveSpeed = _moveSpeed;
            _actualAngle = _angle;
        }
        
        private void Update()
        {
            if (_actualRotationSpeed < _maxRotationSpeed)
            {
                _actualRotationSpeed += _rotationAcceleration * Time.deltaTime;
                if (_actualRotationSpeed >= _maxRotationSpeed)
                {
                    _actualRotationSpeed = _maxRotationSpeed;
                }
            }

            if (_actualAngle > -90f)
            {
                _actualAngle -= _angleDropOff * Time.deltaTime;
            }

            if (_actualMoveSpeed > 0f)
            {
                _actualMoveSpeed -= _moveSpeedDropOff * Time.deltaTime;
            }
            
            transform.rotation *= Quaternion.Euler(0, 0, _actualRotationSpeed * Time.deltaTime);

            transform.position += Quaternion.Euler(_sideAngle, 0, -_actualAngle) * new Vector3(-_actualMoveSpeed * Time.deltaTime, 0, 0);
        }

        public void SetSideAngle(float sideAngle)
        {
            _sideAngle = sideAngle;
        }
        
        public void SetAngle(float angle)
        {
            _angle = angle;
            _actualAngle = _angle;
        }
    }
}