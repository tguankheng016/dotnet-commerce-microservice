import { DataTableSortEvent, SortOrder } from "primereact/datatable";
import { PaginatorPageChangeEvent } from "primereact/paginator";
import { useState } from "react";

interface LazyTableState {
    first: number;
    rows: number;
    page: number;
    sortField?: string;
    sortOrder?: SortOrder;
}

export class PrimengTableHelper {
    static predefinedRecordsCountPerPage = [5, 10, 25, 50, 100, 250, 500];
    static defaultRecordsCountPerPage = 10;
    static defaultPaginatorTemplate = "CurrentPageReport FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink RowsPerPageDropdown";
    static defaultCurrentReportTemplate = "Total: {totalRecords}";

    static getSorting(state: LazyTableState): string {
        let sorting = '';

        if (state.sortField) {
            sorting = state.sortField;
            if (state.sortOrder === 1) {
                sorting += ' ASC';
            } else if (state.sortOrder === -1) {
                sorting += ' DESC';
            }
        }

        return sorting;
    }
}

const useDataTable = () => {
    const [loading, setLoading] = useState(false);
    const [totalRecords, setTotalRecords] = useState(0);
    const [lazyState, setLazyState] = useState<LazyTableState>({
        first: 0,
        rows: PrimengTableHelper.defaultRecordsCountPerPage,
        page: 1,
        sortField: undefined,
        sortOrder: undefined,
    });

    const onSort = (event: DataTableSortEvent) => {
        setLazyState((prevState) => ({
            ...prevState,
            sortField: event.sortField,
            sortOrder: event.sortOrder,
        }));
    };

    const onPageChange = (event: PaginatorPageChangeEvent) => {
        setLazyState((prevState) => ({
            ...prevState,
            first: event.first,
            rows: event.rows,
            page: event.page,
        }));
    }

    return {
        loading,
        setLoading,
        totalRecords,
        setTotalRecords,
        lazyState,
        setLazyState,
        onSort,
        onPageChange,
    }
};

export default useDataTable