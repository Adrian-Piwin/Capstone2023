using UnityEngine;
using System.Data;
using System.Data.SqlClient;

public class DBService : MonoBehaviour
{
    private string connectionString;
    private SqlConnection connection;

    // Constructor
    public DBService()
    {
        // Create the connection string
        connectionString = "Server=142.93.1.207;Database=CampusQuest;User Id=sa;Password=FraPre!A;"; ;

        // Initialize and open the connection
        connection = new SqlConnection(connectionString);
        connection.Open();
    }

    // Close the database connection when the object is destroyed
    private void OnDestroy()
    {
        if (connection != null && connection.State == ConnectionState.Open)
        {
            connection.Close();
        }
    }

    // Execute a SQL query and return the result as a DataTable
    public DataTable ExecuteQuery(string query)
    {
        DataTable dataTable = new DataTable();
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                adapter.Fill(dataTable);
            }
        }
        return dataTable;
    }

    // Execute a non-query SQL command (e.g., INSERT, UPDATE, DELETE)
    public int ExecuteNonQuery(string query)
    {
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            return command.ExecuteNonQuery();
        }
    }

    // Example: Select all players from a "players" table
    public DataTable SelectAllPlayers()
    {
        string query = "SELECT * FROM players";
        return ExecuteQuery(query);
    }

    // Example: Insert a new player into the "players" table
    public int InsertPlayer(string playerName)
    {
        string query = $"INSERT INTO players (Name) VALUES ('{playerName}')";
        return ExecuteNonQuery(query);
    }
}
