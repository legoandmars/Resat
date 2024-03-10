using System.Collections.Generic;
using Resat.Models;
using Resat.Quests;
using UnityEngine;

namespace Resat.Dialogue
{
    [CreateAssetMenu(fileName = "TextAudio", menuName = "ScriptableObjects/TextAudio", order = 1)]
    public class TextAudioSO : ScriptableObject
    {
        public List<AudioClip> AudioClips = new();
        // meant to have pitch but i don't wanna rewrite audiopool rn
    }
}