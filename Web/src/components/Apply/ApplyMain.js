import { useState } from "react";
import "../../styles/ApplyMain.css";

export default function ApplyMain({ changePageIndex, setError }) {
  const [id, setId] = useState(null);
  const [phone, setPhone] = useState(null);
  const [resumeAr, setResumeAr] = useState("");
  const [skillsEng, setSkillsEng] = useState("");
  const [check, setCheck] = useState(null);

  function validate() {
    if (!id || isNaN(+id) || id.length < 8) return false;
    if (!phone || isNaN(+phone) || phone.length < 8) return false;
    if (!resumeAr || resumeAr.length < 10) return false;
    if (!check) return false;

    return true;
  }

  function Warn() {
    if (!validate()) {
      window.scrollTo(0, 0);
      alert(" الرجاء ادخال البيانات المطلوبة بالشكل المطلوب");
      return false;
    }
    return true;
  }

  async function submit() {
    if (!Warn()) return;

    const applicant = {
      ID: "",
      LebaneseID: id,
      Phone: phone,
      ResumeAr: resumeAr,
      SkillsEng: skillsEng,
    };
    try {
      var response = await fetch("https://localhost:7076/api/NRS/AddPerson", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(applicant),
        credentials: "same-origin",
      });

      if (response.ok) {
        var data = await response.json();

        var ID = data.id;
        localStorage.setItem("ID", ID);
        changePageIndex(false);
      }
    } catch (error) {
      setError(true);
      return;
    }
  }

  return (
    <>
      <span onClick={() => changePageIndex(false)}>العودة</span>
      <main>
        <div>
          <h2>ثق بنا!</h2>
          <p className="pp">ان موقعنا يعمل على تشفير معلوماتك الخاصة.</p>
        </div>

        <div>
          <label>-رقم الهوية:</label>
          <div>
            <input
              value={id}
              onChange={(e) => setId(e.target.value)}
              type="text"
              maxLength={8}
            ></input>
            <span>0000</span>
          </div>
        </div>
        <div>
          <label>-رقم الهاتف:</label>
          <div>
            <input
              value={phone}
              onChange={(e) => setPhone(e.target.value)}
              type="text"
              maxLength={8}
            ></input>
            <span>961+</span>
          </div>
        </div>
        <section>
          <label>-سيرتك الذاتية(مهارات،كفاءات،مؤهلات،خبرات،مشاريع)</label>
          <textarea
            value={resumeAr}
            onChange={(e) => setResumeAr(e.target.value)}
          ></textarea>
        </section>
        <section>
          <label style={{ direction: "ltr" }}>
            -English Keywords(optional)
          </label>
          <textarea
            value={skillsEng}
            onChange={(e) => setSkillsEng(e.target.value)}
            style={{ direction: "ltr" }}
          ></textarea>
        </section>
        <label>
          أتعهد بأن المعلومات المقدمة صحيحة.
          <input type="checkbox" value={check} onChange={setCheck}></input>
        </label>
        <button onClick={submit}>تقديم</button>
        <p>
          ملاحظة:أي ذكر لاسم أو طائفة... سيؤدي الى استبعادك من لوائح التوظيف.
        </p>
      </main>
    </>
  );
}
