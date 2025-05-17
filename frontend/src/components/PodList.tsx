import axios from "axios";

interface Pod {
    name: string;
    ip?: string;
    route?: string;
    message?: string;
    status?: string;
    code?: string;
}

interface PodListProps {
    pods: Pod[];
}

function PodList({ pods }: PodListProps) {
    if (!pods.length) return <p>No pods running yet.</p>;

    const stopPod = (pod: Pod) => {
        const response = axios.post(`api/stop`, {
            code: pod.code
        })
        // @ts-ignore
        return response.message!;
    }

    return (
        <div style={{ marginTop: '2rem' }}>
            <h2>Running Pods</h2>
            <ul>
                {pods.map((pod, idx) => (
                    <li key={idx}>
                        {pod.name} - {pod.ip || 'No IP'} - {pod.route || 'No Route'} - {pod.status || 'Status Unknown'}
                        <button onClick={ () => stopPod(pod)}>Stop</button>
                    </li>
                ))}
            </ul>
        </div>
    );
}

export default PodList;
