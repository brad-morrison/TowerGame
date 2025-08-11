// ReactLayerUI.tsx
import React from "react";
import styled from "styled-components";
import GameOverModal from "../Components/GameOverModal";

interface ReactLayerUIProps {
  open: boolean;
  coins: number;
  onClose: () => void;
}

export default function ReactLayerUI({
  open,
  coins,
  onClose,
}: ReactLayerUIProps) {
  return (
    <Overlay>
      <GameOverModal isOpen={open} coins={coins} onClose={onClose} />
    </Overlay>
  );
}

const Overlay = styled.div`
  position: absolute;
  display: flex;
  justify-content: center;
  inset: 0;
  z-index: 10;
  pointer-events: none;
  top: 0;
  bottom: 0;
  left: 0;
  right: 0;

  & > * {
    pointer-events: auto;
  }
`;
