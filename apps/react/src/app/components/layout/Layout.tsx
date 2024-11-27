import { useEffect } from "react";
import { Outlet, useLocation } from "react-router-dom"
import { Header, HeaderNavbarMobileContainer } from "../header";
import { AnimatePresence, motion } from "framer-motion";
import { Footer } from "../footer";

const pageVariants = {
    initial: { y: '-10%', opacity: 0 },
    enter: { y: '0%', opacity: 1 }
};

const Layout = () => {
    const location = useLocation();
    
    useEffect(() => {
        const bodyClassNames = 'app-layout header-fixed header-tablet-and-mobile-fixed toolbar-enabled';
        const classNamesToAdd = bodyClassNames.split(' ');

        classNamesToAdd.forEach(className => {
            document.body.classList.add(className);
        });

        return () => {
            classNamesToAdd.forEach(className => {
                document.body.classList.remove(className);
            });
        };
    }, []);

    return (
        <>
            <HeaderNavbarMobileContainer />
            <div className="d-flex flex-column flex-root">
                <div className="page d-flex flex-row flex-column-fluid">
                    <div className="wrapper d-flex flex-column flex-row-fluid" id="kt_wrapper">
                        <Header />
                        <AnimatePresence>
                            <motion.div
                                className="animated-box"
                                key={location.pathname}
                                initial="initial"
                                animate="enter"
                                variants={pageVariants}
                                transition={{ duration: 0.5 }}
                            >
                                <Outlet />
                            </motion.div>
                        </AnimatePresence>
                        <Footer />
                    </div>
                </div>
            </div>
        </>
    )
}

export default Layout