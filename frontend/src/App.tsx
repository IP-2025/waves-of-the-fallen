import { useState } from 'react';
import axios from 'axios';
import PodList from './components/PodList';
import StartButton from './components/StartButton';

interface Pod {
    name: string;
    ip?: string;
    route?: string;
}

function App() {
    const [pods, setPods] = useState<Pod[]>([]);
    const [message, setMessage] = useState<string>('');

    const startNewPod = async () => {
        try {
            const response = await axios.post('/api/start');
            setMessage(response.data.message || 'Pod started!');
        } catch (error) {
            console.error(error);
            setMessage('Failed to start pod.');
        }
    };

    const fetchPods = async () => {
        try {
            const response = await axios.get('/api/pods');
            setPods([])
            setPods(response.data.pods || []);
            console.log(response.data.pods);
        } catch (error) {
            console.error(error);
            setMessage('Failed to fetch pods.');
        }
    };

    return (
        <div className="App" style={{ padding: '2rem' }}>
            <h1>🎮 Pod Spawner</h1>
            <StartButton onClick={startNewPod} />
            <button onClick={fetchPods} style={{ marginTop: '1rem' }}>Refresh Pod List</button>
            <p>{message}</p>
            <PodList pods={pods} />
        </div>
    );
}

export default App;
