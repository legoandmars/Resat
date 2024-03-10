﻿using Resat.Biomes;
using Resat.Dialogue;
using Resat.Npcs;

namespace Resat.Models.Events
{
    public struct DialogueStartedEvent
    {
        public DialogueSO Dialogue;
        public NpcSO Npc;

        public DialogueStartedEvent(DialogueSO dialogue, NpcSO npc)
        {
            Dialogue = dialogue;
            Npc = npc;
        }
    }
}