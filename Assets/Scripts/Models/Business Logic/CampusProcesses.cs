using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

public  class CampusProcesses
{
    private DBService dbContext;

    public CampusProcesses(DBService context)
    {
        dbContext = context;
    }

    public bool isLobbyValid(string code)
    {
        Campus found = DataTableMapper.MapDataRowToObject<Campus>(dbContext.executeQuery($"select * from Campus where lobbyID = {code}"));
        return found != null;
    }

}

