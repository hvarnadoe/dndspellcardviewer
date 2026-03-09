import { dotnet } from "./_framework/dotnet.js";

const is_browser = typeof window != "undefined";
if (!is_browser) throw new Error(`Expected to be running in a browser`);

const dotnetRuntime = await dotnet
  .withDiagnosticTracing(false)
  .withApplicationArgumentsFromQuery()
  .create();

const config = dotnetRuntime.getConfig();

await dotnetRuntime.runMain(config.mainAssemblyName, [
  globalThis.location.href,
]);

const splash = document.getElementById("custom-splash");
if (splash) {
  splash.style.opacity = "0";
  setTimeout(() => splash.remove(), 500);
}
