using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

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

        public static bool GenerateScript()
        {
            // Query to check if DB exists
            string query = "SELECT 1 FROM sys.databases WHERE name = N'NRS'";

            // Script to create the database
            string createDbScript = "CREATE DATABASE [NRS]\r\n CONTAINMENT = NONE\r\n ON  PRIMARY \r\n( NAME = N'NRS', FILENAME = N'C:\\Program Files\\Microsoft SQL Server\\MSSQL16.MSSQLSERVER\\MSSQL\\DATA\\NRS.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )\r\n LOG ON \r\n( NAME = N'NRS_log', FILENAME = N'C:\\Program Files\\Microsoft SQL Server\\MSSQL16.MSSQLSERVER\\MSSQL\\DATA\\NRS_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )\r\n WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF\r\nGO\r\nALTER DATABASE [NRS] SET COMPATIBILITY_LEVEL = 160\r\nGO\r\nIF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))\r\nbegin\r\nEXEC [NRS].[dbo].[sp_fulltext_database] @action = 'enable'\r\nend\r\nGO\r\nALTER DATABASE [NRS] SET ANSI_NULL_DEFAULT OFF \r\nGO\r\nALTER DATABASE [NRS] SET ANSI_NULLS OFF \r\nGO\r\nALTER DATABASE [NRS] SET ANSI_PADDING OFF \r\nGO\r\nALTER DATABASE [NRS] SET ANSI_WARNINGS OFF \r\nGO\r\nALTER DATABASE [NRS] SET ARITHABORT OFF \r\nGO\r\nALTER DATABASE [NRS] SET AUTO_CLOSE OFF \r\nGO\r\nALTER DATABASE [NRS] SET AUTO_SHRINK OFF \r\nGO\r\nALTER DATABASE [NRS] SET AUTO_UPDATE_STATISTICS ON \r\nGO\r\nALTER DATABASE [NRS] SET CURSOR_CLOSE_ON_COMMIT OFF \r\nGO\r\nALTER DATABASE [NRS] SET CURSOR_DEFAULT  GLOBAL \r\nGO\r\nALTER DATABASE [NRS] SET CONCAT_NULL_YIELDS_NULL OFF \r\nGO\r\nALTER DATABASE [NRS] SET NUMERIC_ROUNDABORT OFF \r\nGO\r\nALTER DATABASE [NRS] SET QUOTED_IDENTIFIER OFF \r\nGO\r\nALTER DATABASE [NRS] SET RECURSIVE_TRIGGERS OFF \r\nGO\r\nALTER DATABASE [NRS] SET  DISABLE_BROKER \r\nGO\r\nALTER DATABASE [NRS] SET AUTO_UPDATE_STATISTICS_ASYNC OFF \r\nGO\r\nALTER DATABASE [NRS] SET DATE_CORRELATION_OPTIMIZATION OFF \r\nGO\r\nALTER DATABASE [NRS] SET TRUSTWORTHY OFF \r\nGO\r\nALTER DATABASE [NRS] SET ALLOW_SNAPSHOT_ISOLATION OFF \r\nGO\r\nALTER DATABASE [NRS] SET PARAMETERIZATION SIMPLE \r\nGO\r\nALTER DATABASE [NRS] SET READ_COMMITTED_SNAPSHOT OFF \r\nGO\r\nALTER DATABASE [NRS] SET HONOR_BROKER_PRIORITY OFF \r\nGO\r\nALTER DATABASE [NRS] SET RECOVERY FULL \r\nGO\r\nALTER DATABASE [NRS] SET  MULTI_USER \r\nGO\r\nALTER DATABASE [NRS] SET PAGE_VERIFY CHECKSUM  \r\nGO\r\nALTER DATABASE [NRS] SET DB_CHAINING OFF \r\nGO\r\nALTER DATABASE [NRS] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) \r\nGO\r\nALTER DATABASE [NRS] SET TARGET_RECOVERY_TIME = 60 SECONDS \r\nGO\r\nALTER DATABASE [NRS] SET DELAYED_DURABILITY = DISABLED \r\nGO\r\nALTER DATABASE [NRS] SET ACCELERATED_DATABASE_RECOVERY = OFF  \r\nGO\r\nEXEC sys.sp_db_vardecimal_storage_format N'NRS', N'ON'\r\nGO\r\nALTER DATABASE [NRS] SET QUERY_STORE = ON\r\nGO\r\nALTER DATABASE [NRS] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)\r\nGO";
            // Script to run after creation inside the new database
            string postCreationScript = "\r\n/****** Object:  Table [dbo].[People]    Script Date: 6/17/2025 2:50:37 PM ******/\r\nSET ANSI_NULLS ON\r\nGO\r\nSET QUOTED_IDENTIFIER ON\r\nGO\r\nCREATE TABLE [dbo].[People](\r\n\t[ID] [nvarchar](36) NOT NULL,\r\n\t[LebaneseID] [nvarchar](255) NOT NULL,\r\n\t[Phone] [nvarchar](255) NOT NULL,\r\n\t[ResumeAr] [nvarchar](max) NOT NULL,\r\n\t[SkillsEng] [nvarchar](max) NULL,\r\n CONSTRAINT [PK_People] PRIMARY KEY CLUSTERED \r\n(\r\n\t[ID] ASC\r\n)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]\r\n) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]\r\nGO\r\nINSERT [dbo].[People] ([ID], [LebaneseID], [Phone], [ResumeAr], [SkillsEng]) VALUES (N'7438b399-b117-4824-92e1-73b2bd7bdf5a', N'KFf8ZPkxUh6XF/RbuMotcA==', N'aOYZMPWeTbgKSyQ8xmeojQ==', N'مهندس ويب و خبير ذكاء اصطناعي', N'python')\r\nGO\r\nINSERT [dbo].[People] ([ID], [LebaneseID], [Phone], [ResumeAr], [SkillsEng]) VALUES (N'9f336128-1ca0-427c-b11f-92a71d22fd1e', N'gNORhIW0Hd4MQxGG7vgzpQ==', N'9SX6PmN6MgiYCQJoNRjrBg==', N'مبرمج ويب و مهندس برمجيات', N'developer programmer')\r\nGO\r\nUSE [master]\r\nGO\r\nALTER DATABASE [NRS] SET  READ_WRITE \r\nGO\r\n";
            using (SqlConnection masterConnection = new SqlConnection("server=.;database=master;trusted_connection=true;TrustServerCertificate=True;"))
            {
                masterConnection.Open();

                object result = new SqlCommand(query, masterConnection).ExecuteScalar();

                if (result == null) // DB does NOT exist
                {
                    // Create the database
                    string[] batches = Regex.Split(createDbScript, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    foreach (string batch in batches)
                    {
                        if (!string.IsNullOrWhiteSpace(batch))
                        {
                            using (SqlCommand cmd = new SqlCommand(batch, masterConnection))
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    // Run post creation script in new DB
                    using (SqlConnection DVLDConnection = new SqlConnection("server=.;database=NRS;trusted_connection=true;TrustServerCertificate=True;"))
                    {
                        DVLDConnection.Open();
                        string[] batches2 = Regex.Split(postCreationScript, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                        foreach (string batch in batches2)
                        {
                            if (!string.IsNullOrWhiteSpace(batch))
                            {
                                using (SqlCommand cmd = new SqlCommand(batch, DVLDConnection))
                                {
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }

                    return true; // DB created and scripts run
                }
                else
                {
                    return false; // DB exists, nothing done
                }
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            GenerateScript();
        }
    }
}
