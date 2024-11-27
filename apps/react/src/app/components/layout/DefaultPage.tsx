import { Fragment, ReactNode } from "react";
import { Link } from "react-router-dom";

export class BreadcrumbItem {
    text: string;
    routerLink?: string;

    constructor(text: string, routerLink?: string) {
        this.text = text;
        this.routerLink = routerLink;
    }

    isLink(): boolean {
        return !!this.routerLink;
    }
}

interface Props {
    title: string;
    breadcrumbs: BreadcrumbItem[];
    children: ReactNode;
}

const DefaultPage = ({ title, breadcrumbs, children }: Props) => {
    return (
        <>
            <div className="toolbar py-5 py-lg-15" id="kt_toolbar">
                <div id="kt_toolbar_container" className="container-xl d-flex flex-stack flex-wrap">
                    <div className="page-title d-flex flex-column">
                        <h1 className="d-flex text-white fw-bold fs-2qx my-1 me-5">
                            {title}
                        </h1>
                        <ul className="breadcrumb breadcrumb-separatorless fw-semibold fs-7 my-1">
                            <li className="breadcrumb-item text-white opacity-75">
                                <Link to="/" className="text-white text-hover-primary">
                                    <i className="ki-duotone ki-home fs-3 text-gray-400 me-n1"></i>
                                </Link>
                            </li>
                            { 
                                breadcrumbs && breadcrumbs.length > 0 && 
                                <li className="breadcrumb-item">
                                    <i className="ki-duotone ki-right fs-4 text-gray-700 mx-n1"></i>
                                </li> 
                            }
                            {
                                breadcrumbs.map((item, index) => (
                                    <Fragment key={'breadcrum-item' + index}>
                                        <li className="breadcrumb-item text-gray-700 fw-bold lh-1">
                                            {
                                                item.isLink() &&
                                                <a
                                                    className="text-muted cursor-pointer"
                                                >
                                                    {item.text}
                                                </a>
                                            }
                                            {
                                                item.isLink() ||
                                                <span>{item.text}</span>
                                            }
                                        </li>
                                        {
                                            index !== breadcrumbs.length - 1 &&
                                            <li className="breadcrumb-item">
                                                <i className="ki-duotone ki-right fs-4 text-gray-700 mx-n1"></i>
                                            </li>
                                        }
                                    </Fragment>
                                ))
                            }
                        </ul>
                    </div>
                </div>
            </div>
            <div id="kt_content_container" className="d-flex flex-column-fluid align-items-start container-xl">
                <div className="content flex-row-fluid" id="kt_content">
                    {children}
                </div>
            </div>
        </>
    )
}

export default DefaultPage