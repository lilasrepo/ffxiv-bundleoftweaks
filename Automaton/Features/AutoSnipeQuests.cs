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
    // 2026-07-28). RESOLVED 2026-07-29: upstream rescanned the signature at f569256 (2025-08-10), three
    // days after the game-7.3 wave, and Memory.cs now carries that value via 9f3c68b. The earlier claim
    // that upstream never rescanned it was wrong for an avoidable reason -- JP/ffxiv-bundleoftweaks is
    // checked out AT the walk-back pin, so "JP HEAD" was the pin, not upstream's origin/Master.
    // The guard stays: it costs nothing and turns a future signature drift into one warning instead of an
    // NRE on every Enable.
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
