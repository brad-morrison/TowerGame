import styled from 'styled-components';
import UnityComponent from '../Unity/UnityComponent';

const UnityFrame: React.FC = () => (
    <Frame>
        <Wrap>
            <UnityComponent />
        </Wrap>
    </Frame>
);

export default UnityFrame;

const Frame = styled.div`
    display: flex;
    justify-content: center;
    align-items: center;
    height: 100vh;
    aspect-ratio: 9 / 16;
`;

const Wrap = styled.div`
    padding: 1rem;
`;
