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

    public PlayerProcesses(DBService context)
    {
        dbContext = context;
    }

    public bool createPlayer(string name, string code)
    {
        code = code.Trim();
        int a = dbContext.executeNonQuery($"insert into Players (lobbyID, username) values ('{code}', '{name}')");
        return a > 0; 
    }

    public List<Player> getPlayers(string code)
    {
        return DataTableMapper.MapDataTableToObjectList<Player>(dbContext.executeQuery($"select * from Players where lobbyID = {code}"));
    }
}

