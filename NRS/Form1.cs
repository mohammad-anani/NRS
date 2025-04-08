using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Net.Http;
using System.Text;

namespace NRS
{
    public partial class Main : Form
    {

        public DataTable view = new DataTable();
        public DataTable people = new DataTable();
        public int index = 0;
        public List<int> saved = new List<int>();


        static void GenerateHtmlFile(DataTable dataTable)
        {
            StringBuilder html = new StringBuilder();

            // Start the HTML table
            html.AppendLine("<html>");
            html.AppendLine("<head><title>Applicants</title></head>");
            html.AppendLine("<body>");
            html.AppendLine("<h1>Applicants</h1>");
            html.AppendLine("<table border='1'>");

            // Create table headers from DataTable columns
            html.AppendLine("<thead><tr>");
            foreach (DataColumn column in dataTable.Columns)
            {
                html.AppendLine($"<th>{column.ColumnName}</th>");
            }
            html.AppendLine("</tr></thead>");

            // Create table rows from DataTable data
            html.AppendLine("<tbody>");
            foreach (DataRow row in dataTable.Rows)
            {
                html.AppendLine("<tr>");
                foreach (var item in row.ItemArray)
                {
                    html.AppendLine($"<td>{item}</td>");
                }
                html.AppendLine("</tr>");
            }
            html.AppendLine("</tbody>");

            // Close the table and HTML tags
            html.AppendLine("</table>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");


            File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads/Applicants.html"), html.ToString());
        }
        public string GenerateQuery()
        {
            string query = "select ID,Phone,ResumeAr,SkillsEng from People where ";

            if (textBox3.TextLength != 0)
            {
                foreach (string skill in textBox3.Text.Split(' '))
                {

                    query += $" SkillsEng like '%{skill}%' or";
                }

            }

            if (textBox1.TextLength != 0)
            {
                foreach (string skill in textBox1.Text.Split(" "))
                {
                    query += $" ResumeAr like '%{skill}%' or";
                }


                query = query.Remove(query.Length - 2);
            }
            else
            {
                query = query.Remove(query.Length - 2);
            }

            return query;
        }

        public void GetPeople()
        {
            SqlConnection sqlConnection = new SqlConnection("server=.;database=NRS;trusted_connection=true;TrustServerCertificate=True;");

            string query = GenerateQuery();

            SqlCommand command = new SqlCommand(query, sqlConnection);

            DataTable dtPersons = new DataTable();

            try
            {
                sqlConnection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    dtPersons.Load(reader);
                }
            }
            finally { sqlConnection.Close(); }

            people = dtPersons;
        }




        public Main()
        {
            InitializeComponent();

        }



        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "1234")
            {
                tabControl1.SelectedIndex = 1;
            }
            else
            {
                MessageBox.Show("كلمة المرور غير صحيحة.حاول مرة أخرى", "خطأ", MessageBoxButtons.OK, MessageBoxIcon
                    .Error);
            }
        }




        private void button2_Click(object sender, EventArgs e)
        {

            if (textBox1.TextLength == 0)
            {
                MessageBox.Show("الرجاء ادخال البيانات المطلوبة", "خطأ", MessageBoxButtons.OK, MessageBoxIcon
 .Error);
                return;
            }


            GetPeople();
            if (people.Rows.Count == 0)
            {
                MessageBox.Show("لا يوجد سير ذاتية تتوافق مع متطلباتك", "خطأ", MessageBoxButtons.OK, MessageBoxIcon
    .Error);
                return;
            }

            lblcount.Text = people.Rows.Count.ToString();
            getPerson(0);
            tabControl1.SelectedIndex = 2;
        }


        public void getPerson(int index)
        {
            flowLayoutPanel1.VerticalScroll.Value = 0;
            lblid.Text = (string)people.Rows[index][0];
            lblphone.Text = (string)people.Rows[index][1];
            lblar.Text = (string)people.Rows[index][2];
            lbleng.Text = (string)people.Rows[index][3];


            if (saved.Contains(index))
            {
                button3.Text = "ازالة";
                button3.BackColor = Color.DarkGray;



            }
            else
            {
                button3.Text = "حفظ";
                button3.BackColor = Color.Silver;

            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (index > 0)
            {
                index--;
                getPerson(index);

            }
            else
            {
                saved.Clear();
                tabControl1.SelectedIndex = 1;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (index < people.Rows.Count - 1)
            {
                index++;
                getPerson(index);
            }
            else
            {
                view = people.Clone();


                for (int i = 0; i < people.Rows.Count; i++)
                {
                    if (saved.Contains(i))
                    {
                        view.ImportRow(people.Rows[i]);
                    }
                }

                DataTable filtered = view.DefaultView.ToTable(false, "ID", "Phone");


                dataGridView1.DataSource = filtered;
                tabControl1.SelectedIndex = 3;
            }
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (saved.Contains(index))
            {
                button3.Text = "حفظ";
                button3.BackColor = Color.Silver;
                saved.Remove(index);
            }
            else
            {
                button3.Text = "ازالة";
                button3.BackColor = Color.DarkGray;
                saved.Add(index);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            GenerateHtmlFile(view);
            MessageBox.Show("تم تحميل الملف بنجاح.", "نجاح");
               
          
        }

        private void button6_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 4;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 3;
        }
    }
}
