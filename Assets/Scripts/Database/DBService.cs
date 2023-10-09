using UnityEngine;
using System.Data;
using System.Data.SqlClient;

public class DBService : MonoBehaviour
{
    private string connectionString = "Server=142.93.1.207;Database=CampusQuest;User Id=sa;Password=FraPre!A;";

    // Start is called before the first frame update
    void Start()
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM players";
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Debug.Log("id: " + reader[0].ToString());
                        Debug.Log("val: " + reader[1].ToString());
                    }
                }
            }

            connection.Close();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
