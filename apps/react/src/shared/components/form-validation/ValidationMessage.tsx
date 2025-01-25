interface Props {
    errorMessage?: string
}

const ValidationMessage = ({ errorMessage }: Props) => {
    if (!errorMessage) return null;

    return (
        <div className="has-danger">
            <div className="form-control-feedback">
                {errorMessage}
            </div>
        </div>
    );
}

export default ValidationMessage