namespace Automaton.Features;

[Tweak]
internal class AutoSnipeQuests : Tweak
{
    public override string Name => "Sniper no sniping";
    public override string Description => "Automatically completes snipe quests.";

    private readonly Memory.SnipeQuestSequence SnipeQuestSequence = new();

    // B1(api13 / TC game v7.20): Memory.Signatures.EnqueueSnipeTask does not resolve on the TC game
    // binary, so ECommons' EzSignatureHelper swallows the scan failure and leaves SnipeHook null --
    // upstream's bare `SnipeHook.Enable()` then NREs on every Enable and spams the log (runtime-observed
    // 2026-07-28). That value is byte-equal to JP HEAD: upstream last scanned it at cdb126c (2025-01-29)
    // and never re-scanned for 7.3, and no rung of the CLAUDE.md §4-7-C ladder has a candidate -- TC_ok
    // has no Automaton, no other plugin in the tree hooks this function, and FFXIVClientStructs 6966
    // does not name it (EventSceneModuleImplBase exposes only the EventSceneModule field). So this
    // guards rather than fixes: the one tweak stays inactive with a single honest warning and the other
    // tweaks are unaffected. Re-check when HoshinoLYK's opcode chain reaches v7.20 or upstream rescans.
    private bool HookAvailable => SnipeQuestSequence.SnipeHook != null;

    public override void Enable()
    {
        if (!HookAvailable)
        {
            Warning("snipe-task signature did not resolve on this game version -- feature inactive.");
            return;
        }
        SnipeQuestSequence.SnipeHook.Enable();
    }

    public override void Disable()
    {
        if (!HookAvailable) return;
        SnipeQuestSequence.SnipeHook.Disable();
    }
}
