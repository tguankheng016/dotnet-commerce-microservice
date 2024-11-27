import { ReactNode } from "react";

interface Props {
    id?: string;
    className?: string;
    type?: "submit" | "button";
    children?: ReactNode;
    busyIf?: boolean;
    onClick?: () => void;
}

const BusyButton = ({ id, className, type = 'submit', busyIf, children, onClick }: Props) => {
    return (
        <button 
            id={id}
            type={type}
            className={className}
            disabled={busyIf}
            onClick={onClick}
        >
            {busyIf ? <i className="fa fa-spin fa-spinner ps-2"></i> : null}
            <span className="indicator-label">
                {children}
            </span>
            {busyIf ? ' ...' : ''}
        </button>
    )
}

export default BusyButton