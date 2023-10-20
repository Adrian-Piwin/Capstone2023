using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

public  class CampusProcesses
{
    private DBService dbContext;
    private FirebaseService fbContext;
    private string codeContext;

    public CampusProcesses(DBService context, string code)
    {
        dbContext = context;
        codeContext = code;
    }

    public CampusProcesses(FirebaseService fbContext, DBService dbContext, string code)
    {
        this.fbContext = fbContext;
        this.dbContext = dbContext;
        codeContext = code;
    }

    public Campus getCampus() 
    {
        Campus found = DataTableMapper.MapDataRowToObject<Campus>(dbContext.executeQuery($"select * from Campus where lobbyID = {codeContext}"));
        return found;
    }

    public bool isLobbyValid()
    {
        Campus found = DataTableMapper.MapDataRowToObject<Campus>(dbContext.executeQuery($"select * from Campus where lobbyID = {codeContext}"));
        return found != null;
    }

    public bool isGameStarted()
    {
        Campus found = DataTableMapper.MapDataRowToObject<Campus>(dbContext.executeQuery($"select * from Campus where lobbyID = {codeContext}"));
        return found.gameStarted;
    }

    public async Task<byte[]> getMap()
    {
        Campus found = DataTableMapper.MapDataRowToObject<Campus>(dbContext.executeQuery($"select * from Campus where lobbyID = {codeContext}"));
        return await fbContext.GetImageBytes(found.lobbyID + "/" + found.map);
    }
}

