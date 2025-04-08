import "../../styles/HomeMain.css";

export default function HomeMain({ changePageIndex }) {
  return (
    <main>
      <p>
        مرحبًا بك في نظام التوظيف المحايد حيث يمكنك التقديم على الوظائف دون
        الكشف عن أي بيانات يمكن أن تؤدي إلى تحيز مسؤول التوظيف.
      </p>
      <p>
        الغاء الطائفية السياسية هدف وطني اساسي يقتضي العمل على تحقيقه وفق خطة
        مرحلية.
        <span>مقدمة الدستور اللبناني - مادة ح</span>
      </p>
      <article>
        <img src="job-hiring-concept-png.webp" alt=""></img>
        <figcaption>الكفاءة معيارنا الوحيد</figcaption>
      </article>
      <section>
        <span>محمد</span>
        <span>مسيحي</span>
        <span>درزي</span>
        <img src="./arrow.jpeg" alt=""></img>
        <span>f6f4a153-9d6c-4e89-9026-ede4f60b7f60</span>
      </section>
      <button onClick={() => changePageIndex(true)}>ابدأ</button>
    </main>
  );
}
