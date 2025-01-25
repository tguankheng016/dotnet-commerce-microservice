import { BreadcrumbItem, DefaultPage } from "@app/components/layout"
import { useCartStore } from "@shared/carts";
import { PrimengTableHelper, TextBodyTemplate, useDataTable } from "@shared/primeng";
import { APIClient, ApiException } from "@shared/service-proxies";
import { CartDto, UpdateCartDto } from "@shared/service-proxies/cart-service-proxies";
import { SwalMessageService, SwalNotifyService } from "@shared/sweetalert2";
import { useLayoutStore } from "@shared/theme";
import { Column } from "primereact/column";
import { DataTable } from "primereact/datatable";
import { InputNumber } from "primereact/inputnumber";
import { Paginator } from "primereact/paginator";
import { Skeleton } from "primereact/skeleton";
import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { useDebounceCallback } from "usehooks-ts";

interface CartTableProps {
    filterText: string | undefined;
}

const CartTable = ({ filterText }: CartTableProps) => {
    const { isMobileView } = useLayoutStore();
    const { setRefreshingCart } = useCartStore();
    const [reloading, setReloading] = useState(false);
    const {
        loading,
        setLoading,
        totalRecords,
        setTotalRecords,
        lazyState,
        onSort,
        onPageChange,
    } = useDataTable();
    const [carts, setCarts] = useState<CartDto[]>([]);

    useEffect(() => {
        const abortController = new AbortController();
        const signal = abortController.signal;

        loadLazyData(signal);

        return () => {
            abortController.abort();
        };
    }, [lazyState, filterText, reloading]);

    const loadLazyData = (signal: AbortSignal) => {
        const cartService = APIClient.getCartService();

        const loadingTimer = setTimeout(() => {
            setLoading(true);
        }, 200);

        cartService.getCarts(
            lazyState.first,
            lazyState.rows,
            filterText,
            PrimengTableHelper.getSorting(lazyState),
            signal
        ).then((res) => {
            setCarts(res.items ?? []);
            setTotalRecords(res.totalCount ?? 0);
        }).finally(() => {
            clearTimeout(loadingTimer);
            setLoading(false);
        });
    };

    const images: string[] = [];

    for (let i = 1; i <= 6; i++) {
        images.push('/assets/images/fake_product_0' + i + '.png')
    }

    const getImage = () => {
        return images[Math.floor(Math.random() * images.length)];
    }

    const getPrice = (price: number) => {
        return price.toLocaleString('en-US', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2,
        });
    }

    const getTotalPrice = (cart: CartDto) => {
        const totalPrice = cart.productPrice * cart.quantity;

        return totalPrice.toLocaleString('en-US', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2,
        });
    }

    const productBodyTemplate = (rowData: CartDto) => {
        if (loading) {
            return <Skeleton></Skeleton>;
        }

        const imageUrl = getImage();

        return (
            <>
                <div className="w-md-100 d-flex align-items-center">
                    {
                        !isMobileView &&
                        <>
                            <Link to={imageUrl} target='_blank'>
                                <img className="w-75px rounded-3" src={imageUrl} />
                            </Link>
                            <div className="ps-3">
                                <div>
                                    {rowData.productName}
                                </div>
                                <div>
                                    {rowData.productDescription}
                                </div>
                            </div>
                        </>
                    }
                    {
                        isMobileView &&
                        <>
                            <Link to={imageUrl} target='_blank'>
                                <img className="img-circle rounded-circle" src={imageUrl} />
                            </Link>
                            <div>
                                <div>
                                    {rowData.productName}
                                </div>
                            </div>
                        </>
                    }
                </div >
            </>
        );
    };

    const setDebouncedQuantityChanged = useDebounceCallback((rowData: CartDto, value: number) => handleQuantityChanged(rowData, value), 500);

    const quantityBodyTemplate = (rowData: CartDto) => {
        if (loading) {
            return <Skeleton></Skeleton>;
        }

        return (
            <div className="d-flex align-items-end justify-content-end w-md-100 w-100px">
                <InputNumber
                    value={rowData.quantity}
                    onValueChange={(e) => setDebouncedQuantityChanged(rowData, e.value!)}
                    showButtons
                    min={1}
                    max={rowData.isOutOfStock ? rowData.quantity : undefined}
                />
            </div>
        )
    }

    const actionBodyTemplate = (rowData: CartDto) => {
        if (loading) {
            return <Skeleton></Skeleton>;
        }

        return (
            <div>
                <button className="btn btn-sm btn-danger" onClick={(e) => handleDelete(rowData)}>
                    <i className="fas fa-trash pe-0"></i>
                </button>
            </div>
        )
    }

    const handleQuantityChanged = (rowData: CartDto, newQuantity: number) => {
        if (rowData.quantity == newQuantity) {
            if ((rowData.isOutOfStock && rowData.quantity <= newQuantity)) {
                SwalNotifyService.error('The selected quantity exceeds the remaining stock');
                return;
            }

            return;
        }

        const cartService = APIClient.getCartService();
        const updateCartDto = new UpdateCartDto();
        updateCartDto.id = rowData.id;
        updateCartDto.quantity = newQuantity;

        cartService.updateCart(updateCartDto)
            .then((res) => {
                setReloading(!reloading);
                setRefreshingCart();
            }).catch((err) => {
                const axiosError = err as ApiException;
                if (axiosError && axiosError.detail == "The selected quantity exceeds the remaining stock") {
                    setReloading(!reloading);
                }
            });
    }

    const handleDelete = (rowData: CartDto) => {

        SwalMessageService.showConfirmation("Are you sure?", `${rowData.productName} will be deleted`, () => {
            const cartService = APIClient.getCartService();
            cartService.deleteCart(rowData.id).then(() => {
                SwalNotifyService.success('Deleted successfully');
                setReloading(!reloading);
                setRefreshingCart();
            });
        });
    }

    return (
        <div className="row align-items-center mx-0">
            <div className="primeng-datatable-container">
                <DataTable
                    value={carts}
                    lazy
                    rows={lazyState.rows}
                    onSort={onSort}
                    sortField={lazyState.sortField}
                    sortOrder={lazyState.sortOrder}
                    responsiveLayout={isMobileView ? "stack" : "scroll"}
                >
                    <Column field="product.name" header="Product" sortable body={(data: CartDto) => productBodyTemplate(data)} />
                    <Column field="product.price" header="Unit Price" body={(data: CartDto) => TextBodyTemplate(getPrice(data.productPrice), "", loading)} />
                    <Column header="Quantity" body={(data: CartDto) => quantityBodyTemplate(data)} className="w-md-150px" />
                    <Column header="Total" body={(data: CartDto) => TextBodyTemplate(getTotalPrice(data), "", loading)} />
                    <Column body={(data: CartDto) => actionBodyTemplate(data)} />
                </DataTable>
                {
                    totalRecords == 0 && !loading &&
                    <div className="primeng-no-data">
                        No record found
                    </div>
                }
                <Paginator
                    template={PrimengTableHelper.defaultPaginatorTemplate}
                    first={lazyState.first}
                    rows={lazyState.rows}
                    totalRecords={totalRecords}
                    rowsPerPageOptions={PrimengTableHelper.predefinedRecordsCountPerPage}
                    onPageChange={onPageChange}
                    currentPageReportTemplate={PrimengTableHelper.defaultCurrentReportTemplate}
                />
            </div>
        </div>
    )
}

const CartPage = () => {
    const breadcrumbs: BreadcrumbItem[] = [
        new BreadcrumbItem('My Carts')
    ];

    const [filterText, setFilterText] = useState('');

    return (
        <DefaultPage title="My Carts" breadcrumbs={breadcrumbs}>
            <div className="card">
                <div className="card-body p-lg-17">
                    <CartTable filterText={filterText} />
                </div>
            </div>
        </DefaultPage>
    )
}

export default CartPage