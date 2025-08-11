// GameOverModal.tsx
import React from "react";
import styled from "styled-components";

interface GameOverModalProps {
  isOpen: boolean;
  coins: number;
  onClose: () => void;
}

export default function GameOverModal({
  isOpen,
  coins,
  onClose,
}: GameOverModalProps) {
  if (!isOpen) return null;

  return (
    <Overlay>
      <Modal>
        <Title>Game Over</Title>
        <Message>You have collected {coins} coins!</Message>
        <Button onClick={onClose}>Restart</Button>
      </Modal>
    </Overlay>
  );
}

const Overlay = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 999;
`;

const Modal = styled.div`
  background: white;
  padding: 2rem;
  border-radius: 10px;
  text-align: center;
  min-width: 300px;
  box-shadow: 0 0 20px rgba(0, 0, 0, 0.3);
`;

const Title = styled.h1`
  margin-bottom: 1rem;
`;

const Message = styled.p`
  margin-bottom: 1.5rem;
`;

const Button = styled.button`
  padding: 0.5rem 1.5rem;
  background: #333;
  color: white;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  &:hover {
    background: #555;
  }
`;
