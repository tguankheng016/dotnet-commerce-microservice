import ClipLoader from "react-spinners/ClipLoader";

const LoadingScreen = () => {
    const color = 'rgb(54, 215, 183)';
    
    return (
        <div
            style={{
                position: 'fixed',
                top: 0,
                left: 0,
                width: '100%',
                height: '100%',
                display: 'flex',
                justifyContent: 'center',
                alignItems: 'center',
                zIndex: 1000,
            }}
        >
            <div className="sweet-loading">
                <ClipLoader
                    color={color}
                    loading={true}
                    size={100}
                    aria-label="Loading Spinner"
                    data-testid="loader"
                />
            </div>
        </div>
    )
}

export default LoadingScreen