import { useCartStore } from "@shared/carts";
import { CSSProperties } from "react";
import { Link } from "react-router-dom";

const cartItemCountStyle: CSSProperties = {
    right: "-1px",
    top: "-1px",
};

const HeaderCart = () => {
    const { cartItemCount } = useCartStore();

    return (
        <div className="d-flex align-items-center ms-1">
            <Link to="/app/cart" className="btn btn-icon btn-custom btn-color-gray-600 btn-active-color-primary w-35px h-35px w-md-40px h-md-40px position-relative">
                <i className={`fas fa-shopping-cart fs-3 ${cartItemCount > 0 ? ' pt-2' : 'pt-1'}`}></i>
                {
                    cartItemCount > 0 &&
                    <span style={cartItemCountStyle} className="badge badge-primary navbar-badge position-absolute">{cartItemCount}</span>
                }
            </Link>
        </div>
    )
}

export default HeaderCart