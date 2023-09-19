import React from "react";
import { MainPage } from "./components/MainPage";
import { Header } from "./components/Header";
import { Footer } from "./components/Footer";
import { UnityCanvas } from "./components/UnityCanvas";

function App() {
    return (
        <>
            {/* <UnityCanvas /> */}
            <Header />
            <MainPage />
            <Footer />
        </>
    );
}

export default App;
