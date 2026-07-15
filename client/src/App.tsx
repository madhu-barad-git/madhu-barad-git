import { BrowserRouter, Routes, Route } from 'react-router-dom';
import CreatePollPage from './pages/CreatePollPage';
import PollPage from './pages/PollPage';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<CreatePollPage />} />
        <Route path="/poll/:id" element={<PollPage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
