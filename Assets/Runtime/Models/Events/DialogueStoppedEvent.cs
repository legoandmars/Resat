using Resat.Biomes;
using Resat.Dialogue;
using Resat.Npcs;

namespace Resat.Models.Events
{
    public struct DialogueStoppedEvent
    {
        public DialogueSO Dialogue;
        public NpcSO Npc;

        public DialogueStoppedEvent(DialogueSO dialogue, NpcSO npc)
        {
            Dialogue = dialogue;
            Npc = npc;
        }
    }
}