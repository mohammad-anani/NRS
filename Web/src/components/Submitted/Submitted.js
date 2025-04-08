import "../../styles/Submitted.css";

export default function Submitted({ changePageIndex }) {
  return (
    <main>
      <article>
        <img src="success.png" alt=""></img>
        <figcaption>تم استقبال سيرتك الذاتية بنجاح!</figcaption>
      </article>
      <button onClick={() => changePageIndex(true)}>تعديل</button>
    </main>
  );
}
