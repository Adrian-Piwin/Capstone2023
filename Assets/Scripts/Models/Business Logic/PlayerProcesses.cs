using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

public  class PlayerProcesses
{
    private DBService dbContext;
    private string codeContext;

    public PlayerProcesses(DBService context, string code)
    {
        dbContext = context;
        codeContext = code;
    }

    public bool createPlayer(string name)
    {
        int a = dbContext.executeNonQuery($"insert into Players (lobbyID, username) values ('{codeContext}', '{name}')");
        return a > 0; 
    }

    public List<Player> getPlayers()
    {
        return DataTableMapper.MapDataTableToObjectList<Player>(dbContext.executeQuery($"select * from Players where lobbyID = {codeContext}"));
    }
}

