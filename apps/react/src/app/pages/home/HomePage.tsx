import { BreadcrumbItem, DefaultPage } from "@app/components/layout"

const HomePage = () => {
    const breadcrumbs: BreadcrumbItem[] = [
        new BreadcrumbItem('About')
    ];

    return (
        <DefaultPage title="About" breadcrumbs={breadcrumbs}>
            <div className="card">
                <div className="card-body p-lg-17">
                    <p>
                        This is a simple startup template based on ASP.NET Boilerplate
                        framework and Module Zero. If you need an enterprise startup
                        project, check
                        <a href="http://aspnetzero.com?ref=abptmpl" target="_blank">
                            ASP.NET ZERO
                        </a>.
                    </p>
                    <h3>What is ASP.NET Boilerplate?</h3>
                </div>
            </div>
        </DefaultPage>
    )
}

export default HomePage