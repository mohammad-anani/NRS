import Header from "./General/Header";
import HomeMain from "./HomePage/HomeMain";
import "../styles/App.css";
import Footer from "./General/Footer";
import { useState } from "react";
import ApplyMain from "./Apply/ApplyMain";
import Submitted from "./Submitted/Submitted";
import Update from "./Update/Update";

function App() {
  const [pageIndex, setPageIndex] = useState(0);
  const [error, setError] = useState(false);
  const ID = localStorage.getItem("ID") || null;

  function changePageIndex(increment) {
    setPageIndex(increment);
    window.scrollTo(0, 0);
  }

  function goToHomepage() {
    setPageIndex(false);
    setError(false);
  }

  if (error)
    return (
      <>
        <Header />
        <main>
          <h1
            style={{ textAlign: "center", fontSize: "40px", direction: "rtl" }}
          >
            حصل خطأ في النظام.حاول لاحقا...
          </h1>
          <button
            style={{ alignSelf: "center", scale: 1 }}
            onClick={goToHomepage}
          >
            العودة
          </button>
        </main>
        <Footer />
      </>
    );

  if (ID && pageIndex)
    return (
      <>
        <Header />
        <Update setError={setError} changePageIndex={changePageIndex} ID={ID} />
        <Footer />
      </>
    );

  if (ID) {
    return (
      <>
        <Header />
        <Submitted changePageIndex={() => changePageIndex(true)} />
        <Footer />
      </>
    );
  }

  return (
    <>
      <Header />
      {!pageIndex && <HomeMain changePageIndex={changePageIndex} />}
      {pageIndex && (
        <ApplyMain changePageIndex={changePageIndex} setError={setError} />
      )}
      <Footer />
    </>
  );
}

export default App;
