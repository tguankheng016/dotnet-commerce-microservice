import { LazyLoadEvent } from "primeng/api";
import { Paginator } from "primeng/paginator";
import { Table } from "primeng/table";

export class PrimengTableHelper {
    predefinedRecordsCountPerPage = [5, 10, 25, 50, 100, 250, 500];

    defaultRecordsCountPerPage = 10;

    resizableColumns = true;

    reorderableColumns = true;

    totalRecordsCount = 0;

    records: any[];

    isLoading = false;

    showLoadingIndicator(): void {
        setTimeout(() => {
            this.isLoading = true;
        }, 0);
    }

    hideLoadingIndicator(): void {
        setTimeout(() => {
            this.isLoading = false;
        }, 0);
    }

    getSorting(table: Table): string {
        let sorting = '';

        if (table.sortField) {
            sorting = table.sortField;
            if (table.sortOrder === 1) {
                sorting += ' ASC';
            } else if (table.sortOrder === -1) {
                sorting += ' DESC';
            }
        }

        return sorting;
    }

    getMaxResultCount(paginator: Paginator, event: LazyLoadEvent): number | undefined {
        if (paginator.rows) {
            return paginator.rows;
        }

        if (!event) {
            return 0;
        }

        return event.rows;
    }

    getSkipCount(paginator: Paginator, event: LazyLoadEvent): number | undefined {
        if (paginator.first) {
            return paginator.first;
        }

        if (!event) {
            return 0;
        }

        return event.first;
    }

    shouldResetPaging(event: LazyLoadEvent): boolean {
        if (!event) {
            return true;
        }

        return false;
    }

    setInitialRows(rowNumber: number = 10) {
        // For skeleton display
        this.records = Array.from({ length: rowNumber }).map((_, i) => `Item #${i}`);
    }

    getSkeletonCols(colNumber: number): number[] {
        let cols = [];

        for(let i = 0; i < colNumber; i++) {
            cols.push(i);
        }

        return cols;
    }
}