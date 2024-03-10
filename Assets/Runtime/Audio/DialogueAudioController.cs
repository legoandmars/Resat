using System.Collections.Generic;
using Resat.Dialogue;
using Resat.Models;
using UnityEngine;

namespace Resat.Audio
{
    // TODO: Pool audio
    public class DialogueAudioController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField]
        private AudioPool _dialogueAudioPool = null!;
        
        public void PlayDialogueSound(TextAudioSO textAudio)
        {
            var clip = RandomDialogueClip(textAudio);
            if (clip == null) 
                return;

            _dialogueAudioPool.Play(clip);
        }

        private AudioClip? RandomDialogueClip(TextAudioSO textAudio)
        {
            if (textAudio.AudioClips.Count == 0)
                return null;

            return textAudio.AudioClips[Random.Range(0, textAudio.AudioClips.Count - 1)];
        }
    }
}