using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Resat.Audio;
using Resat.Cameras;
using Resat.Dialogue;
using Resat.Intermediates;
using Resat.Quests;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Resat.Behaviours
{
    public class BottleThrowerBehaviour : NpcTriggerBehaviour
    {
        [SerializeField]
        private PhotoController _photoController = null!;
        
        [SerializeField]
        private DialogueController _dialogueController = null!;
        
        [SerializeField]
        private QuestController _questController = null!;
        
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

        [SerializeField]
        private QuestSO? _quest;

        [SerializeField]
        private DialogueSO? _failedDialogue;

        [SerializeField]
        private DialogueSO? _completedDialogue;

        private bool _complete = false;
        
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
            if (_complete) 
                return;
            
            // force cam
            Debug.Log("FORCING");

            _dialogueController.ToggleDialogue(false);
            await _photoController.ToggleCameraExternal(true, true);
            
            try
            {
                await ThrowingLogic();
            }
            catch (Exception ex)
            {
                // really really really do not want anything to accidentally break the camera
                Debug.Log(ex);
            }
            
            await _photoController.ToggleCameraExternal(false, true);
            _dialogueController.ToggleDialogue(true);
        }

        private async UniTask ThrowingLogic()
        {
            // warmup a few sounds
            for (int i = 0; i < _warmupSoundCount; i++)
            {
                ThrowBottles(0).Forget();
                await UniTask.WaitForSeconds(_firstStageDelay);
            }

            // first, second, third stage
            for (int i = 0; i < 3; i++)
            {
                ThrowBottles(1).Forget();
                await UniTask.WaitForSeconds(_firstStageDelay);
            }

            for (int i = 0; i < 2; i++)
            {
                ThrowBottles(Random.Range(1, 3)).Forget();
                await UniTask.WaitForSeconds(_secondStageDelay);
            }

            for (int i = 0; i < 4; i++)
            {
                ThrowBottles(Random.Range(2, 5)).Forget();
                await UniTask.WaitForSeconds(_thirdStageDelay);
            }

            var seenCount = _spawnedBottles.Where(x => !x.Seen).Count();
            Debug.Log("SEEN");
            Debug.Log(seenCount);

            if (seenCount == 0)
            {
                CurrentDialogue = _completedDialogue;
                _complete = true;
                _questController.ForceCompleteQuestItems(_quest!);
            }
            else
            {
                CurrentDialogue = _failedDialogue;
            }

            foreach (var bottle in _spawnedBottles)
            {
                if (bottle != null)
                {
                    bottle.gameObject.SetActive(false);
                    Destroy(bottle);
                }
            }
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