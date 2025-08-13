// UnityComponent.tsx
import { useRef, useEffect } from "react";

export default function UnityComponent() {
  const unityRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!unityRef.current) return;

    // Create Unity canvas
    const canvas = document.createElement("canvas");
    canvas.id = "unity-canvas";
    canvas.style.width = "100%";
    canvas.style.height = "100%";
    unityRef.current.appendChild(canvas);

    // Load Unity WebGL build
    const script = document.createElement("script");
    script.src = "/Build/Build.loader.js";
    script.onload = () => {
      (window as any)
        .createUnityInstance(canvas, {
          dataUrl: "/Build/Build.data.br",
          frameworkUrl: "/Build/Build.framework.js.br",
          codeUrl: "/Build/Build.wasm.br",
          streamingAssetsUrl: "/StreamingAssets",
          companyName: "YourCompany",
          productName: "YourGame",
          productVersion: "1.0",
        })
        .then((instance: any) => {
          console.log("Unity Loaded!", instance);
          (window as any).unityInstance = instance;
        })
        .catch((error: any) => {
          console.error("Unity loading failed:", error);
        });
    };

    document.body.appendChild(script);

    
  }, []);

  return (
    <div style={{ position: "relative", width: "100%", height: "100%" }}>
      <div ref={unityRef} style={{ width: "100%", height: "100%", aspectRatio: "9/16"}} />
    </div>
  );
}