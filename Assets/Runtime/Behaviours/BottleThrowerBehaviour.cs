﻿using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Resat.Audio;
using Resat.Cameras;
using Resat.Intermediates;
using UnityEngine;

namespace Resat.Behaviours
{
    public class BottleThrowerBehaviour : NpcTriggerBehaviour
    {
        [SerializeField]
        private PhotoController _photoController = null!;
        
        [SerializeField]
        private Camera _actualPhotoCameraWithSmallViewport = null!;

        [SerializeField]
        private CameraIntermediate _cameraIntermediate = null!;
        
        [SerializeField]
        private ThrownBottle? _bottleTemplate;

        [SerializeField]
        private AudioPool _thrownBottleAudioPool = null!;

        [SerializeField]
        private AudioClip? _throwBottleAudioClip;
        
        [SerializeField]
        private Vector2 _sideAngleRange = Vector2.zero;

        [SerializeField]
        private Vector2 _angleRange = Vector2.zero;

        [SerializeField]
        private float _soundDelay;
        
        [SerializeField]
        private int _warmupSoundCount;

        [SerializeField]
        private float _firstStageDelay;

        [SerializeField]
        private float _secondStageDelay;

        [SerializeField]
        private float _thirdStageDelay;

        private List<ThrownBottle> _spawnedBottles = new();
        
        public override void OnDialogueAnimationEnded()
        {
            // force cam
            Debug.Log("FORCING");

            StartThrowing().Forget();
        }

        // TODO: Power up / power off sound
        private void Start()
        {
            Debug.Log("Starting...");
        }

        private void OnEnable()
        {
            _cameraIntermediate.PhotoTaken += OnPhotoTaken;
        }
        
        private void OnDisable()
        {
            _cameraIntermediate.PhotoTaken += OnPhotoTaken;
        }

        private void OnPhotoTaken()
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(_actualPhotoCameraWithSmallViewport);
            
            // check bottles
            foreach (var bottle in _spawnedBottles.Where(x => !x.Seen))
            {
                if (bottle.Renderer != null && bottle.Renderer.isVisible && bottle.Collider != null &&
                    GeometryUtility.TestPlanesAABB(planes, bottle.Collider.bounds))
                {
                    bottle.Seen = true;
                }
            }
        }

        private async UniTask StartThrowing()
        {
            // force cam
            Debug.Log("FORCING");

            _photoController.EnableCamera(true, true);
           
            // warmup a few sounds
            for (int i = 0; i < _warmupSoundCount; i++)
            {
                await UniTask.WaitForSeconds(_firstStageDelay);
                ThrowBottles(0).Forget();
            }
            
            // first, second, third stage
            for (int i = 0; i < 3; i++)
            {
                await UniTask.WaitForSeconds(_firstStageDelay);
                ThrowBottles(1).Forget();
            }
            for (int i = 0; i < 2; i++)
            {
                await UniTask.WaitForSeconds(_secondStageDelay);
                ThrowBottles(Random.Range(1, 3)).Forget();
            }
            for (int i = 0; i < 2; i++)
            {
                await UniTask.WaitForSeconds(_thirdStageDelay);
                ThrowBottles(Random.Range(2,5)).Forget();
            }

            var seenCount = _spawnedBottles.Where(x => !x.Seen).Count();
            Debug.Log("SEEN");            
            Debug.Log(seenCount);
        }
        
        private async UniTask ThrowBottles(int count)
        {
            if (_throwBottleAudioClip != null)
            {
                _thrownBottleAudioPool.Play(_throwBottleAudioClip);
            }

            await UniTask.WaitForSeconds(_soundDelay);

            for (int i = 0; i < count; i++)
            {
                ThrowBottle();
            }
        }
        
        private void ThrowBottle()
        {
            var bottle = Instantiate(_bottleTemplate);
            bottle!.gameObject.SetActive(true);
            bottle.SetSideAngle(Random.Range(_sideAngleRange.x, _sideAngleRange.y));
            bottle.SetAngle(Random.Range(_angleRange.x, _angleRange.y));
            
            _spawnedBottles.Add(bottle);
        }
    }
}