using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlTypes;
using System.Numerics;
using System.Reflection;

namespace NRS_Web.Controllers
{

    public class Person
    {
        public string ID { get; set; }
        public string LebaneseID { get; set; }

        public string Phone {  get; set; }
        public string ResumeAr {  get; set; }
        public string SkillsEng {  get; set; }

        public Person(string id,string lid,string phone,string rar,string seng)
        {
            this.ID = id;
            this.LebaneseID = lid;
            this.Phone = phone;
            this.ResumeAr = rar;
            this.SkillsEng = seng;
        }
            
        public Person()
        {
            this.ID = "";
            this.LebaneseID = "";
            this.Phone = "";
            this.ResumeAr = "";
            this.SkillsEng = "";
        }

        public string AddPerson()
        {
            Guid newGuid = Guid.NewGuid();        
            this.ID = newGuid.ToString();


            SqlConnection connection = new SqlConnection("server=.;database=NRS;trusted_connection=true;TrustServerCertificate=True;");

            string query = "insert into people values (@id,@lid,@phone,@ar,@eng)";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@id",this.ID);
            command.Parameters.AddWithValue("@lid",this.LebaneseID);
            command.Parameters.AddWithValue("@phone", this.Phone);
            command.Parameters.AddWithValue("@ar",this.ResumeAr);
            command.Parameters.AddWithValue("@eng",this.SkillsEng??"");


       

            try
            {
                connection.Open();

                command.ExecuteNonQuery();

                return this.ID;

            }
            finally { connection.Close(); }
            
        }

        public static Person FindPerson(string ID)
        {
            SqlConnection connection = new SqlConnection("server=.;database=NRS;trusted_connection=true;TrustServerCertificate=True;");

            string query = "select * from people where ID=@id";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@id", ID);

            Person person = new Person();

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

               

                if (reader.Read())
                {
                    person = new Person(ID, "", (string)reader["Phone"], (string)reader["ResumeAr"], (string)reader["SkillsEng"]);
                }
                reader.Close();
            }
            finally
            { connection.Close(); }
            return person;
        }


        public bool UpdatePerson()
        {

            SqlConnection connection = new SqlConnection("server=.;database=NRS;trusted_connection=true;TrustServerCertificate=True;");

            string query = "update people set Phone=@phone,ResumeAr=@ar,SkillsEng=@eng where ID=@ID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@phone", this.Phone);
            command.Parameters.AddWithValue("@ID", this.ID);
            command.Parameters.AddWithValue("@ar", this.ResumeAr);
            command.Parameters.AddWithValue("@eng", this.SkillsEng??"");

            bool isupdated = false;

            try
            {
                connection.Open();

                int rowsaffected = command.ExecuteNonQuery();

                if (rowsaffected > 0)
                {
                    isupdated = true;
                }

            }
            finally { connection.Close(); }
            return isupdated;
        }


    }


    [Route("api/NRS")]
    [ApiController]
    public class NRSController : ControllerBase
    {

       

        [HttpPost("AddPerson")]
        public ActionResult AddPerson([FromBody] Person person)
        {
            if(String.IsNullOrEmpty(person.LebaneseID) || String.IsNullOrEmpty(person.Phone) || String.IsNullOrEmpty(person.ResumeAr))
            {
                return BadRequest();
            }

            string newID = person.AddPerson();
            if (newID != "")
                return Ok(new clsID(newID));
            else
                return StatusCode(StatusCodes.Status500InternalServerError);

        }



        public class clsID
        {
            public string? ID { get; set; }

            public clsID(string id)
            {
                ID = id;
            }
        }

        [HttpPost("GetPerson")]
        public ActionResult GetPerson([FromBody] clsID id)
        {
            if (string.IsNullOrEmpty(id.ID)) {
                return BadRequest();
            }


            Person person = Person.FindPerson(id.ID);


            if (string.IsNullOrEmpty(person.ResumeAr))
                return NotFound();
            else
                return Ok(person);
              

        }

        [HttpPut("UpdatePerson")]
        public ActionResult UpdatePerson([FromBody] Person person)
        {
            if ( String.IsNullOrEmpty(person.ResumeAr) || String.IsNullOrEmpty(person.ResumeAr))
            {
                return BadRequest();
            }

            if(person.UpdatePerson())
                return Ok();
            else
                return StatusCode(StatusCodes.Status500InternalServerError);

        }
    }
}
