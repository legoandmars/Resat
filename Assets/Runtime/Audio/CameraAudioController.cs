using System.Collections.Generic;
using Resat.Models;
using UnityEngine;

namespace Resat.Audio
{
    // TODO: Pool audio
    public class CameraAudioController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField]
        private AudioSource _cameraAudioSource = null!;

        [Header("Clips")]
        [SerializeField]
        private AudioClip? _cameraShutterClip;
        
        [SerializeField]
        private AudioClip? _menuOpenClip;

        [SerializeField]
        private AudioClip? _menuCloseClip;
        
        [SerializeField]
        private AudioClip? _invalidOperationClip;

        public void PlaySoundEffect(SoundEffect soundEffect)
        {
            AudioClip? clip = soundEffect switch
            {
                SoundEffect.CameraShutter => _cameraShutterClip,
                SoundEffect.MenuOpen => _menuOpenClip,
                SoundEffect.MenuClose => _menuCloseClip,
                SoundEffect.InvalidOperation => _invalidOperationClip,
                _ => null
            };

            if (clip == null) 
                return;
            
            _cameraAudioSource.PlayOneShot(clip);
        }
    }
}