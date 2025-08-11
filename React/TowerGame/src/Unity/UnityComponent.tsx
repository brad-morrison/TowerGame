// UnityComponent.tsx
import React, { useRef, useEffect, useState } from "react";
import ReactLayerUI from "./ReactLayerUI";
import { styled } from "styled-components";

export default function UnityComponent() {
  const unityRef = useRef<HTMLDivElement>(null);
  const [coins, setCoins] = useState(0);
  const [gameOver, setGameOver] = useState(false);

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

    // Unity → React bridge functions
    (window as any).IncrementReactCounter = () => {
      setCoins((prev) => prev + 1);
    };

    (window as any).GameOver_Show = () => {
      setGameOver(true);
    };

    (window as any).GameOver_Hide = () => {
      setGameOver(false);
    };

    (window as any).GameWon_Show = () => {
      setGameOver(true);
    };

    (window as any).GameWon_Hide = () => {
      setGameOver(false);
    };

    
  }, []);

  return (
    <div style={{ position: "relative", width: "100%", height: "100%" }}>
      <div ref={unityRef} style={{ width: "100%", height: "100%", aspectRatio: "9/16"}} />
    </div>
  );
}