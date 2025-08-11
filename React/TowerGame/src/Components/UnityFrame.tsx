import React from 'react';
import styled from 'styled-components';
import UnityComponent from '../Unity/UnityComponent';

const UnityFrame: React.FC = () => (
    <Wrapper>
        <Frame>
            <UnityComponent />
        </Frame>
    </Wrapper>
);

export default UnityFrame;

const Wrapper = styled.div`
    padding: 5rem;
    display: flex;
    justify-content: center;
    align-items: center;
    margin: auto;
`;

const Frame = styled.div`
    width: 500px;
    height: 100%;
`;
