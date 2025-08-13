import styled from 'styled-components';
import UnityComponent from '../Unity/UnityComponent';

const UnityFrame: React.FC = () => (
    <Frame>
        <UnityComponent />
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
