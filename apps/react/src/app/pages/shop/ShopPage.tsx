import { BreadcrumbItem, DefaultPage } from "@app/components/layout"
import { APIClient } from "@shared/service-proxies";
import { useEffect, useState } from "react";
import { Skeleton } from "primereact/skeleton";
import { ProductDto } from "@shared/service-proxies/product-service-proxies";
import { useCartStore } from "@shared/carts";
import { AddCartDto } from "@shared/service-proxies/cart-service-proxies";
import { SwalNotifyService } from "@shared/sweetalert2";
import { useSessionStore } from "@shared/session";
import { useNavigate } from "react-router-dom";

interface CatalogCardViewProps {
    loading: boolean;
    products: ProductDto[]
}

interface CatalogCardProps {
    loading: boolean;
    product: ProductDto
}

const CatalogCard = ({ loading, product }: CatalogCardProps) => {
    const { setRefreshingCart } = useCartStore();
    const { user } = useSessionStore();
    const navigate = useNavigate();

    if (loading) {
        return (
            <div className="card">
                <div className="card-body">
                    <div className="d-flex flex-row justify-content-between">
                        <div>
                            <Skeleton width="2.5rem" height="1.5rem"></Skeleton>
                        </div>
                        <div>
                            <Skeleton width="2.5rem" height="1.5rem"></Skeleton>
                        </div>
                    </div>
                    <div className="d-flex flex-column justify-content-center align-items-center pt-3">
                        <Skeleton height="7rem"></Skeleton>
                    </div>
                    <div className="d-flex flex-row justify-content-between align-items-center pt-3">
                        <div>
                            <Skeleton width="3.5rem" height="2.5rem"></Skeleton>
                        </div>
                        <div>
                            <Skeleton shape="circle" size="3rem"></Skeleton>
                        </div>
                    </div>
                </div>
            </div>
        )
    }

    const getStockStatus = (product: ProductDto) => {
        if (product.stockQuantity > 0) {
            return <span className="badge badge-light-success fw-bold fs-8 px-2 py-1 ms-2">In-Stock</span>
        }

        return <span className="badge badge-light-danger fw-bold fs-8 px-2 py-1 ms-2">Out-Of-Stock</span>
    }

    const getPrice = (product: ProductDto) => {
        return product.price.toLocaleString('en-US', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2,
        });
    }

    const addCart = (product: ProductDto) => {
        if (!user || user.id == 0) {
            navigate('/account/login');
            return;
        }

        const cartService = APIClient.getCartService();
        const addCartDto = new AddCartDto();
        addCartDto.productId = product.id;

        cartService.addCart(addCartDto)
            .then(() => {
                SwalNotifyService.info(`${product.name} added to cart successfully`);
                setRefreshingCart();
            });
    }

    const images: string[] = [];

    for (let i = 1; i <= 6; i++) {
        images.push('/src/assets/images/fake_product_0' + i + '.png')
    }

    const getImage = () => {
        return images[Math.floor(Math.random() * images.length)];
    }

    return (
        <div className="card">
            <div className="card-body">
                <div className="d-flex flex-row justify-content-between">
                    <div>
                        <span><i className="fas fa-tag pe-2"></i></span>
                        <span className="text-dark">{product.categoryName}</span>
                    </div>
                    <div>
                        {getStockStatus(product)}
                    </div>
                </div>
                <div className="d-flex flex-column justify-content-center align-items-center pt-3">
                    <img className="w-75 rounded-3" src={getImage()} alt={product.name} />
                </div>
                <div className="d-flex flex-column justify-content-center align-items-center pt-3">
                    <span className="fw-bold">
                        {product.name}
                    </span>
                </div>
                <div className="d-flex flex-row justify-content-between align-items-center pt-3">
                    <div>
                        <span className="text-dark fs-6">RM {getPrice(product)}</span>
                    </div>
                    <div>
                        <button className="btn btn-primary btn-icon btn-circle" onClick={(e) => addCart(product)} disabled={product.stockQuantity <= 0}>
                            <i className="fas fa-shopping-cart"></i>
                        </button>
                    </div>
                </div>
            </div>
        </div>
    )
}

const CatalogCardView = ({ loading, products }: CatalogCardViewProps) => {
    if (loading) {
        for (let i = 0; i < 8; i++) {
            products.push(new ProductDto());
        }
    }

    return (
        <div className="row">
            {
                products.map((product) =>
                    <div key={product.id} className="col-12 col-md-3 mt-3">
                        <CatalogCard loading={loading} product={product} />
                    </div>
                )
            }
        </div>
    )
}

const ShopPage = () => {
    const breadcrumbs: BreadcrumbItem[] = [
        new BreadcrumbItem('Shop')
    ];

    const [loading, setLoading] = useState(false);
    const [products, setProducts] = useState<ProductDto[]>([]);

    useEffect(() => {
        const abortController = new AbortController();
        const signal = abortController.signal;

        const productService = APIClient.getProductService();

        const loadingTimer = setTimeout(() => {
            setLoading(true);
        }, 200);

        productService.getProducts(
            undefined,
            0,
            0,
            undefined,
            undefined,
            signal
        ).then((res) => {
            setProducts(res.items ?? []);
        }).finally(() => {
            clearTimeout(loadingTimer);
            setLoading(false);
        });

        return () => {
            abortController.abort();
        };
    }, []);

    return (
        <DefaultPage title="Shop" breadcrumbs={breadcrumbs}>
            <div className="card">
                <div className="card-body p-lg-17">
                    <CatalogCardView loading={loading} products={products} />
                </div>
            </div>
        </DefaultPage>
    )
}

export default ShopPage