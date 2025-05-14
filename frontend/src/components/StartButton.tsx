interface StartButtonProps {
    onClick: () => void;
}

function StartButton({ onClick }: StartButtonProps) {
    return (
        <button onClick={onClick} style={{ padding: '1rem', marginTop: '2rem' }}>
            🚀 Start New Pod
        </button>
    );
}

export default StartButton;
