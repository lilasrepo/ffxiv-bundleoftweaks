namespace Automaton.Features;

[Tweak(debug: true)]
public unsafe class InstantReturn : Tweak
{
    public override string Name => "Quick Return";
    public override string Description => "Calls the return function directly";

    private readonly Memory.AgentReturn Return = new();

    // B1(api13 / TC game v7.20): same shape as AutoSnipeQuests -- Memory.Signatures.AgentReturnReceiveEvent
    // does not resolve on the TC binary, EzSignatureHelper swallows the scan failure and leaves ReturnHook
    // null, and the bare Enable() then NREs on every Enable. No candidate on the CLAUDE.md §4-7-C ladder:
    // the value is byte-equal to JP HEAD, TC_ok has no Automaton, and the function is a helper called from
    // AgentReturn::ReceiveEvent that FFXIVClientStructs 6966 does not name (its AgentReturn exposes only
    // Addresses.Return, whose Void(AgentReturn*) shape does not match byte(AgentInterface*)). This is a
    // debug-only tweak, so guarding costs nothing. Re-check when upstream rescans.
    private bool HookAvailable => Return.ReturnHook != null;

    public override void Enable()
    {
        if (!HookAvailable)
        {
            Warning("return-handler signature did not resolve on this game version -- feature inactive.");
            return;
        }
        Return.ReturnHook.Enable();
    }

    public override void Disable()
    {
        if (!HookAvailable) return;
        Return.ReturnHook.Disable();
    }
}
