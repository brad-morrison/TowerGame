import UnityFrame from "./Components/UnityFrame";
import styled from "styled-components";

function App() {

  return (
    <A>
    <Wrapper>
     <UnityFrame />
    </Wrapper></A>
  )
}

export default App

const Wrapper = styled.div`
  display: flex;
  flex-grow: 1;
  justify-content: center;
  align-items: center;
  height: 100vh;
  width: 100%;
  background-color: #f0f0f0;
  margin: auto;
`;

const A = styled.div`
  display: flex;
  flex-grow: 1;
  width: 100%;
  min-width: 100%;
`;