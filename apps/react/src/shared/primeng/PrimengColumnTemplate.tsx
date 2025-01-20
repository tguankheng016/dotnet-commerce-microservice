import moment from 'moment';
import { Skeleton } from 'primereact/skeleton';

export const TextBodyTemplate = (content: string | undefined, header: string, loading: boolean): JSX.Element => {
    if (loading) {
        return <Skeleton></Skeleton>;
    }

    return (
        <>
            <span className="p-column-title">{header}</span>
            {content}
        </>
    );
}

export const DateTimeBodyTemplate = (content: moment.Moment | undefined, header: string, loading: boolean, format: string = "YYYY-MM-DD HH:mm:ss"): JSX.Element => {
    if (loading) {
        return <Skeleton></Skeleton>;
    }

    const formattedDate = content ? moment(content).format(format) : '';

    return (
        <>
            <span className="p-column-title">{header}</span>
            {formattedDate}
        </>
    );
}
