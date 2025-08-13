import UnityFrame from "./Components/UnityFrame";
import styled from "styled-components";

function App() {

  return (
    <Wrapper>
     <UnityFrame />
    </Wrapper>
  )
}

export default App

const Wrapper = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100%;
  width: 100%;
  background-color: #f0f0f0;
  margin: auto;
`;