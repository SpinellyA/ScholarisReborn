using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

// Must be a cached singleton, not constructed inline in markup (`new InteractiveServerRenderMode(...)`
// on every render) - the renderer relies on stable render-mode identity to decide whether to keep
// reusing an existing render boundary (and everything mounted under it, like MudPopoverProvider) or
// tear it down and recreate it. A fresh instance per render destabilizes that boundary.
public static class RenderModes
{
    public static readonly IComponentRenderMode InteractiveServerNoPrerender = new InteractiveServerRenderMode(prerender: false);
}
