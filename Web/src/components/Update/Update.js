import { useEffect, useState } from "react";

export default function Update({ setError, changePageIndex, ID }) {
  const [loading, setLoading] = useState(false);
  const [phone, setPhone] = useState(null);
  const [resumeAr, setResumeAr] = useState(null);
  const [skillsEng, setSkillsEng] = useState(null);
  const [check, setCheck] = useState(null);

  function validate() {
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
      LebaneseID: "",
      Phone: phone,
      ResumeAr: resumeAr,
      SkillsEng: skillsEng,
    };

    try {
      var response = await fetch(
        "https://localhost:7076/api/NRS/UpdatePerson",
        {
          method: "PUT",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(applicant),
          credentials: "same-origin",
        }
      );
    } catch (error) {
      setError(true);
      return;
    }
    if (response.ok) {
      changePageIndex(false);
    }
  }

  useEffect(() => {
    async function fetchData() {
      try {
        var response = await fetch("https://localhost:7076/api/NRS/GetPerson", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ id: ID }),
          credentials: "same-origin",
        });
      } catch (error) {
        setError(true);
      }
      if (response.ok) {
        var data = await response.json();

        console.log(data);
        setPhone(data.phone);
        setResumeAr(data.resumeAr);
        setSkillsEng(data.skillsEng);
        setLoading(false);
      }
    }

    fetchData();
  }, []);

  if (loading)
    return (
      <main>
        <h2>جاري التحميل...</h2>
      </main>
    );

  return (
    <>
      <span onClick={() => changePageIndex(false)}>العودة</span>
      <main>
        <div>
          <h2>ثق بنا!</h2>
          <p className="pp">ان موقعنا يعمل على تشفير معلوماتك الخاصة.</p>
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
